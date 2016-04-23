using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.DataSources
{

    /// <summary>
    /// Class DataSource.
    /// </summary>
    /// <typeparam name="TConnection">The type of the t connection.</typeparam>
    /// <typeparam name="TTransaction">The type of the t transaction.</typeparam>
    /// <typeparam name="TCommand">The type of the command used.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    /// <seealso cref="DataSource" />
    public abstract class DataSource<TConnection, TTransaction, TCommand, TParameter> : DataSource, ICommandDataSource<TCommand, TParameter>, IOperationDataSource<TConnection, TTransaction>
         where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TCommand : DbCommand
        where TParameter : DbParameter
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSource{TConnection, TTransaction,TCommand, TParameter}"/> class.
        /// </summary>
        /// <param name="settings">Optional settings object.</param>
        protected DataSource(DataSourceSettings settings) : base(settings) { }

        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="state">User supplied state.</param>
        protected internal abstract void Execute(CommandExecutionToken<TCommand, TParameter> executionToken, CommandImplementation<TCommand> implementation, object state);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User supplied state.</param>
        /// <returns>Task.</returns>
        protected internal abstract Task ExecuteAsync(CommandExecutionToken<TCommand, TParameter> executionToken, CommandImplementationAsync<TCommand> implementation, CancellationToken cancellationToken, object state);

        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="state">The state.</param>
        protected internal abstract int? Execute(OperationExecutionToken<TConnection, TTransaction> executionToken, OperationImplementation<TConnection, TTransaction> implementation, object state);

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">The state.</param>
        /// <returns>Task.</returns>
        protected internal abstract Task<int?> ExecuteAsync(OperationExecutionToken<TConnection, TTransaction> executionToken, OperationImplementationAsync<TConnection, TTransaction> implementation, CancellationToken cancellationToken, object state);

        void ICommandDataSource<TCommand, TParameter>.Execute(CommandExecutionToken<TCommand, TParameter> executionToken, CommandImplementation<TCommand> implementation, object state)
        {
            Execute(executionToken, implementation, state);
        }

        Task ICommandDataSource<TCommand, TParameter>.ExecuteAsync(CommandExecutionToken<TCommand, TParameter> executionToken, CommandImplementationAsync<TCommand> implementation, CancellationToken cancellationToken, object state)
        {
            return ExecuteAsync(executionToken, implementation, cancellationToken, state);
        }

        int? IOperationDataSource<TConnection, TTransaction>.Execute(OperationExecutionToken<TConnection, TTransaction> executionToken, OperationImplementation<TConnection, TTransaction> implementation, object state)
        {
            return Execute(executionToken, implementation, state);
        }

        Task<int?> IOperationDataSource<TConnection, TTransaction>.ExecuteAsync(OperationExecutionToken<TConnection, TTransaction> executionToken, OperationImplementationAsync<TConnection, TTransaction> implementation, CancellationToken cancellationToken, object state)
        {
            return ExecuteAsync(executionToken, implementation, cancellationToken, state);
        }
    }
}
