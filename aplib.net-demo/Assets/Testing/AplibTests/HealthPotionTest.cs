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
using Goal = Aplib.Core.Desire.Goals.Goal<Testing.AplibTests.HealthPotionBeliefSet>;
using Action = Aplib.Core.Intent.Actions.Action<Testing.AplibTests.HealthPotionBeliefSet>;
using Tactic = Aplib.Core.Intent.Tactics.Tactic<Testing.AplibTests.HealthPotionBeliefSet>;
using GoalStructure = Aplib.Core.Desire.GoalStructures.GoalStructure<Testing.AplibTests.HealthPotionBeliefSet>;
using BdiAgent = Aplib.Core.Agents.BdiAgent<Testing.AplibTests.HealthPotionBeliefSet>;
using TransformPathfinderAction = Aplib.Integrations.Unity.Actions.TransformPathfinderAction<Testing.AplibTests.HealthPotionBeliefSet>;

namespace Testing.AplibTests
{
    public class HealthPotionBeliefSet : BeliefSet
    {
        /// <summary>
        /// The player object in the scene.
        /// </summary>
        public Belief<GameObject, GameObject> Player = new(GameObject.Find("Player"), x => x);

        /// <summary>
        /// The inventory in the scene.
        /// </summary>
        public Belief<GameObject, Inventory> InventoryObject = new(GameObject.Find("InventoryObject"), x => x.GetComponent<Inventory>());

        /// <summary>
        /// The player health value.
        /// </summary>
        public Belief<GameObject, HealthComponent> PlayerHealth = new(GameObject.Find("Player"), x => x.GetComponent<HealthComponent>());

        /// <summary>
        /// The target position that the player needs to move towards.
        /// Find the first Health Potion in the scene.
        /// </summary>
        public Belief<GameObject, Vector3> TargetPosition = new(GameObject.Find("HealthPotion"), x => x == null ? Vector3.zero : x.transform.position);

        /// <summary>
        /// The health potion in the scene.
        /// </summary>
        public Belief<GameObject, bool> PotionExists = new(GameObject.Find("HealthPotion"), x => x != null && x.activeSelf && x.activeInHierarchy);
    }

    public class HealthPotionAplibTest
    {
        private readonly int _reduceHealthAmount = 50;

        [SetUp]
        public void SetUp()
        {
            Debug.Log("Starting test HealthPotionTest");
            SceneManager.LoadScene("HealthPotionTestScene");
        }

        /// <summary>
        /// The player will walk up to a health potion and use it.
        /// </summary>
        [UnityTest]
        public IEnumerator PerformPotionTest()
        {
            HealthPotionBeliefSet beliefSet = new();

            Tactic moveTactic = new TransformPathfinderAction (
                objectQuery: beliefSet =>
                {
                    GameObject player = beliefSet.Player;
                    return player.GetComponent<Rigidbody>();
                },
                location: beliefSet => beliefSet.TargetPosition,
                heightOffset: 0.3f
            );

            Tactic decreaseHealth = new Action(
                beliefSet =>
                {
                    HealthComponent playerHealth = beliefSet.PlayerHealth;
                    playerHealth.ReduceHealth(_reduceHealthAmount);
                }
            );

            Tactic usePotion = new Action(
                beliefSet =>
                {
                    Inventory inventory = beliefSet.InventoryObject;
                    inventory.ActivateItem();
                }
            );

            // Goal: Decrease health, reach the target position and use the health potion.
            GoalStructure decreaseHealthGoal = new Goal(decreaseHealth, HealthNotFullPredicate);
            GoalStructure moveGoal = new Goal(moveTactic, PotionPickedupPredicate);
            GoalStructure usePotionGoal = new Goal(usePotion, HealthFullPredicate);
            GoalStructure finalGoal = Seq(decreaseHealthGoal, moveGoal, usePotionGoal);

            // Create a new agent with the goal.
            BdiAgent agent = new(beliefSet, finalGoal.Lift());

            AplibRunner testRunner = new(agent);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return testRunner.Test();

            // Assert that the player has reached the target position.
            Assert.IsTrue(condition: agent.Status == CompletionStatus.Success);
            yield break;

            bool PotionPickedupPredicate(HealthPotionBeliefSet beliefSet)
            {
                // The potion does not exist.
                bool potionExists = beliefSet.PotionExists;
                return !potionExists;
            }

            bool HealthFullPredicate(HealthPotionBeliefSet beliefSet)
            {
                // The player's health is full.
                HealthComponent playerHealth = beliefSet.PlayerHealth;
                return playerHealth.Health >= playerHealth.MaxHealth;
            }

            bool HealthNotFullPredicate(HealthPotionBeliefSet beliefSet)
            {
                // The player's health is not full.
                HealthComponent playerHealth = beliefSet.PlayerHealth;
                return playerHealth.Health < playerHealth.MaxHealth;
            }
        }
    }
}
