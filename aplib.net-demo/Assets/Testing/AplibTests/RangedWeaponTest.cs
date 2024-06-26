// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using Aplib.Core;
using Aplib.Core.Belief.Beliefs;
using Aplib.Core.Belief.BeliefSets;
using Aplib.Core.Intent.Tactics;
using Aplib.Integrations.Unity;
using Entities.Weapons;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Goal = Aplib.Core.Desire.Goals.Goal<Testing.AplibTests.RangedWeaponTestBeliefSet>;
using Action = Aplib.Core.Intent.Actions.Action<Testing.AplibTests.RangedWeaponTestBeliefSet>;
using Tactic = Aplib.Core.Intent.Tactics.Tactic<Testing.AplibTests.RangedWeaponTestBeliefSet>;
using PrimitiveTactic = Aplib.Core.Intent.Tactics.PrimitiveTactic<Testing.AplibTests.RangedWeaponTestBeliefSet>;
using GoalStructure = Aplib.Core.Desire.GoalStructures.GoalStructure<Testing.AplibTests.RangedWeaponTestBeliefSet>;
using BdiAgent = Aplib.Core.Agents.BdiAgent<Testing.AplibTests.RangedWeaponTestBeliefSet>;

namespace Testing.AplibTests
{
    public class RangedWeaponTestBeliefSet : BeliefSet
    {
        /// <summary>
        /// The player object in the scene.
        /// </summary>
        public Belief<GameObject, GameObject> Player = new(GameObject.Find("Player Ranged"), x => x);

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

        /// <summary>
        /// Bool that returns true if enemy is in front of player.
        /// </summary>
        public Belief<GameObject, bool> IsEnemyInFront = new(
            reference: GameObject.Find("Player Ranged"),
            getObservationFromReference: x =>
            {
                if (!Physics.Raycast(x.transform.position, x.transform.forward, out RaycastHit hit, 100))
                    return false;

                return hit.collider.gameObject.name == "Target Dummy Body";
            }
        );
    }

    /// <summary>
    /// Tests the ranged weapon.
    /// Simply rotates towards the enemy and shoots.
    /// Then checks if the enemy is dead.
    /// </summary>
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
            // Make beliefset instance
            RangedWeaponTestBeliefSet beliefSet = new();

            // Action: shoot in front
            Action ShootEnemyInFront = new(beliefSet =>
            {
                RangedWeapon crossbow = beliefSet.Rangedweapon;
                crossbow.UseWeapon();
            });

            // Action: rotate player
            Action RotatePlayerToEnemy = new(beliefSet =>
            {
                GameObject playerRotation = beliefSet.Player;
                Vector3 enemyPosition = beliefSet.EnemyPosition;

                // Weapon viewpoint should be set to player rotation in editor
                playerRotation.transform.LookAt(enemyPosition);
            });

            Tactic KillEnemy = new PrimitiveTactic(ShootEnemyInFront, IsEnemyInFrontPredicate);
            Tactic TurnToEnemy = new PrimitiveTactic(RotatePlayerToEnemy);
            Tactic FinalTactic = new FirstOfTactic<RangedWeaponTestBeliefSet>(KillEnemy, TurnToEnemy);

            // Goal: enemy is dead
            GoalStructure goal = new Goal(FinalTactic, EnemyKilledPredicate);

            // Create a new agent with the goal
            BdiAgent agent = new(beliefSet, goal.Lift());

            AplibRunner testRunner = new(agent);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return testRunner.Test();

            // Assert that the player has reached the target position
            Assert.IsTrue(condition: agent.Status == CompletionStatus.Success);
            yield break;

            bool EnemyKilledPredicate(RangedWeaponTestBeliefSet beliefSet) => beliefSet.IsEnemyDead;

            bool IsEnemyInFrontPredicate(RangedWeaponTestBeliefSet beliefSet) => beliefSet.IsEnemyInFront;
        }
    }
}
