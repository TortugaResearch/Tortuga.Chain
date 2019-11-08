using System;
using System.Data.Common;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// Materializes the result set as a decimal.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    internal sealed class DecimalMaterializer<TCommand, TParameter> : ScalarMaterializer<TCommand, TParameter, decimal> where TCommand : DbCommand
        where TParameter : DbParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalMaterializer{TCommand, TParameter}"/> class.
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="columnName">Name of the desired column.</param>
        public DecimalMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName = null)
            : base(commandBuilder, columnName)
        { }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override decimal Execute(object? state = null)
        {
            object? temp = null;
            ExecuteCore(cmd => temp = cmd.ExecuteScalar(), state);
            if (temp == DBNull.Value)
                throw new MissingDataException("Unexpected null result");

            return Convert.ToDecimal(temp, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<decimal> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
        {
            object? temp = null;
            await ExecuteCoreAsync(async cmd => temp = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false), cancellationToken, state).ConfigureAwait(false);
            if (temp == DBNull.Value)
                throw new MissingDataException("Unexpected null result");

            return Convert.ToDecimal(temp, CultureInfo.InvariantCulture);
        }
    }
}