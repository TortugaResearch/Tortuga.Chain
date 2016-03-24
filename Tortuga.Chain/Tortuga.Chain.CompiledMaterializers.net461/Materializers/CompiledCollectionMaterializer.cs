using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    internal sealed class CompiledCollectionMaterializer<TCommand, TParameter, TObject, TCollection> : Materializer<TCommand, TParameter, TCollection>
            where TCommand : DbCommand
            where TObject : class, new()
            where TCollection : ICollection<TObject>, new()
            where TParameter : DbParameter
    {

        //System.Collections.Concurrent.ConcurrentDictionary<string, >

        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The associated operation.</param>
        public CompiledCollectionMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder)
            : base(commandBuilder)
        {
        }

        public override TCollection Execute(object state = null)
        {
            var result = new TCollection();
            ExecuteCore(cmd =>
            {
                using (var reader = cmd.ExecuteReader())
                {
                    var factory = CompiledMaterializers.CreateBuilder<TObject>(DataSource, cmd.CommandText, reader);
                    while (reader.Read())
                    {
                        result.Add(factory(reader));
                    }
                    return result.Count;
                }
            }, state);

            return result;
        }

        public override async Task<TCollection> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var result = new TCollection();

            await ExecuteCoreAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    var factory = CompiledMaterializers.CreateBuilder<TObject>(DataSource, cmd.CommandText, reader);
                    while (await reader.ReadAsync())
                    {
                        result.Add(factory(reader));
                    }
                    return result.Count;
                }
            }, cancellationToken, state).ConfigureAwait(false);

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
