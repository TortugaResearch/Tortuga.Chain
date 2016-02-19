using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Tortuga.Chain.Formatters
{
    /// <summary>
    /// Formats the result set as an integer.
    /// </summary>
    /// <typeparam name="TTCommandType">The type of the tt command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public class Int64OrNullResult<TTCommandType, TParameterType> : SingleColumnFormatter<TTCommandType, TParameterType, long?> where TTCommandType : DbCommand
        where TParameterType : DbParameter
    {
        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="columnName">Name of the desired column.</param>
        public Int64OrNullResult(DbCommandBuilder<TTCommandType, TParameterType> commandBuilder, string columnName = null)
            : base(commandBuilder, columnName)
        { }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override long? Execute(object state = null)
        {
            object temp = null;
            ExecuteCore(cmd => temp = cmd.ExecuteScalar(), state);
            if (temp == null || temp == DBNull.Value)
                return null;

            return Convert.ToInt64(temp);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<long?> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            object temp = null;
            await ExecuteCoreAsync(async cmd => temp = await cmd.ExecuteScalarAsync(cancellationToken), cancellationToken, state).ConfigureAwait(false);
            if (temp == null || temp == DBNull.Value)
                return null;

            return Convert.ToInt64(temp);
        }
    }
}
