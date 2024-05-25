using Aplib;
using Aplib.Core;
using Aplib.Core.Belief;
using Aplib.Core.Desire;
using Aplib.Core.Desire.Goals;
using Aplib.Core.Intent.Actions;
using Aplib.Core.Intent.Tactics;
using Aplib.Integrations.Unity.Actions;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

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
            InputManager.Instance.enabled = false;
            RangedEnemyBeliefSet rangedEnemyBeliefSet = new();

            // Create an intent for the agent that moves the agent towards the target position.
            TransformPathfinderAction<RangedEnemyBeliefSet> transformPathfinderAction = new(
                beliefSet =>
                {
                    GameObject player = beliefSet.Player;
                    return player.GetComponent<Rigidbody>();
                },
                beliefSet => beliefSet.TargetPosition,
                0.9f
            );

            // Tactic: Wait and do nothing.
            PrimitiveTactic<RangedEnemyBeliefSet> waitTactic = new(new Action<RangedEnemyBeliefSet>(_ => { }));

            // Tactic: Move the player towards the target position.
            PrimitiveTactic<RangedEnemyBeliefSet> moveTowardsTarget = new(transformPathfinderAction);

            // Goal: Reach the target position.
            PrimitiveGoalStructure<RangedEnemyBeliefSet> targetReached = new(goal: new Goal<RangedEnemyBeliefSet>(moveTowardsTarget, MovePredicate));

            // Goal: Wait until killed by the enemy.
            PrimitiveGoalStructure<RangedEnemyBeliefSet> killedByEnemy = new(goal: new Goal<RangedEnemyBeliefSet>(waitTactic, PlayerRespawnedPredicate));

            // Goal: Wait and check if enemy is not following player.
            PrimitiveGoalStructure<RangedEnemyBeliefSet> notFollowedByEnemy = new(goal: new Goal<RangedEnemyBeliefSet>(waitTactic, EnemyNotMovingPredicate));

            // Final Goal: Wait until killed by enemy, then check if out of enemy vision range.
            SequentialGoalStructure<RangedEnemyBeliefSet> finalGoal = new SequentialGoalStructure<RangedEnemyBeliefSet>(targetReached, killedByEnemy, notFollowedByEnemy);

            DesireSet<RangedEnemyBeliefSet> desire = new(finalGoal);

            // Create a new agent with the goal.
            BdiAgent<RangedEnemyBeliefSet> agent = new(rangedEnemyBeliefSet, desire);

            AplibRunner testRunner = new(agent);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return testRunner.Test();

            // Assert that the player has reached the target position.
            Assert.AreEqual(agent.Status, CompletionStatus.Success);

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