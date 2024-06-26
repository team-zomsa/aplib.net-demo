// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using Aplib.Core;
using Aplib.Core.Belief.Beliefs;
using Aplib.Core.Belief.BeliefSets;
using Aplib.Integrations.Unity;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Goal = Aplib.Core.Desire.Goals.Goal<Testing.AplibTests.PathfindingBeliefSet>;
using Tactic = Aplib.Core.Intent.Tactics.Tactic<Testing.AplibTests.PathfindingBeliefSet>;
using GoalStructure = Aplib.Core.Desire.GoalStructures.GoalStructure<Testing.AplibTests.PathfindingBeliefSet>;
using BdiAgent = Aplib.Core.Agents.BdiAgent<Testing.AplibTests.PathfindingBeliefSet>;
using TransformPathfinderAction = Aplib.Integrations.Unity.Actions.TransformPathfinderAction<Testing.AplibTests.PathfindingBeliefSet>;

namespace Testing.AplibTests
{
    public class PathfindingBeliefSet : BeliefSet
    {
        /// <summary>
        /// The player object in the scene.
        /// </summary>
        public Belief<GameObject, GameObject> Player = new(reference: GameObject.Find("Player"), x => x);

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
            SceneManager.LoadScene("PathfindingTest");
        }

        [UnityTest]
        public IEnumerator TransformPathfindingTest()
        {
            PathfindingBeliefSet rootBeliefSet = new();

            // Action: Move the player towards the target position
            Tactic move = new TransformPathfinderAction(beliefSet =>
                {
                    GameObject player = beliefSet.Player;
                    return player.GetComponent<Rigidbody>();
                },
                rootBeliefSet.TargetPosition,
                0.9f
            );

            GoalStructure goal = new Goal(move, Predicate);
            BdiAgent agent = new(rootBeliefSet, goal.Lift());

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
