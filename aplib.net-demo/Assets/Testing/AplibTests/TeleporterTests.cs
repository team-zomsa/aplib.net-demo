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
using static Aplib.Core.Combinators;
using Goal = Aplib.Core.Desire.Goals.Goal<Testing.AplibTests.TeleporterBeliefSet>;
using Action = Aplib.Core.Intent.Actions.Action<Testing.AplibTests.TeleporterBeliefSet>;
using Tactic = Aplib.Core.Intent.Tactics.Tactic<Testing.AplibTests.TeleporterBeliefSet>;
using PrimitiveTactic = Aplib.Core.Intent.Tactics.PrimitiveTactic<Testing.AplibTests.TeleporterBeliefSet>;
using GoalStructure = Aplib.Core.Desire.GoalStructures.GoalStructure<Testing.AplibTests.TeleporterBeliefSet>;
using BdiAgent = Aplib.Core.Agents.BdiAgent<Testing.AplibTests.TeleporterBeliefSet>;
using TransformPathfinderAction = Aplib.Integrations.Unity.Actions.TransformPathfinderAction<Testing.AplibTests.TeleporterBeliefSet>;

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
            Tactic approachTeleporter = new TransformPathfinderAction (
                objectQuery: beliefSet => beliefSet.PlayerRigidbody,
                location: rootBeliefSet.TeleporterInLocation,
                heightOffset: .7f);

            Action waitForTeleport = new(_ => Debug.Log("Waiting for teleport..."));

            Tactic walkOutOfTeleporter = new TransformPathfinderAction (
                objectQuery: beliefSet => beliefSet.PlayerRigidbody,
                location: beliefSet => beliefSet.TeleporterOutLocation + new Vector3(3, 0, 0),
                heightOffset: .7f);

            // Arrange ==> Tactics
            Tactic waitForTeleportTactic = new PrimitiveTactic(waitForTeleport,
                guard: beliefSet => (rootBeliefSet.TeleporterInLocation -
                                     ((Rigidbody)beliefSet.PlayerRigidbody).position).magnitude < 0.4f);
            Tactic teleportOrApproachTactic = FirstOf(waitForTeleportTactic, approachTeleporter);

            // Arrange ==> Goals and GoalStructures
            GoalStructure teleportOnceGoal = new Goal(teleportOrApproachTactic,
                beliefSet => (rootBeliefSet.TeleporterOutLocation - ((Rigidbody)beliefSet.PlayerRigidbody).position)
                    .magnitude < 0.2f);

            GoalStructure walkOutOfTeleporterGoal = new Goal(walkOutOfTeleporter,
                beliefSet => (rootBeliefSet.TeleporterOutLocation - ((Rigidbody)beliefSet.PlayerRigidbody).position)
                    .magnitude > 2f);

            GoalStructure teleportAndWalkOutSequence = Seq(teleportOnceGoal, walkOutOfTeleporterGoal);

            // Act
            BdiAgent agent = new(rootBeliefSet, teleportAndWalkOutSequence.Lift());
            AplibRunner testRunner = new(agent);

            yield return testRunner.Test();

            // Assert
            Assert.AreEqual(CompletionStatus.Success, agent.Status);
        }
    }
}
