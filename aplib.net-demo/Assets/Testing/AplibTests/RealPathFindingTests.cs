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
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.AplibTests
{
    public class RpfBeliefset : BeliefSet
    {
        /// <summary>
        /// The player object in the scene.
        /// </summary>
        public readonly Belief<GameObject, GameObject> Player =
            new(reference: GameObject.Find("PlayerRotation"), x => x);

        /// <summary>
        /// The target position that the player needs to move towards.
        /// </summary>
        public readonly Belief<Transform, Vector3> TargetPosition =
            new(GameObject.Find("Target").transform, x => x.position);
    }

    public class RealPathFindingTests : InputTestFixture
    {
        public override void Setup()
        {
            base.Setup();

            // Load the scene
            SceneManager.LoadScene("PathfindingTest3");
            Debug.Log("DOING THIS SHIT!!");
        }

        public override void TearDown()
        {
            SceneManager.UnloadSceneAsync("PathfindingTest3");
            Object.Destroy(InputManager.Instance);
        }

        [UnityTest]
        public IEnumerator RealPathfindingTests()
        {
            Debug.Log("DOING THIS SHIT?!!?!");
            RpfBeliefset beliefSet = new();

            // Create a input device for the player.
            Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
            Mouse mouse = InputSystem.AddDevice<Mouse>();

            Debug.Log("DOING THIS SHIT!!!");

            // Create an intent for the agent that moves the agent towards the target position.
            UnityPathfinderAction<RpfBeliefset> unityPathfinderAction = new(
                b =>
                {
                    Release(keyboard.wKey);
                    GameObject player = b.Player;
                    return player.transform;
                },
                _ => beliefSet.TargetPosition,
                (beliefset, desiredLocation) =>
                {
                    GameObject player = beliefset.Player;
                    Vector3 playerPosition = player.transform.position;

                    // Get the direction to the target position
                    Vector3 direction = desiredLocation - playerPosition;
                    direction.Normalize();

                    // Let the player look towards the direction with the mouse
                    float horizontalSpeed = 10f;
                    float multiplier = 5f;

                    // Get the angle between the player's forward direction and the direction to the target position
                    float angle = Vector3.SignedAngle(player.transform.forward, direction, Vector3.up);

                    Set(mouse.delta, state: new Vector2(x: angle * multiplier / horizontalSpeed, 0));

                    // Use the input device to move the player towards the target position
                    Press(keyboard.wKey);
                },
                0.9f
            );

            // Create a desire for the agent to reach the target position.
            PrimitiveTactic<RpfBeliefset> moveTowardsTargetTactic = new(unityPathfinderAction);
            PrimitiveGoalStructure<RpfBeliefset> locationGoal = new(goal: new Goal<RpfBeliefset>(
                    moveTowardsTargetTactic,
                    beliefset =>
                    {
                        Release(keyboard.wKey);
                        Set(mouse.delta, Vector2.zero);

                        GameObject player = beliefset.Player;
                        Vector3 target = beliefset.TargetPosition;
                        return Vector3.Distance(player.transform.position, target) < 1f;
                    }
                )
            );

            DesireSet<RpfBeliefset> desireSet = new(locationGoal);

            // Create a new agent with the goal
            BdiAgent<RpfBeliefset> agent = new(beliefSet, desireSet);

            AplibRunner testRunner = new(agent);

            Debug.Log("Testing the runner");

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return testRunner.Test();

            InputSystem.RemoveDevice(mouse);
            InputSystem.RemoveDevice(keyboard);

            // Assert that the player has reached the target position
            Assert.IsTrue(condition: agent.Status == CompletionStatus.Success);
        }
    }
}
