#if !DataTable_Missing

using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// Materializes the result set as a single row.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    internal sealed class DataRowMaterializer<TCommand, TParameter> : Materializer<TCommand, TParameter, DataRow> where TCommand : DbCommand
        where TParameter : DbParameter
    {
        readonly RowOptions m_RowOptions;

        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="rowOptions">The row options.</param>
        public DataRowMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, RowOptions rowOptions)
            : base(commandBuilder)
        {
            m_RowOptions = rowOptions;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Globalization", "CA1306:SetLocaleForDataTypes")]
        public override DataRow Execute(object state = null)
        {
            var executionToken = Prepare();

            var table = new DataTable();
            executionToken.Execute(cmd =>
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    table.Load(reader);
                    return table.Rows.Count;
                }
            }, state);


            if (table.Rows.Count == 0)
            {
                if (m_RowOptions.HasFlag(RowOptions.AllowEmptyResults))
                    return null;
                else
                    throw new MissingDataException("No rows were returned");
            }
            else if (table.Rows.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
            {
                throw new UnexpectedDataException("Expected 1 row but received " + table.Rows.Count + " rows");
            }
            return table.Rows[0];
        }


        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Globalization", "CA1306:SetLocaleForDataTypes")]
        public override async Task<DataRow> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var executionToken = Prepare();

            var table = new DataTable();
            await executionToken.ExecuteAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    table.Load(reader);
                    return table.Rows.Count;
                }
            }, cancellationToken, state).ConfigureAwait(false);


            if (table.Rows.Count == 0)
            {
                if (m_RowOptions.HasFlag(RowOptions.AllowEmptyResults))
                    return null;
                else
                    throw new MissingDataException("No rows were returned");
            }
            else if (table.Rows.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
            {
                throw new UnexpectedDataException("Expected 1 row but received " + table.Rows.Count + " rows");
            }
            return table.Rows[0];
        }

        public override IReadOnlyList<string> DesiredColumns()
        {
            return AllColumns;
        }
    }
}
#endif
