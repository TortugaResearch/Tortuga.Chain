using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// Materializes the result set as a list of chars.
/// </summary>
/// <typeparam name="TCommand">The type of the t command type.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
internal sealed class CharOrNullListMaterializer<TCommand, TParameter> : ListColumnMaterializer<TCommand, TParameter, char?> where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CharOrNullListMaterializer{TCommand, TParameter}"/> class.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <param name="columnName">Name of the desired column.</param>
	/// <param name="listOptions">The list options.</param>
	public CharOrNullListMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName = null, ListOptions listOptions = ListOptions.None)
		: base(commandBuilder, columnName, listOptions)
	{
	}

	private protected override char? ReadValue(DbDataReader reader, int ordinal)
	{
		if (reader.GetFieldType(ordinal) == typeof(string))
		{
			var temp = reader.GetString(ordinal);
			return temp.Length > 1 ? temp[0] : default;
		}
		else
			return reader.GetChar(ordinal);
	}
}
