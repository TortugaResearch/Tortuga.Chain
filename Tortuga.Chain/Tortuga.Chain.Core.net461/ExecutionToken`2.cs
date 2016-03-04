using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain
{
    /// <summary>
    /// This class represents the actual preparation and execution of a command.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the command used.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public class ExecutionToken<TCommandType, TParameterType>
        where TCommandType : DbCommand
        where TParameterType : DbParameter
    {
        private readonly CommandType m_CommandType;
        private readonly DataSource<TCommandType, TParameterType> m_DataSource;
        private readonly string m_CommandText;
        private readonly string m_OperationName;
        private readonly IReadOnlyList<TParameterType> m_Parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionToken{TCommandType, TParameterType}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="operationName">Name of the operation. This is used for logging.</param>
        /// <param name="commandText">The SQL to be executed.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        public ExecutionToken(DataSource<TCommandType, TParameterType> dataSource, string operationName, string commandText, IReadOnlyList<TParameterType> parameters, CommandType commandType = CommandType.Text)
        {
            m_Parameters = parameters;
            m_CommandType = commandType;
            m_DataSource = dataSource;
            m_OperationName = operationName;
            m_CommandText = commandText;
        }


        /// <summary>
        /// Executes the specified implementation.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="state">The state.</param>
        public void Execute(Func<TCommandType, int?> implementation, object state)
        {
            m_DataSource.Execute(this, implementation, state);
        }

        /// <summary>
        /// Executes the specified implementation asynchronously.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">The state.</param>
        /// <returns>Task.</returns>
        public Task ExecuteAsync(Func<TCommandType, Task<int?>> implementation, CancellationToken cancellationToken, object state)
        {
            return m_DataSource.ExecuteAsync(this, implementation, cancellationToken, state);
        }

        /// <summary>
        /// Gets the command text, which is usually SQL.
        /// </summary>
        /// <value>The command text.</value>
        public string CommandText
        {
            get { return m_CommandText; }
        }

        /// <summary>
        /// Gets the type of the command.
        /// </summary>
        /// <value>The type of the command.</value>
        public CommandType CommandType
        {
            get { return m_CommandType; }
        }

        /// <summary>
        /// Gets the name of the operation.
        /// </summary>
        /// <value>The name of the operation.</value>
        public string OperationName
        {
            get { return m_OperationName; }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public IReadOnlyList<TParameterType> Parameters
        {
            get { return m_Parameters; }
        }
    }
}
