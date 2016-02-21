using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Tortuga.Chain.Formatters
{
    /// <summary>
    /// Formats the result set as a single row.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public class RowResult<TCommandType, TParameterType> : Formatter<TCommandType, TParameterType, IReadOnlyDictionary<string, object>> where TCommandType : DbCommand
        where TParameterType : DbParameter
    {
        readonly RowOptions m_RowOptions;

        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="rowOptions">The row options.</param>
        public RowResult(DbCommandBuilder<TCommandType, TParameterType> commandBuilder, RowOptions rowOptions)
            : base(commandBuilder)
        {
            m_RowOptions = rowOptions;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyDictionary<string, object> Execute(object state = null)
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
                    var ex = new DataException("No rows were returned");
                    ex.Data["DataSource"] = CommandBuilder.DataSource.Name;
                    ex.Data["Operation"] = executionToken.OperationName;
                    ex.Data["CommandText"] = executionToken.CommandText;
                    ex.Data["Parameters"] = executionToken.Parameters;
                    throw ex;
                }
            }
            else if (table.Rows.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
            {
                var ex = new DataException("Expected 1 row but received " + table.Rows.Count + " rows");
                ex.Data["DataSource"] = CommandBuilder.DataSource.Name;
                ex.Data["Operation"] = executionToken.OperationName;
                ex.Data["CommandText"] = executionToken.CommandText;
                ex.Data["Parameters"] = executionToken.Parameters;
                ex.Data["RowCount"] = table.Rows.Count;
                throw ex;
            }
            return table.Rows[0];
        }


        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<IReadOnlyDictionary<string, object>> ExecuteAsync(CancellationToken cancellationToken, object state = null)
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
                    var ex = new DataException("No rows were returned");
                    ex.Data["DataSource"] = CommandBuilder.DataSource.Name;
                    ex.Data["Operation"] = executionToken.OperationName;
                    ex.Data["CommandText"] = executionToken.CommandText;
                    ex.Data["Parameters"] = executionToken.Parameters;
                    throw ex;
                }
            }
            else if (table.Rows.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
            {
                var ex = new DataException("Expected 1 row but received " + table.Rows.Count + " rows");
                ex.Data["DataSource"] = CommandBuilder.DataSource.Name;
                ex.Data["Operation"] = executionToken.OperationName;
                ex.Data["CommandText"] = executionToken.CommandText;
                ex.Data["Parameters"] = executionToken.Parameters;
                ex.Data["RowCount"] = table.Rows.Count;
                throw ex;
            }
            return table.Rows[0];
        }
    }
}
