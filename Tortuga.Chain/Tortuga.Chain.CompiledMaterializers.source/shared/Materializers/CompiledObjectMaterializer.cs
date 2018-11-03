using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    internal sealed class CompiledObjectMaterializer<TCommand, TParameter, TObject> : Materializer<TCommand, TParameter, TObject>
        where TCommand : DbCommand
        where TObject : class, new()
        where TParameter : DbParameter
    {
        RowOptions m_RowOptions;

        public CompiledObjectMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, RowOptions rowOptions) : base(commandBuilder)
        {
            m_RowOptions = rowOptions;

            if (rowOptions.HasFlag(RowOptions.InferConstructor))
                throw new NotSupportedException("Compiled materializers do not support non-default constructors");
        }

        /// <summary>
        /// Returns the list of columns the materializer would like to have.
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyList<string> DesiredColumns() => MetadataCache.GetMetadata(typeof(TObject)).ColumnsFor;

        public override TObject Execute(object state = null)
        {
            var result = new List<TObject>();

            var executionToken = Prepare();
            executionToken.Execute(cmd =>
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    var factory = CompiledMaterializers.CreateBuilder<TObject>(DataSource, cmd.CommandText, reader);
                    while (reader.Read())
                        result.Add(factory(reader));
                    return result.Count;
                }
            }, state);

            if (result.Count == 0)
            {
                if (m_RowOptions.HasFlag(RowOptions.AllowEmptyResults))
                    return null;
                else
                    throw new DataException("No rows were returned");
            }
            else if (result.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
            {
                throw new DataException("Expected 1 row but received " + result.Count + " rows");
            }
            return result.First();
        }

        public override async Task<TObject> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var result = new List<TObject>();

            var executionToken = Prepare();
            await executionToken.ExecuteAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    var factory = CompiledMaterializers.CreateBuilder<TObject>(DataSource, cmd.CommandText, reader);
                    while (await reader.ReadAsync())
                        result.Add(factory(reader));
                    return result.Count;
                }
            }, cancellationToken, state).ConfigureAwait(false);


            if (result.Count == 0)
            {
                if (m_RowOptions.HasFlag(RowOptions.AllowEmptyResults))
                    return null;
                else
                    throw new DataException("No rows were returned");
            }
            else if (result.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
            {
                throw new DataException("Expected 1 row but received " + result.Count + " rows");
            }
            return result.First();
        }
    }
}
