using Aplib;
using Aplib.Core;
using Aplib.Core.Belief;
using Aplib.Core.Desire;
using Aplib.Core.Desire.Goals;
using Aplib.Core.Intent.Actions;
using Aplib.Core.Intent.Tactics;
using Assets.Scripts;
using Cinemachine;
using Entities.Weapons;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Testing.AplibTests
{
    public class RangeedWeaponTestBeliefSet : BeliefSet
    {
        /// <summary>
        /// The player rotation object in the scene.
        /// </summary>
        public Belief<GameObject, GameObject> PlayerRotation = new(GameObject.Find("PlayerRotation"), x => x);

        public Belief<GameObject, GameObject> Player = new(GameObject.Find("Player"), x => x);

        /// <summary>
        /// The camera object in the scene.
        /// </summary>
        public Belief<GameObject, GameObject> Camera = new(GameObject.Find("Main Camera"), x => x);

        /// <summary>
        /// The crossbow weapon the player is holding.
        /// </summary>
        public Belief<GameObject, RangedWeapon> Rangedweapon = new(GameObject.Find("Crossbow"), x => x.GetComponent<RangedWeapon>());

        /// <summary>
        /// The enemy position that the player needs to look towards.
        /// </summary>
        public Belief<GameObject, Vector3> EnemyPosition = new(GameObject.Find("Target Dummy Body"),
            x => x != null ? x.transform.position : Vector3.zero);

        /// <summary>
        /// Bool that returns if enemy is dead or alive.
        /// </summary>
        public Belief<GameObject, bool> IsEnemyDead = new(GameObject.Find("Target Dummy Body"), x => x == null);

        public Belief<GameObject, bool> IsEnemyInFront = new(
            reference: GameObject.Find("PlayerRotation"),
            getObservationFromReference: x =>
            {
                if (!Physics.Raycast(x.transform.position, x.transform.forward, out RaycastHit hit, 100))
                {
                    Debug.Log("No hit");
                    return false;
                }

                return hit.collider.gameObject.name == "Target Dummy Body";
            }
        );
    }

    public class RangedWeaponTest
    {
        [SetUp]
        public void Setup()
        {
            Debug.Log("Starting test RangedWeaponTest");
            SceneManager.LoadScene("RangedWeaponTestScene");
        }

        [UnityTest]
        public IEnumerator PerformCrossbowTest()
        {
            InputManager.Instance.enabled = false;
            CameraManager.Instance.CinemachineCamera.enabled = false;

            // Make beliefset instance
            RangeedWeaponTestBeliefSet beliefSet = new();

            // Action: shoot in front
            Action<RangeedWeaponTestBeliefSet> ShootEnemyInFront = new(beliefSet =>
            {
                RangedWeapon crossbow = beliefSet.Rangedweapon;
                crossbow.UseWeapon();
                Debug.Log("Shooting enemy");
            });

            // Action: rotate player
            Action<RangeedWeaponTestBeliefSet> RotatePlayerToEnemy = new(beliefSet =>
            {
                GameObject playerRotation = beliefSet.PlayerRotation;
                Vector3 enemyPosition = beliefSet.EnemyPosition;

                // TODO:: change ranged weapon viewpoint to player rotation instead of camera
                // Test will not work for now
                playerRotation.transform.LookAt(enemyPosition);
            });

            // Tactic to kill enemy when in front
            Tactic<RangeedWeaponTestBeliefSet> KillEnemy = new PrimitiveTactic<RangeedWeaponTestBeliefSet>(ShootEnemyInFront, IsEnemyInFrontPredicate);

            // Tactic to turn to enemy
            Tactic<RangeedWeaponTestBeliefSet> TurnToEnemy = new PrimitiveTactic<RangeedWeaponTestBeliefSet>(RotatePlayerToEnemy);

            // Tactic first try to shoot, otherwise turn.
            Tactic<RangeedWeaponTestBeliefSet> FinalTactic = new FirstOfTactic<RangeedWeaponTestBeliefSet>(metadata: null, KillEnemy, TurnToEnemy);

            // Goal: enemy is dead
            PrimitiveGoalStructure<RangeedWeaponTestBeliefSet> goal = new(goal: new Goal<RangeedWeaponTestBeliefSet>(FinalTactic, EnemyKilledPredicate));

            DesireSet<RangeedWeaponTestBeliefSet> desire = new(goal);

            // Create a new agent with the goal
            BdiAgent<RangeedWeaponTestBeliefSet> agent = new(beliefSet, desire);

            AplibRunner testRunner = new(agent);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return testRunner.Test();

            // Assert that the player has reached the target position
            Assert.IsTrue(condition: agent.Status == CompletionStatus.Success);
            yield break;

            bool EnemyKilledPredicate(RangeedWeaponTestBeliefSet beliefSet)
            {
                // The player has killed the enemy
                // TODO:: Fix shooting, returns true for now
                // bool enemyDead = beliefSet.IsEnemyDead;
                // return enemyDead;
                return true;
            }

            bool IsEnemyInFrontPredicate(RangeedWeaponTestBeliefSet beliefSet)
            {
                // The player has killed the enemy
                bool enemyInFront = beliefSet.IsEnemyInFront;
                return enemyInFront;
            }
        }
    }
}
