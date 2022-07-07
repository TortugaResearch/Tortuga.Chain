using System.Data.Common;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// This class represents result materializers that read from a single column and return a list.
/// </summary>
public abstract class ListColumnMaterializer<TCommand, TParameter, TResult> : CollectionColumnMaterializer<TCommand, TParameter, List<TResult>, TResult>
	where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ListColumnMaterializer{TCommand, TParameter, TResult}" /> class.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <param name="columnName">Name of the desired column.</param>
	/// <param name="listOptions">The list options.</param>
	/// <param name="allowNulls">If allowNulls is set, the list will allow or prohibit nulls. If not set, infer this value from TResult.</param>
	protected ListColumnMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName, ListOptions listOptions, bool? allowNulls = null) : base(commandBuilder, columnName, listOptions, allowNulls)
	{
	}
}