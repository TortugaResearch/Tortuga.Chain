using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Formatters
{
    /// <summary>
    /// Formats the result set as an instance of the indicated type.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <seealso cref="Formatters.Formatter{TCommandType, TParameterType, TModel}" />
    public class ModelResult<TCommandType, TParameterType, TModel> : Formatter<TCommandType, TParameterType, TModel>
        where TCommandType : DbCommand
        where TModel : class, new()
        where TParameterType : DbParameter
    {

        readonly RowOptions m_RowOptions;

        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="rowOptions">The row options.</param>
        public ModelResult(DbCommandBuilder<TCommandType, TParameterType> commandBuilder, RowOptions rowOptions)
            : base(commandBuilder)
        {
            m_RowOptions = rowOptions;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override TModel Execute(object state = null)
        {
            Table table = null;
            var executionToken = Prepare();
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
            return table.ToModels<TModel>().First();
        }


        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        /// <exception cref="DataException">Unexpected null result</exception>
        public override async Task<TModel> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            Table table = null;

            var executionToken = Prepare();
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
            return table.ToModels<TModel>().First();
        }

        /// <summary>
        /// Returns the list of columns the result formatter would like to have.
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyList<string> DesiredColumns()
        {
            return MetadataCache.GetMetadata(typeof(TModel)).ColumnsFor;
        }

    }
}
