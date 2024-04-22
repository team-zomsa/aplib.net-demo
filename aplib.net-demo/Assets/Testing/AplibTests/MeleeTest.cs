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

namespace Tests.AplibTests
{
    public class MeleeBeliefSet : BeliefSet
    {
        /// <summary>
        /// The player object in the scene.
        /// </summary>
        public Belief<GameObject, GameObject> Player = new(reference: GameObject.Find("PlayerPhysics"), x => x);

        /// <summary>
        /// If the enemy exists in the scene
        /// </summary>
        public Belief<GameObject, bool> EnemyExists = new(GameObject.Find("Target Dummy Body"), x => x != null);

        /// <summary>
        /// The target position that the player needs to move towards.
        /// Find the first enemy in the scene.
        /// </summary>
        public Belief<GameObject, Vector3> EnemyPosition = new(GameObject.Find("Target Dummy Body"), x =>
        {
            if (x == null)
                return Vector3.zero;
            return x.transform.position;
        });
    }

    public class MeleeAplibTest
    {
        [SetUp]
        public void SetUp()
        {
            Debug.Log("Starting test MeleeTest");
            SceneManager.LoadScene("MeleeTestScene");
        }

        [UnityTest]
        public IEnumerator PerformMeleeTest()
        {
            InputManager.Instance.enabled = false;
            MeleeBeliefSet beliefSet = new();

            // Create an intent for the agent that moves the agent towards the target position.
            TransformPathfinderAction<MeleeBeliefSet> transformPathfinderAction = new(
                b =>
                {
                    GameObject player = b.Player;
                    return player.transform;
                },
                beliefSet.EnemyPosition,
                0.9f
            );

            // Create an attack action for the agent, that attacks the enemy
            Action<MeleeBeliefSet> attackAction = new(
                b =>
                {
                    GameObject player = b.Player;
                    MeleeWeapon weapon = player.GetComponentInChildren<MeleeWeapon>();
                    weapon.UseWeapon();
                }
            );

            // Tactic: Move the player towards the target position
            PrimitiveTactic<MeleeBeliefSet> moveTactic = new(transformPathfinderAction);

            // Tactic: Attack the enemy
            PrimitiveTactic<MeleeBeliefSet> attackTactic = new(attackAction);

            // Goal: Reach the target position and attack the enemy
            PrimitiveGoalStructure<MeleeBeliefSet> moveGoal = new(goal: new Goal<MeleeBeliefSet>(moveTactic, MovePredicate));
            PrimitiveGoalStructure<MeleeBeliefSet> attackGoal = new(goal: new Goal<MeleeBeliefSet>(attackTactic, EnemyKilledPredicate));
            SequentialGoalStructure<MeleeBeliefSet> finalGoal = new SequentialGoalStructure<MeleeBeliefSet>(moveGoal, attackGoal);

            DesireSet<MeleeBeliefSet> desire = new(finalGoal);

            // Create a new agent with the goal
            BdiAgent<MeleeBeliefSet> agent = new(beliefSet, desire);

            AplibRunner testRunner = new(agent);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return testRunner.Test();

            // Assert that the player has reached the target position
            Assert.IsTrue(condition: agent.Status == CompletionStatus.Success);
            yield break;

            bool MovePredicate(MeleeBeliefSet beliefSet)
            {
                // The player has reached the target position
                GameObject player = beliefSet.Player;
                Vector3 target = beliefSet.EnemyPosition;

                return Vector3.Distance(player.transform.position, target) < 2f;
            }

            bool EnemyKilledPredicate(MeleeBeliefSet beliefSet) => !beliefSet.EnemyExists;
        }
    }
}
