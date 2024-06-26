// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using Aplib.Core;
using Aplib.Core.Belief.Beliefs;
using Aplib.Core.Belief.BeliefSets;
using Aplib.Integrations.Unity;
using Assets.Scripts.Items;
using Entities.Weapons;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static Aplib.Core.Combinators;
using Action = Aplib.Core.Intent.Actions.Action<Testing.AplibTests.GameplayBeliefSet>;
using BdiAgent = Aplib.Core.Agents.BdiAgent<Testing.AplibTests.GameplayBeliefSet>;
using DesireSet = Aplib.Core.Desire.DesireSets.DesireSet<Testing.AplibTests.GameplayBeliefSet>;
using Goal = Aplib.Core.Desire.Goals.Goal<Testing.AplibTests.GameplayBeliefSet>;
using GoalStructure = Aplib.Core.Desire.GoalStructures.GoalStructure<Testing.AplibTests.GameplayBeliefSet>;
using IGoalStructure = Aplib.Core.Desire.GoalStructures.IGoalStructure<Testing.AplibTests.GameplayBeliefSet>;
using InterruptGuard = System.Func<Testing.AplibTests.GameplayBeliefSet, bool>;
using PrimitiveTactic = Aplib.Core.Intent.Tactics.PrimitiveTactic<Testing.AplibTests.GameplayBeliefSet>;
using SequentialGoalStructure = Aplib.Core.Desire.GoalStructures.SequentialGoalStructure<Testing.AplibTests.GameplayBeliefSet>;
using Tactic = Aplib.Core.Intent.Tactics.Tactic<Testing.AplibTests.GameplayBeliefSet>;
using TransformPathfinderAction = Aplib.Integrations.Unity.Actions.TransformPathfinderAction<Testing.AplibTests.GameplayBeliefSet>;

namespace Testing.AplibTests
{
    // The realistic gameplay, ordered on priority (not fully implemented yet):
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

        /// <summary> The inventory in the scene. </summary>
        public readonly Belief<Inventory, Inventory> Inventory =
            new(GameObject.Find("InventoryObject").GetComponent<Inventory>(), x => x);

        /// <summary> The rigidbody of the player. </summary>
        public readonly Belief<Rigidbody, Rigidbody> PlayerRigidBody = new(
            GameObject.Find("Player").GetComponent<Rigidbody>(), x => x);

        public readonly Belief<GameObject, GameObject> PlayerRotation = new(
            GameObject.Find("PlayerRotation"), x => x);

        #region fetchElixir

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

        #endregion

        #region fetchVisibleItem

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

        public readonly Belief<ItemsAndPlayerEyesReference, (Key item, float distance)[]> VisibleKeys = new(
            new ItemsAndPlayerEyesReference
            {
                ItemsObject = GameObject.Find("LevelGeneration"),
                Player = GameObject.Find("Player").GetComponent<PlayerLogic>()
            },
            referenceTo => referenceTo.ItemsObject.GetComponentsInChildren<Key>()
                .Select(item => ItemVisibleFrom(item, referenceTo.Player.EyesPosition, out float distance)
                    ? (item, distance) : (null, 0f))
                .Where(x => x.item != null)
                .ToArray());

        public readonly Belief<ItemsAndPlayerEyesReference, bool> AnyItemIsVisible = new(
            new ItemsAndPlayerEyesReference
            {
                ItemsObject = GameObject.Find("LevelGeneration"),
                Player = GameObject.Find("Player").GetComponent<PlayerLogic>()
            },
            referenceTo => referenceTo.ItemsObject.GetComponentsInChildren<Item>()
                .Any(item => ItemVisibleFrom(item, referenceTo.Player.EyesPosition, out _)));

        /// <summary> The position to which the player must navigate in order to fetch the end item. </summary>
        public readonly Belief<GameObject, Vector3> EndItemPosition = new(
            GameObject.Find(EndItemName), x => x.transform.position,
            _ => !GameObject.Find("InventoryObject").GetComponent<Inventory>().ContainsItem(EndItemName));

        /// <summary> The position of where the player started </summary>
        public readonly Belief<Transform, Vector3> WinAreaPosition = new(
            GameObject.FindWithTag("Win").transform, x => x.position);

        #endregion

        #region reactToEnemy

        public readonly Belief<HealthComponent, float> PlayerHealthPercentage = new(
            GameObject.Find("Player").GetComponent<HealthComponent>(), x => (float)x.Health / x.MaxHealth * 100);

        public readonly Belief<AmmoPouch, int> AmmoCount = new(
            GameObject.Find("EquipmentInventory").GetComponent<AmmoPouch>(), x => x.CurrentAmmoCount);

        public readonly Belief<MeleeWeapon, MeleeWeapon> MeleeWeapon = new(
            GameObject.Find("Player").GetComponentInChildren<MeleeWeapon>(true), x => x);

        public readonly Belief<RangedWeapon, RangedWeapon> RangedWeapon = new(
            GameObject.Find("Player").GetComponentInChildren<RangedWeapon>(true), x => x);

        public readonly Belief<EquipmentInventory, EquipmentInventory> EquipmentInventory = new(
            GameObject.Find("EquipmentInventory").GetComponent<EquipmentInventory>(), x => x);

        public readonly Belief<Animator, Animator> Animator = new(
            GameObject.Find("Player").GetComponent<Animator>(), x =>
            {
                x.SetFloat("PlayerVelocity", 3f);
                x.SetBool("PlayerGrounded", true);
                return x;
                // \(^.^)/ manually setting the animator to walk
            });

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
                    .Aggregate((new AbstractEnemy[] { }, new float[] { }), (acc, x)
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
        public AbstractEnemy DetermineEnemyToFocus(out float distance)
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
                    RangedEnemy => 1,
                    _ => 2
                };
                groupedEnemies[enemyPriority].Add(i);
            }

            // Determine enemy to focus
            for (int i = 0; i < groupedEnemies.Length; i++)
            {
                IEnumerable<int> group = groupedEnemies[i];

                // If low on health, filter over untriggered enemies (do not attack them)
                if (PlayerHealthPercentage.Observation < 30)
                {
                    group = group.Where(enemyIndex => !enemies[enemyIndex].IsTriggered());
                }

                int[] groupArray = group.ToArray();

                if (!groupArray.Any()) continue;

                // Ranged enemies further away should be targeted first
                int enemyToFocusIndex = groupArray
                    .OrderBy(enemyIndex => distances[enemyIndex] * (enemies[enemyIndex] is RangedEnemy ? -1 : 1))
                    .First();
                distance = distances[enemyToFocusIndex];
                return enemies[enemyToFocusIndex];
            }

            distance = 0;
            return null;
        }

        #endregion

        #region HelperMethodsAndTypes

        private static bool ItemVisibleFrom(Item item, Vector3 origin, out float itemDistance)
            // Tests if vision has been obstructed towards the item, where the Player, Item and Ignore Raycast layers do not count to being obstructed
            => IsVisibleFrom(item.transform.position, origin, ~LayerMask.GetMask("PlayerSelf", "Item", "Ignore Raycast"), out itemDistance);

        private static bool EnemyVisibleFrom(AbstractEnemy enemy, Vector3 origin, out float enemyDistance)
            // Tests if vision has been obstructed towards the enemy, where the Player, Enemy and Ignore Raycast layers do not count to being obstructed
            => IsVisibleFrom(enemy.transform.position, origin, ~LayerMask.GetMask("PlayerSelf", "Enemy", "Ignore Raycast"), out enemyDistance);

        private static bool IsVisibleFrom(Vector3 target, Vector3 origin, LayerMask layerMask, out float enemyDistance)
        {
            enemyDistance = Vector3.Distance(origin, target);

            if (!Physics.Raycast(origin, target - origin, out _, enemyDistance, layerMask))
                return true; // Visible, nothing in the way

            enemyDistance = 0;
            return false; // Something is in the way
        }

        /// Merely here to simplify types above
        public class EnemiesObjectAndPlayerEyesReference
        {
            public GameObject EnemiesObject;
            public PlayerLogic Player;
        }

        /// Merely here to simplify types above
        public class ItemsAndPlayerEyesReference
        {
            public GameObject ItemsObject;
            public PlayerLogic Player;
        }

        #endregion
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
        /// Given that the smart agent mimics godlike gameplay,
        /// When the agent is tasked to complete the game from start to end,
        /// The game must be winnable.
        /// </summary>
        /// <returns>An IEnumerator usable to iterate the test.</returns>
        [UnityTest]
        [Timeout(300000)]
        public IEnumerator GodlikeGameplayCanWinTheGame()
        {
            GameplayBeliefSet mainBeliefSet = new();

            #region fetchElixir

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
                return Vector3.Distance(playerPos, centreOfWinArea) < 10f; // 10 is approximately the radius of the room
            }

            #endregion

            #endregion

            #region fetchVisibleItem

            Action rotateInventory = new(beliefSet => beliefSet.Inventory.Observation.SwitchItem());
            Goal equipHealingPotion = new(rotateInventory.Lift(),
                beliefSet => beliefSet.Inventory.Observation.EquippedItem is HealthPotion);
            PrimitiveTactic equipRagePotion = new(rotateInventory,
                beliefSet => beliefSet.Inventory.Observation.EquippedItem is not RagePotion);

            Action useEquippedItemAction = new(beliefSet => beliefSet.Inventory.Observation.ActivateItem());
            bool hasNotYetUsedEquippedItem = true;
            bool hasUsedEquippedItem = false;
            PrimitiveTactic useEquippedItemTactic = new(useEquippedItemAction,
                _ => hasNotYetUsedEquippedItem = !hasNotYetUsedEquippedItem);
            Goal useEquippedItemGoal = new(useEquippedItemAction.Lift(),
                _ => hasUsedEquippedItem = !hasUsedEquippedItem);

            SequentialGoalStructure restoreHealth = Seq(equipHealingPotion.Lift(), useEquippedItemGoal.Lift());
            Tactic useRagePotion = FirstOf(equipRagePotion, useEquippedItemTactic);


            TransformPathfinderAction moveToVisibleHealthPotionAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.VisibleHealthPotions.Observation
                    .Select(x => x.transform.position)
                    .OrderBy(x => Vector3.Distance(x, beliefSet.PlayerRigidBody.Observation.position)) // TODO distance is already calculated
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
            TransformPathfinderAction moveToVisibleKeyAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.VisibleKeys.Observation
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
            PrimitiveTactic pickUpVisibleKey = new(moveToVisibleKeyAction,
                beliefSet => beliefSet.VisibleKeys.Observation.Length != 0);

            Tactic fetchPotionIfDistancedMelee = FirstOf(guard: beliefSet =>
                    beliefSet.DetermineEnemyToFocus(out float distance) is MeleeEnemy && distance > 5 &&
                    ((!beliefSet.Inventory.Observation.ContainsItem<RagePotion>() &&
                      beliefSet.VisibleRagePotions.Observation.Length != 0) ||
                     (beliefSet.PlayerHealthPercentage.Observation < 30 &&
                      beliefSet.VisibleHealthPotions.Observation.Length != 0)),
                pickUpVisibleHealthPotion, pickUpVisibleRagePotion);

            Tactic fetchVisibleItem = FirstOf(pickUpVisibleHealthPotion, pickUpVisibleRagePotion, pickUpVisibleAmmo, pickUpVisibleKey);
            GoalStructure fetchVisibleItemGoal = new Goal(fetchVisibleItem, beliefSet => !beliefSet.AnyItemIsVisible);

            #endregion

            #region reactToEnemy

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
                    // Tests if anything towards the left can collide within 0.5m. Do not collide with said layermasks
                    ~LayerMask.GetMask("PlayerSelf", "Item", "Ignore Raycast"));
            });
            PrimitiveTactic stepAsideRight = new(stepAsideRightAction, beliefSet =>
            {
                Rigidbody playerRigidBody = beliefSet.PlayerRigidBody;

                Vector3 left = playerRigidBody.transform.TransformDirection(Vector3.right);
                return Physics.Raycast(playerRigidBody.transform.position, left, 0.5f,
                    // Tests if anything towards the right can collide within 0.5m. Do not collide with said layermasks
                    ~LayerMask.GetMask("PlayerSelf", "Item", "Ignore Raycast"));
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
                             && enemyDistance < 9
                             && beliefSet.Inventory.Observation.ContainsItem<RagePotion>(),
                useRagePotion);

            Action aimAndHitEnemyAction = new(beliefSet =>
            {
                Transform playerRotation = beliefSet.PlayerRotation.Observation.transform;
                Vector3 enemyPosition = beliefSet.DetermineEnemyToFocus(out _).transform.position;
                playerRotation.transform.LookAt(enemyPosition); // Weapon viewpoint should be set to player rotation in editor
                beliefSet.MeleeWeapon.Observation.UseWeapon();
                beliefSet.Animator.Observation.SetTrigger("PlayerAttack");
            });

            Action switchWeapon = new(beliefSet => beliefSet.EquipmentInventory.Observation.MoveNext());
            PrimitiveTactic equipCrossbow = new(switchWeapon,
                beliefSet => beliefSet.EquipmentInventory.Observation.CurrentEquipment is not RangedWeapon);
            PrimitiveTactic equipBat = new(switchWeapon,
                beliefSet => beliefSet.EquipmentInventory.Observation.CurrentEquipment is not MeleeWeapon);

            TransformPathfinderAction approachEnemyAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.DetermineEnemyToFocus(out _).transform.position,
                0.3f
            );
            PrimitiveTactic approachEnemyToShoot = new(approachEnemyAction, beliefSet =>
            {
                _ = beliefSet.DetermineEnemyToFocus(out float distance);
                return distance > beliefSet.RangedWeapon.Observation.Range;
            });
            Action aimAndShootCrossbowAction = new(beliefSet =>
            {
                Transform playerRotation = beliefSet.PlayerRotation.Observation.transform;
                Vector3 enemyPosition = beliefSet.DetermineEnemyToFocus(out _).transform.position;
                playerRotation.transform.LookAt(enemyPosition); // Weapon viewpoint should be set to player rotation in editor
                beliefSet.RangedWeapon.Observation.UseWeapon();
                beliefSet.Animator.Observation.SetTrigger("PlayerAttack");
            });
            Tactic shootEnemy = FirstOf(guard: beliefSet =>
            {
                AbstractEnemy enemyToFocus = beliefSet.DetermineEnemyToFocus(out float enemyDistance);
                return enemyToFocus != null
                       && beliefSet.AmmoCount > 0
                       && enemyDistance > beliefSet.MeleeWeapon.Observation.Range
                       && (enemyToFocus is RangedEnemy || enemyToFocus is MeleeEnemy && beliefSet.AmmoCount > 3);
            }, equipCrossbow, approachEnemyToShoot, aimAndShootCrossbowAction.Lift());

            PrimitiveTactic approachEnemyToMelee = new(approachEnemyAction, beliefSet =>
            {
                _ = beliefSet.DetermineEnemyToFocus(out float distance);
                return distance > beliefSet.MeleeWeapon.Observation.Range;
            });

            Tactic hitEnemy = FirstOf(equipBat, approachEnemyToMelee, aimAndHitEnemyAction.Lift());

            Tactic reactToEnemyTactic = FirstOf(fetchPotionIfDistancedMelee, dodgeCrossbow, fetchAmmo, useRagePotionIfUseful, shootEnemy, hitEnemy);
            GoalStructure reactToEnemy = new Goal(reactToEnemyTactic, beliefSet => !beliefSet.AnyEnemyVisible || beliefSet.DetermineEnemyToFocus(out _) == null);

            #endregion


            DesireSet desireSet = new(
                mainGoal: fetchElixir, // Fetch the elixir and bring it to the final room
                sideGoals: new (IGoalStructure, System.Predicate<GameplayBeliefSet>)[]
                {
                    // But, when an item can be picked up, do so
                    (fetchVisibleItemGoal, beliefSet =>
                        beliefSet.AnyItemIsVisible),

                    // But, when an enemy is visible, react to it
                    (reactToEnemy, beliefSet =>
                        beliefSet.AnyEnemyVisible && beliefSet.DetermineEnemyToFocus(out _) != null),

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
            GameplayBeliefSet mainBeliefSet = new();

            #region fetchElixir

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
                return Vector3.Distance(playerPos, centreOfWinArea) < 10f; // 10 is approximately the radius of the room
            }

            #endregion

            #endregion

            #region fetchVisibleItem

            Action rotateInventory = new(beliefSet => beliefSet.Inventory.Observation.SwitchItem());
            Goal equipHealingPotion = new(rotateInventory.Lift(),
                beliefSet => beliefSet.Inventory.Observation.EquippedItem is HealthPotion);
            PrimitiveTactic equipRagePotion = new(rotateInventory,
                beliefSet => beliefSet.Inventory.Observation.EquippedItem is not RagePotion);

            Action useEquippedItemAction = new(beliefSet => beliefSet.Inventory.Observation.ActivateItem());
            bool hasNotYetUsedEquippedItem = true;
            bool hasUsedEquippedItem = false;
            PrimitiveTactic useEquippedItemTactic = new(useEquippedItemAction,
                _ => hasNotYetUsedEquippedItem = !hasNotYetUsedEquippedItem);
            Goal useEquippedItemGoal = new(useEquippedItemAction.Lift(),
                _ => hasUsedEquippedItem = !hasUsedEquippedItem);

            SequentialGoalStructure restoreHealth = Seq(equipHealingPotion.Lift(), useEquippedItemGoal.Lift());
            Tactic useRagePotion = FirstOf(equipRagePotion, useEquippedItemTactic);


            TransformPathfinderAction moveToVisibleHealthPotionAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.VisibleHealthPotions.Observation
                    .Select(x => x.transform.position)
                    .OrderBy(x => Vector3.Distance(x, beliefSet.PlayerRigidBody.Observation.position)) // TODO distance is already calculated
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
            TransformPathfinderAction moveToVisibleKeyAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.VisibleKeys.Observation
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
            PrimitiveTactic pickUpVisibleKey = new(moveToVisibleKeyAction,
                beliefSet => beliefSet.VisibleKeys.Observation.Length != 0);

            Tactic fetchPotionIfDistancedMelee = FirstOf(guard: beliefSet =>
                    beliefSet.DetermineEnemyToFocus(out float distance) is MeleeEnemy && distance > 5 &&
                    ((!beliefSet.Inventory.Observation.ContainsItem<RagePotion>() &&
                      beliefSet.VisibleRagePotions.Observation.Length != 0) ||
                     (beliefSet.PlayerHealthPercentage.Observation < 30 &&
                      beliefSet.VisibleHealthPotions.Observation.Length != 0)),
                pickUpVisibleHealthPotion, pickUpVisibleRagePotion);

            Tactic fetchVisibleItem = FirstOf(pickUpVisibleHealthPotion, pickUpVisibleRagePotion, pickUpVisibleAmmo, pickUpVisibleKey);
            GoalStructure fetchVisibleItemGoal = new Goal(fetchVisibleItem, beliefSet => !beliefSet.AnyItemIsVisible);

            #endregion

            #region reactToEnemy

            float wakeUpTimeStamp = -1;
            const float sleepDuration = 0.3f; // seconds
            Action sleepAction = new(beliefSet =>
            {
                Transform playerRotation = beliefSet.PlayerRotation.Observation.transform;
                Vector3 enemyPosition = beliefSet.DetermineEnemyToFocus(out _).transform.position;
                playerRotation.transform.LookAt(enemyPosition); // Weapon viewpoint should be set to player rotation in editor
            });
            PrimitiveTactic sleep = new(sleepAction, _ => Time.time < wakeUpTimeStamp);


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
                    // Tests if anything towards the left can collide within 0.5m. Do not collide with said layermasks
                    ~LayerMask.GetMask("PlayerSelf", "Item", "Ignore Raycast"));
            });
            PrimitiveTactic stepAsideRight = new(stepAsideRightAction, beliefSet =>
            {
                Rigidbody playerRigidBody = beliefSet.PlayerRigidBody;

                Vector3 left = playerRigidBody.transform.TransformDirection(Vector3.right);
                return Physics.Raycast(playerRigidBody.transform.position, left, 0.5f,
                    // Tests if anything towards the right can collide within 0.5m. Do not collide with said layermasks
                    ~LayerMask.GetMask("PlayerSelf", "Item", "Ignore Raycast"));
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
                             && enemyDistance < 9
                             && beliefSet.Inventory.Observation.ContainsItem<RagePotion>(),
                useRagePotion);

            Action aimAndHitEnemyAction = new(beliefSet =>
            {
                Transform playerRotation = beliefSet.PlayerRotation.Observation.transform;
                Vector3 enemyPosition = beliefSet.DetermineEnemyToFocus(out _).transform.position;
                playerRotation.transform.LookAt(enemyPosition); // Weapon viewpoint should be set to player rotation in editor
                beliefSet.MeleeWeapon.Observation.UseWeapon();
                wakeUpTimeStamp = Time.time + sleepDuration;
                beliefSet.Animator.Observation.SetTrigger("PlayerAttack");
            });

            Action switchWeapon = new(beliefSet => beliefSet.EquipmentInventory.Observation.MoveNext());
            PrimitiveTactic equipCrossbow = new(switchWeapon,
                beliefSet => beliefSet.EquipmentInventory.Observation.CurrentEquipment is not RangedWeapon);
            PrimitiveTactic equipBat = new(switchWeapon,
                beliefSet => beliefSet.EquipmentInventory.Observation.CurrentEquipment is not MeleeWeapon);

            TransformPathfinderAction approachEnemyAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.DetermineEnemyToFocus(out _).transform.position,
                0.3f
            );
            PrimitiveTactic approachEnemyToShoot = new(approachEnemyAction, beliefSet =>
            {
                _ = beliefSet.DetermineEnemyToFocus(out float distance);
                return distance > beliefSet.RangedWeapon.Observation.Range;
            });
            Action aimAndShootCrossbowAction = new(beliefSet =>
            {
                Transform playerRotation = beliefSet.PlayerRotation.Observation.transform;
                Vector3 enemyPosition = beliefSet.DetermineEnemyToFocus(out _).transform.position;
                playerRotation.transform.LookAt(enemyPosition); // Weapon viewpoint should be set to player rotation in editor
                beliefSet.RangedWeapon.Observation.UseWeapon();
                wakeUpTimeStamp = Time.time + sleepDuration;
                beliefSet.Animator.Observation.SetTrigger("PlayerAttack");
            });
            Tactic shootEnemy = FirstOf(guard: beliefSet =>
            {
                AbstractEnemy enemyToFocus = beliefSet.DetermineEnemyToFocus(out float enemyDistance);
                return enemyToFocus != null
                       && beliefSet.AmmoCount > 0
                       && enemyDistance > beliefSet.MeleeWeapon.Observation.Range
                       && (enemyToFocus is RangedEnemy || enemyToFocus is MeleeEnemy && beliefSet.AmmoCount > 3);
            }, equipCrossbow, approachEnemyToShoot, aimAndShootCrossbowAction.Lift());

            PrimitiveTactic approachEnemyToMelee = new(approachEnemyAction, beliefSet =>
            {
                _ = beliefSet.DetermineEnemyToFocus(out float distance);
                return distance > beliefSet.MeleeWeapon.Observation.Range;
            });

            Tactic hitEnemy = FirstOf(equipBat, approachEnemyToMelee, aimAndHitEnemyAction.Lift());

            Tactic reactToEnemyTactic = FirstOf(sleep, fetchPotionIfDistancedMelee, dodgeCrossbow, fetchAmmo, useRagePotionIfUseful, shootEnemy, hitEnemy);
            GoalStructure reactToEnemy = new Goal(reactToEnemyTactic, beliefSet => !beliefSet.AnyEnemyVisible || beliefSet.DetermineEnemyToFocus(out _) == null);

            #endregion


            DesireSet desireSet = new(
                mainGoal: fetchElixir, // Fetch the elixir and bring it to the final room
                sideGoals: new (IGoalStructure, System.Predicate<GameplayBeliefSet>)[]
                {
                    // But, when an item can be picked up, do so
                    (fetchVisibleItemGoal, beliefSet =>
                        beliefSet.AnyItemIsVisible),

                    // But, when an enemy is visible, react to it
                    (reactToEnemy, beliefSet =>
                        beliefSet.AnyEnemyVisible && beliefSet.DetermineEnemyToFocus(out _) != null),

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
        }
    }
}
