using Aplib.Core;
using Aplib.Core.Belief.Beliefs;
using Aplib.Core.Belief.BeliefSets;
using Aplib.Integrations.Unity;
using Assets.Scripts.Items;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static Aplib.Core.Combinators;
using Goal = Aplib.Core.Desire.Goals.Goal<Testing.AplibTests.ItemSpawningBeliefSet>;
using Tactic = Aplib.Core.Intent.Tactics.Tactic<Testing.AplibTests.ItemSpawningBeliefSet>;
using GoalStructure = Aplib.Core.Desire.GoalStructures.GoalStructure<Testing.AplibTests.ItemSpawningBeliefSet>;
using BdiAgent = Aplib.Core.Agents.BdiAgent<Testing.AplibTests.ItemSpawningBeliefSet>;
using TransformPathfinderAction = Aplib.Integrations.Unity.Actions.TransformPathfinderAction<Testing.AplibTests.ItemSpawningBeliefSet>;

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
        [Timeout(300000)]
        public IEnumerator CanFindEndItem()
        {
            // Arrange
            ItemSpawningBeliefSet beliefSet = new();

            GameObject[] keys = GameObject.FindGameObjectsWithTag("Key");
            foreach (GameObject key in keys) key.AddComponent<BeforeDestroyKey>();

            // Create an intent for the agent that moves the agent towards the target position.
            Tactic moveToTarget = new TransformPathfinderAction (
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.TargetPosition,
                0.3f
            );

            Tactic moveToKey = new TransformPathfinderAction (
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.TargetKeyPosition,
                0.3f
            );

            GoalStructure moveToTargetGoal = new Goal(moveToTarget, EndItemPickedUpPredicate);
            GoalStructure moveToKeyGoal = new Goal(moveToKey, EndItemReachablePredicate);

            GoalStructure moveAndPickupSequence = Seq(moveToKeyGoal, moveToTargetGoal);

            // Act
            BdiAgent agent = new(beliefSet, moveAndPickupSequence.Lift());
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
