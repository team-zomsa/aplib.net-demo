using Aplib.Core;
using Aplib.Core.Belief.Beliefs;
using Aplib.Core.Belief.BeliefSets;
using Aplib.Integrations.Unity;
using LevelGeneration;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static Aplib.Core.Combinators;
using Action = Aplib.Core.Intent.Actions.Action<Testing.AplibTests.ConnectedComponentsBeliefSet>;
using BdiAgent = Aplib.Core.Agents.BdiAgent<Testing.AplibTests.ConnectedComponentsBeliefSet>;
using DesireSet = Aplib.Core.Desire.DesireSets.DesireSet<Testing.AplibTests.ConnectedComponentsBeliefSet>;
using Goal = Aplib.Core.Desire.Goals.Goal<Testing.AplibTests.ConnectedComponentsBeliefSet>;
using PrimitiveTactic = Aplib.Core.Intent.Tactics.PrimitiveTactic<Testing.AplibTests.ConnectedComponentsBeliefSet>;
using Tactic = Aplib.Core.Intent.Tactics.Tactic<Testing.AplibTests.ConnectedComponentsBeliefSet>;
using TransformPathfinderAction = Aplib.Integrations.Unity.Actions.TransformPathfinderAction<Testing.AplibTests.ConnectedComponentsBeliefSet>;

namespace Testing.AplibTests
{
    public class ConnectedComponentsBeliefSet : IBeliefSet
    {
        public Rigidbody PlayerRigidbody { get; } = GameObject.Find("Player").GetComponent<Rigidbody>();

        public Belief<Queue<Vector3>, Vector3> GetCurrentTarget;

        public Queue<Vector3> TargetPositionsInConnectedComponents { get; } = new(GetTargetPositionsInConnectedComponents());

        public Queue<Vector3> TeleporterPositions { get; } = new(GetTeleporterLandingPoints());

        public ConnectedComponentsBeliefSet()
        {
            GetCurrentTarget = new(
                reference: TargetPositionsInConnectedComponents,
                getObservationFromReference: positions => positions.Peek(),
                shouldUpdate: () =>
                {
                    if (TargetPositionsInConnectedComponents.Count == 0) return false;

                    Vector3 currentTarget = TargetPositionsInConnectedComponents.Peek();

                    if (Vector3.Distance(PlayerRigidbody.position, currentTarget) > 0.4f)
                        return false;

                    Debug.Log($"Reached target at {currentTarget}");
                    TargetPositionsInConnectedComponents.Dequeue();
                    Debug.Log($"Remaining targets: {TargetPositionsInConnectedComponents.Count}");

                    return TargetPositionsInConnectedComponents.Count > 0;
                }
            );
        }

        public void UpdateBeliefs()
        {
            GetCurrentTarget.UpdateBelief();
        }

        private static IEnumerable<Vector3> GetTargetPositionsInConnectedComponents()
        {
            GameObject gridGameObject = GameObject.Find("LevelGeneration");
            LevelGenerationPipeline levelGenerationPipeline = gridGameObject.GetComponent<LevelGenerationPipeline>();
            SpawningExtensions spawningExtensions = gridGameObject.GetComponent<SpawningExtensions>();

            Vector3 centreOfCellHeightOffset = Vector3.up * 1.5f;

            return levelGenerationPipeline.Grid
                .DetermineConnectedComponents()
                .Select(cells => spawningExtensions.CenterOfCell(cells.First()) + centreOfCellHeightOffset);
        }

        private static IEnumerable<Vector3> GetTeleporterLandingPoints()
            => GameObject.Find("Teleporters")
                .GetComponentsInChildren<Teleporter.Teleporter>()
                .Select(x => x.LandingPoint);
    }

    public class ConnectedComponentsTests
    {
        private const string _sceneName = "ConnectedComponentsTestScene";

        [SetUp]
        public void SetUp()
        {
            Debug.Log($"Starting {nameof(ConnectedComponentsTests)}");
            SceneManager.LoadScene(_sceneName);
        }

        /// <summary>
        /// This test first collects a single cell from every connected component.
        /// It then tries to make the player - spawning in an arbitrary room - visit every one of those cells
        /// in an arbitrary order. If it succeeds in visiting every cell, it has visited every connected component.
        /// </summary>
        /// <returns>An IEnumerator usable to iterate the test.</returns>
        [UnityTest]
        public IEnumerator CanVisitEveryConnectedComponent()
        {

            // Arrange
            ConnectedComponentsBeliefSet connectedComponentBeliefSet = new();

            // Remove doors and keys from the level.
            GameObject doorsAndKeys = GameObject.Find("Doors and keys");
            Object.Destroy(doorsAndKeys);

            Tactic moveToNextComponent = new TransformPathfinderAction(
                beliefSet => beliefSet.PlayerRigidbody,
                beliefSet => beliefSet.GetCurrentTarget,
                1.5f);

            DesireSet visitAllComponents = new Goal(moveToNextComponent, HasVisitedEveryConnectedComponent);

            // Act
            BdiAgent agent = new(connectedComponentBeliefSet, visitAllComponents);
            AplibRunner testRunner = new(agent);

            yield return testRunner.Test();

            // Assert
            Assert.AreEqual(CompletionStatus.Success, agent.Status);

            // Local functions
            static bool HasVisitedEveryConnectedComponent(ConnectedComponentsBeliefSet beliefSet)
                => beliefSet.TargetPositionsInConnectedComponents.Count == 0;
        }
    }
}
