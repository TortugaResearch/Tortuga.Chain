using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    internal class InitializedImmutableDictionary<TCommand, TParameter, TKey, TObject> : Materializer<TCommand, TParameter, ImmutableDictionary<TKey, TObject>>
        where TCommand : DbCommand
        where TParameter : DbParameter
        where TObject : class
    {
        private readonly Func<TObject, TKey> m_KeyFunction;
        private readonly string m_KeyColumn;
        //private readonly DictionaryOptions m_DictionaryOptions;
        private readonly Type[] m_ConstructorSignature;

        public InitializedImmutableDictionary(DbCommandBuilder<TCommand, TParameter> commandBuilder, string keyColumn, Type[] constructorSignature, DictionaryOptions dictionaryOptions) : base(commandBuilder)
        {
            if (dictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
                throw new NotImplementedException("DiscardDuplicates is not implemented for ImmutableDictionary with non-default constructors.");

            m_KeyColumn = keyColumn;
            //m_DictionaryOptions = dictionaryOptions;
            m_ConstructorSignature = constructorSignature;
        }

        public InitializedImmutableDictionary(DbCommandBuilder<TCommand, TParameter> commandBuilder, Func<TObject, TKey> keyFunction, Type[] constructorSignature, DictionaryOptions dictionaryOptions) : base(commandBuilder)
        {
            if (dictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
                throw new NotImplementedException("DiscardDuplicates is not implemented for ImmutableDictionary with non-default constructors.");

            m_KeyFunction = keyFunction;
            //m_DictionaryOptions = dictionaryOptions;
            m_ConstructorSignature = constructorSignature;
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
                return ImmutableDictionary.CreateRange(table.ToObjects<TObject>(m_ConstructorSignature).Select(x => new KeyValuePair<TKey, TObject>(m_KeyFunction(x), x)));

            if (!table.ColumnNames.Contains(m_KeyColumn))
                throw new MappingException("The result set does not contain a column named " + m_KeyColumn);

            return ImmutableDictionary.CreateRange(table.ToObjectsWithEcho<TObject>(m_ConstructorSignature).Select(x => new KeyValuePair<TKey, TObject>((TKey)x.Key[m_KeyColumn], x.Value)));
        }

        /// <summary>
        /// Returns the list of columns the materializer would like to have.
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyList<string> DesiredColumns()
        {
            var desiredType = typeof(TObject);
            var constructor = desiredType.GetConstructor(m_ConstructorSignature);

            if (constructor == null)
            {
                var types = string.Join(", ", m_ConstructorSignature.Select(t => t.Name));
                throw new MappingException($"Cannot find a constructor on {desiredType.Name} with the types [{types}]");
            }

            return ImmutableList.CreateRange(constructor.GetParameters().Select(p => p.Name));
        }
    }
}
