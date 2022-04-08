using System.Collections.Immutable;
using System.Data.Common;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// Class MultipleRowDbCommandBuilder is an adapter used to add a return type for subclasses of MultipleRowDbCommandBuilder{TCommand, TParameter}.
/// </summary>
/// <typeparam name="TCommand">The type of the t command.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
/// <typeparam name="TObject">The type of the t object.</typeparam>
public class MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> : MultipleRowDbCommandBuilder<TCommand, TParameter>, IMultipleRowDbCommandBuilder<TObject>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObject : class
{
	readonly MultipleRowDbCommandBuilder<TCommand, TParameter> m_CommandBuilder;

	/// <summary>
	/// Initializes a new instance of the <see cref="MultipleRowDbCommandBuilder{TCommand, TParameter, TObject}"/> class.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <exception cref="System.ArgumentNullException">commandBuilder</exception>
	public MultipleRowDbCommandBuilder(MultipleRowDbCommandBuilder<TCommand, TParameter> commandBuilder)
		: base(commandBuilder?.DataSource ?? throw new ArgumentNullException(nameof(commandBuilder), $"{nameof(commandBuilder)} is null."))
	{
		m_CommandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder), $"{nameof(commandBuilder)} is null.");
	}

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <param name="materializer">The materializer.</param>
	/// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
	public override CommandExecutionToken<TCommand, TParameter> Prepare(Materializer<TCommand, TParameter> materializer)
	{
		return m_CommandBuilder.Prepare(materializer);
	}

	/// <summary>
	/// Materializes the result as a list of objects.
	/// </summary>
	/// <param name="collectionOptions">The collection options.</param>
	/// <returns>IConstructibleMaterializer&lt;List&lt;TObject&gt;&gt;.</returns>
	public IConstructibleMaterializer<List<TObject>> ToCollection(CollectionOptions collectionOptions = CollectionOptions.None)
	{
		return m_CommandBuilder.ToCollection<TObject>(collectionOptions);
	}

	/// <summary>
	/// Materializes the result as a dictionary of objects.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="keyColumn">The key column.</param>
	/// <param name="dictionaryOptions">The dictionary options.</param>
	/// <returns>IConstructibleMaterializer&lt;Dictionary&lt;TKey, TObject&gt;&gt;.</returns>
	/// <exception cref="System.NotImplementedException"></exception>
	public IConstructibleMaterializer<Dictionary<TKey, TObject>> ToDictionary<TKey>(string keyColumn, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
		where TKey : notnull
	{
		return m_CommandBuilder.ToDictionary<TKey, TObject>(keyColumn, dictionaryOptions);
	}

	/// <summary>
	/// Materializes the result as a dictionary of objects.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="keyFunction">The key function.</param>
	/// <param name="dictionaryOptions">The dictionary options.</param>
	/// <returns>IConstructibleMaterializer&lt;Dictionary&lt;TKey, TObject&gt;&gt;.</returns>

	public IConstructibleMaterializer<Dictionary<TKey, TObject>> ToDictionary<TKey>(Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
		where TKey : notnull
	{
		return m_CommandBuilder.ToDictionary<TKey, TObject>(keyFunction, dictionaryOptions);
	}

	/// <summary>
	/// Materializes the result as an immutable array of objects.
	/// </summary>
	/// <param name="collectionOptions">The collection options.</param>
	/// <remarks>In theory this will offer better performance than ToImmutableList if you only intend to read the result.</remarks>
	public IConstructibleMaterializer<ImmutableArray<TObject>> ToImmutableArray(CollectionOptions collectionOptions = CollectionOptions.None)
	{
		return m_CommandBuilder.ToImmutableArray<TObject>(collectionOptions);
	}

	/// <summary>
	/// Materializes the result as a immutable dictionary of objects.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="keyFunction">The key function.</param>
	/// <param name="dictionaryOptions">The dictionary options.</param>
	/// <returns>IConstructibleMaterializer&lt;ImmutableDictionary&lt;TKey, TObject&gt;&gt;.</returns>

	public IConstructibleMaterializer<ImmutableDictionary<TKey, TObject>> ToImmutableDictionary<TKey>(Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
		where TKey : notnull
	{
		return m_CommandBuilder.ToImmutableDictionary<TKey, TObject>(keyFunction, dictionaryOptions);
	}

	/// <summary>
	/// Materializes the result as a immutable dictionary of objects.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="keyColumn">The key column.</param>
	/// <param name="dictionaryOptions">The dictionary options.</param>
	/// <returns>IConstructibleMaterializer&lt;ImmutableDictionary&lt;TKey, TObject&gt;&gt;.</returns>

	public IConstructibleMaterializer<ImmutableDictionary<TKey, TObject>> ToImmutableDictionary<TKey>(string keyColumn, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
		where TKey : notnull
	{
		return m_CommandBuilder.ToImmutableDictionary<TKey, TObject>(keyColumn, dictionaryOptions);
	}

	/// <summary>
	/// Materializes the result as an immutable list of objects.
	/// </summary>
	/// <param name="collectionOptions">The collection options.</param>
	/// <remarks>In theory this will offer better performance than ToImmutableArray if you intend to further modify the result.</remarks>
	public IConstructibleMaterializer<ImmutableList<TObject>> ToImmutableList(CollectionOptions collectionOptions = CollectionOptions.None)
	{
		return m_CommandBuilder.ToImmutableList<TObject>(collectionOptions);
	}

	/// <summary>
	/// Materializes the result as an instance of the indicated type
	/// </summary>
	/// <param name="rowOptions">The row options.</param>
	/// <returns>IConstructibleMaterializer&lt;TObject&gt;.</returns>
	public IConstructibleMaterializer<TObject> ToObject(RowOptions rowOptions = RowOptions.None)
	{
		return m_CommandBuilder.ToObject<TObject>(rowOptions);
	}

	/// <summary>
	/// Materializes the result as an instance of the indicated type
	/// </summary>
	/// <param name="rowOptions">The row options.</param>
	/// <returns>IConstructibleMaterializer&lt;TObject&gt;.</returns>
	public IConstructibleMaterializer<TObject?> ToObjectOrNull(RowOptions rowOptions = RowOptions.None)
	{
		return m_CommandBuilder.ToObjectOrNull<TObject>(rowOptions);
	}

	/// <summary>
	/// Returns the column associated with the column name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns>If the column name was not found, this will return null</returns>
	public override ColumnMetadata? TryGetColumn(string columnName)
	{
		return m_CommandBuilder.TryGetColumn(columnName);
	}

	/// <summary>
	/// Returns a list of columns known to be non-nullable.
	/// </summary>
	/// <returns>If the command builder doesn't know which columns are non-nullable, an empty list will be returned.</returns>
	/// <remarks>This is used by materializers to skip IsNull checks.</remarks>
	public override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns()
	{
		return m_CommandBuilder.TryGetNonNullableColumns();
	}
}
