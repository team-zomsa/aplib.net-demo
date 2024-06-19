using Aplib.Core;
using Aplib.Core.Agents;
using Aplib.Core.Belief.Beliefs;
using Aplib.Core.Belief.BeliefSets;
using Aplib.Core.Desire.DesireSets;
using Aplib.Core.Desire.Goals;
using Aplib.Core.Desire.GoalStructures;
using Aplib.Core.Intent.Actions;
using Aplib.Core.Intent.Tactics;
using Aplib.Integrations.Unity;
using Aplib.Integrations.Unity.Actions;
using Entities.Weapons;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Testing.AplibTests
{
    public class MeleeBeliefSet : BeliefSet
    {
        /// <summary>
        /// Whether the enemy exists in the scene.
        /// </summary>
        public Belief<GameObject, bool> EnemyExists = new
        (
            GameObject.FindWithTag("Enemy"),
            enemy => enemy
        );

        /// <summary>
        /// The target position that the player needs to move towards.
        /// Find the first enemy in the scene.
        /// </summary>
        public Belief<GameObject, Vector3> EnemyPosition = new(
            GameObject.FindWithTag("Enemy"),
            enemy => enemy ? enemy.transform.position : Vector3.zero
        );

        /// <summary>
        /// The player object in the scene.
        /// </summary>
        public Belief<GameObject, GameObject> Player = new(GameObject.Find("Player"), x => x);
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
            MeleeBeliefSet beliefSet = new();

            // Create an intent for the agent that moves the agent towards the target position.
            TransformPathfinderAction<MeleeBeliefSet> transformPathfinderAction = new(
                beliefSet =>
                {
                    GameObject player = beliefSet.Player;
                    return player.GetComponent<Rigidbody>();
                },
                beliefSet => beliefSet.EnemyPosition,
                0.9f
            );

            // Create an attack action for the agent, that attacks the enemy.
            Action<MeleeBeliefSet> attackAction = new(
                beliefSet =>
                {
                    GameObject player = beliefSet.Player;
                    MeleeWeapon weapon = player.GetComponentInChildren<MeleeWeapon>();
                    weapon.UseWeapon();
                }
            );

            // Tactic: Move the player towards the target position
            PrimitiveTactic<MeleeBeliefSet> moveTactic = new(transformPathfinderAction);

            // Tactic: Attack the enemy
            PrimitiveTactic<MeleeBeliefSet> attackTactic = new(attackAction);

            // Goal: Reach the target position and attack the enemy
            PrimitiveGoalStructure<MeleeBeliefSet> moveGoal = new(new Goal<MeleeBeliefSet>(moveTactic, MovePredicate));
            PrimitiveGoalStructure<MeleeBeliefSet> attackGoal = new(new Goal<MeleeBeliefSet>(attackTactic, EnemyKilledPredicate));
            SequentialGoalStructure<MeleeBeliefSet> finalGoal = new(moveGoal, attackGoal);

            DesireSet<MeleeBeliefSet> desire = new(finalGoal);

            // Create a new agent with the goal
            BdiAgent<MeleeBeliefSet> agent = new(beliefSet, desire);

            AplibRunner testRunner = new(agent);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return testRunner.Test();

            // Assert that the player has reached the target position
            Assert.IsTrue(agent.Status == CompletionStatus.Success);
            yield break;

            bool MovePredicate(MeleeBeliefSet beliefSet)
            {
                // The player has reached the target position
                GameObject player = beliefSet.Player;
                Vector3 target = beliefSet.EnemyPosition;

                return Vector3.Distance(player.transform.position, target) < 1.5f;
            }

            bool EnemyKilledPredicate(MeleeBeliefSet beliefSet)
            {
                return !beliefSet.EnemyExists;
            }
        }
    }
}
