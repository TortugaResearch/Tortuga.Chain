using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// Materializes the result set as a dictionary of the indicated type.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TParameter">The type of the parameter.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TObject">The type of the object.</typeparam>
/// <typeparam name="TDictionary">The type of the dictionary.</typeparam>
/// <seealso cref="Materializer{TCommand, TParameter, TDictionary}"/>
internal class DictionaryMaterializer<TCommand, TParameter, TKey, TObject, TDictionary> : ConstructibleMaterializer<TCommand, TParameter, TDictionary, TObject>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TKey : notnull
	where TObject : class
	where TDictionary : IDictionary<TKey, TObject>, new()
{
	readonly DictionaryOptions m_DictionaryOptions;
	readonly string? m_KeyColumn;
	readonly Func<TObject, TKey>? m_KeyFunction;

	public DictionaryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions) : base(commandBuilder)
	{
		m_KeyFunction = keyFunction;
		m_DictionaryOptions = dictionaryOptions;

		if (m_DictionaryOptions.HasFlag(DictionaryOptions.InferConstructor))
			Constructor = InferConstructor();
	}

	public DictionaryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string keyColumn, DictionaryOptions dictionaryOptions) : base(commandBuilder)
	{
		m_KeyColumn = commandBuilder.TryGetColumn(keyColumn)?.SqlName ?? keyColumn;
		m_DictionaryOptions = dictionaryOptions;

		if (m_DictionaryOptions.HasFlag(DictionaryOptions.InferConstructor))
			Constructor = InferConstructor();
	}

	public override TDictionary Execute(object? state = null)
	{
		var result = new TDictionary();
		Prepare().Execute(cmd =>
		{
			using (var reader = cmd.ExecuteReader().AsObjectConstructor<TObject>(Constructor, CommandBuilder.TryGetNonNullableColumns(), Converter))
			{
				while (reader.Read(out var value))
					AddToDictionary(result, reader, value);

				return reader.RowsRead;
			}
		}, state);

		return result;
	}

	public override async Task<TDictionary> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		var result = new TDictionary();
		await Prepare().ExecuteAsync(async cmd =>
		{
			using (var reader = (await cmd.ExecuteReaderAsync(CommandBehavior, cancellationToken).ConfigureAwait(false)).AsObjectConstructor<TObject>(Constructor, CommandBuilder.TryGetNonNullableColumns(), Converter))
			{
				while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
					AddToDictionary(result, reader, reader.Current!);

				return reader.RowsRead;
			}
		}, cancellationToken, state).ConfigureAwait(false);

		return result;
	}

	void AddToDictionary(TDictionary result, StreamingObjectConstructor<TObject> source, TObject value)
	{
		if (m_KeyFunction != null)
		{
			if (m_DictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
				result[m_KeyFunction(value)] = value;
			else
				result.Add(m_KeyFunction(value), value);
		}
		else if (m_KeyColumn != null)
		{
			if (!source.CurrentDictionary.ContainsKey(m_KeyColumn))
				throw new MappingException("The result set does not contain a column named " + m_KeyColumn);

			var key = source.CurrentDictionary[m_KeyColumn];
			if (key == null)
				throw new MissingDataException("Key column is null");

			if (m_DictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
				result[(TKey)key] = value;
			else
				result.Add((TKey)key, value);
		}
	}
}
