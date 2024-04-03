using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Aplib.Core;
using Aplib.Core.Belief;
using Aplib.Core.Desire;
using Aplib.Core.Desire.Goals;
using Aplib.Core.Intent.Actions;
using Aplib.Core.Intent.Tactics;
using Aplib.Integrations.Unity;
using UnityEngine.SceneManagement;

public class MyFirstBeliefSet : BeliefSet
{
    /// <summary>
    /// The player object in the scene.
    /// </summary>
    public Belief<GameObject, GameObject> Player = new(GameObject.Find("Player"), x => x);

    /// <summary>
    /// The target position that the player needs to move towards.
    /// </summary>
    public Belief<Transform, Vector3> TargetPosition = new(GameObject.Find("Target").transform, x => x.position);
}

public class MyFirstAplibTest
{
    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("FirstTestScene");
    }

    /// <summary>
    /// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use `yield return null;` to skip a frame.
    /// </summary>
    [UnityTest]
    public IEnumerator MySecondAplibTestWithEnumeratorPasses()
    {
        // Arrange
        // Create a belief set for the agent.
        MyFirstBeliefSet beliefSet = new();

        // Create an intent for the agent that moves the agent towards the target position.
        Action<MyFirstBeliefSet> moveTowardsTargetAction = new(
            effect: beliefSet =>
            {
                GameObject player = beliefSet.Player;
                Vector3 playerPosition = player.transform.position;
                Vector3 targetPosition = beliefSet.TargetPosition;
                player.transform.position = Vector3.MoveTowards(playerPosition, targetPosition, Time.deltaTime);
            }
        );
        PrimitiveTactic<MyFirstBeliefSet> moveTowardsTargetTactic = new(action: moveTowardsTargetAction);

        // Create a desire for the agent to reach the target position.
        Goal<MyFirstBeliefSet> reachTargetGoal = new(
            tactic: moveTowardsTargetTactic,
            predicate: beliefSet =>
            {
                GameObject player = beliefSet.Player;
                Vector3 playerPosition = player.transform.position;
                Vector3 targetPosition = beliefSet.TargetPosition;
                return Vector3.Distance(playerPosition, targetPosition) < 0.1f;
            }
        );
        PrimitiveGoalStructure<MyFirstBeliefSet> reachTargetGoalStructure = new(goal: reachTargetGoal);
        DesireSet<MyFirstBeliefSet> desireSet = new(mainGoal: reachTargetGoalStructure);

        // Setup the agent with the belief set and desire set and initialize the test runner.
        BdiAgent<MyFirstBeliefSet> agent = new(beliefSet, desireSet);
        AplibRunner testRunner = new(agent);

        // Act
        yield return testRunner.Test();

        // Assert
        Assert.AreEqual(agent.Status, CompletionStatus.Success);
    }
}
