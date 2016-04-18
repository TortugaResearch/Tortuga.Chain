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
    /// <summary>
    /// Materializes the result set as a dictionary of the indicated type.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <typeparam name="TDictionary">The type of the dictionary.</typeparam>
    /// <seealso cref="Materializer{TCommand, TParameter, TDictionary}" />
    internal class InitializedDictionaryMaterializer<TCommand, TParameter, TKey, TObject, TDictionary> : Materializer<TCommand, TParameter, TDictionary>
        where TCommand : DbCommand
        where TObject : class
        where TDictionary : IDictionary<TKey, TObject>, new()
        where TParameter : DbParameter
    {

        private readonly Func<TObject, TKey> m_KeyFunction;
        private readonly string m_KeyColumn;
        private readonly DictionaryOptions m_DictionaryOptions;
        readonly Type[] m_ConstructorSignature;

        public InitializedDictionaryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, Func<TObject, TKey> keyFunction, Type[] constructorSignature, DictionaryOptions dictionaryOptions) : base(commandBuilder)
        {
            m_KeyFunction = keyFunction;
            m_DictionaryOptions = dictionaryOptions;
            m_ConstructorSignature = constructorSignature;
        }

        public InitializedDictionaryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string keyColumn, Type[] constructorSignature, DictionaryOptions dictionaryOptions) : base(commandBuilder)
        {
            m_KeyColumn = keyColumn;
            m_DictionaryOptions = dictionaryOptions;
            m_ConstructorSignature = constructorSignature;
        }

        public override TDictionary Execute(object state = null)
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

        public override async Task<TDictionary> ExecuteAsync(CancellationToken cancellationToken, object state = null)
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

        private TDictionary ToDictionary(Table table)
        {
            var result = new TDictionary();
            if (m_KeyFunction != null)
            {
                if (m_DictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
                {
                    foreach (var item in table.ToObjects<TObject>(m_ConstructorSignature))
                        result[m_KeyFunction(item)] = item;
                }
                else
                {
                    foreach (var item in table.ToObjects<TObject>(m_ConstructorSignature))
                        result.Add(m_KeyFunction(item), item);
                }
            }
            else
            {
                if (!table.ColumnNames.Contains(m_KeyColumn))
                    throw new MappingException("The result set does not contain a column named " + m_KeyColumn);

                if (m_DictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
                {
                    foreach (var item in table.ToObjectsWithEcho<TObject>(m_ConstructorSignature))
                        result[(TKey)item.Key[m_KeyColumn]] = item.Value;
                }
                else
                {
                    foreach (var item in table.ToObjectsWithEcho<TObject>(m_ConstructorSignature))
                        result.Add((TKey)item.Key[m_KeyColumn], item.Value);
                }
            }

            return result;
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
