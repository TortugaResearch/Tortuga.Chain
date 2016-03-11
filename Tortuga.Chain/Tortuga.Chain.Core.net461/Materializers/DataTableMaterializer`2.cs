using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// Materializes the result set as a DataTable.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    public class DataTableMaterializer<TCommand, TParameter> : Materializer<TCommand, TParameter, DataTable> where TCommand : DbCommand
        where TParameter : DbParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableMaterializer{TCommand, TParameter}"/> class.
        /// </summary>
        /// <param name="commandBuilder">The associated operation.</param>
        public DataTableMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder)
            : base(commandBuilder)
        { }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Globalization", "CA1306:SetLocaleForDataTypes")]
        public override DataTable Execute(object state = null)
        {
            DataTable dt = new DataTable();
            ExecuteCore(cmd =>
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    dt.Load(reader);
                    return dt.Rows.Count;
                }
            }, state);

            return dt;
        }



        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<DataTable> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            DataTable dt = new DataTable();
            await ExecuteCoreAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken))
                {
                    dt.Load(reader);
                    return dt.Rows.Count;
                }
            }, cancellationToken, state);

            return dt;
        }
    }
}
