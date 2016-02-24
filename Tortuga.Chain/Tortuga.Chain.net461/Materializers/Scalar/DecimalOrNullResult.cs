using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// Formats the result set as a decimal.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public class DecimalOrNullMaterializer<TCommandType, TParameterType> : SingleColumnFormatter<TCommandType, TParameterType, decimal?> where TCommandType : DbCommand
        where TParameterType : DbParameter

    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalOrNullMaterializer{TCommandType, TParameterType}"/> class.
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="columnName">Name of the desired column.</param>
        public DecimalOrNullMaterializer(DbCommandBuilder<TCommandType, TParameterType> commandBuilder, string columnName = null)
            : base(commandBuilder, columnName)
        { }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override decimal? Execute(object state = null)
        {
            object temp = null;
            ExecuteCore(cmd => temp = cmd.ExecuteScalar(), state);
            if (temp == null || temp == DBNull.Value)
                return null;

            return (decimal)temp;
        }


        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<decimal?> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            object temp = null;
            await ExecuteCoreAsync(async cmd => temp = await cmd.ExecuteScalarAsync(cancellationToken), cancellationToken, state).ConfigureAwait(false);
            if (temp == null || temp == DBNull.Value)
                return null;

            return (decimal)temp;
        }
    }
}
