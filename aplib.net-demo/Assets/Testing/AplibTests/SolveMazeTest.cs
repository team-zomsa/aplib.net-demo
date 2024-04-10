using System.Collections;
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

public class SolveMazeBeliefSet : BeliefSet
{
    /// <summary>
    /// The player object in the scene.
    /// </summary>
    public Belief<GameObject, GameObject> Player = new(GameObject.Find("Player"), x => x);

    /// <summary>
    /// Whether or not an obstacle is in front of the player, preventing it to move forward.
    /// </summary>
    public Belief<Transform, bool> IsObstacleInFront = new
        (
            reference: GameObject.Find("Player").transform,
            getObservationFromReference: playerTransform =>
            {
                // Determine the eye position of the player, since the player transform is at the feet of the player.
                // Also determine the direction that the player should look in.
                Vector3 playerPosition = playerTransform.position;
                Vector3 eyePosition = playerPosition + new Vector3(0, 1.75f, 0);
                Vector3 lookFrontDirection = playerPosition - eyePosition + playerTransform.forward;
                Vector3 lookRightDirection = playerPosition - eyePosition + (playerTransform.forward + 0.5f * playerTransform.right);

                // Shoot two raycasts from the eye of the player. We shoot two so that the player has some margin to move.
                bool isHitInFront = Physics.Raycast(eyePosition, lookFrontDirection, out RaycastHit hitFront, 4f);
                bool isHitToRight = Physics.Raycast(eyePosition, lookRightDirection, out RaycastHit hitRight, 4f);

                Debug.DrawRay(eyePosition, lookFrontDirection, Color.red);
                Debug.DrawRay(eyePosition, lookRightDirection, Color.red);

                // Check if there is a hole in front of the player.
                bool isHoleInFront = !isHitInFront && !isHitToRight;

                // Check if the raycast actually hit a wall or just the floor.
                bool isWallHitInFront = isHitInFront && hitFront.point.y > playerPosition.y + Mathf.Epsilon;
                bool isWallHitToRight = isHitToRight && hitRight.point.y > playerPosition.y + Mathf.Epsilon;
                bool isWallInFront = isWallHitInFront || isWallHitToRight;

                Debug.Log($"IsHoleInFront: {isHoleInFront}, IsWallInFront: {isWallInFront}");

                // Return whether there is any obstacle in front of the player.
                return isHoleInFront || isWallInFront;
            }
        );

    /// <summary>
    /// Whether or not an obstacle is to the right of the player, preventing it to turn right.
    /// </summary>
    public Belief<Transform, bool> IsObstacleToRight = new
        (
            reference: GameObject.Find("Player").transform,
            getObservationFromReference: playerTransform =>
            {
                // Determine the eye position of the player, since the player transform is at the feet of the player.
                // Also determine the direction that the player should look in.
                Vector3 playerPosition = playerTransform.position;
                Vector3 eyePosition = playerPosition + new Vector3(0, 1.75f, 0);
                Vector3 lookFrontDirection = playerPosition - eyePosition + (playerTransform.forward + playerTransform.right);
                Vector3 lookRightDirection = playerPosition - eyePosition + playerTransform.right;

                // Shoot two raycasts from the eye of the player.
                bool isHitInFront = Physics.Raycast(eyePosition, lookFrontDirection, out RaycastHit hitFront, 4f);
                bool isHitToRight = Physics.Raycast(eyePosition, lookRightDirection, out RaycastHit hitRight, 4f);

                Debug.DrawRay(eyePosition, lookFrontDirection, Color.blue);
                Debug.DrawRay(eyePosition, lookRightDirection, Color.blue);

                // Check if there is a hole to the right of the player.
                bool isHoleToRight = !isHitInFront || !isHitToRight;

                // Check if the raycast actually hit a wall or just the floor.
                bool isWallHitInFront = isHitInFront && hitFront.point.y > playerPosition.y + Mathf.Epsilon;
                bool isWallHitToRight = isHitToRight && hitRight.point.y > playerPosition.y + Mathf.Epsilon;
                bool isWallToRight = isWallHitInFront || isWallHitToRight;

                // Return whether there is any obstacle to the right of the player.
                return isHoleToRight || isWallToRight;
            }
        );

    /// <summary>
    /// The name of the current room that the player is in.
    /// </summary>
    public Belief<Transform, string> CurrentRoom = new
        (
            reference: GameObject.Find("Player").transform,
            getObservationFromReference: playerTransform =>
            {
                bool isHit = Physics.Raycast(playerTransform.position - new Vector3(0, Mathf.Epsilon, 0), -playerTransform.up, out RaycastHit hitRight, 4f);

                if (isHit) return hitRight.collider.name;
                else return "None";
            }
        );
}

public class SolveMazeTest
{
    [SetUp]
    public void SetUp()
    {
        Debug.Log("Starting test SolveMazeTest");
        SceneManager.LoadScene("SolveMazeTestScene");
    }

    /// <summary>
    /// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use `yield return null;` to skip a frame.
    /// </summary>
    [UnityTest]
    public IEnumerator PerformSolveMazeTest()
    {
        // Arrange
        // Create a belief set for the agent.
        SolveMazeBeliefSet beliefSet = new();

        // Create an intent for the agent that moves the agent towards the target position.
        Action<SolveMazeBeliefSet> rotateRightAction = new
            (
                effect: beliefSet =>
                {
                    Debug.Log("Rotating right");
                    GameObject player = beliefSet.Player;
                    player.transform.Rotate(0, 5, 0);
                }
            );
        PrimitiveTactic<SolveMazeBeliefSet> rotateRight = new
            (
                action: rotateRightAction,
                guard: beliefSet => !beliefSet.IsObstacleToRight
            );

        Action<SolveMazeBeliefSet> walkForwardAction = new
            (
                effect: beliefSet =>
                {
                    Debug.Log("Walking forward");
                    GameObject player = beliefSet.Player;
                    float playerSpeed = 10f;
                    player.transform.position += playerSpeed * Time.deltaTime * player.transform.forward;
                }
            );
        PrimitiveTactic<SolveMazeBeliefSet> walkForward = new
            (
                action: walkForwardAction,
                guard: beliefSet => !beliefSet.IsObstacleInFront
            );

        Action<SolveMazeBeliefSet> rotateLeftAction = new
            (
                effect: beliefSet =>
                {
                    Debug.Log("Rotating left");
                    GameObject player = beliefSet.Player;
                    player.transform.Rotate(0, -5, 0);
                }
            );
        PrimitiveTactic<SolveMazeBeliefSet> rotateLeft = new(action: rotateLeftAction);

        FirstOfTactic<SolveMazeBeliefSet> walkAlongsideObstacles = new(null, rotateRight, walkForward, rotateLeft);

        // Create a desire for the agent to reach the target position.
        Goal<SolveMazeBeliefSet> reachTargetRoomGoal = new
            (
                tactic: walkAlongsideObstacles,
                predicate: beliefSet =>
                {
                    string currentRoom = beliefSet.CurrentRoom;
                    return currentRoom == "TargetRoom";
                }
            );
        PrimitiveGoalStructure<SolveMazeBeliefSet> reachTargetRoomGoalStructure = new(goal: reachTargetRoomGoal);
        DesireSet<SolveMazeBeliefSet> desireSet = new(mainGoal: reachTargetRoomGoalStructure);

        // Setup the agent with the belief set and desire set and initialize the test runner.
        BdiAgent<SolveMazeBeliefSet> agent = new(beliefSet, desireSet);
        AplibRunner testRunner = new(agent);

        // Act
        yield return testRunner.Test();

        // Assert
        Assert.AreEqual(agent.Status, CompletionStatus.Success);
    }
}
