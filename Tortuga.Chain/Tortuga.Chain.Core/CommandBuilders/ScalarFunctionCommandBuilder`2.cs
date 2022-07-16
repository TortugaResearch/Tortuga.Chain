using System.Collections.Immutable;
using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// Base class for scalar function command builders.
/// </summary>
/// <typeparam name="TCommand">The type of the t command.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
public abstract class ScalarFunctionCommandBuilder<TCommand, TParameter> : ScalarDbCommandBuilder<TCommand, TParameter>
	where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>Initializes a new instance of the <see cref="ScalarFunctionCommandBuilder{TCommand, TParameter}"/> class.</summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="functionArgumentValue">The function argument.</param>
	protected ScalarFunctionCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource, object? functionArgumentValue) : base(dataSource)
	{
		FunctionArgumentValue = functionArgumentValue;
	}

	/// <summary>
	/// Gets the function argument value.
	/// </summary>
	/// <value>The function argument value.</value>
	protected object? FunctionArgumentValue { get; }

	/// <summary>
	/// Returns the column associated with the column name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns>ColumnMetadata.</returns>
	/// <remarks>Always returns null since this command builder has no columns</remarks>
	public sealed override ColumnMetadata? TryGetColumn(string columnName) => null;

	/// <summary>
	/// Returns a list of columns.
	/// </summary>
	/// <returns>If the command builder doesn't know which columns are available, an empty list will be returned.</returns>
	/// <remarks>This is used by materializers to skip exclude columns.</remarks>
	public sealed override IReadOnlyList<ColumnMetadata> TryGetColumns() => ImmutableList<ColumnMetadata>.Empty;

	/// <summary>
	/// Returns a list of columns known to be non-nullable.
	/// </summary>
	/// <returns>
	/// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
	/// </returns>
	/// <remarks>
	/// This is used by materializers to skip IsNull checks.
	/// </remarks>
	public sealed override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => ImmutableList<ColumnMetadata>.Empty;
}
