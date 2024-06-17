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
using Assets.Scripts.Wfc;
using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class KeyTests
{
    /// <summary>
    /// Tests start function and variables of the key.
    /// Also tests id the initialize method works correctly.
    /// </summary>
    [Test]
    public void KeyBaseFunctionTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<Key>();
        gameObject.AddComponent<MeshRenderer>();

        // Act
        gameObject.GetComponent<Key>().Initialize(10, Color.blue);

        // Assert
        Assert.IsFalse(gameObject.GetComponent<Key>().stackable);
        Assert.AreEqual(0, gameObject.GetComponent<Key>().uses);
        Assert.AreEqual(1, gameObject.GetComponent<Key>().usesAddedPerPickup);
        Assert.AreEqual(10, gameObject.GetComponent<Key>().Id);
        Assert.AreEqual(Color.blue, gameObject.GetComponent<MeshRenderer>().material.color);
    }

    [SetUp]
    public void SetUpScene()
    {
        SceneManager.LoadScene("KeyDoorTest");
    }

    public class KeyDoorBeliefSet : BeliefSet
    {
        /// <summary>
        /// Key position in the scene.
        /// </summary>
        public Belief<GameObject, Vector3> KeyPosition = new(GameObject.FindGameObjectWithTag("Key"),
            key => key ? key.transform.position : Vector3.zero
        );

        /// <summary>
        /// Player object in the scene.
        /// </summary>
        public Belief<GameObject, GameObject> Player = new(GameObject.FindGameObjectWithTag("Player"), p => p);

        /// <summary>
        /// Door position in the scene.
        /// </summary>
        public Belief<GameObject, Vector3> DoorPosition = new(GameObject.FindGameObjectWithTag("Door"),
            door => door ? door.transform.position : Vector3.zero);

        /// <summary>
        /// If the door is open, return true;
        /// </summary>
        public Belief<GameObject, bool> IsDoorOpen = new
            (
                GameObject.FindGameObjectWithTag("Door"),
                x =>
                {
                    return x == null;
                }
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
        TransformPathfinderAction<KeyDoorBeliefSet> transformPathfinderPlayerToDoor = new(
            beliefSet =>
            {
                GameObject player = beliefSet.Player;
                return player.GetComponent<Rigidbody>();
            },
            beliefSet => beliefSet.DoorPosition,
            0.9f
            );

        // Tactic : Move player to the key.
        PrimitiveTactic<KeyDoorBeliefSet> moveToKeyTactic = new(transformPathfinderFromPlayerToKey);

        // Tactic : Move player to the door.
        PrimitiveTactic<KeyDoorBeliefSet> moveToDoorTactic = new(transformPathfinderPlayerToDoor);

        // Goal : Grab the key and open the door.
        PrimitiveGoalStructure<KeyDoorBeliefSet> findKeyGoal = new(new Goal<KeyDoorBeliefSet>(moveToKeyTactic, IsKeyInInventory));
        PrimitiveGoalStructure<KeyDoorBeliefSet> moveToDoorGoal = new(new Goal<KeyDoorBeliefSet>(moveToDoorTactic, IsDoorOpen));
        SequentialGoalStructure<KeyDoorBeliefSet> finalGoal = new SequentialGoalStructure<KeyDoorBeliefSet>(findKeyGoal, moveToDoorGoal);

        // Check if there is a key in inventory -> false
        bool IsKeyInInventory(KeyDoorBeliefSet beliefSet)
        {
            return true;
        }

        bool IsDoorOpen(KeyDoorBeliefSet beliefSet)
        {
            return beliefSet.IsDoorOpen;
        }

        yield return null;
    }
}
