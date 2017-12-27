using System;
using System.Collections.Generic;
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
    internal class DictionaryMaterializer<TCommand, TParameter, TKey, TObject, TDictionary> : ConstructibleMaterializer<TCommand, TParameter, TDictionary, TObject>
        where TCommand : DbCommand
        where TObject : class
        where TDictionary : IDictionary<TKey, TObject>, new()
        where TParameter : DbParameter
    {

        readonly DictionaryOptions m_DictionaryOptions;
        readonly string m_KeyColumn;
        readonly Func<TObject, TKey> m_KeyFunction;
        public DictionaryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions) : base(commandBuilder)
        {
            m_KeyFunction = keyFunction;
            m_DictionaryOptions = dictionaryOptions;

            if (m_DictionaryOptions.HasFlag(DictionaryOptions.InferConstructor))
            {
                var constructors = ObjectMetadata.Constructors.Where(x => x.Signature.Length > 0).ToList();
                if (constructors.Count == 0)
                    throw new MappingException($"Type {typeof(TObject).Name} has does not have any non-default constructors.");
                if (constructors.Count > 1)
                    throw new MappingException($"Type {typeof(TObject).Name} has more than one non-default constructor. Please use the WithConstructor method to specify which one to use.");
                ConstructorSignature = constructors[0].Signature;
            }
        }

        public DictionaryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string keyColumn, DictionaryOptions dictionaryOptions) : base(commandBuilder)
        {
            m_KeyColumn = commandBuilder.TryGetColumn(keyColumn)?.SqlName ?? keyColumn;
            m_DictionaryOptions = dictionaryOptions;

            if (m_DictionaryOptions.HasFlag(DictionaryOptions.InferConstructor))
            {
                var constructors = ObjectMetadata.Constructors.Where(x => x.Signature.Length > 0).ToList();
                if (constructors.Count == 0)
                    throw new MappingException($"Type {typeof(TObject).Name} has does not have any non-default constructors.");
                if (constructors.Count > 1)
                    throw new MappingException($"Type {typeof(TObject).Name} has more than one non-default constructor. Please use the WithConstructor method to specify which one to use.");
                ConstructorSignature = constructors[0].Signature;
            }
        }

        /// <summary>
        /// Returns the list of columns the materializer would like to have.
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyList<string> DesiredColumns()
        {
            if (ConstructorSignature == null)
                return ObjectMetadata.ColumnsFor;

            var desiredType = typeof(TObject);
            var constructor = ObjectMetadata.Constructors.Find(ConstructorSignature);

            if (constructor == null)
            {
                var types = string.Join(", ", ConstructorSignature.Select(t => t.Name));
                throw new MappingException($"Cannot find a constructor on {desiredType.Name} with the types [{types}]");
            }

            return constructor.ParameterNames;
        }

        public override TDictionary Execute(object state = null)
        {
            var result = new TDictionary();
            Prepare().Execute(cmd =>
            {
                using (var reader = cmd.ExecuteReader().AsObjectConstructor<TObject>(ConstructorSignature))
                {
                    while (reader.Read())
                        AddToDictionary(result, reader);

                    return reader.RowsRead;
                }
            }, state);

            return result;
        }

        public override async Task<TDictionary> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {

            var result = new TDictionary();
            await Prepare().ExecuteAsync(async cmd =>
            {
                using (var reader = (await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false)).AsObjectConstructor<TObject>(ConstructorSignature))
                {
                    while (await reader.ReadAsync())
                        AddToDictionary(result, reader);

                    return reader.RowsRead;
                }
            }, cancellationToken, state).ConfigureAwait(false);


            return result;
        }

        void AddToDictionary(TDictionary result, StreamingObjectConstructor<TObject> source)
        {
            if (m_KeyFunction != null)
            {
                if (m_DictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
                    result[m_KeyFunction(source.Current)] = source.Current;
                else
                    result.Add(m_KeyFunction(source.Current), source.Current);
            }
            else
            {
                if (!source.CurrentDictionary.ContainsKey(m_KeyColumn))
                    throw new MappingException("The result set does not contain a column named " + m_KeyColumn);

                if (m_DictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
                    result[(TKey)source.CurrentDictionary[m_KeyColumn]] = source.Current;
                else
                    result.Add((TKey)source.CurrentDictionary[m_KeyColumn], source.Current);
            }

        }
    }
}
