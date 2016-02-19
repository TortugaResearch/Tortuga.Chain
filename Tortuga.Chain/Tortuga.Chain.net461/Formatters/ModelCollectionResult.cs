using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Formatters
{
    /// <summary>
    /// Formats the result set as a collection of the indicated type.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TCollection">The type of the collection.</typeparam>
    /// <seealso cref="Formatters.Formatter{TCommandType, TParameterType, TCollection}" />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public class ModelCollectionResult<TCommandType, TParameterType, TModel, TCollection> : Formatter<TCommandType, TParameterType, TCollection>
        where TCommandType : DbCommand
        where TModel : class, new()
        where TCollection : ICollection<TModel>, new()
        where TParameterType : DbParameter
    {

        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The associated operation.</param>
        public ModelCollectionResult(DbCommandBuilder<TCommandType, TParameterType> commandBuilder)
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

            foreach (var item in table.ToModels<TModel>())
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

            foreach (var item in table.ToModels<TModel>())
                result.Add(item);
            return result;
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
