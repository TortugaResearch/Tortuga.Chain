using System.Data.Common;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// Class SingleRowDbCommandBuilder is an adapter used to add a return type for subclasses of SingleRowDbCommandBuilder{TCommand, TParameter}.
/// </summary>
/// <typeparam name="TCommand">The type of the t command.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
/// <typeparam name="TObject">The type of the t object.</typeparam>
public class SingleRowDbCommandBuilder<TCommand, TParameter, TObject> : SingleRowDbCommandBuilder<TCommand, TParameter>, ISingleRowDbCommandBuilder<TObject>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObject : class
{
	readonly SingleRowDbCommandBuilder<TCommand, TParameter> m_CommandBuilder;

	/// <summary>
	/// Initializes a new instance of the <see cref="SingleRowDbCommandBuilder{TCommand, TParameter, TObject}"/> class.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <exception cref="System.ArgumentNullException">commandBuilder</exception>
	public SingleRowDbCommandBuilder(SingleRowDbCommandBuilder<TCommand, TParameter> commandBuilder)
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
