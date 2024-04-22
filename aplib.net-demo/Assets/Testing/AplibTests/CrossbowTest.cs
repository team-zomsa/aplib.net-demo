using Aplib;
using Aplib.Core;
using Aplib.Core.Belief;
using Aplib.Core.Desire;
using Aplib.Core.Desire.Goals;
using Aplib.Core.Intent.Actions;
using Aplib.Core.Intent.Tactics;
using Assets.Scripts;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class CrossbowTestBeliefSet : BeliefSet
{
    /// <summary>
    /// The player object in the scene.
    /// </summary>
    public Belief<GameObject, GameObject> Player = new(reference: GameObject.Find("PlayerRotation"), x => x);

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
                return false;
            }

            return hit.collider.gameObject.name == "Target Dummy Body";
        }
    );
}

public class CrossbowTest : InputTestFixture
{
    Mouse _mouse;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        SceneManager.LoadScene("RangedWeaponTestScene");
    }

    [TearDown]
    public override void TearDown()
    {
        InputSystem.RemoveDevice(_mouse);
        Object.Destroy(InputManager.Instance);
    }

    [UnityTest]
    public IEnumerator PerformCrossbowTest()
    {
        // Make beliefset instance
        CrossbowTestBeliefSet beliefSet = new();

        // Add mouse
        _mouse = InputSystem.AddDevice<Mouse>();

        // Action: shoot in front | might not work
        Action<CrossbowTestBeliefSet> ShootEnemyInFront = new(beliefSet => Click(_mouse.leftButton));

        // Action: rotate player
        Action<CrossbowTestBeliefSet> RotatePlayerToEnemy = new(beliefSet =>
        {
            GameObject player = beliefSet.Player;
            Vector3 enemyPosition = beliefSet.EnemyPosition;

            // Get desired direction the player needs to lookat.
            Vector3 lookAtDirection = enemyPosition - player.transform.position;

            lookAtDirection.Normalize();

            // Let the player look towards the direction with the mouse
            float horizontalSpeed = 10f;
            float multiplier = 5f;

            // Get the angle between the player's forward direction and the direction to the target position
            float angle = Vector3.SignedAngle(player.transform.forward, lookAtDirection, Vector3.up);

            Set(_mouse.delta, state: new Vector2(angle * multiplier / horizontalSpeed, 0));
        });

        // Tactic to kill enemy when in front
        Tactic<CrossbowTestBeliefSet> KillEnemy = new PrimitiveTactic<CrossbowTestBeliefSet>(ShootEnemyInFront, x => x.IsEnemyInFront);

        // Tactic to turn to enemy
        Tactic<CrossbowTestBeliefSet> TurnToEnemy = new PrimitiveTactic<CrossbowTestBeliefSet>(RotatePlayerToEnemy);

        // Tactic first try to shoot, otherwise turn.
        Tactic<CrossbowTestBeliefSet> UltraTactic = new FirstOfTactic<CrossbowTestBeliefSet>(metadata: null, KillEnemy, TurnToEnemy);

        // Goal: enemy is dead
        PrimitiveGoalStructure<CrossbowTestBeliefSet> goal = new(goal: new Goal<CrossbowTestBeliefSet>(UltraTactic, Predicate));

        DesireSet<CrossbowTestBeliefSet> desire = new(goal);

        // Create a new agent with the goal
        BdiAgent<CrossbowTestBeliefSet> agent = new(beliefSet, desire);

        AplibRunner testRunner = new(agent);

        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return testRunner.Test();

        // Assert that the player has reached the target position
        Assert.IsTrue(condition: agent.Status == CompletionStatus.Success);
        yield break;

        bool Predicate(CrossbowTestBeliefSet beliefSet)
        {
            Set(_mouse.delta, Vector2.zero);

            // The player has killed the enemy
            return beliefSet.IsEnemyDead;
        }
    }
}
