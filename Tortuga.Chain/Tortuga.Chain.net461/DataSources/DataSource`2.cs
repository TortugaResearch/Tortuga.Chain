using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Tortuga.Chain.DataSources
{
    /// <summary>
    /// Class DataSource.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the command used.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public abstract class DataSource<TCommandType, TParameterType>
        where TCommandType : DbCommand
        where TParameterType : DbParameter
    {

        /// <summary>
        /// Gets or sets the default connection timeout.
        /// </summary>
        /// <value>
        /// The default connection timeout.
        /// </value>
        public TimeSpan? DefaultConnectionTimeout { get; set; }

        /// <summary>
        /// Gets or sets the default command timeout.
        /// </summary>
        /// <value>The default command timeout.</value>
        public TimeSpan? DefaultCommandTimeout { get; set; }

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

        /// <summary>
        /// Gets the name of the data source.
        /// </summary>
        /// <value>
        /// The name of the data source.
        /// </value>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether strict mode is enabled.
        /// </summary>
        /// <remarks>Strict mode requires all properties that don't represent columns to be marked with the NotMapped attribute.</remarks>
        public bool StrictMode { get; set; }

    }
}
