using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// Materializes the result set as a set of DateTimeOffset.
/// </summary>
/// <typeparam name="TCommand">The type of the t command type.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
internal sealed class DateTimeOffsetSetMaterializer<TCommand, TParameter> : SetColumnMaterializer<TCommand, TParameter, DateTimeOffset> where TCommand : DbCommand
	where TParameter : DbParameter

{
	/// <summary>
	/// Initializes a new instance of the <see cref="DateTimeOffsetListMaterializer{TCommand, TParameter}"/> class.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <param name="listOptions">The list options.</param>
	/// <param name="columnName">Name of the desired column.</param>
	public DateTimeOffsetSetMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName = null, ListOptions listOptions = ListOptions.None)
		: base(commandBuilder, columnName, listOptions)
	{
	}

	private protected override DateTimeOffset ReadValue(DbDataReader reader, int ordinal) => (DateTimeOffset)reader.GetValue(ordinal);
}
