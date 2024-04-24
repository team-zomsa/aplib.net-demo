using Aplib;
using Aplib.Core;
using Moq;
using NUnit.Framework;
using System.Collections;

namespace Testing.Aplib.Integrations.Unity
{
    public class AplibRunnerTests
    {
        [Test]
        public void Test_ShouldCallAgentUpdate_WhenAgentStatusIsUnfinished()
        {
            // Arrange
            Mock<IAgent> mockAgent = new();
            mockAgent.SetupSequence(agent => agent.Status)
                .Returns(CompletionStatus.Unfinished)
                .Returns(CompletionStatus.Success);
            mockAgent.Setup(agent => agent.Update());

            AplibRunner aplibRunner = new(mockAgent.Object);

            // Act
            IEnumerator enumerator = aplibRunner.Test();
            while (enumerator.MoveNext())
            {
                // Do nothing
            }

            // Assert
            mockAgent.Verify(agent => agent.Update(), Times.Once);
        }
    }
}
