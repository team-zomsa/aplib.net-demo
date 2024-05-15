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
            InputManager.Instance.enabled = false;
            HealthPotionBeliefSet beliefSet = new();

            // Create an intent for the agent that moves the agent towards the target position.
            TransformPathfinderAction<HealthPotionBeliefSet> transformPathfinderAction = new(
                objectQuery: beliefSet =>
                {
                    GameObject player = beliefSet.Player;
                    return player.GetComponent<Rigidbody>();
                },
                location: beliefSet => beliefSet.TargetPosition,
                heightOffset: 0.3f
            );

            // Action: Decrease the player's health.
            Action<HealthPotionBeliefSet> decreaseHealth = new(
                b =>
                {
                    HealthComponent playerHealth = b.PlayerHealth;
                    playerHealth.ReduceHealth(_reduceHealthAmount);
                }
            );

            // Action: Use current inventory item (health potion).
            Action<HealthPotionBeliefSet> activateInventoryItem = new(
                b =>
                {
                    Inventory inventory = b.InventoryObject;
                    inventory.ActivateItem();
                }
            );

            // Tactic: Move the player towards the target position.
            PrimitiveTactic<HealthPotionBeliefSet> moveTactic = new(transformPathfinderAction);

            // Tactic: Decrease the player's health.
            PrimitiveTactic<HealthPotionBeliefSet> decreaseHealthTactic = new(decreaseHealth);

            // Tactic: Use the health potion.
            PrimitiveTactic<HealthPotionBeliefSet> usePotionTactic = new(activateInventoryItem);

            // Goal: Decrease health, reach the target position and use the health potion.
            PrimitiveGoalStructure<HealthPotionBeliefSet> decreaseHealthGoal = new(goal: new Goal<HealthPotionBeliefSet>(decreaseHealthTactic, HealthNotFullPredicate));
            PrimitiveGoalStructure<HealthPotionBeliefSet> moveGoal = new(goal: new Goal<HealthPotionBeliefSet>(moveTactic, PotionPickedupPredicate));
            PrimitiveGoalStructure<HealthPotionBeliefSet> usePotionGoal = new(goal: new Goal<HealthPotionBeliefSet>(usePotionTactic, HealthFullPredicate));
            SequentialGoalStructure<HealthPotionBeliefSet> finalGoal = new(decreaseHealthGoal, moveGoal, usePotionGoal);

            DesireSet<HealthPotionBeliefSet> desire = new(finalGoal);

            // Create a new agent with the goal.
            BdiAgent<HealthPotionBeliefSet> agent = new(beliefSet, desire);

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
