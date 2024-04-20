using Aplib.Core;
using System.Collections;

namespace Aplib
{
    public class AplibRunner
    {
        /// <summary>
        /// The agent that the test runner is testing.
        /// </summary>
        private readonly IAgent _agent;

        public AplibRunner(IAgent agent)
        {
            _agent = agent;
        }

        /// <summary>
        /// Runs the test for the agent. The test continues until the agent's status is no longer Unfinished.
        /// </summary>
        /// <returns>An IEnumerator that can be used to control the execution of the test.</returns>
        public IEnumerator Test()
        {
            while (_agent.Status == CompletionStatus.Unfinished)
            {
                // Perform computation or update the agent here
                _agent.Update();

                // Wait for the next frame
                yield return null;
            }
        }
    }
}
