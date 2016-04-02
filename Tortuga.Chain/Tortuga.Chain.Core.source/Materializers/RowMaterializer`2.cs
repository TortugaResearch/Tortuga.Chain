using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
    internal sealed class RowMaterializer<TCommand, TParameter> : Materializer<TCommand, TParameter, Row> where TCommand : DbCommand
        where TParameter : DbParameter
    {
        readonly RowOptions m_RowOptions;

        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="rowOptions">The row options.</param>
        public RowMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, RowOptions rowOptions)
            : base(commandBuilder)
        {
            m_RowOptions = rowOptions;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override Row Execute(object state = null)
        {
            var executionToken = Prepare();

            Table table = null;
            executionToken.Execute(cmd =>
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    table = new Table(reader);
                    return table.Rows.Count;
                }
            }, state);


            if (table.Rows.Count == 0)
            {
                if (m_RowOptions.HasFlag(RowOptions.AllowEmptyResults))
                    return null;
                else
                {
                    throw new MissingDataException("No rows were returned");
                }
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
        public override async Task<Row> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var executionToken = Prepare();

            Table table = null;
            await executionToken.ExecuteAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    table = new Table(reader);
                    return table.Rows.Count;
                }
            }, cancellationToken, state).ConfigureAwait(false);


            if (table.Rows.Count == 0)
            {
                if (m_RowOptions.HasFlag(RowOptions.AllowEmptyResults))
                    return null;
                else
                {
                    throw new MissingDataException("No rows were returned");
                }
            }
            else if (table.Rows.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
            {
                throw new UnexpectedDataException("Expected 1 row but received " + table.Rows.Count + " rows");
            }
            return table.Rows[0];
        }

        /// <summary>
        /// Returns the list of columns the materializer would like to have.
        /// </summary>
        /// <returns>
        /// IReadOnlyList&lt;System.String&gt;.
        /// </returns>
        /// <remarks>
        /// If AutoSelectDesiredColumns is returned, the command builder is allowed to choose which columns to return. If NoColumns is returned, the command builder should omit the SELECT/OUTPUT clause.
        /// </remarks>
        public override IReadOnlyList<string> DesiredColumns()
        {
            return AllColumns;
        }
    }
}
