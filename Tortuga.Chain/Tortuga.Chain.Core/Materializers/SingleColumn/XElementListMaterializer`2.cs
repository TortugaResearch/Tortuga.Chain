using System.Data.Common;
using System.Xml.Linq;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// Materializes the result set as a list of strings.
/// </summary>
/// <typeparam name="TCommand">The type of the t command type.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
internal sealed class XElementListMaterializer<TCommand, TParameter> : ListColumnMaterializer<TCommand, TParameter, XElement> where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="XElementListMaterializer{TCommand, TParameter}"/> class.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <param name="columnName">Name of the desired column.</param>
	/// <param name="listOptions">The list options.</param>
	public XElementListMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName = null, ListOptions listOptions = ListOptions.None)
		: base(commandBuilder, columnName, listOptions, false)
	{
	}

	private protected override XElement ReadValue(DbDataReader reader, int ordinal) => XElement.Parse(reader.GetString(ordinal));
}
