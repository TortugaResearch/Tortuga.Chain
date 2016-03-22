using System.Data;
using Tortuga.Chain.DataSources;
using System.Data.Common;
using System;
namespace Tortuga.Chain.Core
{
    /// <summary>
    /// This class represents the actual preparation and execution of a command.
    /// </summary>

    public abstract class ExecutionToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionToken" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="operationName">Name of the operation.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        protected ExecutionToken(DataSource dataSource, string operationName, string commandText, CommandType commandType)
        {
            DataSource = dataSource;
            OperationName = operationName;
            CommandText = commandText;
            CommandType = commandType;
        }

        /// <summary>
        /// Gets the command text, which is usually SQL.
        /// </summary>
        /// <value>The command text.</value>
        public string CommandText { get; }

        /// <summary>
        /// Gets the type of the command.
        /// </summary>
        /// <value>The type of the command.</value>
        public CommandType CommandType { get; }

        /// <summary>
        /// Gets the name of the operation being performed.
        /// </summary>
        public string OperationName { get; }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public DataSource DataSource { get; }

        /// <summary>
        /// Occurs when a command has been built.
        /// </summary>
        /// <remarks>This is mostly used by appenders to override command behavior.</remarks>
        public event EventHandler<CommandBuiltEventArgs> CommandBuilt;

        internal void RaiseCommandBuild(DbCommand command)
        {
            CommandBuilt?.Invoke(this, new CommandBuiltEventArgs(command));
        }
    }
}
