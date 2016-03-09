using System;

namespace Tortuga.Chain.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecutionTokenPreparedEventArgs : EventArgs
    {
        private readonly ExecutionToken m_ExecutionToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBuiltEventArgs" /> class.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        public ExecutionTokenPreparedEventArgs(ExecutionToken executionToken)
        {
            m_ExecutionToken = executionToken;
        }

        /// <summary>
        /// Gets the execution token.
        /// </summary>
        /// <value>The execution token.</value>
        public ExecutionToken ExecutionToken
        {
            get { return m_ExecutionToken; }
        }
    }
}
