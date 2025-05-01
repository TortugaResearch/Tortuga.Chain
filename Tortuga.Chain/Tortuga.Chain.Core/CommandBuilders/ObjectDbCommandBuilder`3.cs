using System.Collections.Immutable;
using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// This represents command builders that operate on single object parameters: Insert, Update, Upsert, Delete
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TParameter">The type of the parameter.</typeparam>
/// <typeparam name="TArgument">The type of the argument.</typeparam>
public abstract class ObjectDbCommandBuilder<TCommand, TParameter, TArgument> : SingleRowDbCommandBuilder<TCommand, TParameter>, IObjectDbCommandBuilder<TArgument>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TArgument : class
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ObjectDbCommandBuilder{TCommand, TParameter, TArgument}"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="argumentValue">The argument value.</param>
	protected ObjectDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource, TArgument argumentValue) : base(dataSource)
	{
		ArgumentValue = argumentValue;
	}

	/// <summary>
	/// Gets the argument value passed to the command builder.
	/// </summary>
	/// <value>The argument value.</value>
	public TArgument ArgumentValue { get; }

	/// <summary>
	/// Gets the set of key column(s) to use instead of the primary key(s).
	/// </summary>
	protected ImmutableHashSet<string> KeyColumns { get; private set; } = [];

	/// <summary>
	/// Materializes the result as a new instance of the same type as the argumentValue
	/// </summary>
	/// <param name="rowOptions">The row options.</param>
	/// <returns></returns>
	/// <remarks>To update the argumentValue itself, use WithRefresh() instead.</remarks>
	public ILink<TArgument> ToObject(RowOptions rowOptions = RowOptions.None)
	{
		return new ObjectMaterializer<TCommand, TParameter, TArgument>(this, rowOptions);
	}

	/// <summary>
	/// Materializes the result as a new instance of the same type as the argumentValue
	/// </summary>
	/// <param name="rowOptions">The row options.</param>
	/// <returns></returns>
	/// <remarks>To update the argumentValue itself, use WithRefresh() instead.</remarks>
	public ILink<TArgument?> ToObjectOrNull(RowOptions rowOptions = RowOptions.None) => new ObjectOrNullMaterializer<TCommand, TParameter, TArgument>(this, rowOptions);

	/// <summary>
	/// Returns the column associated with the column name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns></returns>
	/// <remarks>
	/// If the column name was not found, this will return null
	/// </remarks>
	public override sealed ColumnMetadata? TryGetColumn(string columnName)
	{
		return OnGetTable().Columns.TryGetColumn(columnName);
	}

	/// <summary>
	/// Returns a list of columns.
	/// </summary>
	/// <returns>If the command builder doesn't know which columns are available, an empty list will be returned.</returns>
	/// <remarks>This is used by materializers to skip exclude columns.</remarks>
	public sealed override IReadOnlyList<ColumnMetadata> TryGetColumns() => OnGetTable().Columns;

	/// <summary>
	/// Returns a list of columns known to be non-nullable.
	/// </summary>
	/// <returns>
	/// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
	/// </returns>
	/// <remarks>
	/// This is used by materializers to skip IsNull checks.
	/// </remarks>
	public sealed override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => OnGetTable().NonNullableColumns;

	/// <summary>
	/// Uses an explicitly specified set of key column(s). This overrides the UseKeyAttribute option.
	/// </summary>
	/// <param name="columnNames">The column names that form a unique key.</param>
	/// <returns></returns>
	public ObjectDbCommandBuilder<TCommand, TParameter, TArgument> WithKeys(params string[] columnNames)
	{
		var table = OnGetTable();

		//normalize the column names.
		KeyColumns = [.. columnNames.Select(c => table.Columns[c].SqlName)];
		return this;
	}

	IObjectDbCommandBuilder<TArgument> IObjectDbCommandBuilder<TArgument>.WithKeys(params string[] columnNames)
	{
		return WithKeys(columnNames);
	}

	/// <summary>
	/// After executing the operation, refreshes the properties on the argumentValue by reading the updated values from the database.
	/// </summary>
	/// <returns></returns>
	public ILink<TArgument> WithRefresh()
	{
		if (ArgumentValue == null)
			throw new InvalidOperationException("Cannot use .WithRefresh() if a null argument is provided.");
		return new RefreshMaterializer<TCommand, TParameter, TArgument>(this).NeverNull();
	}

	/// <summary>
	/// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
	/// </summary>
	/// <returns></returns>
	protected abstract TableOrViewMetadata OnGetTable();
}
