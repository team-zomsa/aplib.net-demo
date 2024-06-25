using Aplib.Core;
using Aplib.Core.Agents;
using Aplib.Core.Belief.Beliefs;
using Aplib.Core.Belief.BeliefSets;
using Aplib.Core.Desire.Goals;
using Aplib.Core.Desire.GoalStructures;
using Aplib.Integrations.Unity;
using Aplib.Integrations.Unity.Actions;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Testing.AplibTests
{
    public class ASimpleKeyTests
    {
        private const string _sceneName = "KeyDoorTest";

        [SetUp]
        public void SetUpScene()
        {
            Debug.Log($"Starting {nameof(ASimpleKeyTests)}");
            SceneManager.LoadScene(_sceneName);
        }

        public class KeyDoorBeliefSet : BeliefSet
        {
            /// <summary>
            /// Key position in the scene.
            /// </summary>
            public Belief<GameObject, Vector3> KeyPosition = new(GameObject.FindGameObjectWithTag("Key"),
                getObservationFromReference: key => key == null ? Vector3.zero : key.transform.position
            );

            /// <summary>
            /// Player object in the scene.
            /// </summary>
            public GameObject Player { get; } = GameObject.Find("Player");

            /// <summary>
            /// Door position in the scene.
            /// </summary>
            public Belief<GameObject, Vector3> DoorPosition = new(GameObject.FindGameObjectWithTag("Door"),
                getObservationFromReference: door => door == null ? Vector3.zero : door.transform.position);

            /// <summary>
            /// If the door is open, return true;
            /// </summary>
            public Belief<GameObject, bool> IsDoorOpen = new
                (
                    GameObject.FindGameObjectWithTag("Door"),
                    door => door == null
                );
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator FindKeyAndCorrectDoorOpensTest()
        {
            // Make beliefset instance
            KeyDoorBeliefSet beliefSet = new();

            // Action : Walk to key
            TransformPathfinderAction<KeyDoorBeliefSet> transformPathfinderFromPlayerToKey = new(
                beliefSet =>
                    {
                        GameObject player = beliefSet.Player;
                        return player.GetComponent<Rigidbody>();
                    },
            beliefSet => beliefSet.KeyPosition,
            0.9f
            );

            // Action : Walk to door
            TransformPathfinderAction<KeyDoorBeliefSet> moveToDoor = new(
                beliefSet =>
                {
                    GameObject player = beliefSet.Player;
                    return player.GetComponent<Rigidbody>();
                },
                beliefSet => beliefSet.DoorPosition,
                0.9f
                );

            // Goal : Grab the key and open the door.
            PrimitiveGoalStructure<KeyDoorBeliefSet> moveToDoorFirstGoal = new Goal<KeyDoorBeliefSet>(moveToDoor.Lift(), IsNextToDoor).Lift();
            PrimitiveGoalStructure<KeyDoorBeliefSet> moveToKeyGoal = new Goal<KeyDoorBeliefSet>(transformPathfinderFromPlayerToKey.Lift(), IsKeyInInventory).Lift();
            PrimitiveGoalStructure<KeyDoorBeliefSet> moveToDoorWithKeyGoal = new Goal<KeyDoorBeliefSet>(moveToDoor.Lift(), x => x.IsDoorOpen).Lift();
            SequentialGoalStructure<KeyDoorBeliefSet> finalGoal = new(moveToDoorFirstGoal, moveToKeyGoal, moveToDoorWithKeyGoal);

            // Create a new agent with the goal
            BdiAgent<KeyDoorBeliefSet> agent = new(beliefSet, finalGoal.Lift());

            AplibRunner testRunner = new(agent);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return testRunner.Test();

            // Assert that the player has reached the target position.
            Assert.IsTrue(agent.Status == CompletionStatus.Success);
            yield break;

            // Check if there is a key in inventory -> true
            bool IsKeyInInventory(KeyDoorBeliefSet beliefSet) => GameObject.Find("Key") == null;

            // Check if player has reached the door.
            bool IsNextToDoor(KeyDoorBeliefSet beliefSet)
            {
                Transform player = beliefSet.Player.transform;
                Vector3 door = beliefSet.DoorPosition;

                if (player.position.z > 4.3f) return true;
                return false;
            }
        }
    }
}
