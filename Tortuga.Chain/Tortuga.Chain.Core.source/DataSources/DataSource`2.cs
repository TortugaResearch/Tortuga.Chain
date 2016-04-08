using System;
using System.Data.Common;
using System.Threading.Tasks;
using Tortuga.Chain.Core;
namespace Tortuga.Chain.DataSources
{

    /// <summary>
    /// Class DataSource.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command used.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    public abstract class DataSource<TCommand, TParameter> : DataSource
        where TCommand : DbCommand
        where TParameter : DbParameter
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSource{TCommand, TParameter}"/> class.
        /// </summary>
        /// <param name="settings">Optional settings object.</param>
        protected DataSource(DataSourceSettings settings) : base(settings) { }

        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="state">User supplied state.</param>
        protected internal abstract void Execute(ExecutionToken<TCommand, TParameter> executionToken, Func<TCommand, int?> implementation, object state);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User supplied state.</param>
        /// <returns>Task.</returns>
        protected internal abstract Task ExecuteAsync(ExecutionToken<TCommand, TParameter> executionToken, Func<TCommand, Task<int?>> implementation, System.Threading.CancellationToken cancellationToken, object state);

    }
}
