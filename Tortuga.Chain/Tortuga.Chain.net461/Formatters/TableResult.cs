using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Formatters
{
    /// <summary>
    /// Formats the result set as a Table.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public class TableResult<TCommandType, TParameterType> : Formatter<TCommandType, TParameterType, Table> where TCommandType : DbCommand
        where TParameterType : DbParameter
    {
        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The associated operation.</param>
        public TableResult(DbCommandBuilder<TCommandType, TParameterType> commandBuilder)
            : base(commandBuilder)
        { }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override Table Execute(object state)
        {
            Table t = null;
            ExecuteCore(cmd =>
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    t = new Table(reader);
                    return t.Rows.Count;
                }
            }, state);

            return t;
        }


        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<Table> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            Table t = null;
            await ExecuteCoreAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    t = new Table(reader); 
                    return t.Rows.Count;
                }
            }, cancellationToken, state).ConfigureAwait(false);

            return t;
        }
    }
}
