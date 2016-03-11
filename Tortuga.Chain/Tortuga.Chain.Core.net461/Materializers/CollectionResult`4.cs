using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// Materializes the result set as a collection of the indicated type.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    /// <typeparam name="TObject">The type of the object returned.</typeparam>
    /// <typeparam name="TCollection">The type of the collection.</typeparam>
    /// <seealso cref="Materializer{TCommand, TParameter, TCollection}" />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public class CollectionMaterializer<TCommand, TParameter, TObject, TCollection> : Materializer<TCommand, TParameter, TCollection>
        where TCommand : DbCommand
        where TObject : class, new()
        where TCollection : ICollection<TObject>, new()
        where TParameter : DbParameter
    {

        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The associated operation.</param>
        public CollectionMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder)
            : base(commandBuilder)
        {
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DataException">Unexpected null result</exception>
        public override TCollection Execute(object state = null)
        {
            var result = new TCollection();
            Table table = null;
            ExecuteCore(cmd =>
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    table = new Table(reader);
                    return table.Rows.Count;
                }
            }, state);

            foreach (var item in table.ToObjects<TObject>())
                result.Add(item);
            return result;
        }


        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        /// <exception cref="DataException">Unexpected null result</exception>
        public override async Task<TCollection> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var result = new TCollection();

            Table table = null;
            await ExecuteCoreAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    table = new Table(reader);
                    return table.Rows.Count;
                }
            }, cancellationToken, state).ConfigureAwait(false);

            foreach (var item in table.ToObjects<TObject>())
                result.Add(item);
            return result;
        }

        /// <summary>
        /// Returns the list of columns the materializer would like to have.
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyList<string> DesiredColumns()
        {
            return MetadataCache.GetMetadata(typeof(TObject)).ColumnsFor;
        }
    }
}
