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
using Goal = Aplib.Core.Desire.Goals.Goal<Testing.AplibTests.MeleeEnemyBeliefSet>;
using Action = Aplib.Core.Intent.Actions.Action<Testing.AplibTests.MeleeEnemyBeliefSet>;
using Tactic = Aplib.Core.Intent.Tactics.Tactic<Testing.AplibTests.MeleeEnemyBeliefSet>;
using GoalStructure = Aplib.Core.Desire.GoalStructures.GoalStructure<Testing.AplibTests.MeleeEnemyBeliefSet>;
using BdiAgent = Aplib.Core.Agents.BdiAgent<Testing.AplibTests.MeleeEnemyBeliefSet>;
using TransformPathfinderAction = Aplib.Integrations.Unity.Actions.TransformPathfinderAction<Testing.AplibTests.MeleeEnemyBeliefSet>;

namespace Testing.AplibTests
{
    public class MeleeEnemyBeliefSet : BeliefSet
    {
        /// <summary>
        /// The player object in the scene.
        /// </summary>
        public Belief<GameObject, GameObject> Player = new(reference: GameObject.Find("Player"), x => x);

        /// <summary>
        /// The player health value.
        /// </summary>
        public Belief<GameObject, HealthComponent> PlayerHealth = new(GameObject.Find("Player"), x => x.GetComponent<HealthComponent>());

        /// <summary>
        /// If the enemy exists in the scene.
        /// </summary>
        public Belief<GameObject, bool> EnemyExists = new(GameObject.Find("Melee Enemy Body"), x => x != null);

        /// <summary>
        /// The target position that the player needs to move towards.
        /// Find the first enemy in the scene.
        /// </summary>
        public Belief<GameObject, Vector3> EnemyPosition = new(GameObject.Find("Melee Enemy Body"),
            x => x == null ? Vector3.zero : x.transform.position);
    }

    /// <summary>
    /// A basic test for the melee enemy.
    /// The player will first let himself be damaged by the enemy.
    /// Then, it will kill the enemy.
    /// </summary>
    public class MeleeEnemyAplibTest
    {
        private string _sceneName = "MeleeEnemyTestScene";

        [SetUp]
        public void SetUp()
        {
            Debug.Log($"Starting {nameof(MeleeEnemyAplibTest)}");
            SceneManager.LoadScene(_sceneName);
        }

        [TearDown]
        public void TearDown()
        {
            Debug.Log($"Finished {nameof(MeleeEnemyAplibTest)}");
            SceneManager.UnloadSceneAsync(_sceneName);
        }

        [UnityTest]
        public IEnumerator PerformMeleeEnemyTest()
        {
            MeleeEnemyBeliefSet beliefSet = new();

            // Create an intent for the agent that moves the agent towards the target position.
            Tactic move = new TransformPathfinderAction(
                b =>
                {
                    GameObject player = b.Player;
                    return player.GetComponent<Rigidbody>();
                },
                beliefSet.EnemyPosition,
                0.9f
            );

            Tactic attack = new Action(beliefSet =>
            {
                GameObject player = beliefSet.Player;
                MeleeWeapon weapon = player.GetComponentInChildren<MeleeWeapon>();
                weapon.UseWeapon();
            });

            Tactic waitTactic = new Action(_ => { });
            GoalStructure waitGoal = new Goal(waitTactic, PlayerDamagedPredicate);

            GoalStructure moveGoal = new Goal(move, MovePredicate);
            GoalStructure attackGoal = new Goal(attack, EnemyKilledPredicate);
            GoalStructure finalGoal = Seq(waitGoal, moveGoal, attackGoal);

            // Create a new agent with the goal
            BdiAgent agent = new(beliefSet, finalGoal.Lift());

            AplibRunner testRunner = new(agent);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return testRunner.Test();

            // Assert that the player has reached the target position
            Assert.IsTrue(condition: agent.Status == CompletionStatus.Success);
            yield break;

            bool MovePredicate(MeleeEnemyBeliefSet beliefSet)
            {
                // The player has reached the target position
                GameObject player = beliefSet.Player;
                Vector3 target = beliefSet.EnemyPosition;

                return Vector3.Distance(player.transform.position, target) < 2f;
            }

            bool EnemyKilledPredicate(MeleeEnemyBeliefSet beliefSet) => !beliefSet.EnemyExists;

            bool PlayerDamagedPredicate(MeleeEnemyBeliefSet beliefSet)
            {
                HealthComponent health = beliefSet.PlayerHealth;

                return health.Health < health.MaxHealth;
            }
        }
    }
}
