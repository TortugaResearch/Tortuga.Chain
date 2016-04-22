using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Materializers
{
    public class Operation<TConnection, TTransaction> : ILink<int?>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {

        private readonly DbOperationBuilder<TConnection, TTransaction> m_OperationBuilder;

        public Operation(DbOperationBuilder<TConnection, TTransaction> operationBuilder)
        {
            m_OperationBuilder = operationBuilder;
        }

        public IDataSource DataSource
        {
            get { return m_OperationBuilder.DataSource; }
        }

        public event EventHandler<ExecutionTokenPreparedEventArgs> ExecutionTokenPrepared;
        public event EventHandler<ExecutionTokenPreparingEventArgs> ExecutionTokenPreparing;

        public string CommandText()
        {
            throw new NotImplementedException();
        }

        public int? Execute(object state = null)
        {
            var token = Prepare();
            OperationImplementation<TConnection, TTransaction> implementation = m_OperationBuilder.Implementation;
            return token.Execute(implementation, state);

        }

        public Task<int?> ExecuteAsync(object state = null)
        {
            return ExecuteAsync(CancellationToken.None, state);
        }

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
