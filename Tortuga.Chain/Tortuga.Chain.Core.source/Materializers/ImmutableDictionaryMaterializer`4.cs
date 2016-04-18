using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// Materializes the result set as a dictionary of the indicated type.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <seealso cref="Materializer{TCommand, TParameter, TDictionary}" />
    internal class ImmutableDictionaryMaterializer<TCommand, TParameter, TKey, TObject> : Materializer<TCommand, TParameter, ImmutableDictionary<TKey, TObject>>
        where TCommand : DbCommand
        where TObject : class, new()
        where TParameter : DbParameter
    {

        private readonly Func<TObject, TKey> m_KeyFunction;
        private readonly string m_KeyColumn;
        //private readonly DictionaryOptions m_DictionaryOptions;

        public ImmutableDictionaryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions) : base(commandBuilder)
        {
            if (dictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
                throw new NotImplementedException("DiscardDuplicates is not implemented for ImmutableDictionary with default constructors.");

            m_KeyFunction = keyFunction;
            //m_DictionaryOptions = dictionaryOptions;
        }

        public ImmutableDictionaryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string keyColumn, DictionaryOptions dictionaryOptions) : base(commandBuilder)
        {
            if (dictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
                throw new NotImplementedException("DiscardDuplicates is not implemented for ImmutableDictionary with default constructors.");

            m_KeyColumn = keyColumn;
            //m_DictionaryOptions = dictionaryOptions;
        }

        public override ImmutableDictionary<TKey, TObject> Execute(object state = null)
        {
            Table table = null;
            ExecuteCore(cmd =>
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    table = new Table(reader);
                    return table.Rows.Count;
                }
            }, state);

            return ToDictionary(table);
        }

        public override async Task<ImmutableDictionary<TKey, TObject>> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {

            Table table = null;
            await ExecuteCoreAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    table = new Table(reader);
                    return table.Rows.Count;
                }
            }, cancellationToken, state).ConfigureAwait(false);


            return ToDictionary(table);
        }

        private ImmutableDictionary<TKey, TObject> ToDictionary(Table table)
        {
            if (m_KeyFunction != null)
                return ImmutableDictionary.CreateRange(table.ToObjects<TObject>().Select(x => new KeyValuePair<TKey, TObject>(m_KeyFunction(x), x)));

            if (!table.ColumnNames.Contains(m_KeyColumn))
                throw new MappingException("The result set does not contain a column named " + m_KeyColumn);

            return ImmutableDictionary.CreateRange(table.ToObjectsWithEcho<TObject>().Select(x => new KeyValuePair<TKey, TObject>((TKey)x.Key[m_KeyColumn], x.Value)));
        }

        /// <summary>
        /// Returns the list of columns the materializer would like to have.
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyList<string> DesiredColumns()
        {
            var columns = MetadataCache.GetMetadata(typeof(TObject)).ColumnsFor;

            if (m_KeyColumn != null && !columns.Contains(m_KeyColumn))
                columns = columns.Add(m_KeyColumn);

            return columns;
        }

    }
}
