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
    public class MeleeEnemyBeliefSet : BeliefSet
    {
        /// <summary>
        /// The player object in the scene.
        /// </summary>
        public Belief<GameObject, GameObject> Player = new(reference: GameObject.Find("Player"), x => x);

        /// <summary>
        /// The palyer Health value
        /// </summary>
        public Belief<GameObject, HealthComponent> PlayerHealth = new(GameObject.Find("Player"), x => x.GetComponent<HealthComponent>());

        /// <summary>
        /// If the enemy exists in the scene
        /// </summary>
        public Belief<GameObject, bool> EnemyExists = new(GameObject.Find("Melee Enemy Body"), x => x != null);

        /// <summary>
        /// The target position that the player needs to move towards.
        /// Find the first enemy in the scene.
        /// </summary>
        public Belief<GameObject, Vector3> EnemyPosition = new(GameObject.Find("Melee Enemy Body"), x =>
        {
            if (x == null)
                return Vector3.zero;
            return x.transform.position;
        });
    }

    /// <summary>
    /// A basic test for the melee enemy.
    /// The player will first let himself be damaged by the enemy.
    /// Then, it will kill the enemy.
    /// </summary>
    public class MeleeEnemyAplibTest
    {
        [SetUp]
        public void SetUp()
        {
            Debug.Log("Starting test MeleeEnemyTest");
            SceneManager.LoadScene("MeleeEnemyTestScene");
        }

        [UnityTest]
        public IEnumerator PerformMeleeEnemyTest()
        {
            InputManager.Instance.enabled = false;
            MeleeEnemyBeliefSet beliefSet = new();

            // Create an intent for the agent that moves the agent towards the target position.
            TransformPathfinderAction<MeleeEnemyBeliefSet> transformPathfinderAction = new(
                b =>
                {
                    GameObject player = b.Player;
                    return player.transform;
                },
                beliefSet.EnemyPosition,
                0.9f
            );

            // Create an attack action for the agent, that attacks the enemy
            Action<MeleeEnemyBeliefSet> attackAction = new(
                b =>
                {
                    GameObject player = b.Player;
                    MeleeWeapon weapon = player.GetComponentInChildren<MeleeWeapon>();
                    weapon.UseWeapon();
                }
            );

            // Tactic: Wait and do nothing
            PrimitiveTactic<MeleeEnemyBeliefSet> waitTactic = new(new Action<MeleeEnemyBeliefSet>(b => { }));

            // Tactic: Move the player towards the target position
            PrimitiveTactic<MeleeEnemyBeliefSet> moveTactic = new(transformPathfinderAction);

            // Tactic: Attack the enemy
            PrimitiveTactic<MeleeEnemyBeliefSet> attackTactic = new(attackAction);

            // Goal: Wait until damaged by the enemy
            PrimitiveGoalStructure<MeleeEnemyBeliefSet> waitGoal = new(goal: new Goal<MeleeEnemyBeliefSet>(waitTactic, PlayerDamagedPredicate));

            // Goal: Reach the target position and attack the enemy
            PrimitiveGoalStructure<MeleeEnemyBeliefSet> moveGoal = new(goal: new Goal<MeleeEnemyBeliefSet>(moveTactic, MovePredicate));
            PrimitiveGoalStructure<MeleeEnemyBeliefSet> attackGoal = new(goal: new Goal<MeleeEnemyBeliefSet>(attackTactic, EnemyKilledPredicate));

            // Final Goal: Wait until damaged by the enemy, then reach the target position and attack the enemy
            SequentialGoalStructure<MeleeEnemyBeliefSet> finalGoal = new SequentialGoalStructure<MeleeEnemyBeliefSet>(waitGoal, moveGoal, attackGoal);

            DesireSet<MeleeEnemyBeliefSet> desire = new(finalGoal);

            // Create a new agent with the goal
            BdiAgent<MeleeEnemyBeliefSet> agent = new(beliefSet, desire);

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
