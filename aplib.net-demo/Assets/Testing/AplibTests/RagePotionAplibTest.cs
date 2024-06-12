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
using Assets.Scripts.Items;
using Entities.Weapons;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Testing.AplibTests
{
    public class RagePotionBeliefSet : BeliefSet
    {
        /// <summary>
        /// The player object in the scene.
        /// </summary>
        public Belief<GameObject, GameObject> Player = new(GameObject.Find("Player"), x => x);

        /// <summary>
        /// The rage potion in the scene.
        /// </summary>
        public Belief<GameObject, RagePotion> RagePotion = new(GameObject.Find("RagePotion"), x => x != null ? x.GetComponent<RagePotion>() : null);

        /// <summary>
        /// The target dummy health in the scene.
        /// </summary>
        // public Belief<GameObject, GameObject> TargetDummy = new(GameObject.Find("Target Dummy"), x => x);
        public MemoryBelief<GameObject, int> TargetDummyHealth = new(GameObject.Find("Target Dummy"), x => x.GetComponentInChildren<HealthComponent>().Health, 60);


        /// <summary>
        /// The inventory in the scene.
        /// </summary>
        public Belief<GameObject, Inventory> InventoryObject = new(GameObject.Find("InventoryObject"), x => x.GetComponent<Inventory>());

        /// <summary>
        /// The player weapon in the scene.
        /// </summary>
        public Belief<GameObject, Weapon> PlayerWeapon = new(GameObject.Find("Player"), x => x.GetComponentInChildren<EquipmentInventory>().CurrentEquipment as Weapon);

        /// <summary>
        /// The target position that the player needs to move towards.
        /// Find the first Rage Potion in the scene.
        /// </summary>
        public Belief<GameObject, Vector3> PotionPosition = new(GameObject.Find("RagePotion"), x => x == null ? Vector3.zero : x.transform.position);

        public Belief<GameObject, Vector3> EnemyPosition = new(GameObject.Find("Target Dummy"), x => x == null ? Vector3.zero : x.transform.position);

        /// <summary>
        /// The rage potion in the scene.
        /// </summary>
        public Belief<GameObject, bool> PotionExists = new(GameObject.Find("RagePotion"), x => x != null && x.activeSelf && x.activeInHierarchy);
    }

    public class RagePotionAblibTest
    {
        private int _increasedDamage;
        private int _startDamage;
        private int _enemyStartHealth;

        [SetUp]
        public void SetUp()
        {
            Debug.Log("Starting test RagePotionAplibTest");
            SceneManager.LoadScene("RagePotionTestScene");
        }

        /// <summary>
        /// The player will walk up to a rage potion and use it.
        /// Then check if the rage potion doubles the players damage when damaging an entity.
        /// </summary>
        [UnityTest]
        public IEnumerator PerformRagePotionAplibTest()
        {
            InputManager.Instance.enabled = false;
            var beliefSet = new RagePotionBeliefSet();

            // Calculate the expected damage increase
            int _damageIncreasePercentage = beliefSet.RagePotion.Observation.DamageIncreasePercentage;
            _startDamage = beliefSet.PlayerWeapon.Observation.Damage;
            _increasedDamage = _startDamage * _damageIncreasePercentage / 100;

            // Grab the enemy's health at the start
            _enemyStartHealth = beliefSet.TargetDummyHealth.Observation;

            // Action: Create an action that moves towards the potion
            TransformPathfinderAction<RagePotionBeliefSet> moveToPotionAction = new(
                objectQuery: beliefSet => beliefSet.Player.Observation.GetComponent<Rigidbody>(),
                location: beliefSet => beliefSet.PotionPosition,
                heightOffset: 0.3f
            );

            // Action: Create an action that moves towards the enemy
            TransformPathfinderAction<RagePotionBeliefSet> moveToEnemyAction = new(
                objectQuery: beliefSet => beliefSet.Player.Observation.GetComponent<Rigidbody>(),
                location: beliefSet => beliefSet.EnemyPosition,
                heightOffset: 0.3f
            );

            // Action: Use current inventory item (health potion).
            Action<RagePotionBeliefSet> activateInventoryItem = new(b => b.InventoryObject.Observation.ActivateItem());
            // Action: Attack the enemy, when at its position
            Action<RagePotionBeliefSet> useWeaponAction = new(b => b.PlayerWeapon.Observation.UseWeapon());
            PrimitiveTactic<RagePotionBeliefSet> useWeaponTactic = new(useWeaponAction, AtEnemyPositionPredicate);
            FirstOfTactic<RagePotionBeliefSet> attackEnemyTactic = new(useWeaponTactic, moveToEnemyAction.Lift());

            // Goals: Reach the target position and use the rage potion.
            Goal<RagePotionBeliefSet> PotionPickedUpGoal = new(moveToPotionAction.Lift(), PotionPickedupPredicate);
            Goal<RagePotionBeliefSet> DamageIncreasedGoal = new(activateInventoryItem.Lift(), DamageIncreasedPredicate);
            // Goal: The enemy should take increased damage from the player
            Goal<RagePotionBeliefSet> EnemyTookIncreasedDamageGoal = new(attackEnemyTactic, EnemyTookIncreasedDamagePredicate);

            // Desire: Reach the target position and use the rage potion.
            DesireSet<RagePotionBeliefSet> desire = Combinators.Seq(PotionPickedUpGoal.Lift(), DamageIncreasedGoal.Lift(), EnemyTookIncreasedDamageGoal.Lift());

            // Create a new agent with the goal.
            BdiAgent<RagePotionBeliefSet> agent = new(beliefSet, desire);

            AplibRunner runner = new(agent);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return runner.Test();

            // Assert that the agent has completed the goal.
            Assert.IsTrue(condition: agent.Status == CompletionStatus.Success);
            yield break;

            bool PotionPickedupPredicate(RagePotionBeliefSet beliefSet)
            {
                // The potion is in the player's inventory
                bool potionExists = beliefSet.PotionExists;
                bool potionInInventory = beliefSet.InventoryObject.Observation.ContainsItem("RagePotion");
                return !potionExists && potionInInventory;
            }

            bool DamageIncreasedPredicate(RagePotionBeliefSet beliefSet)
                => beliefSet.PlayerWeapon.Observation.Damage == _startDamage + _increasedDamage;

            bool AtEnemyPositionPredicate(RagePotionBeliefSet beliefSet)
                => Vector3.Distance(beliefSet.Player.Observation.transform.position, beliefSet.EnemyPosition) < 0.5f;

            bool EnemyTookIncreasedDamagePredicate(RagePotionBeliefSet beliefSet)
            {
                int prevEnemyHealth = beliefSet.TargetDummyHealth.GetMostRecentMemory();
                int enemyHealth = beliefSet.TargetDummyHealth.Observation;
                return prevEnemyHealth - enemyHealth == _increasedDamage;
            }
        }
    }
}
