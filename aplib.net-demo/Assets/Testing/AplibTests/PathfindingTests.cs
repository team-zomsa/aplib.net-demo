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

namespace Testing.AplibTests
{
    public class PathfindingBeliefSet : BeliefSet
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
        public void Setup()
        {
            SceneManager.LoadScene("PathfindingTest2");
        }

        [UnityTest]
        public IEnumerator TransformPathfindingTest()
        {
            PathfindingBeliefSet rootBeliefSet = new();

            // Action: Move the player towards the target position
            TransformPathfinderAction<PathfindingBeliefSet> transformPathfinderAction = new(
                b =>
                {
                    GameObject player = b.Player;
                    return player.GetComponent<Rigidbody>();
                },
                rootBeliefSet.TargetPosition,
                0.9f
            );

            // Tactic: Move the player towards the target position
            PrimitiveTactic<PathfindingBeliefSet> moveTactic = new(transformPathfinderAction);

            // Goal: Reach the target position
            PrimitiveGoalStructure<PathfindingBeliefSet> goal = new(goal: new Goal<PathfindingBeliefSet>(moveTactic, Predicate));

            DesireSet<PathfindingBeliefSet> desire = new(goal);

            // Create a new agent with the goal
            BdiAgent<PathfindingBeliefSet> agent = new(rootBeliefSet, desire);

            AplibRunner testRunner = new(agent);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return testRunner.Test();

            // Assert that the player has reached the target position
            Assert.IsTrue(condition: agent.Status == CompletionStatus.Success);
            yield break;

            bool Predicate(PathfindingBeliefSet beliefSet)
            {
                // The player has reached the target position
                GameObject player = beliefSet.Player;
                Vector3 target = beliefSet.TargetPosition;

                return Vector3.Distance(player.transform.position, target) < 1f;
            }
        }
    }
}
