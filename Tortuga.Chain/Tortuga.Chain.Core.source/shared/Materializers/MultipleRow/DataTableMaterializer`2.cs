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
    /// Materializes the result set as a DataTable.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    internal sealed class DataTableMaterializer<TCommand, TParameter> : Materializer<TCommand, TParameter, DataTable> where TCommand : DbCommand
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
        /// Returns the list of columns the materializer would like to have.
        /// </summary>
        /// <returns>
        /// IReadOnlyList&lt;System.String&gt;.
        /// </returns>
        /// <remarks>
        /// If AutoSelectDesiredColumns is returned, the command builder is allowed to choose which columns to return. If NoColumns is returned, the command builder should omit the SELECT/OUTPUT clause.
        /// </remarks>
        public override IReadOnlyList<string> DesiredColumns() => AllColumns;

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        [SuppressMessage("Microsoft.Globalization", "CA1306:SetLocaleForDataTypes")]
        public override DataTable Execute(object state = null)
        {
            var ds = new DataSet() { EnforceConstraints = false /*needed for PostgreSql*/};
            var dt = new DataTable();
            ds.Tables.Add(dt);
            Prepare().Execute(cmd =>
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
            var ds = new DataSet() { EnforceConstraints = false /*needed for PostgreSql*/};
            var dt = new DataTable();
            ds.Tables.Add(dt);
            await Prepare().ExecuteAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    dt.Load(reader);
                    return dt.Rows.Count;
                }
            }, cancellationToken, state);

            return dt;
        }
    }
}
#endif