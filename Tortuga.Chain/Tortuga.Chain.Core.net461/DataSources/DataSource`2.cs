using System;
using System.Data.Common;
using System.Threading.Tasks;
using Tortuga.Chain.Core;
namespace Tortuga.Chain.DataSources
{

    /// <summary>
    /// Class DataSource.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the command used.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public abstract class DataSource<TCommandType, TParameterType> : DataSource
        where TCommandType : DbCommand
        where TParameterType : DbParameter
    {

        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="state">User supplied state.</param>
        protected internal abstract void Execute(ExecutionToken<TCommandType, TParameterType> executionToken, Func<TCommandType, int?> implementation, object state);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User supplied state.</param>
        /// <returns>Task.</returns>
        protected internal abstract Task ExecuteAsync(ExecutionToken<TCommandType, TParameterType> executionToken, Func<TCommandType, Task<int?>> implementation, System.Threading.CancellationToken cancellationToken, object state);

    }
}
