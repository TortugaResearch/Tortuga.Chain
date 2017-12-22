using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// This is the operation equivalent to the NonQueryMaterializer.
    /// </summary>
    /// <typeparam name="TConnection">The type of the t connection.</typeparam>
    /// <typeparam name="TTransaction">The type of the t transaction.</typeparam>
    public class Operation<TConnection, TTransaction> : ILink<int?>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {

        readonly DbOperationBuilder<TConnection, TTransaction> m_OperationBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="Operation{TConnection, TTransaction}"/> class.
        /// </summary>
        /// <param name="operationBuilder">The operation builder.</param>
        public Operation(DbOperationBuilder<TConnection, TTransaction> operationBuilder)
        {
            m_OperationBuilder = operationBuilder;
        }

        /// <summary>
        /// Occurs when an execution token has been prepared.
        /// </summary>
        /// <remarks>This is mostly used by appenders to override command behavior.</remarks>
        public event EventHandler<ExecutionTokenPreparedEventArgs> ExecutionTokenPrepared;

        /// <summary>
        /// Occurs when an execution token is about to be prepared.
        /// </summary>
        /// <remarks>This is mostly used by appenders to override SQL generation.</remarks>
        public event EventHandler<ExecutionTokenPreparingEventArgs> ExecutionTokenPreparing;

        /// <summary>
        /// Gets the data source that is associated with this materializer or appender.
        /// </summary>
        /// <value>The data source.</value>
        /// <remarks>This is only used for</remarks>
        public IDataSource DataSource => m_OperationBuilder.DataSource;
        string ILink<int?>.CommandText() => null;


        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
        public int? Execute(object state = null)
        {
            var token = Prepare();
            OperationImplementation<TConnection, TTransaction> implementation = m_OperationBuilder.Implementation;
            return token.Execute(implementation, state);

        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns>Task&lt;System.Nullable&lt;System.Int32&gt;&gt;.</returns>
        public Task<int?> ExecuteAsync(object state = null)
        {
            return ExecuteAsync(CancellationToken.None, state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns>Task&lt;System.Nullable&lt;System.Int32&gt;&gt;.</returns>
        public Task<int?> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var token = Prepare();
            OperationImplementationAsync<TConnection, TTransaction> implementation = m_OperationBuilder.ImplementationAsync;
            return token.ExecuteAsync(implementation, cancellationToken, state);
        }

        /// <summary>
        /// Prepares this operation for execution.
        /// </summary>
        /// <returns>ExecutionToken&lt;TCommand, TParameter&gt;.</returns>
        protected OperationExecutionToken<TConnection, TTransaction> Prepare()
        {
            ExecutionTokenPreparing?.Invoke(this, new ExecutionTokenPreparingEventArgs(m_OperationBuilder));

            var executionToken = m_OperationBuilder.Prepare();

            ExecutionTokenPrepared?.Invoke(this, new ExecutionTokenPreparedEventArgs(executionToken));

            return executionToken;
        }
    }
}
