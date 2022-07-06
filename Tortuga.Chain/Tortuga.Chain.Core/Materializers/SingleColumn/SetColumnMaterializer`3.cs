using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// This class represents result materializers that read from a single column and return a hash set.
/// </summary>
public abstract class SetColumnMaterializer<TCommand, TParameter, TResult> : CollectionColumnMaterializer<TCommand, TParameter, HashSet<TResult>, TResult>
	where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ListColumnMaterializer{TCommand, TParameter, TResult}" /> class.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <param name="columnName">Name of the desired column.</param>
	/// <param name="listOptions">The list options.</param>
	protected SetColumnMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName, ListOptions listOptions) : base(commandBuilder, columnName, listOptions, false)
	{
	}
}