using Aplib.Core;
using Aplib.Core.Agents;
using Aplib.Core.Belief.Beliefs;
using Aplib.Core.Belief.BeliefSets;
using Aplib.Core.Desire.DesireSets;
using Aplib.Core.Desire.Goals;
using Aplib.Core.Desire.GoalStructures;
using Aplib.Core.Intent.Actions;
using Aplib.Core.Intent.Tactics;
using Aplib.Integrations.Unity;
using Aplib.Integrations.Unity.Actions;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Testing.AplibTests
{
    public class TeleporterBeliefSet : BeliefSet
    {
        /// <summary>
        /// The player object in the scene.
        /// </summary>
        public Belief<GameObject, Rigidbody> PlayerRigidbody = new(
            reference: GameObject.Find("Player"),
            x => x.GetComponent<Rigidbody>());

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
                objectQuery: beliefSet => beliefSet.PlayerRigidbody,
                location: rootBeliefSet.TeleporterInLocation,
                heightOffset: .7f);

            Action<TeleporterBeliefSet> waitForTeleportAction = new(_ => { Debug.Log("Waiting for teleport..."); });

            TransformPathfinderAction<TeleporterBeliefSet> walkOutOfTeleporter = new(
                objectQuery: beliefSet => beliefSet.PlayerRigidbody,
                location: beliefSet => beliefSet.TeleporterOutLocation + new Vector3(3, 0, 0),
                heightOffset: .7f);

            // Arrange ==> Tactics
            PrimitiveTactic<TeleporterBeliefSet> approachTeleporterTactic = new(approachTeleporterAction);
            PrimitiveTactic<TeleporterBeliefSet> waitForTeleportTactic = new(waitForTeleportAction,
                guard: beliefSet => (rootBeliefSet.TeleporterInLocation - ((Rigidbody)beliefSet.PlayerRigidbody).position)
                    .magnitude < 0.4f);
            PrimitiveTactic<TeleporterBeliefSet> walkOutOfTeleporterTactic = new(walkOutOfTeleporter);
            FirstOfTactic<TeleporterBeliefSet> teleportOrApproachTactic = new(new Metadata("Teleport or approach teleporter"),
                waitForTeleportTactic,
                approachTeleporterTactic);

            // Arrange ==> Goals
            Goal<TeleporterBeliefSet> teleportOnceGoal = new(teleportOrApproachTactic,
                beliefSet => (rootBeliefSet.TeleporterOutLocation - ((Rigidbody)beliefSet.PlayerRigidbody).position)
                    .magnitude < 0.2f);

            Goal<TeleporterBeliefSet> walkOutOfTeleporterGoal = new(walkOutOfTeleporterTactic,
                beliefSet => (rootBeliefSet.TeleporterOutLocation - ((Rigidbody)beliefSet.PlayerRigidbody).position)
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
    }
}
