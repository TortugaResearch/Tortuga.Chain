using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// Materializes the result set as a TableSet.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public class TableSetMaterializer<TCommandType, TParameterType> : Materializer<TCommandType, TParameterType, TableSet> where TCommandType : DbCommand
        where TParameterType : DbParameter
    {
        readonly string[] m_TableNames;

        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="tableNames">The table names.</param>
        public TableSetMaterializer(DbCommandBuilder<TCommandType, TParameterType> commandBuilder, string[] tableNames)
            : base(commandBuilder)
        {
            m_TableNames = tableNames;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override TableSet Execute(object state = null)
        {
            TableSet result = null;

            var executionToken = Prepare();
            executionToken.Execute(cmd =>
                        {
                            using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                            {
                                result = new TableSet(reader, m_TableNames);
                                return result.Sum(t => t.Rows.Count);
                            }
                        }, state);

            if (m_TableNames.Length > result.Count)
            {
                var ex = new DataException(string.Format("Expected at least {0} tables but received {1} tables", m_TableNames.Length, result.Count));
                ex.Data["DataSource"] = CommandBuilder.DataSource.Name;
                ex.Data["Operation"] = executionToken.OperationName;
                ex.Data["CommandText"] = executionToken.CommandText;
                ex.Data["Parameters"] = executionToken.Parameters;
                ex.Data["TableCount"] = result.Count;
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<TableSet> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            TableSet result = null;

            var executionToken = Prepare();

            await executionToken.ExecuteAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess).ConfigureAwait(false))
                {
                    result = new TableSet(reader, m_TableNames);
                    return result.Sum(t => t.Rows.Count);
                }
            }, cancellationToken, state).ConfigureAwait(false);

            if (m_TableNames.Length > result.Count)
            {
                var ex = new DataException(string.Format("Expected at least {0} tables but received {1} tables", m_TableNames.Length, result.Count));
                ex.Data["DataSource"] = CommandBuilder.DataSource.Name;
                ex.Data["Operation"] = executionToken.OperationName;
                ex.Data["CommandText"] = executionToken.CommandText;
                ex.Data["Parameters"] = executionToken.Parameters;
                ex.Data["TableCount"] = result.Count;
                throw ex;
            }

            return result;
        }
    }
}
