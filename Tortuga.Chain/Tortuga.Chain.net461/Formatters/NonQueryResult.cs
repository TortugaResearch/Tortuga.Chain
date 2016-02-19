using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Tortuga.Chain.Formatters
{
    /// <summary>
    /// This class indicates the associated operation should be executed without returning a result set.
    /// </summary>
    public class NonQueryResult<TCommandType, TParameterType> : Formatter<TCommandType, TParameterType>
        where TCommandType : DbCommand
        where TParameterType : DbParameter
    {
        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The associated command builder.</param>
        public NonQueryResult(DbCommandBuilder<TCommandType, TParameterType> commandBuilder)
            : base(commandBuilder)
        { }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        public void Execute(object state = null)
        {
            ExecuteCore(cmd => cmd.ExecuteNonQuery(), state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public Task ExecuteAsync(object state = null)
        {
            return ExecuteAsync(CancellationToken.None, state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public Task ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            return ExecuteCoreAsync(async cmd => await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false), cancellationToken, state);
        }

    }

}
