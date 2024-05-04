using Aplib;
using Aplib.Core;
using Aplib.Core.Belief;
using Aplib.Core.Desire;
using Aplib.Core.Desire.Goals;
using Aplib.Core.Intent.Actions;
using Aplib.Core.Intent.Tactics;
using Aplib.Integrations.Unity.Actions;
using Assets.Scripts.Wfc;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.AplibTests
{
    public class ConnectedComponentsBeliefSet : BeliefSet
    {
        public Belief<GameObject, Transform> PlayerTransform = new(
            reference: GameObject.Find("Player"),
            x => x.transform);
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
        /// It then tries to make the player - spawning in an arbitrary room - visit every one of those cells,
        /// in an arbitrary order. If it succeeds in visiting every cell, it has visited every connected component.
        /// </summary>
        /// <returns>An IEnumerator usable to iterate the test.</returns>
        [UnityTest]
        public IEnumerator CanVisitEveryConnectedComponent()
        {
            // Before the tests can begin, we must generate Nev Mash Data such that path finding works. Furthermore,
            // Nav Mesh Data from different connected components must be linked through Nav Mesh Links.
            NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByIndex(0);
            NavMeshData navMeshData = NavMeshBuilder.BuildNavMeshData(buildSettings, collectNavMeshSources(),
                new Bounds(Vector3.zero, new Vector3(300, 50, 300)), Vector3.zero, Quaternion.identity); // TODO make grid the bounds
            NavMesh.AddNavMeshData(navMeshData);
            NavMeshBuilder.UpdateNavMeshData(navMeshData, buildSettings, collectNavMeshSources(),
                new Bounds(Vector3.zero, new Vector3(300, 50, 300)));

            List<NavMeshBuildSource> collectNavMeshSources()
            {
                GameObject environment = GameObject.Find("Environment");

                List<NavMeshBuildSource> sources = environment.GetComponentsInChildren<MeshFilter>()
                    .Select(meshFilter => new NavMeshBuildSource // TODO exclude modivied meshes?
                    {
                        shape = NavMeshBuildSourceShape.Mesh,
                        sourceObject = meshFilter.sharedMesh,
                        transform = meshFilter.gameObject.transform.localToWorldMatrix,
                        area = 0,
                    }).ToList();

                return sources.ToList();
            }

            // Arrange
            ConnectedComponentsBeliefSet rootBeliefSet = new();
            GridPlacer gridPlacer = GameObject.Find("Grid").GetComponent<GridPlacer>();

            // Arrange ==> Level information
            Vector3[] cellsToVisit = gridPlacer.Grid.DetermineConnectedComponents()
                .Select(cells => gridPlacer.CentreOfCell(cells.First())).ToArray();
            Vector3[] teleporterPositions = GameObject.Find("Teleporters")
                .GetComponentsInChildren<Teleporter.Teleporter>()
                .Select(x => x.LandingPoint).ToArray();

            int currentCellToVisitIndex = 0;
            Vector3 currentCellPosition() => cellsToVisit[currentCellToVisitIndex];

            // Arrange ==> GoalStructure: Visit cell of the current connected component
            TransformPathfinderAction<ConnectedComponentsBeliefSet> approachCurrentCellAction = new(
                objectQuery: beliefSet => beliefSet.PlayerTransform,
                location: currentCellPosition(),
                heightOffset: .7f);

            Action<ConnectedComponentsBeliefSet> waitForTeleportAction = new(
                effect: _ => { Debug.Log("Waiting for teleport..."); },
                guard: beliefSet => teleporterPositions.Any(teleporterPosition =>
                    (teleporterPosition - ((Transform)beliefSet.PlayerTransform).position).magnitude < 0.4f));

            PrimitiveTactic<ConnectedComponentsBeliefSet> approachCurrentCellTactic = new(approachCurrentCellAction);
            PrimitiveTactic<ConnectedComponentsBeliefSet> waitForTeleportTactic = new(waitForTeleportAction);
            FirstOfTactic<ConnectedComponentsBeliefSet> waitForTeleportOrApproachCurrentCellTactic = new(metadata: null,
                waitForTeleportTactic,
                approachCurrentCellTactic);

            Goal<ConnectedComponentsBeliefSet> approachCurrentCellGoal = new(waitForTeleportOrApproachCurrentCellTactic,
                beliefSet => (currentCellPosition() - ((Transform)beliefSet.PlayerTransform).position).magnitude < 0.2f);
            PrimitiveGoalStructure<ConnectedComponentsBeliefSet> approachCurrentCellGoalStructure = new(approachCurrentCellGoal);
            RepeatGoalStructure<ConnectedComponentsBeliefSet> visitCurrentCellGoalStructure = new(approachCurrentCellGoalStructure);

            // Arrange ==> GoalStructure: Target the next cell until every cell has been targeted
            Action<ConnectedComponentsBeliefSet> targetNextCellAction = new(effect: _ =>
            {
                currentCellToVisitIndex++;
                Debug.Log($"Targeting next cell at {currentCellPosition()}");
            });

            PrimitiveTactic<ConnectedComponentsBeliefSet> targetNextCellTactic = new(targetNextCellAction);
            Goal<ConnectedComponentsBeliefSet> visitedEveryCellGoal = new(targetNextCellTactic,
                _ => currentCellToVisitIndex >= cellsToVisit.Length);
            PrimitiveGoalStructure<ConnectedComponentsBeliefSet> visitedEveryCellGoalStructure = new(visitedEveryCellGoal);

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

        [TearDown]
        public void TearDown()
        {
            Debug.Log($"Finished {nameof(ConnectedComponentsTests)}");
            SceneManager.UnloadSceneAsync(_sceneName);
        }
    }
}
