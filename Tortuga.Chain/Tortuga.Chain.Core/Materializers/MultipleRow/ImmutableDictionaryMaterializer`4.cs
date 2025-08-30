﻿using System.Collections.Frozen;
using System.Collections.Immutable;
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
/// <seealso cref="Materializer{TCommand, TParameter, TDictionary}" />
internal class ImmutableDictionaryMaterializer<TCommand, TParameter, TKey, TObject> : ConstructibleMaterializer<TCommand, TParameter, ImmutableDictionary<TKey, TObject>, TObject>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TKey : notnull
	where TObject : class
{
	readonly DictionaryOptions m_DictionaryOptions;
	readonly string? m_KeyColumn;
	readonly Func<TObject, TKey>? m_KeyFunction;

	public ImmutableDictionaryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions) : base(commandBuilder)
	{
		if (dictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
			throw new NotImplementedException("DiscardDuplicates is not implemented for ImmutableDictionary with default constructors.");

		m_KeyFunction = keyFunction;
		m_DictionaryOptions = dictionaryOptions;

		if (m_DictionaryOptions.HasFlag(DictionaryOptions.InferConstructor))
			Constructor = InferConstructor();
	}

	public ImmutableDictionaryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string keyColumn, DictionaryOptions dictionaryOptions) : base(commandBuilder)
	{
		if (dictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
			throw new NotImplementedException("DiscardDuplicates is not implemented for ImmutableDictionary with default constructors.");

		m_KeyColumn = commandBuilder.TryGetColumn(keyColumn)?.SqlName ?? keyColumn;
		m_DictionaryOptions = dictionaryOptions;

		if (m_DictionaryOptions.HasFlag(DictionaryOptions.InferConstructor))
			Constructor = InferConstructor();
	}

	public override ImmutableDictionary<TKey, TObject> Execute(object? state = null)
	{
		Table? table = null;
		Prepare().Execute(cmd =>
		{
			using (var reader = cmd.ExecuteReader(CommandBehavior))
			{
				table = new Table(reader, Converter);
				return table.Rows.Count;
			}
		}, state);

		return ToDictionary(table!);
	}

	public override async Task<ImmutableDictionary<TKey, TObject>> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		Table? table = null;
		await Prepare().ExecuteAsync(async cmd =>
		{
			using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior, cancellationToken).ConfigureAwait(false))
			{
				table = new Table(reader, Converter);
				return table.Rows.Count;
			}
		}, cancellationToken, state).ConfigureAwait(false);

		return ToDictionary(table!);
	}

	ImmutableDictionary<TKey, TObject> ToDictionary(Table table)
	{
		if (m_KeyFunction != null)
			return ImmutableDictionary.CreateRange(table.ToObjects<TObject>(Constructor).Select(x => new KeyValuePair<TKey, TObject>(m_KeyFunction(x), x)));

		if (!table.ColumnNames.Contains(m_KeyColumn))
			throw new MappingException("The result set does not contain a column named " + m_KeyColumn);

		return ImmutableDictionary.CreateRange(table.ToObjectsWithEcho<TObject>(Constructor).Select(x => NewMethod(x)));

		KeyValuePair<TKey, TObject> NewMethod(KeyValuePair<Row, TObject> x)
		{
			var key = x.Key[m_KeyColumn!];
			if (key == null)
				throw new UnexpectedDataException("Key is null");
			return new KeyValuePair<TKey, TObject>((TKey)key, x.Value);
		}
	}
}



/// <summary>
/// Materializes the result set as a dictionary of the indicated type.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TParameter">The type of the parameter.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TObject">The type of the object.</typeparam>
/// <seealso cref="Materializer{TCommand, TParameter, TDictionary}" />
internal class FrozenDictionaryMaterializer<TCommand, TParameter, TKey, TObject> : ConstructibleMaterializer<TCommand, TParameter, FrozenDictionary<TKey, TObject>, TObject>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TKey : notnull
	where TObject : class
{
	readonly DictionaryOptions m_DictionaryOptions;
	readonly string? m_KeyColumn;
	readonly Func<TObject, TKey>? m_KeyFunction;

	public FrozenDictionaryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions) : base(commandBuilder)
	{
		if (dictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
			throw new NotImplementedException("DiscardDuplicates is not implemented for FrozenDictionary with default constructors.");

		m_KeyFunction = keyFunction;
		m_DictionaryOptions = dictionaryOptions;

		if (m_DictionaryOptions.HasFlag(DictionaryOptions.InferConstructor))
			Constructor = InferConstructor();
	}

	public FrozenDictionaryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string keyColumn, DictionaryOptions dictionaryOptions) : base(commandBuilder)
	{
		if (dictionaryOptions.HasFlag(DictionaryOptions.DiscardDuplicates))
			throw new NotImplementedException("DiscardDuplicates is not implemented for FrozenDictionary with default constructors.");

		m_KeyColumn = commandBuilder.TryGetColumn(keyColumn)?.SqlName ?? keyColumn;
		m_DictionaryOptions = dictionaryOptions;

		if (m_DictionaryOptions.HasFlag(DictionaryOptions.InferConstructor))
			Constructor = InferConstructor();
	}

	public override FrozenDictionary<TKey, TObject> Execute(object? state = null)
	{
		Table? table = null;
		Prepare().Execute(cmd =>
		{
			using (var reader = cmd.ExecuteReader(CommandBehavior))
			{
				table = new Table(reader, Converter);
				return table.Rows.Count;
			}
		}, state);

		return ToDictionary(table!);
	}

	public override async Task<FrozenDictionary<TKey, TObject>> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		Table? table = null;
		await Prepare().ExecuteAsync(async cmd =>
		{
			using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior, cancellationToken).ConfigureAwait(false))
			{
				table = new Table(reader, Converter);
				return table.Rows.Count;
			}
		}, cancellationToken, state).ConfigureAwait(false);

		return ToDictionary(table!);
	}

	FrozenDictionary<TKey, TObject> ToDictionary(Table table)
	{
		if (m_KeyFunction != null)
			return table.ToObjects<TObject>(Constructor).Select(x => new KeyValuePair<TKey, TObject>(m_KeyFunction(x), x)).ToFrozenDictionary();

		if (!table.ColumnNames.Contains(m_KeyColumn))
			throw new MappingException("The result set does not contain a column named " + m_KeyColumn);

		return table.ToObjectsWithEcho<TObject>(Constructor).Select(x => NewMethod(x)).ToFrozenDictionary();

		KeyValuePair<TKey, TObject> NewMethod(KeyValuePair<Row, TObject> x)
		{
			var key = x.Key[m_KeyColumn!];
			if (key == null)
				throw new UnexpectedDataException("Key is null");
			return new KeyValuePair<TKey, TObject>((TKey)key, x.Value);
		}
	}
}
