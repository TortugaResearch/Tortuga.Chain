using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.DataSources
{





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
