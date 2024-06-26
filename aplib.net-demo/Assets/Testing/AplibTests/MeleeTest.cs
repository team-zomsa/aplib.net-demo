// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using Aplib.Core;
using Aplib.Core.Belief.Beliefs;
using Aplib.Core.Belief.BeliefSets;
using Aplib.Integrations.Unity;
using Entities.Weapons;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static Aplib.Core.Combinators;
using Goal = Aplib.Core.Desire.Goals.Goal<Testing.AplibTests.MeleeBeliefSet>;
using Action = Aplib.Core.Intent.Actions.Action<Testing.AplibTests.MeleeBeliefSet>;
using Tactic = Aplib.Core.Intent.Tactics.Tactic<Testing.AplibTests.MeleeBeliefSet>;
using GoalStructure = Aplib.Core.Desire.GoalStructures.GoalStructure<Testing.AplibTests.MeleeBeliefSet>;
using BdiAgent = Aplib.Core.Agents.BdiAgent<Testing.AplibTests.MeleeBeliefSet>;
using TransformPathfinderAction = Aplib.Integrations.Unity.Actions.TransformPathfinderAction<Testing.AplibTests.MeleeBeliefSet>;

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
            Tactic move = new TransformPathfinderAction(
                beliefSet =>
                {
                    GameObject player = beliefSet.Player;
                    return player.GetComponent<Rigidbody>();
                },
                beliefSet => beliefSet.EnemyPosition,
                0.9f
            );

            // Create an attack action for the agent, that attacks the enemy.
            Tactic attack = new Action (
                beliefSet =>
                {
                    GameObject player = beliefSet.Player;
                    MeleeWeapon weapon = player.GetComponentInChildren<MeleeWeapon>();
                    weapon.UseWeapon();
                }
            );

            // Goal: Reach the target position and attack the enemy
            GoalStructure moveGoal = new Goal(move, MovePredicate);
            GoalStructure attackGoal = new Goal(attack, EnemyKilledPredicate);
            GoalStructure finalGoal = Seq(moveGoal, attackGoal);

            // Create a new agent with the goal
            BdiAgent agent = new(beliefSet, finalGoal.Lift());

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
