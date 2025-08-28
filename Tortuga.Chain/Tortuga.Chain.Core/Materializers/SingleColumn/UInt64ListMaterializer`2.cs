using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;


/// <summary>
/// Materializes the result set as a list of integers.
/// </summary>
/// <typeparam name="TCommand">The type of the t command type.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
internal sealed class UInt64ListMaterializer<TCommand, TParameter> : ListColumnMaterializer<TCommand, TParameter, ulong> where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <param name="listOptions">The list options.</param>
	/// <param name="columnName">Name of the desired column.</param>
	public UInt64ListMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName = null, ListOptions listOptions = ListOptions.None)
		: base(commandBuilder, columnName, listOptions)
	{
	}

	private protected override ulong ReadValue(DbDataReader reader, int ordinal) => reader.GetFieldValue<ulong>(ordinal);
}
