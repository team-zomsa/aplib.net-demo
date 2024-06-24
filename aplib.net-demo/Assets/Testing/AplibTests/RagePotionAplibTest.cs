using Aplib.Core;
using Aplib.Core.Belief.Beliefs;
using Aplib.Core.Belief.BeliefSets;
using Aplib.Integrations.Unity;
using Assets.Scripts.Items;
using Entities.Weapons;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static Aplib.Core.Combinators;
using Goal = Aplib.Core.Desire.Goals.Goal<Testing.AplibTests.RagePotionBeliefSet>;
using Action = Aplib.Core.Intent.Actions.Action<Testing.AplibTests.RagePotionBeliefSet>;
using Tactic = Aplib.Core.Intent.Tactics.Tactic<Testing.AplibTests.RagePotionBeliefSet>;
using FirstOfTactic = Aplib.Core.Intent.Tactics.FirstOfTactic<Testing.AplibTests.RagePotionBeliefSet>;
using PrimitiveTactic = Aplib.Core.Intent.Tactics.PrimitiveTactic<Testing.AplibTests.RagePotionBeliefSet>;
using DesireSet = Aplib.Core.Desire.DesireSets.DesireSet<Testing.AplibTests.RagePotionBeliefSet>;
using BdiAgent = Aplib.Core.Agents.BdiAgent<Testing.AplibTests.RagePotionBeliefSet>;
using TransformPathfinderAction = Aplib.Integrations.Unity.Actions.TransformPathfinderAction<Testing.AplibTests.RagePotionBeliefSet>;

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

    public class RagePotionAplibTest
    {
        private int _increasedDamage;
        private int _startDamage;

        [SetUp]
        public void SetUp()
        {
            Debug.Log($"Starting test {nameof(RagePotionAplibTest)}");
            SceneManager.LoadScene("RagePotionTestScene");
        }

        /// <summary>
        /// The player will walk up to a rage potion and use it.
        /// Then check if the rage potion doubles the players damage when damaging an entity.
        /// </summary>
        [UnityTest]
        public IEnumerator PerformRagePotionAplibTest()
        {
            RagePotionBeliefSet ragePotionBeliefSet = new();

            // Calculate the expected damage increase
            int damageIncreasePercentage = ragePotionBeliefSet.RagePotion.Observation.DamageIncreasePercentage;
            _startDamage = ragePotionBeliefSet.PlayerWeapon.Observation.Damage;
            _increasedDamage = _startDamage + _startDamage * damageIncreasePercentage / 100;

            // Action: Create an action that moves towards the potion
            Tactic moveToPotion = new TransformPathfinderAction(
                objectQuery: beliefSet => beliefSet.Player.Observation.GetComponent<Rigidbody>(),
                location: beliefSet => beliefSet.PotionPosition,
                heightOffset: 0.3f
            );

            // Action: Create an action that moves towards the enemy
            Tactic moveToEnemy = new TransformPathfinderAction (
                objectQuery: beliefSet => beliefSet.Player.Observation.GetComponent<Rigidbody>(),
                location: beliefSet => beliefSet.EnemyPosition,
                heightOffset: 0.3f
            );

            // Action: Use current inventory item (health potion).
            Tactic activateInventoryItem = new Action(b => b.InventoryObject.Observation.ActivateItem());
            // Action: Attack the enemy, when at its position
            Action useWeaponAction = new(b => b.PlayerWeapon.Observation.UseWeapon());
            PrimitiveTactic useWeaponTactic = new(useWeaponAction, AtEnemyPositionPredicate);
            FirstOfTactic attackEnemyTactic = new(useWeaponTactic, moveToEnemy);

            // Goals: Reach the target position and use the rage potion.
            Goal potionPickedUpGoal = new(moveToPotion, PotionPickedUpPredicate);
            Goal damageIncreasedGoal = new(activateInventoryItem, DamageIncreasedPredicate);
            // Goal: The enemy should take increased damage from the player
            Goal enemyTookIncreasedDamageGoal = new(attackEnemyTactic, EnemyTookIncreasedDamagePredicate);

            // Desire: Reach the target position and use the rage potion.
            DesireSet desire = Seq(potionPickedUpGoal.Lift(), damageIncreasedGoal.Lift(), enemyTookIncreasedDamageGoal.Lift());

            // Create a new agent with the goal.
            BdiAgent agent = new(ragePotionBeliefSet, desire);

            AplibRunner runner = new(agent);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return runner.Test();

            // Assert that the agent has completed the goal.
            Assert.AreEqual(CompletionStatus.Success, agent.Status);
            yield break;

            bool PotionPickedUpPredicate(RagePotionBeliefSet beliefSet)
            {
                // The potion is in the player's inventory
                bool potionExists = beliefSet.PotionExists;
                bool potionInInventory = beliefSet.InventoryObject.Observation.ContainsItem("RagePotion");
                return !potionExists && potionInInventory;
            }

            bool DamageIncreasedPredicate(RagePotionBeliefSet beliefSet)
                => beliefSet.PlayerWeapon.Observation.Damage == _increasedDamage;

            bool AtEnemyPositionPredicate(RagePotionBeliefSet beliefSet)
                => Vector3.Distance(beliefSet.Player.Observation.transform.position, beliefSet.EnemyPosition) < 1f;

            bool EnemyTookIncreasedDamagePredicate(RagePotionBeliefSet beliefSet)
            {
                int prevEnemyHealth = beliefSet.TargetDummyHealth.GetMostRecentMemory();
                int enemyHealth = beliefSet.TargetDummyHealth.Observation;
                return prevEnemyHealth - enemyHealth == _increasedDamage;
            }
        }
    }
}
