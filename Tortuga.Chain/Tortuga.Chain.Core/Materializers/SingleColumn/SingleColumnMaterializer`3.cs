using System.Collections.Immutable;
using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// This class represents result materializers that read from a single column.
/// </summary>
public abstract class SingleColumnMaterializer<TCommand, TParameter, TResult> : Materializer<TCommand, TParameter, TResult>
		where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SingleColumnMaterializer{TCommand, TParameter, TResult}" /> class.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <param name="columnName">Name of the desired column.</param>
	protected SingleColumnMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName)
		: base(commandBuilder)
	{
		ColumnName = columnName;
	}

	private protected string? ColumnName { get; }

	/// <summary>
	/// Returns the list of columns the result materializer would like to have.
	/// </summary>
	public override IReadOnlyList<string> DesiredColumns()
	{
		if (ColumnName != null)
			return ImmutableArray.Create(ColumnName);
		else
			return base.DesiredColumns();
	}
}