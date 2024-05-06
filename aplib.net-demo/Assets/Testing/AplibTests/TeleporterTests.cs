using Aplib;
using Aplib.Core;
using Aplib.Core.Belief;
using Aplib.Core.Desire;
using Aplib.Core.Desire.Goals;
using Aplib.Core.Intent.Actions;
using Aplib.Core.Intent.Tactics;
using Aplib.Integrations.Unity.Actions;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.AplibTests
{
    public class TeleporterBeliefSet : BeliefSet
    {
        /// <summary>
        /// The player object in the scene.
        /// </summary>
        public Belief<GameObject, Transform> PlayerTransform = new(
            reference: GameObject.Find("Player"),
            x => x.transform);

        public Belief<Teleporter.Teleporter, Vector3> TeleporterInLocation = new(
            reference: GameObject.Find("Teleporter 1").GetComponent<Teleporter.Teleporter>(),
            x => x.LandingPoint);
        public Belief<Teleporter.Teleporter, Vector3> TeleporterOutLocation = new(
            reference: GameObject.Find("Teleporter 2").GetComponent<Teleporter.Teleporter>(),
            x => x.LandingPoint);
    }

    public class TeleporterAplibTests
    {
        private const string _sceneName = "TeleporterTestScene";

        [SetUp]
        public void SetUp()
        {
            Debug.Log($"Starting {nameof(TeleporterAplibTests)}");
            SceneManager.LoadScene(_sceneName);
        }

        [UnityTest]
        public IEnumerator TeleportOnce()
        {
            // Arrange
            TeleporterBeliefSet rootBeliefSet = new();

            // Arrange ==> Actions
            TransformPathfinderAction<TeleporterBeliefSet> approachTeleporterAction = new(
                objectQuery: beliefSet => beliefSet.PlayerTransform,
                location: rootBeliefSet.TeleporterInLocation,
                heightOffset: .7f);

            Action<TeleporterBeliefSet> waitForTeleportAction = new(
                effect: _ => { Debug.Log("Waiting for teleport..."); },
                guard: beliefSet => (rootBeliefSet.TeleporterInLocation - ((Transform)beliefSet.PlayerTransform).position)
                    .magnitude < 0.4f);

            TransformPathfinderAction<TeleporterBeliefSet> walkOutOfTeleporter = new(
                objectQuery: beliefSet => beliefSet.PlayerTransform,
                location: beliefSet => beliefSet.TeleporterOutLocation + new Vector3(3, 0, 0),
                heightOffset: .7f);

            // Arrange ==> Tactics
            PrimitiveTactic<TeleporterBeliefSet> approachTeleporterTactic = new(approachTeleporterAction);
            PrimitiveTactic<TeleporterBeliefSet> waitForTeleportTactic = new(waitForTeleportAction);
            PrimitiveTactic<TeleporterBeliefSet> walkOutOfTeleporterTactic = new(walkOutOfTeleporter);
            FirstOfTactic<TeleporterBeliefSet> teleportOrApproachTactic = new(new Metadata("Teleport or approach teleporter"),
                waitForTeleportTactic,
                approachTeleporterTactic);

            // Arrange ==> Goals
            Goal<TeleporterBeliefSet> teleportOnceGoal = new(teleportOrApproachTactic,
                beliefSet => (rootBeliefSet.TeleporterOutLocation - ((Transform)beliefSet.PlayerTransform).position)
                    .magnitude < 0.2f);

            Goal<TeleporterBeliefSet> walkOutOfTeleporterGoal = new(walkOutOfTeleporterTactic,
                beliefSet => (rootBeliefSet.TeleporterOutLocation - ((Transform)beliefSet.PlayerTransform).position)
                    .magnitude > 2f);

            // Arrange ==> GoalStructures
            PrimitiveGoalStructure<TeleporterBeliefSet> teleportOnceGoalStructurePrimitive = new(teleportOnceGoal);
            RepeatGoalStructure<TeleporterBeliefSet> teleportOnceGoalStructure = new(teleportOnceGoalStructurePrimitive);

            PrimitiveGoalStructure<TeleporterBeliefSet> walkOutOfTeleporterGoalStructurePrimitive = new(walkOutOfTeleporterGoal);
            RepeatGoalStructure<TeleporterBeliefSet> walkOutOfTeleporterGoalStructure = new(walkOutOfTeleporterGoalStructurePrimitive);

            SequentialGoalStructure<TeleporterBeliefSet> teleportAndWalkOutSequence = new(teleportOnceGoalStructure, walkOutOfTeleporterGoalStructure);

            // Arrange ==> DesireSet
            DesireSet<TeleporterBeliefSet> desireSet = new(teleportAndWalkOutSequence);

            // Act
            BdiAgent<TeleporterBeliefSet> agent = new(rootBeliefSet, desireSet);
            AplibRunner testRunner = new(agent);

            yield return testRunner.Test();

            // Assert
            Assert.AreEqual(CompletionStatus.Success, agent.Status);
        }

        [TearDown]
        public void TearDown()
        {
            Debug.Log($"Finished {nameof(TeleporterAplibTests)}");
            SceneManager.UnloadSceneAsync(_sceneName);
        }
    }
}
