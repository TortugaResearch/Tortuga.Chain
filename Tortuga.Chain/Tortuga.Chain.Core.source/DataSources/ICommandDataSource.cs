using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.DataSources
{



    public interface ICommandDataSource<TCommand, TParameter> : IDataSource
     where TCommand : DbCommand
        where TParameter : DbParameter
    {
        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="state">User supplied state.</param>
        void Execute(ExecutionToken<TCommand, TParameter> executionToken, CommandImplementation<TCommand> implementation, object state);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User supplied state.</param>
        /// <returns>Task.</returns>
        Task ExecuteAsync(ExecutionToken<TCommand, TParameter> executionToken, CommandImplementationAsync<TCommand> implementation, CancellationToken cancellationToken, object state);

    }
}
