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
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static Aplib.Core.Combinators;
using Goal = Aplib.Core.Desire.Goals.Goal<Testing.AplibTests.GameplayBeliefSet>;
using Action = Aplib.Core.Intent.Actions.Action<Testing.AplibTests.GameplayBeliefSet>;
using Tactic = Aplib.Core.Intent.Tactics.Tactic<Testing.AplibTests.GameplayBeliefSet>;
using GoalStructure = Aplib.Core.Desire.GoalStructures.GoalStructure<Testing.AplibTests.GameplayBeliefSet>;
using SequentialGoalStructure = Aplib.Core.Desire.GoalStructures.SequentialGoalStructure<Testing.AplibTests.GameplayBeliefSet>;
using DesireSet = Aplib.Core.Desire.DesireSets.DesireSet<Testing.AplibTests.GameplayBeliefSet>;
using BdiAgent = Aplib.Core.Agents.BdiAgent<Testing.AplibTests.GameplayBeliefSet>;
using TransformPathfinderAction = Aplib.Integrations.Unity.Actions.TransformPathfinderAction<Testing.AplibTests.GameplayBeliefSet>;
using IGoalStructure = Aplib.Core.Desire.GoalStructures.IGoalStructure<Testing.AplibTests.GameplayBeliefSet>;
using InterruptGuard = System.Func<Testing.AplibTests.GameplayBeliefSet, bool>;

namespace Testing.AplibTests
{
    // The realistic gameplay, ordered on priority:
    // - If health below 60% and have health potion, use it
    //
    // - Determine with which enemy to engage
    //  - Given a list of all enemies, determine enemy to focus X:
    //      - group by [close melee enemy, crossbow enemy, distanced melee enemy]
    //      - take group I = 0
    //      - filter group over which enemies are not visible, and sort within groups on distance (but inverted sort for crossbow enemy)
    //          - If low on health, filter over untriggered enemies (do not attack them)
    //      - take first enemy within group, else increment I
    //
    //  - If enemy to focus X is not null:
    //      - IF X is distanced melee enemy AND ((rage potion is visible AND do not possess rage) OR (healing potion is visible AND health is low)) THEN do not attack but get that item // Implies no crossbow enemy is visible
    //      - ELSE IF X is crossbow enemy AND X is has targeted THEN dodge sideways
    //      - ELSE IF X is crossbow enemy AND Bullets in visible AND bullets closer than 1/3th of enemy distance AND and low on bullets THEN go get bullets
    //      - ELSE: // attack
    //          - IF in range of enemy, AND possess rage potion, THEN use rage potion
    //          - ELSE Attack X with: crossbow IF possess arrows AND X is out of melee range AND (X is crossbow enemy OR (X is melee AND many bullets)), ELSE melee
    //
    // - Gather first one of [healing > rage > bullets] when visible
    //
    // - Navigate towards [next key > end item > start room]

    public class GameplayBeliefSet : BeliefSet
    {
        public const string EndItemName = "The Eternal Elixir";
        public const string HealingPotionName = "Health Potion";

        /// <summary> The inventory in the scene. </summary>
        public readonly Belief<Inventory, Inventory> Inventory =
            new(GameObject.Find("InventoryObject").GetComponent<Inventory>(), x => x);

        /// <summary> The rigidbody of the player </summary>
        public readonly Belief<Rigidbody, Rigidbody> PlayerRigidBody = new(
            GameObject.Find("Player").GetComponent<Rigidbody>(), x => x);

        public readonly Belief<HealthComponent, float> PlayerHealthPercentage = new(
            GameObject.Find("Player").GetComponent<HealthComponent>(), x => (float)x.Health / x.MaxHealth * 100);

        public readonly Belief<ItemsAndPlayerEyesReference, HealthPotion[]> VisibleHealthPotions = new(
            new ItemsAndPlayerEyesReference
            {
                ItemsObject = GameObject.Find("Items"),
                Player = GameObject.Find("Player").GetComponent<PlayerLogic>()
            },
            reference => reference.ItemsObject.GetComponentsInChildren<HealthPotion>()
                .Where(item => VisibleTo(reference.Player.EyesPosition, item.transform.position, out _))
                .ToArray());

        public readonly Belief<ItemsAndPlayerEyesReference, RagePotion[]> VisibleRagePotions = new(
            new ItemsAndPlayerEyesReference
            {
                ItemsObject = GameObject.Find("Items"),
                Player = GameObject.Find("Player").GetComponent<PlayerLogic>()
            },
            reference => reference.ItemsObject.GetComponentsInChildren<RagePotion>()
                .Where(item => VisibleTo(reference.Player.EyesPosition, item.transform.position, out _))
                .ToArray());

        public readonly Belief<ItemsAndPlayerEyesReference, AmmoItem[]> VisibleAmmo = new(
            new ItemsAndPlayerEyesReference
            {
                ItemsObject = GameObject.Find("Items"),
                Player = GameObject.Find("Player").GetComponent<PlayerLogic>()
            },
            reference => reference.ItemsObject.GetComponentsInChildren<AmmoItem>()
                .Where(item => VisibleTo(reference.Player.EyesPosition, item.transform.position, out _))
                .ToArray());

        public readonly Belief<ItemsAndPlayerEyesReference, bool> AnyItemIsVisible = new(
            new ItemsAndPlayerEyesReference
            {
                ItemsObject = GameObject.Find("Items"),
                Player = GameObject.Find("Player").GetComponent<PlayerLogic>()
            },
            beliefSet => beliefSet.ItemsObject.GetComponentsInChildren<Item>()
                .Any(item => VisibleTo(item.transform.position, beliefSet.Player.EyesPosition, out _)));

        /// <summary> The position of the next key to obtain to reach the end room. </summary>
        public readonly Belief<IEnumerable<GameObject>, Vector3> TargetKeyPosition = new(
            GameObject.FindGameObjectsWithTag("Key").Where(x => x != null), keys =>
            {
                Vector3 playerPosition = GameObject.Find("Player").GetComponent<Rigidbody>().position;
                NavMeshPath path = new();

                // Find the first key that is reachable.
                foreach (GameObject key in keys)
                {
                    NavMesh.CalculatePath(playerPosition, key.transform.position, NavMesh.AllAreas, path);
                    if (path.status is NavMeshPathStatus.PathComplete)
                        return key.transform.position;
                }

                // If no key is reachable, return Vector3.zero.
                return Vector3.zero;
            });

        /// <summary> The position to which the player must navigate in order to fetch the end item. </summary>
        public readonly Belief<GameObject, Vector3> EndItemPosition = new(
            GameObject.Find(EndItemName), x => x.transform.position,
            () => !GameObject.Find("InventoryObject").GetComponent<Inventory>().ContainsItem(EndItemName));

        /// <summary> The position of where the player started </summary>
        public readonly Belief<Transform, Vector3> WinAreaPosition = new(
            GameObject.FindWithTag("Win").transform, x => x.position);


        /// <summary>
        /// Tests if a ray can be cast from A to B without any collisions, and calculates the distance.
        /// </summary>
        /// <param name="a">Point A.</param>
        /// <param name="b">Point B.</param>
        /// <param name="distance">The distance, if no collisions were found.</param>
        /// <returns>Whether no collisions were found.</returns>
        private static bool VisibleTo(Vector3 a, Vector3 b, out float distance)
        {
            if (Physics.Raycast(a, b, out RaycastHit hitInfo))
            {
                distance = hitInfo.distance;
                return true;
            }

            distance = 0;
            return false;
        }

        /// Merely here to simplify types above
        public class ItemsAndPlayerEyesReference
        {
            public GameObject ItemsObject;
            public PlayerLogic Player;
        }
    }

    public class GameplayTests
    {
        private const string _sceneName = "GameplayTestScene";

        [SetUp]
        public void SetUp()
        {
            Debug.Log($"Starting '{nameof(GameplayTests)}'");
            SceneManager.LoadScene(_sceneName);
        }

        /// <summary>
        /// Given that the smart agent mimics realistic gameplay,
        /// When the agent is tasked to complete the game from start to end,
        /// The game must be winnable.
        /// </summary>
        /// <returns>An IEnumerator usable to iterate the test.</returns>
        [UnityTest]
        [Timeout(300000)]
        public IEnumerator RealisticGameplayCanWinTheGame()
        {
            // Arrange
            InputManager.Instance.enabled = false;
            GameplayBeliefSet mainBeliefSet = new();

            Action mark = new(_ => Debug.Log("MARK"));
            Goal markGoal = new(mark.Lift(), _ => false);


            Action rotateInventory = new(beliefSet => beliefSet.Inventory.Observation.SwitchItem());
            // TODO Assumes the item is in the inventory
            Goal equipHealingPotion = new(rotateInventory.Lift(),
                beliefSet => beliefSet.Inventory.Observation.EquippedItem is HealthPotion);
            // TODO Assumes the item is in the inventory
            Goal equipRagePotion = new(rotateInventory.Lift(),
                beliefSet => beliefSet.Inventory.Observation.EquippedItem is RagePotion);

            Action useEquippedItemAction = new(beliefSet =>
            {
                Debug.Log("Using item");
                beliefSet.Inventory.Observation.ActivateItem();
            });
            bool hasNotYetUsedEquippedItem = true;
            Goal useEquippedItem = new(useEquippedItemAction.Lift(), _ => hasNotYetUsedEquippedItem = !hasNotYetUsedEquippedItem);

            SequentialGoalStructure restoreHealth = Seq(equipHealingPotion.Lift(), useEquippedItem.Lift());
            SequentialGoalStructure useRagePotion = Seq(equipRagePotion.Lift(), useEquippedItem.Lift());


            GameObject[] keys = GameObject.FindGameObjectsWithTag("Key");
            foreach (GameObject key in keys) key.AddComponent<BeforeDestroyKey>();

            TransformPathfinderAction moveToElixir = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.EndItemPosition,
                0.3f
            );

            TransformPathfinderAction moveToNextKeyAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.TargetKeyPosition,
                0.3f
            );

            TransformPathfinderAction moveToWinAreaAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.WinAreaPosition,
                0.3f
            );

            Goal moveToElixirGoal = new(moveToElixir.Lift(), endItemPickedUpPredicate);
            Goal moveToNextKeyGoal = new(moveToNextKeyAction.Lift(), endItemReachablePredicate);
            Goal moveToWinArea = new(moveToWinAreaAction.Lift(), playerIsInWinAreaPredicate);

            GoalStructure fetchElixir = Seq(moveToNextKeyGoal.Lift(), moveToElixirGoal.Lift(), moveToWinArea.Lift());

            TransformPathfinderAction moveToVisibleHealthPotionAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.VisibleHealthPotions.Observation
                    .Select(x => x.transform.position)
                    .OrderBy(x => Vector3.Distance(x, beliefSet.PlayerRigidBody.Observation.position))
                    .First(),
                0.3f);
            TransformPathfinderAction moveToVisibleRagePotionAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.VisibleRagePotions.Observation
                    .Select(x => x.transform.position)
                    .OrderBy(x => Vector3.Distance(x, beliefSet.PlayerRigidBody.Observation.position))
                    .First(),
                0.3f);
            TransformPathfinderAction moveToVisibleAmmoAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.VisibleAmmo.Observation
                    .Select(x => x.transform.position)
                    .OrderBy(x => Vector3.Distance(x, beliefSet.PlayerRigidBody.Observation.position))
                    .First(),
                0.3f);

            Goal moveToVisibleHealthPotion = new(moveToVisibleHealthPotionAction.Lift(),
                beliefSet => beliefSet.VisibleHealthPotions.Observation.Length == 0);
            Goal moveToVisibleRagePotion = new(moveToVisibleRagePotionAction.Lift(),
                beliefSet => beliefSet.VisibleRagePotions.Observation.Length == 0);
            Goal moveToVisibleAmmo = new(moveToVisibleAmmoAction.Lift(),
                beliefSet => beliefSet.VisibleAmmo.Observation.Length == 0);


            GoalStructure fetchVisibleItem = Seq(moveToVisibleHealthPotion.Lift(), moveToVisibleRagePotion.Lift(), moveToVisibleAmmo.Lift());

            DesireSet desireSet = new(
                mainGoal: fetchElixir, // When not doing anything else, fetch the elixir and bring it to the final room
                sideGoals: new (IGoalStructure, InterruptGuard)[]
                {
                    // When low on health and in possession of a healing potion, drink the potion
                    (restoreHealth, beliefSet =>
                        beliefSet.PlayerHealthPercentage.Observation < 60
                        && beliefSet.Inventory.Observation.ContainsItem<HealthPotion>()),

                    // When an item can be picked up, do so
                    (fetchVisibleItem, beliefSet =>
                        beliefSet.AnyItemIsVisible)
                });


            // Act
            BdiAgent agent = new(mainBeliefSet, desireSet);
            AplibRunner testRunner = new(agent);

            yield return testRunner.Test();

            // Assert
            Assert.AreEqual(CompletionStatus.Success, agent.Status);
            yield break;


            #region Predicates

            bool endItemReachablePredicate(GameplayBeliefSet beliefSet)
            {
                Rigidbody playerRigidbody = beliefSet.PlayerRigidBody;
                Vector3 target = beliefSet.EndItemPosition;

                NavMeshPath path = new();
                NavMesh.CalculatePath(playerRigidbody.position, target, NavMesh.AllAreas, path);
                return path.status == NavMeshPathStatus.PathComplete;
            }

            bool endItemPickedUpPredicate(GameplayBeliefSet beliefSet)
            {
                Inventory inventory = beliefSet.Inventory;
                return inventory.ContainsItem(GameplayBeliefSet.EndItemName);
            }

            bool playerIsInWinAreaPredicate(GameplayBeliefSet beliefSet)
            {
                Vector3 playerPos = beliefSet.PlayerRigidBody.Observation.position;
                Vector3 centreOfWinArea = beliefSet.WinAreaPosition;
                return Vector3.Distance(playerPos, centreOfWinArea) < 2f;
            }

            #endregion
        }
    }
}
