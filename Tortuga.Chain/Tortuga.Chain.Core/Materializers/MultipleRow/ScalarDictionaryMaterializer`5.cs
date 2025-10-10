using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// Materializes the result set as a dictionary of the indicated type.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TParameter">The type of the parameter.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <typeparam name="TDictionary">The type of the dictionary.</typeparam>
/// <seealso cref="Materializer{TCommand, TParameter, TDictionary}"/>
internal class ScalarDictionaryMaterializer<TCommand, TParameter, TKey, TValue, TDictionary> : ColumnSelectingMaterializer<TCommand, TParameter, TDictionary>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TKey : notnull
	where TDictionary : IDictionary<TKey, TValue>, new()
{
	readonly DictionaryOptions m_DictionaryOptions;
	readonly string m_KeyColumn;
	readonly string m_ValueColumn;

	public ScalarDictionaryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string keyColumn, string valueColumn, DictionaryOptions dictionaryOptions) : base(commandBuilder)
	{
		if (commandBuilder == null)
			throw new ArgumentNullException(nameof(commandBuilder), $"{nameof(commandBuilder)} is null.");
		if (string.IsNullOrEmpty(keyColumn))
			throw new ArgumentException($"{nameof(keyColumn)} is null or empty.", nameof(keyColumn));
		if (string.IsNullOrEmpty(valueColumn))
			throw new ArgumentException($"{nameof(valueColumn)} is null or empty.", nameof(valueColumn));

		m_KeyColumn = commandBuilder.TryGetColumn(keyColumn)?.SqlName ?? keyColumn;
		m_ValueColumn = commandBuilder.TryGetColumn(valueColumn)?.SqlName ?? valueColumn;
		m_DictionaryOptions = dictionaryOptions;

		if (m_DictionaryOptions.HasFlag(DictionaryOptions.InferConstructor))
			throw new ArgumentException("The InferConstructor option is not supported by scalar dictionary.", nameof(dictionaryOptions));
	}

	public override TDictionary Execute(object? state = null)
	{
		var result = new TDictionary();
		Prepare().Execute(cmd =>
		{
			using (var reader = cmd.ExecuteReader())
			{
				var keyIndex = reader.GetOrdinal(m_KeyColumn);
				var valueIndex = reader.GetOrdinal(m_ValueColumn);
				var rowsRead = 0;

				while (reader.Read())
				{
					AddToDictionary(result, reader, keyIndex, valueIndex);
					rowsRead += 1;
				}

				return rowsRead;
			}
		}, state);

		return result;
	}

	public override async Task<TDictionary> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		var result = new TDictionary();
		await Prepare().ExecuteAsync(async cmd =>
		{
			using (var reader = (await cmd.ExecuteReaderAsync(CommandBehavior, cancellationToken).ConfigureAwait(false)))
			{
				var keyIndex = reader.GetOrdinal(m_KeyColumn);
				var valueIndex = reader.GetOrdinal(m_ValueColumn);
				var rowsRead = 0;

				while (await reader.ReadAsync().ConfigureAwait(false))
				{
					AddToDictionary(result, reader, keyIndex, valueIndex);
					rowsRead += 1;
				}

				return rowsRead;
			}
		}, cancellationToken, state).ConfigureAwait(false);

		return result;
	}

	void AddToDictionary(TDictionary result, DbDataReader source, int keyIndex, int valueIndex)
	{
		if (source.IsDBNull(keyIndex))
			if (m_DictionaryOptions.HasFlag(DictionaryOptions.DiscardNullKeys))
				return;
			else
				throw new MissingDataException($"Key column {m_KeyColumn} is null. Consider using DictionaryOptions.DiscardNullKeys.");
		var key = source.GetFieldValue<TKey>(keyIndex);

		if (source.IsDBNull(valueIndex))
			if (m_DictionaryOptions.HasFlag(DictionaryOptions.DiscardNullValues))
				return;
			else
				throw new MissingDataException($"Value column {m_ValueColumn} is null. Consider using DictionaryOptions.DiscardNullValues.");
		var value = source.GetFieldValue<TValue>(valueIndex);

		if (m_DictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
			result[key] = value;
		else
			result.Add(key, value);
	}
}
