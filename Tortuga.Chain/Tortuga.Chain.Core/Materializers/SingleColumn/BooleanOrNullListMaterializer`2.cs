﻿using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// Materializes the result set as a list of booleans.
/// </summary>
/// <typeparam name="TCommand">The type of the t command type.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
internal sealed class BooleanOrNullListMaterializer<TCommand, TParameter> : ListColumnMaterializer<TCommand, TParameter, bool?>
	where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="BooleanOrNullListMaterializer{TCommand, TParameter}"/> class.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <param name="columnName">Name of the desired column.</param>
	/// <param name="listOptions">The list options.</param>
	public BooleanOrNullListMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName = null, ListOptions listOptions = ListOptions.None)
		: base(commandBuilder, columnName, listOptions)
	{
	}

	private protected override bool? ReadValue(DbDataReader reader, int ordinal) => reader.GetBoolean(ordinal);
}
