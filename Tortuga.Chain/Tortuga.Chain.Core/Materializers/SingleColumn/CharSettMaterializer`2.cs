using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// Materializes the result set as a set of chars.
	/// </summary>
	/// <typeparam name="TCommand">The type of the t command type.</typeparam>
	/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
	internal sealed class CharSetMaterializer<TCommand, TParameter> : SetColumnMaterializer<TCommand, TParameter, char> where TCommand : DbCommand
		where TParameter : DbParameter
	{
		/// <summary>
		/// </summary>
		/// <param name="commandBuilder">The command builder.</param>
		/// <param name="listOptions">The list options.</param>
		/// <param name="columnName">Name of the desired column.</param>
		public CharSetMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName = null, ListOptions listOptions = ListOptions.None)
			: base(commandBuilder, columnName, listOptions)
		{
		}

		private protected override char ReadValue(DbDataReader reader, int ordinal)
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
}
