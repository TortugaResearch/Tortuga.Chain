using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// This is the base class for command builders that perform set-based delete operations.
/// </summary>
/// <typeparam name="TCommand">The type of the t command.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
/// <seealso cref="MultipleRowDbCommandBuilder{TCommand, TParameter}" />
/// <seealso cref="IUpdateSetDbCommandBuilder" />
[SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
public abstract class DeleteSetDbCommandBuilder<TCommand, TParameter> : MultipleRowDbCommandBuilder<TCommand, TParameter>
	where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DeleteSetDbCommandBuilder{TCommand, TParameter}" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="parameters">The parameters.</param>
	/// <param name="expectedRowCount">The expected row count.</param>
	/// <param name="options">The options.</param>
	public DeleteSetDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource, string whereClause, IEnumerable<TParameter> parameters, int? expectedRowCount, DeleteOptions options) : base(dataSource)
	{
		if (options.HasFlag(DeleteOptions.UseKeyAttribute))
			throw new NotSupportedException("Cannot use Key attributes with this operation.");

		WhereClause = whereClause;
		Parameters = parameters;
		Options = options;
		ExpectedRowCount = expectedRowCount;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DeleteSetDbCommandBuilder{TCommand, TParameter}"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="argumentValue">The argument value.</param>
	public DeleteSetDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource, string whereClause, object? argumentValue) : base(dataSource)
	{
		WhereClause = whereClause;
		ArgumentValue = argumentValue;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DeleteSetDbCommandBuilder{TCommand, TParameter}"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The options.</param>
	public DeleteSetDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource, object filterValue, FilterOptions filterOptions) : base(dataSource)
	{
		FilterValue = filterValue;
		FilterOptions = filterOptions;
	}

	/// <summary>
	/// Gets the argument value.
	/// </summary>
	/// <value>The argument value.</value>
	protected object? ArgumentValue { get; }

	/// <summary>
	/// Gets the expected row count.
	/// </summary>
	/// <value>The expected row count.</value>
	protected int? ExpectedRowCount { get; }

	/// <summary>
	/// Gets the filter options.
	/// </summary>
	/// <value>The filter options.</value>
	protected FilterOptions FilterOptions { get; }

	/// <summary>
	/// Gets the filter value.
	/// </summary>
	/// <value>The filter value.</value>
	protected object? FilterValue { get; }

	/// <summary>
	/// Gets the options.
	/// </summary>
	/// <value>The options.</value>
	protected DeleteOptions Options { get; }

	/// <summary>
	/// Gets the parameters.
	/// </summary>
	/// <value>The parameters.</value>
	protected IEnumerable<TParameter>? Parameters { get; }

	/// <summary>
	/// Gets the where clause.
	/// </summary>
	/// <value>The where clause.</value>
	protected string? WhereClause { get; }

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
	/// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
	/// </summary>
	/// <returns></returns>
	protected abstract TableOrViewMetadata OnGetTable();
}
