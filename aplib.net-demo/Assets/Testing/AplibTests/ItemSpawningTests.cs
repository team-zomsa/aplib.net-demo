using Aplib.Core;
using Aplib.Core.Belief;
using Aplib.Core.Desire;
using Aplib.Core.Desire.Goals;
using Aplib.Core.Intent.Tactics;
using Aplib.Integrations.Unity;
using Aplib.Integrations.Unity.Actions;
using Assets.Scripts.Items;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Testing.AplibTests
{
    public class ItemSpawningBeliefSet : BeliefSet
    {
        /// <summary>
        /// The inventory in the scene.
        /// </summary>
        public readonly Belief<GameObject, Inventory> InventoryObject =
            new(GameObject.Find("InventoryObject"), x => x.GetComponent<Inventory>());

        public readonly Belief<GameObject, Rigidbody> PlayerRigidBody = new(
            GameObject.Find("Player"),
            x => x.GetComponent<Rigidbody>());

        /// <summary>
        /// The target position that the player needs to move towards.
        /// Find the end item in the scene.
        /// </summary>
        public readonly Belief<GameObject[], Vector3> TargetKeyPosition = new(GameObject.FindGameObjectsWithTag("Key"),
            x =>
            {
                // If there are no keys, return Vector3.zero.
                if (x.Length == 0) return Vector3.zero;

                GameObject player = GameObject.Find("Player");
                Rigidbody playerRigidBody = player.GetComponent<Rigidbody>();
                Vector3 playerPosition = playerRigidBody.position;

                NavMeshPath path = new();

                // Find the first key that is reachable.
                foreach (GameObject key in x)
                {
                    if (key == null) continue;

                    NavMesh.CalculatePath(playerPosition,
                        key.transform.position,
                        NavMesh.AllAreas,
                        path
                    );

                    if (path.status == NavMeshPathStatus.PathComplete) return key.transform.position;
                }

                // If no key is reachable, return Vector3.zero.
                return Vector3.zero;
            });

        /// <summary>
        /// The target position that the player needs to move towards.
        /// Find the end item in the scene.
        /// </summary>
        public readonly Belief<GameObject, Vector3> TargetPosition =
            new(GameObject.Find("The Eternal Elixir"), x => x == null ? Vector3.zero : x.transform.position);
    }

    public class ItemSpawningTests
    {
        private const string _sceneName = "ItemSpawningTestScene";

        [SetUp]
        public void SetUp()
        {
            Debug.Log($"Starting {nameof(ItemSpawningTests)}");
            SceneManager.LoadScene(_sceneName);
        }

        /// <summary>
        /// The player will walk up to the end item and pick it up.
        /// </summary>
        /// <returns>An IEnumerator usable to iterate the test.</returns>
        [UnityTest]
        public IEnumerator CanFindEndItem()
        {
            // Arrange
            InputManager.Instance.enabled = false;
            ItemSpawningBeliefSet beliefSet = new();

            GameObject[] keys = GameObject.FindGameObjectsWithTag("Key");
            foreach (GameObject key in keys) key.AddComponent<BeforeDestroyKey>();

            // Create an intent for the agent that moves the agent towards the target position.
            TransformPathfinderAction<ItemSpawningBeliefSet> transformPathfinderAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.TargetPosition,
                0.3f
            );

            TransformPathfinderAction<ItemSpawningBeliefSet> transformPathfinderActionKey = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.TargetKeyPosition,
                0.3f
            );

            PrimitiveTactic<ItemSpawningBeliefSet> moveTactic = new(transformPathfinderAction);
            PrimitiveTactic<ItemSpawningBeliefSet> moveTacticKey = new(transformPathfinderActionKey);

            PrimitiveGoalStructure<ItemSpawningBeliefSet> moveGoal =
                new(new Goal<ItemSpawningBeliefSet>(moveTactic, EndItemPickedUpPredicate));

            PrimitiveGoalStructure<ItemSpawningBeliefSet> moveToKeyGoal =
                new(new Goal<ItemSpawningBeliefSet>(moveTacticKey, EndItemReachablePredicate));

            RepeatGoalStructure<ItemSpawningBeliefSet> moveToKeyGoalStructure = new(moveToKeyGoal);

            SequentialGoalStructure<ItemSpawningBeliefSet>
                moveAndPickupSequence = new(moveToKeyGoalStructure, moveGoal);

            // Arrange ==> DesireSet
            DesireSet<ItemSpawningBeliefSet> desire = new(moveAndPickupSequence);

            // Act
            BdiAgent<ItemSpawningBeliefSet> agent = new(beliefSet, desire);
            AplibRunner testRunner = new(agent);

            yield return testRunner.Test();

            // Assert
            Assert.IsTrue(agent.Status == CompletionStatus.Success);
            yield break;

            bool EndItemPickedUpPredicate(ItemSpawningBeliefSet beliefSet)
            {
                Inventory inventory = beliefSet.InventoryObject;

                return inventory.ContainsItem("The Eternal Elixir");
            }

            bool EndItemReachablePredicate(ItemSpawningBeliefSet beliefSet)
            {
                Rigidbody playerRigidbody = beliefSet.PlayerRigidBody;
                NavMeshPath path = new();
                Vector3 target = beliefSet.TargetPosition;

                NavMesh.CalculatePath(playerRigidbody.position,
                    target,
                    NavMesh.AllAreas,
                    path
                );

                return path.status == NavMeshPathStatus.PathComplete;
            }
        }
    }
}
