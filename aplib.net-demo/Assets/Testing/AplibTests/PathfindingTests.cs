using Aplib;
using Aplib.Core;
using Aplib.Core.Belief;
using Aplib.Core.Desire;
using Aplib.Core.Desire.Goals;
using Aplib.Core.Intent.Tactics;
using Aplib.Integrations.Unity.Actions;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MyFirstBeliefSet : BeliefSet
{
    /// <summary>
    /// The player object in the scene.
    /// </summary>
    public Belief<GameObject, GameObject> Player = new(reference: GameObject.Find("PlayerPhysics"), x => x);

    /// <summary>
    /// The target position that the player needs to move towards.
    /// </summary>
    public Belief<Transform, Vector3> TargetPosition = new(GameObject.Find("Target").transform, x => x.position);
}

public class PathfindingTests
{
    [SetUp]
    public void Setup() =>

        // Load the scene
        SceneManager.LoadScene("PathfindingTest2");

    [UnityTest]
    public IEnumerator TransformPathfindingTest()
    {
        MyFirstBeliefSet rootBeliefSet = new();

        // Action: Move the player towards the target position
        TransformPathfinderAction<MyFirstBeliefSet> transformPathfinderAction = new(
            b =>
            {
                GameObject player = b.Player;
                return player.transform;
            },
            rootBeliefSet.TargetPosition,
            0.9f
        );

        // Tactic: Move the player towards the target position
        PrimitiveTactic<MyFirstBeliefSet> moveTactic = new(transformPathfinderAction);

        // Goal: Reach the target position
        PrimitiveGoalStructure<MyFirstBeliefSet> goal = new(goal: new Goal<MyFirstBeliefSet>(moveTactic, Predicate));

        DesireSet<MyFirstBeliefSet> desire = new(goal);

        // Create a new agent with the goal
        BdiAgent<MyFirstBeliefSet> agent = new(rootBeliefSet, desire);

        AplibRunner testRunner = new(agent);

        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return testRunner.Test();

        // Assert that the player has reached the target position
        Assert.IsTrue(condition: agent.Status == CompletionStatus.Success);
        yield break;

        bool Predicate(MyFirstBeliefSet beliefSet)
        {
            // The player has reached the target position
            GameObject player = beliefSet.Player;
            Vector3 target = beliefSet.TargetPosition;

            return Vector3.Distance(player.transform.position, target) < 1f;
        }
    }
}
