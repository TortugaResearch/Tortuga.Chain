using System;
using System.Data.Common;

namespace Tortuga.Chain.Core
{
    /// <summary>
    /// Class CommandBuiltEventArgs.
    /// </summary>
    public class CommandBuiltEventArgs : EventArgs
    {
        private readonly DbCommand m_Command;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBuiltEventArgs"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandBuiltEventArgs(DbCommand command)
        {
            m_Command = command;
        }

        /// <summary>
        /// Gets the command that was just built.
        /// </summary>
        public DbCommand Command
        {
            get { return m_Command; }
        }
    }
}
