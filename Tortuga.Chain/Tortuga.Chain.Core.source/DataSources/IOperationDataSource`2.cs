using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.DataSources
{
    /// <summary>
    /// This interface exposes the execute operation methods. 
    /// </summary>
    /// <typeparam name="TConnection">The type of the t connection.</typeparam>
    /// <typeparam name="TTransaction">The type of the t transaction.</typeparam>
    /// <seealso cref="IDataSource" />
    /// <remarks>This is for internal use only.</remarks>
    public interface IOperationDataSource<TConnection, TTransaction> : IDataSource
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {

        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="state">The state.</param>
        /// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
        int? Execute(OperationExecutionToken<TConnection, TTransaction> executionToken, OperationImplementation<TConnection, TTransaction> implementation, object state);

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">The state.</param>
        /// <returns>Task.</returns>
        Task<int?> ExecuteAsync(OperationExecutionToken<TConnection, TTransaction> executionToken, OperationImplementationAsync<TConnection, TTransaction> implementation, CancellationToken cancellationToken, object state);
    }
}
