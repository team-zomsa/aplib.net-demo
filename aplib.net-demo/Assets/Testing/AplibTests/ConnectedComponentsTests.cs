using Aplib.Core;
using Aplib.Core.Belief;
using Aplib.Core.Desire;
using Aplib.Core.Desire.Goals;
using Aplib.Core.Intent.Actions;
using Aplib.Core.Intent.Tactics;
using Aplib.Integrations.Unity;
using Aplib.Integrations.Unity.Actions;
using Assets.Scripts.Wfc;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Testing.AplibTests
{
    public class ConnectedComponentsBeliefSet : BeliefSet
    {
        public Belief<GameObject, Rigidbody> PlayerRigidbody = new(
            GameObject.Find("Player"),
            x => x.GetComponent<Rigidbody>());
    }

    public class ConnectedComponentsTests
    {
        private const string _sceneName = "ConnectedComponentsTestScene";
        private readonly Vector3 _centreOfCellHeightOffset = Vector3.up * 1.7f;

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
            ConnectedComponentsBeliefSet rootBeliefSet = new();
            GridPlacer gridPlacer = GameObject.Find("Grid").GetComponent<GridPlacer>();
            GameObjectPlacer gameObjectPlacer = GameObject.Find("Grid").GetComponent<GameObjectPlacer>();

            // Arrange ==> Level information
            Vector3[] cellsToVisit = gridPlacer.Grid.DetermineConnectedComponents()
                .Select(cells => gameObjectPlacer.CenterOfCell(cells.First()) + _centreOfCellHeightOffset).ToArray();
            Vector3[] teleporterPositions = GameObject.Find("Teleporters")
                .GetComponentsInChildren<Teleporter.Teleporter>()
                .Select(x => x.LandingPoint).ToArray();

            int currentCellToVisitIndex = 0;

            Vector3 currentCellPosition() => cellsToVisit[currentCellToVisitIndex];

            // Arrange ==> GoalStructure: Visit cell of the current connected component
            TransformPathfinderAction<ConnectedComponentsBeliefSet> approachCurrentCellAction = new(
                beliefSet => beliefSet.PlayerRigidbody,
                currentCellPosition(),
                1.4f);

            Action<ConnectedComponentsBeliefSet> waitForTeleportAction = new(_ => { Debug.Log("Waiting for teleport..."); });

            PrimitiveTactic<ConnectedComponentsBeliefSet> approachCurrentCellTactic = new(approachCurrentCellAction);
            PrimitiveTactic<ConnectedComponentsBeliefSet> waitForTeleportTactic = new(waitForTeleportAction,
                guard: beliefSet => teleporterPositions.Any(teleporterPosition =>
                    (teleporterPosition - ((Rigidbody)beliefSet.PlayerRigidbody).position).magnitude < 0.4f));
            FirstOfTactic<ConnectedComponentsBeliefSet> waitForTeleportOrApproachCurrentCellTactic = new(metadata: null,
                waitForTeleportTactic,
                approachCurrentCellTactic);

            Goal<ConnectedComponentsBeliefSet> approachCurrentCellGoal = new(waitForTeleportOrApproachCurrentCellTactic,
                beliefSet => (currentCellPosition() - ((Rigidbody)beliefSet.PlayerRigidbody).position).magnitude <
                             1.5f);
            PrimitiveGoalStructure<ConnectedComponentsBeliefSet> approachCurrentCellGoalStructure =
                new(approachCurrentCellGoal);
            RepeatGoalStructure<ConnectedComponentsBeliefSet> visitCurrentCellGoalStructure =
                new(approachCurrentCellGoalStructure);

            // Arrange ==> GoalStructure: Target the next cell until every cell has been targeted
            Action<ConnectedComponentsBeliefSet> targetNextCellAction = new(_ =>
            {
                Debug.Log($"Reached cell at {currentCellPosition()}");
                currentCellToVisitIndex++;
            });

            PrimitiveTactic<ConnectedComponentsBeliefSet> targetNextCellTactic = new(targetNextCellAction);
            Goal<ConnectedComponentsBeliefSet> visitedEveryCellGoal = new(targetNextCellTactic,
                _ => currentCellToVisitIndex >= cellsToVisit.Length);
            PrimitiveGoalStructure<ConnectedComponentsBeliefSet> visitedEveryCellGoalStructure =
                new(visitedEveryCellGoal);

            // Arrange ==> GoalStructure: Visit every connected component
            SequentialGoalStructure<ConnectedComponentsBeliefSet> visitCurrentCellAndVisitEveryCellGoalStructure =
                new(visitCurrentCellGoalStructure, visitedEveryCellGoalStructure);
            RepeatGoalStructure<ConnectedComponentsBeliefSet> visitEveryConnectedComponentGoalStructure =
                new(visitCurrentCellAndVisitEveryCellGoalStructure);

            // Arrange ==> DesireSet
            DesireSet<ConnectedComponentsBeliefSet> desireSet = new(visitEveryConnectedComponentGoalStructure);

            // Act
            BdiAgent<ConnectedComponentsBeliefSet> agent = new(rootBeliefSet, desireSet);
            AplibRunner testRunner = new(agent);

            yield return testRunner.Test();

            // Assert
            Assert.AreEqual(CompletionStatus.Success, agent.Status);
        }
    }
}
