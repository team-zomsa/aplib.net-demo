using Aplib.Core;
using Aplib.Core.Belief.Beliefs;
using Aplib.Core.Belief.BeliefSets;
using Aplib.Integrations.Unity;
using LevelGeneration;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static Aplib.Core.Combinators;
using Goal = Aplib.Core.Desire.Goals.Goal<Testing.AplibTests.ConnectedComponentsBeliefSet>;
using Action = Aplib.Core.Intent.Actions.Action<Testing.AplibTests.ConnectedComponentsBeliefSet>;
using Tactic = Aplib.Core.Intent.Tactics.Tactic<Testing.AplibTests.ConnectedComponentsBeliefSet>;
using PrimitiveTactic = Aplib.Core.Intent.Tactics.PrimitiveTactic<Testing.AplibTests.ConnectedComponentsBeliefSet>;
using GoalStructure = Aplib.Core.Desire.GoalStructures.GoalStructure<Testing.AplibTests.ConnectedComponentsBeliefSet>;
using BdiAgent = Aplib.Core.Agents.BdiAgent<Testing.AplibTests.ConnectedComponentsBeliefSet>;
using TransformPathfinderAction = Aplib.Integrations.Unity.Actions.TransformPathfinderAction<Testing.AplibTests.ConnectedComponentsBeliefSet>;

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
            GameObject gridGameObject = GameObject.Find("LevelGeneration");
            LevelGenerationPipeline levelGenerationPipeline = gridGameObject.GetComponent<LevelGenerationPipeline>();
            SpawningExtensions spawningExtensions = gridGameObject.GetComponent<SpawningExtensions>();

            // Arrange ==> Level information
            Vector3[] cellsToVisit = levelGenerationPipeline.Grid.DetermineConnectedComponents()
                .Select(cells => spawningExtensions.CenterOfCell(cells.First()) + _centreOfCellHeightOffset).ToArray();

            Vector3[] teleporterPositions = GameObject.Find("Teleporters")
                .GetComponentsInChildren<Teleporter.Teleporter>()
                .Select(x => x.LandingPoint).ToArray();

            int currentCellToVisitIndex = 0;

            Vector3 currentCellPosition()
            {
                return cellsToVisit[currentCellToVisitIndex];
            }

            // Arrange ==> GoalStructure: Visit cell of the current connected component
            TransformPathfinderAction approachCurrentCellAction = new(
                beliefSet => beliefSet.PlayerRigidbody,
                currentCellPosition(),
                1.4f);

            Action waitForTeleportAction = new(_ => Debug.Log("Waiting for teleport..."));

            PrimitiveTactic waitForTeleportTactic = new(waitForTeleportAction,
                beliefSet => teleporterPositions.Any(teleporterPosition =>
                    (teleporterPosition - ((Rigidbody)beliefSet.PlayerRigidbody).position).magnitude < 0.4f));
            Tactic waitForTeleportOrApproachCurrentCellTactic = FirstOf(
                waitForTeleportTactic,
                approachCurrentCellAction.Lift());

            GoalStructure visitCurrentCell = new Goal(
                waitForTeleportOrApproachCurrentCellTactic,
                beliefSet => (currentCellPosition() - ((Rigidbody)beliefSet.PlayerRigidbody).position).magnitude <
                             1.5f);

            // Arrange ==> GoalStructure: Target the next cell until every cell has been targeted
            Action targetNextCellAction = new(_ =>
            {
                Debug.Log($"Reached cell at {currentCellPosition()}");
                currentCellToVisitIndex++;
            });

            GoalStructure visitedEveryCellGoal = new Goal(
                targetNextCellAction.Lift(),
                _ => currentCellToVisitIndex >= cellsToVisit.Length);

            // Arrange ==> GoalStructure: Visit every connected component
            GoalStructure visitCurrentCellAndVisitEveryCellGoalStructure =
                Seq(visitCurrentCell, visitedEveryCellGoal);

            // Act
            BdiAgent agent = new(rootBeliefSet, visitCurrentCellAndVisitEveryCellGoalStructure.Lift());
            AplibRunner testRunner = new(agent);

            yield return testRunner.Test();

            // Assert
            Assert.AreEqual(CompletionStatus.Success, agent.Status);
        }
    }
}
