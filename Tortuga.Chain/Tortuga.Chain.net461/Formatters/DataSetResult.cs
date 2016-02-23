using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Formatters
{
    /// <summary>
    /// Formats the result set as a DataSet.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public class DataSetResult<TCommandType, TParameterType> : Formatter<TCommandType, TParameterType, DataSet> where TCommandType : DbCommand
        where TParameterType : DbParameter
    {
        readonly string[] m_TableNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSetResult{TCommandType, TParameterType}"/> class.
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="tableNames">The table names.</param>
        public DataSetResult(DbCommandBuilder<TCommandType, TParameterType> commandBuilder, string[] tableNames)
            : base(commandBuilder)
        {
            m_TableNames = tableNames;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Globalization", "CA1306:SetLocaleForDataTypes")]
        public override DataSet Execute(object state = null)
        {
            DataSet ds = new DataSet();
            ExecuteCore(cmd =>
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    ds.Load(reader, LoadOption.OverwriteChanges, m_TableNames);
                    return ds.Tables.Cast<DataTable>().Sum(t => t.Rows.Count);
                }
            }, state);

            return ds;
        }


        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<DataSet> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            DataSet ds = new DataSet();
            await ExecuteCoreAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken))
                {
                    ds.Load(reader, LoadOption.OverwriteChanges, m_TableNames);
                    return ds.Tables.Cast<DataTable>().Sum(t => t.Rows.Count);
                }
            }, cancellationToken, state);

            return ds;
        }
    }
}
