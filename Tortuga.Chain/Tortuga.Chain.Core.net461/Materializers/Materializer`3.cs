using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Materializers
{

    /// <summary>
    /// This is the base class for materializers that return a value. Most operation are not executed without first attaching a materializer subclass.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    /// <typeparam name="TResultType">The type of the t result type.</typeparam>
    /// <seealso cref="Materializer{TCommandType, TParameterType}" />
    public abstract class Materializer<TCommandType, TParameterType, TResultType> : Materializer<TCommandType, TParameterType>, ILink<TResultType>
        where TCommandType : DbCommand
        where TParameterType : DbParameter
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Materializer{TCommandType, TParameterType, TResultType}"/> class.
        /// </summary>
        /// <param name="commandBuilder">The associated operation.</param>
        protected Materializer(DbCommandBuilder<TCommandType, TParameterType> commandBuilder) : base(commandBuilder) { }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public abstract TResultType Execute(object state = null);

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public async Task<TResultType> ExecuteAsync(object state = null)
        {
            return await ExecuteAsync(CancellationToken.None, state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public abstract Task<TResultType> ExecuteAsync(CancellationToken cancellationToken, object state = null);

        /// <summary>
        /// Gets the data source that is associated with this materilizer or appender.
        /// </summary>
        /// <value>The data source.</value>
        public DataSource DataSource
        {
            get { return CommandBuilder.DataSource; }
        }

        /// <summary>
        /// Returns the generated SQL statement without executing it.
        /// </summary>
        /// <returns></returns>
        public string Sql()
        {
            return Prepare().CommandText;
        }
    }
}

