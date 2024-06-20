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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static Aplib.Core.Combinators;
using Goal = Aplib.Core.Desire.Goals.Goal<Testing.AplibTests.GameplayBeliefSet>;
using Action = Aplib.Core.Intent.Actions.Action<Testing.AplibTests.GameplayBeliefSet>;
using Tactic = Aplib.Core.Intent.Tactics.Tactic<Testing.AplibTests.GameplayBeliefSet>;
using FirstOfTactic = Aplib.Core.Intent.Tactics.FirstOfTactic<Testing.AplibTests.GameplayBeliefSet>;
using PrimitiveTactic = Aplib.Core.Intent.Tactics.PrimitiveTactic<Testing.AplibTests.GameplayBeliefSet>;
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
    // - Determine which enemy to engage, IF at least one is visible
    //  - Given a list of all visible enemies and their distances to the player, determine enemy to focus X:
    //      - group by [close melee enemy, crossbow enemy, distanced melee enemy]
    //      - take group I = 0
    //      - If low on health, filter over untriggered enemies (do not attack them)
    //      - sort within group on distance (but inverted sort for crossbow enemy)
    //      - take first enemy within group, else increment I
    //
    //  - React to enemy X:
    //      - IF X is distanced melee enemy AND ((rage potion is visible AND do not possess rage) OR (healing potion is visible AND health is low)) THEN do not attack but get that item // Implies no crossbow enemy is visible
    //      - ELSE IF X is crossbow enemy AND X is has targeted THEN dodge sideways
    //      - ELSE IF X is crossbow enemy AND Bullets are visible AND bullets closer than 1/3th of enemy distance AND and low on bullets THEN go get bullets
    //      - ELSE: // attack
    //          - IF in range of enemy, AND possess rage potion AND rage not applied, THEN use rage potion
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

        /// <summary> The rigidbody of the player. </summary>
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
            referenceTo => referenceTo.ItemsObject.GetComponentsInChildren<HealthPotion>()
                .Where(item => ItemVisibleFrom(item, referenceTo.Player.EyesPosition, out _))
                .ToArray());

        public readonly Belief<ItemsAndPlayerEyesReference, RagePotion[]> VisibleRagePotions = new(
            new ItemsAndPlayerEyesReference
            {
                ItemsObject = GameObject.Find("Items"),
                Player = GameObject.Find("Player").GetComponent<PlayerLogic>()
            },
            referenceTo => referenceTo.ItemsObject.GetComponentsInChildren<RagePotion>()
                .Where(item => ItemVisibleFrom(item, referenceTo.Player.EyesPosition, out _))
                .ToArray());

        public readonly Belief<ItemsAndPlayerEyesReference, (AmmoItem item, float distance)[]> VisibleAmmo = new(
            new ItemsAndPlayerEyesReference
            {
                ItemsObject = GameObject.Find("Items"),
                Player = GameObject.Find("Player").GetComponent<PlayerLogic>()
            },
            referenceTo => referenceTo.ItemsObject.GetComponentsInChildren<AmmoItem>()
                .Select(item => ItemVisibleFrom(item, referenceTo.Player.EyesPosition, out float distance)
                    ? (item, distance) : (null, 0f))
                .Where(x => x.item != null)
                .ToArray());

        public readonly Belief<ItemsAndPlayerEyesReference, bool> AnyItemIsVisible = new(
            new ItemsAndPlayerEyesReference
            {
                ItemsObject = GameObject.Find("Items"),
                Player = GameObject.Find("Player").GetComponent<PlayerLogic>()
            },
            referenceTo => referenceTo.ItemsObject.GetComponentsInChildren<Item>()
                .Any(item => ItemVisibleFrom(item, referenceTo.Player.EyesPosition, out _)));

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


        private static bool ItemVisibleFrom(Item item, Vector3 origin, out float itemDistance)
            => IsVisibleFrom(item.transform.position, origin, ~LayerMask.GetMask("PlayerSelf", "Item"), out itemDistance);

        private static bool EnemyVisibleFrom(AbstractEnemy enemy, Vector3 origin, out float enemyDistance)
            => IsVisibleFrom(enemy.transform.position, origin, ~LayerMask.GetMask("PlayerSelf", "Enemy"), out enemyDistance);

        private static bool IsVisibleFrom(Vector3 target, Vector3 origin, LayerMask layerMask, out float enemyDistance)
        {
            enemyDistance = Vector3.Distance(origin, target);

            if (!Physics.Raycast(origin, target - origin, out _, enemyDistance, layerMask))
                return true; // Visible, nothing in the way

            enemyDistance = 0;
            return false; // Something is in the way

        }

        /// Merely here to simplify types above
        public class ItemsAndPlayerEyesReference
        {
            public GameObject ItemsObject;
            public PlayerLogic Player;
        }


        /// <summary> Determines which enemies are visible and recognizable by the player. </summary>
        public readonly
            Belief<EnemiesObjectAndPlayerEyesReference, (AbstractEnemy[] enemies, float[] distances)>
            VisibleEnemies = new(
                new EnemiesObjectAndPlayerEyesReference
                {
                    EnemiesObject = GameObject.Find("Enemies"),
                    Player = GameObject.Find("Player").GetComponent<PlayerLogic>()
                },
                referenceTo => referenceTo.EnemiesObject.GetComponentsInChildren<AbstractEnemy>()
                    .Select(enemy => EnemyVisibleFrom(enemy, referenceTo.Player.EyesPosition, out float distance)
                        ? (enemy, distance)
                        : (null, 0f))
                    .Where(x => x.enemy != null)
                    .Aggregate((new AbstractEnemy[] {}, new float[] {}), (acc, x)
                        => (acc.Item1.Append(x.enemy).ToArray(), acc.Item2.Append(x.distance).ToArray())));

        public readonly Belief<EnemiesObjectAndPlayerEyesReference, bool> AnyEnemyVisible = new(
            new EnemiesObjectAndPlayerEyesReference
            {
                EnemiesObject = GameObject.Find("Enemies"),
                Player = GameObject.Find("Player").GetComponent<PlayerLogic>()
            },
            referenceTo => referenceTo.EnemiesObject.GetComponentsInChildren<AbstractEnemy>()
                .Any(enemy => EnemyVisibleFrom(enemy, referenceTo.Player.EyesPosition, out _)));

        /// <summary>
        /// Determines which enemy to focus on, based on the visible enemies and their distances.
        /// </summary>
        public AbstractEnemy DetermineEnemyToFocus(out float distance) // TODO move this to a Belief of its own for caching
        {
            (AbstractEnemy[] enemies, float[] distances) = VisibleEnemies.Observation;
            if (enemies.Length == 0)
            {
                distance = 0;
                return null; // No enemies in sight
            }

            // Group and prioritize by [close melee enemy > crossbow enemy > distanced melee enemy]
            List<int>[] groupedEnemies = { new(), new(), new() };
            for (int i = 0; i < enemies.Length; i++)
            {
                int enemyPriority = enemies[i] switch
                {
                    MeleeEnemy when distances[i] < 5 => 0,
                    RangedEnemy                      => 1,
                    _                                => 2
                };
                groupedEnemies[enemyPriority].Add(i);
            }

            // Determine enemy to focus
            for (int i = 0; i < groupedEnemies.Length; i++)
            {
                IEnumerable<int> group = groupedEnemies[i];
                if (!group.Any()) continue;

                // If low on health, filter over untriggered enemies (do not attack them)
                if (PlayerHealthPercentage.Observation < 30)
                {
                    group = group.Where(enemyIndex => !enemies[enemyIndex].IsTriggered());
                }

                // Ranged enemies further away should be targeted first
                int enemyToFocusIndex = group
                    .OrderBy(enemyIndex => distances[enemyIndex] * (enemies[enemyIndex] is RangedEnemy ? -1 : 1))
                    .First();
                distance = distances[enemyToFocusIndex];
                return enemies[enemyToFocusIndex];
            }

            distance = 0;
            return null;
        }

        public readonly Belief<AmmoPouch, int> AmmoCount = new(
            GameObject.Find("EquipmentInventory").GetComponent<AmmoPouch>(), x => x.CurrentAmmoCount);

        public readonly Belief<MeleeWeapon, MeleeWeapon> MeleeWeapon = new(
            GameObject.Find("MeleeWeapon").GetComponent<MeleeWeapon>(), x => x);

        public readonly Belief<RangedWeapon, RangedWeapon> RangedWeapon = new(
            GameObject.Find("Crossbow").GetComponent<RangedWeapon>(), x => x);

        public readonly Belief<EquipmentInventory, EquipmentInventory> EquipmentInventory = new(
            GameObject.Find("EquipmentInventory").GetComponent<EquipmentInventory>(), x => x);

        /// Merely here to simplify types above
        public class EnemiesObjectAndPlayerEyesReference
        {
            public GameObject EnemiesObject;
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
            // InputManager.Instance.enabled = false;
            GameplayBeliefSet mainBeliefSet = new();
            const float lowHealthThreshold = 30f;

            Action mark = new(_ => Debug.Log("MARK"));
            Action mark2 = new(_ => Debug.Log("MARK2"));
            Goal markGoal = new(mark.Lift(), _ => false);
            Goal markGoal2 = new(mark2.Lift(), _ => false);
            bool smort = true;

            Action smartMark = new(_ => Debug.Log("SMART MARK"));
            Goal smartMarkGoal = new(smartMark.Lift(), _ => smort = !smort);
            bool smort2 = true;
            Action smartMark2 = new(_ => Debug.Log("SMART MARK 2"));
            Goal smartMarkGoal2 = new(smartMark2.Lift(), _ => smort2 = !smort2);


            Action rotateInventory = new(beliefSet => beliefSet.Inventory.Observation.SwitchItem());
            // TODO Assumes the item is in the inventory
            Goal equipHealingPotion = new(rotateInventory.Lift(),
                beliefSet => beliefSet.Inventory.Observation.EquippedItem is HealthPotion);
            // TODO Assumes the item is in the inventory
            PrimitiveTactic equipRagePotion = new(rotateInventory,
                beliefSet => beliefSet.Inventory.Observation.EquippedItem is not RagePotion);

            Action useEquippedItemAction = new(beliefSet =>
            {
                Debug.Log("Using item");
                beliefSet.Inventory.Observation.ActivateItem();
            });
            bool hasNotYetUsedEquippedItem = true;
            bool hasUsedEquippedItem = false;
            PrimitiveTactic useEquippedItemTactic = new(useEquippedItemAction,
                _ => hasNotYetUsedEquippedItem = !hasNotYetUsedEquippedItem);
            Goal useEquippedItemGoal = new(useEquippedItemAction.Lift(),
                _ => hasUsedEquippedItem = !hasUsedEquippedItem);

            SequentialGoalStructure restoreHealth = Seq(equipHealingPotion.Lift(), useEquippedItemGoal.Lift());
            Tactic useRagePotion = FirstOf(equipRagePotion, useEquippedItemTactic); // TODO probleem dat SEQ hier nodig was?


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
                    .OrderBy(x => Vector3.Distance(x, beliefSet.PlayerRigidBody.Observation.position)) // TODO distance is already calcualted
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
                    .Select(x => x.item.transform.position)
                    .OrderBy(x => Vector3.Distance(x, beliefSet.PlayerRigidBody.Observation.position))
                    .First(),
                0.3f);

            PrimitiveTactic pickUpVisibleHealthPotion = new(moveToVisibleHealthPotionAction,
                beliefSet => beliefSet.VisibleHealthPotions.Observation.Length != 0);
            PrimitiveTactic pickUpVisibleRagePotion = new(moveToVisibleRagePotionAction,
                beliefSet => beliefSet.VisibleRagePotions.Observation.Length != 0);
            PrimitiveTactic pickUpVisibleAmmo = new(moveToVisibleAmmoAction,
                beliefSet => beliefSet.VisibleAmmo.Observation.Length != 0);

            Tactic fetchPotionIfDistancedMelee = FirstOf(guard: beliefSet =>
                    beliefSet.DetermineEnemyToFocus(out float distance) is MeleeEnemy && distance > 5 &&
                    ((!beliefSet.Inventory.Observation.ContainsItem<RagePotion>() &&
                      beliefSet.VisibleRagePotions.Observation.Length != 0) ||
                     (beliefSet.PlayerHealthPercentage.Observation < 30 &&
                      beliefSet.VisibleHealthPotions.Observation.Length != 0)),
                pickUpVisibleHealthPotion, pickUpVisibleRagePotion);

            Action stepAsideRightAction = new(beliefSet =>
            {
                Rigidbody playerRigidBody = beliefSet.PlayerRigidBody;
                playerRigidBody.AddForce(playerRigidBody.transform.right * 10, ForceMode.VelocityChange);
            });
            Action stepAsideLeftAction = new(beliefSet =>
            {
                Rigidbody playerRigidBody = beliefSet.PlayerRigidBody;
                playerRigidBody.AddForce(playerRigidBody.transform.right * -10, ForceMode.VelocityChange);
            });
            PrimitiveTactic stepAsideLeft = new(stepAsideLeftAction, beliefSet =>
            {
                Rigidbody playerRigidBody = beliefSet.PlayerRigidBody;

                Vector3 left = playerRigidBody.transform.TransformDirection(Vector3.left);
                return Physics.Raycast(playerRigidBody.transform.position, left, 0.5f,
                    ~LayerMask.GetMask("PlayerSelf", "Item")); // Whether something is on the left, but not the player or items
            });
            PrimitiveTactic stepAsideRight = new(stepAsideRightAction, beliefSet =>
            {
                Rigidbody playerRigidBody = beliefSet.PlayerRigidBody;

                Vector3 left = playerRigidBody.transform.TransformDirection(Vector3.right);
                return Physics.Raycast(playerRigidBody.transform.position, left, 0.5f,
                    ~LayerMask.GetMask("PlayerSelf", "Item")); // Whether something is on the left, but not the player or items
            });

            Tactic dodgeCrossbow = FirstOf(guard: beliefSet =>
            {
                AbstractEnemy enemy = beliefSet.DetermineEnemyToFocus(out _);
                return enemy is RangedEnemy && enemy.IsTriggered();
            },
            stepAsideLeft, stepAsideRight);

            Tactic fetchAmmo = FirstOf(guard: beliefSet =>
            {
                AbstractEnemy enemy = beliefSet.DetermineEnemyToFocus(out float enemyDistance);

                return enemy is RangedEnemy
                       && beliefSet.AmmoCount < 4
                       && beliefSet.VisibleAmmo.Observation.Length != 0
                       && beliefSet.VisibleAmmo.Observation.Min(x => x.distance) < enemyDistance / 3;
            },
            pickUpVisibleAmmo);

            Tactic useRagePotionIfUseful = FirstOf(guard:
                beliefSet => beliefSet.DetermineEnemyToFocus(out float enemyDistance) != null
                             && enemyDistance < 5
                             && beliefSet.Inventory.Observation.ContainsItem<RagePotion>(),
                useRagePotion);

            Action aimAtEnemy = new(beliefSet =>
            {
                Transform playerTransform = beliefSet.PlayerRigidBody.Observation.transform;
                Vector3 enemyPosition = beliefSet.DetermineEnemyToFocus(out _).transform.position;
                playerTransform.LookAt(enemyPosition);
                Debug.Log($"Aiming at enemy {Time.time}");
            });
            PrimitiveTactic aimAtEnemyWhenNotAimedYet = new(aimAtEnemy, beliefSet =>
            {
                AbstractEnemy enemyToFocus = beliefSet.DetermineEnemyToFocus(out float distance);
                Transform playerTransform = beliefSet.PlayerRigidBody.Observation.transform;
                if (Physics.Raycast(playerTransform.position, playerTransform.forward, out RaycastHit hitInfo, distance,
                    LayerMask.GetMask("PlayerSelf")))
                {
                    AbstractEnemy hitEnemy = hitInfo.collider.GetComponent<AbstractEnemy>();
                    return hitEnemy != null && hitEnemy == enemyToFocus;
                }
                return false;
            });

            Action switchWeapon = new(beliefSet =>
            {
                beliefSet.EquipmentInventory.Observation.MoveNext();
                Debug.Log($"Switch weapon! {Time.time}");
            });
            PrimitiveTactic equipCrossbow = new(switchWeapon,
                beliefSet => beliefSet.EquipmentInventory.Observation.CurrentEquipment is MeleeWeapon);
            PrimitiveTactic equipBat = new(switchWeapon,
                beliefSet => beliefSet.EquipmentInventory.Observation.CurrentEquipment is RangedWeapon);

            Action shootCrossbowAction = new(beliefSet =>
            {
                beliefSet.RangedWeapon.Observation.UseWeapon();
                Debug.Log($"Shoot! {Time.time}");
            });
            Tactic shootEnemy = FirstOf(guard: beliefSet =>
            {
                AbstractEnemy enemyToFocus = beliefSet.DetermineEnemyToFocus(out float enemyDistance);
                return enemyToFocus != null
                       && beliefSet.AmmoCount > 0
                       && enemyDistance > beliefSet.MeleeWeapon.Observation.Range
                       && (enemyToFocus is RangedEnemy || enemyToFocus is MeleeEnemy && beliefSet.AmmoCount > 3);
            }, equipCrossbow, aimAtEnemyWhenNotAimedYet, shootCrossbowAction.Lift());

            Action swingBat = new(beliefSet =>
            {
                beliefSet.MeleeWeapon.Observation.UseWeapon();
                Debug.Log($"Swing! {Time.time}");
            });
            TransformPathfinderAction approachEnemyAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.DetermineEnemyToFocus(out _).transform.position,
                0.3f
            );
            PrimitiveTactic approachEnemy = new(approachEnemyAction, beliefSet =>
            {
                _ = beliefSet.DetermineEnemyToFocus(out float distance);
                return distance > beliefSet.MeleeWeapon.Observation.Range;
            });

            Tactic hitEnemy = FirstOf(equipBat, aimAtEnemyWhenNotAimedYet, approachEnemy, swingBat.Lift()); // TODO first equip bat

            Tactic reactToEnemyTactic = FirstOf(fetchPotionIfDistancedMelee, dodgeCrossbow, fetchAmmo, useRagePotionIfUseful, shootEnemy, hitEnemy);
            GoalStructure reactToEnemy = new Goal(reactToEnemyTactic, beliefSet => !beliefSet.AnyEnemyVisible);


            Tactic fetchVisibleItem = FirstOf(pickUpVisibleHealthPotion, pickUpVisibleRagePotion, pickUpVisibleAmmo);
            GoalStructure fetchVisibleItemGoal = new Goal(fetchVisibleItem, beliefSet => !beliefSet.AnyItemIsVisible);

            DesireSet desireSet = new(
                mainGoal: fetchElixir, // Fetch the elixir and bring it to the final room
                sideGoals: new (IGoalStructure, InterruptGuard)[]
                {
                    // But, when an item can be picked up, do so
                    (fetchVisibleItemGoal, beliefSet =>
                        beliefSet.AnyItemIsVisible),

                    // But, when an enemy is visible, react to it
                    (reactToEnemy, beliefSet =>
                        beliefSet.AnyEnemyVisible),

                    // But, when low on health and in possession of a healing potion, drink the potion
                    (restoreHealth, beliefSet =>
                        beliefSet.PlayerHealthPercentage.Observation < 60
                        && beliefSet.Inventory.Observation.ContainsItem<HealthPotion>()),
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
