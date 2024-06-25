using Aplib.Core;
using Aplib.Core.Belief.Beliefs;
using Aplib.Core.Belief.BeliefSets;
using Aplib.Integrations.Unity;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static Aplib.Core.Combinators;
using Goal = Aplib.Core.Desire.Goals.Goal<Testing.AplibTests.RangedEnemyBeliefSet>;
using Action = Aplib.Core.Intent.Actions.Action<Testing.AplibTests.RangedEnemyBeliefSet>;
using Tactic = Aplib.Core.Intent.Tactics.Tactic<Testing.AplibTests.RangedEnemyBeliefSet>;
using GoalStructure = Aplib.Core.Desire.GoalStructures.GoalStructure<Testing.AplibTests.RangedEnemyBeliefSet>;
using BdiAgent = Aplib.Core.Agents.BdiAgent<Testing.AplibTests.RangedEnemyBeliefSet>;
using TransformPathfinderAction = Aplib.Integrations.Unity.Actions.TransformPathfinderAction<Testing.AplibTests.RangedEnemyBeliefSet>;

namespace Testing.AplibTests
{
    public class RangedEnemyBeliefSet : BeliefSet
    {
        public const int FramesToRemember = 300;

        /// <summary>
        /// The player object in the scene.
        /// </summary>
        public Belief<GameObject, GameObject> Player = new(reference: GameObject.Find("Player"), x => x);

        /// <summary>
        /// The target position for the player to move to.
        /// Also remembers the target position from a certain amount of frames ago.
        /// </summary>
        public MemoryBelief<GameObject, Vector3> TargetPosition
            = new(GameObject.Find("Ranged Enemy Body"), enemy => enemy.transform.position, FramesToRemember);

        /// <summary>
        /// The respawn position for the player.
        /// </summary>
        public Belief<GameObject, Vector3> RespawnPosition
            = new(GameObject.Find("RespawnPoint"), respawnPoint => respawnPoint.transform.position);
    }

    /// <summary>
    /// A basic test for the ranged enemy.
    /// The player will first move within vision range of the enemy.
    /// The enemy should then walk around the wall to the player and kill him.
    /// The player then respawns out of vision range of the enemy.
    /// If the enemy does not attack him then, the test is successful.
    /// </summary>
    public class RangedEnemyAplibTest
    {
        private float _targetStoppingDistance = 7f;
        private string _sceneName = "RangedEnemyTestScene";

        [SetUp]
        public void SetUp()
        {
            Debug.Log($"Starting {nameof(RangedEnemyAplibTest)}");
            SceneManager.LoadScene(_sceneName);
        }

        [UnityTest]
        public IEnumerator PerformRangedEnemyTest()
        {
            RangedEnemyBeliefSet rangedEnemyBeliefSet = new();

            // Create an intent for the agent that moves the agent towards the target position.
            Tactic move = new TransformPathfinderAction(
                beliefSet =>
                {
                    GameObject player = beliefSet.Player;
                    return player.GetComponent<Rigidbody>();
                },
                beliefSet => beliefSet.TargetPosition,
                0.9f
            );

            // Tactic: Wait and do nothing.
            Tactic waitTactic = new Action(_ => { });

            GoalStructure targetReached = new Goal(move, MovePredicate);
            GoalStructure killedByEnemy = new Goal(waitTactic, PlayerRespawnedPredicate);
            GoalStructure notFollowedByEnemy = new Goal(waitTactic, EnemyNotMovingPredicate);
            GoalStructure finalGoal = Seq(targetReached, killedByEnemy, notFollowedByEnemy);

            // Create a new agent with the goal.
            BdiAgent agent = new(rangedEnemyBeliefSet, finalGoal.Lift());

            AplibRunner testRunner = new(agent);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return testRunner.Test();

            // Assert that the player has reached the target position.
            Assert.AreEqual(agent.Status, CompletionStatus.Success);
            yield break;

            bool MovePredicate(RangedEnemyBeliefSet beliefSet)
            {
                // The player has reached the target position.
                GameObject player = beliefSet.Player;
                Vector3 target = beliefSet.TargetPosition;

                return Vector3.Distance(player.transform.position, target) < _targetStoppingDistance;
            }

            bool PlayerRespawnedPredicate(RangedEnemyBeliefSet beliefSet)
            {
                GameObject player = beliefSet.Player;
                Vector3 respawnPosition = beliefSet.RespawnPosition;

                return Vector3.Distance(player.transform.position, respawnPosition) < 0.5;
            }

            bool EnemyNotMovingPredicate(RangedEnemyBeliefSet beliefSet)
            {
                Vector3 target = beliefSet.TargetPosition;
                Vector3 targetMemory = beliefSet.TargetPosition.GetMemoryAt(RangedEnemyBeliefSet.FramesToRemember - 1, true);
                return Vector3.Distance(target, targetMemory) < 0.05f;
            }
        }
    }
}
