using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.DataSources
{



    /// <summary>
    /// This interface exposes the execute command methods. 
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter.</typeparam>
    /// <seealso cref="IDataSource" />
    /// <remarks>This is for internal use only.</remarks>
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
        int? Execute(CommandExecutionToken<TCommand, TParameter> executionToken, CommandImplementation<TCommand> implementation, object state);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User supplied state.</param>
        /// <returns>Task.</returns>
        Task<int?> ExecuteAsync(CommandExecutionToken<TCommand, TParameter> executionToken, CommandImplementationAsync<TCommand> implementation, CancellationToken cancellationToken, object state);

    }
}
