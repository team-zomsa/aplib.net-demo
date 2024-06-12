using Aplib.Core;
using Aplib.Core.Agents;
using Aplib.Core.Belief.Beliefs;
using Aplib.Core.Belief.BeliefSets;
using Aplib.Core.Desire.Goals;
using Aplib.Integrations.Unity;
using Aplib.Integrations.Unity.Actions;
using Assets.Scripts.Items;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static Aplib.Core.Combinators;

namespace Testing.AplibTests
{
    public class GameplayBeliefSet : BeliefSet
    {
        public const string EndItemName = "The Eternal Elixir";

        /// <summary> The inventory in the scene. </summary>
        public readonly Belief<Inventory, Inventory> InventoryObject =
            new(GameObject.Find("InventoryObject").GetComponent<Inventory>(), x => x);

        /// <summary> The rigidbody of the player </summary>
        public readonly Belief<Rigidbody, Rigidbody> PlayerRigidBody = new(
            GameObject.Find("Player").GetComponent<Rigidbody>(), x => x);

        /// <summary> The position of the next key to obtain to reach the end room. </summary>
        public readonly Belief<IEnumerable<GameObject>, Vector3> TargetKeyPosition = new(
            GameObject.FindGameObjectsWithTag("Key").Where(x => x != null), keys =>
            {
                Vector3 playerPosition = GameObject.Find("Player").GetComponent<Rigidbody>().position;
                NavMeshPath path = new();

                // Find the first key that is reachable.
                foreach (GameObject key in keys)
                {
                    NavMesh.CalculatePath(playerPosition, key.transform.position, NavMesh.AllAreas, path);
                    if (path.status is NavMeshPathStatus.PathComplete)
                        return key.transform.position;
                }

                // If no key is reachable, return Vector3.zero.
                return Vector3.zero;
            });

        /// <summary> The position to which the player must navigate in order to fetch the end item. </summary>
        public readonly Belief<GameObject, Vector3> EndItemPosition = new(
            GameObject.Find(EndItemName), x => x.transform.position,
            () => !GameObject.Find("InventoryObject").GetComponent<Inventory>().ContainsItem(EndItemName));
    }

    public class GameplayTests
    {
        private const string _sceneName = "GameplayTestScene";

        [SetUp]
        public void SetUp()
        {
            Debug.Log($"Starting '{nameof(GameplayTests)}'");
            SceneManager.LoadScene(_sceneName);
        }

        /// <summary>
        /// Given that the smart agent mimicks realistic gameplay,
        /// When the agent is tasked to complete the game from start to end,
        /// The game must be winnable.
        /// </summary>
        /// <returns>An IEnumerator usable to iterate the test.</returns>
        [UnityTest]
        [Timeout(300000)]
        public IEnumerator RealisticGameplayCanWinTheGame()
        {
            // Arrange
            InputManager.Instance.enabled = false;
            GameplayBeliefSet beliefSet = new();

            GameObject[] keys = GameObject.FindGameObjectsWithTag("Key");
            foreach (GameObject key in keys) key.AddComponent<BeforeDestroyKey>();

            // Create an intent for the agent that moves the agent towards the target position.
            TransformPathfinderAction<GameplayBeliefSet> moveToEndItemAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.EndItemPosition,
                0.3f
            );

            TransformPathfinderAction<GameplayBeliefSet> moveToNextKeyAction = new(
                beliefSet => beliefSet.PlayerRigidBody,
                beliefSet => beliefSet.TargetKeyPosition,
                0.3f
            );

            Goal<GameplayBeliefSet> moveToEndItemGoal = new(moveToEndItemAction.Lift(), endItemPickedUpPredicate);
            Goal<GameplayBeliefSet> moveToNextKeyGoal = new(moveToNextKeyAction.Lift(), endItemReachablePredicate);

            // Act
            BdiAgent<GameplayBeliefSet> agent = new(beliefSet, Seq(moveToNextKeyGoal.Lift(), moveToEndItemGoal.Lift()).Lift());
            AplibRunner testRunner = new(agent);

            yield return testRunner.Test();

            // Assert
            Assert.AreEqual(CompletionStatus.Success, agent.Status);
            yield break;

            bool endItemReachablePredicate(GameplayBeliefSet beliefSet)
            {
                Rigidbody playerRigidbody = beliefSet.PlayerRigidBody;
                Vector3 target = beliefSet.EndItemPosition;

                NavMeshPath path = new();
                NavMesh.CalculatePath(playerRigidbody.position, target, NavMesh.AllAreas, path);
                return path.status == NavMeshPathStatus.PathComplete;
            }

            bool endItemPickedUpPredicate(GameplayBeliefSet beliefSet)
            {
                Inventory inventory = beliefSet.InventoryObject;
                return inventory.ContainsItem(GameplayBeliefSet.EndItemName);
            }
        }
    }
}
