using System;

namespace Tortuga.Chain.Core
{
    /// <summary>
    /// This occurs just after an execution token is prepared.
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class ExecutionTokenPreparedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBuiltEventArgs" /> class.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        public ExecutionTokenPreparedEventArgs(ExecutionToken executionToken)
        {
            ExecutionToken = executionToken;
        }

        /// <summary>
        /// Gets the execution token.
        /// </summary>
        /// <value>The execution token.</value>
        public ExecutionToken ExecutionToken { get; }
    }



}
