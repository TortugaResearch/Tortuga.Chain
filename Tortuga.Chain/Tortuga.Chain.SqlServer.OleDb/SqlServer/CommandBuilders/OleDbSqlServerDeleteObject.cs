﻿using System.Data.OleDb;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SqlServer.CommandBuilders;

/// <summary>
/// Class SqlServerDeleteObject.
/// </summary>
internal sealed class OleDbSqlServerDeleteObject<TArgument> : OleDbSqlServerObjectCommand<TArgument>
	where TArgument : class
{
	readonly DeleteOptions m_Options;

	/// <summary>
	/// Initializes a new instance of the <see cref="OleDbSqlServerDeleteObject{TArgument}"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="options">The options.</param>
	public OleDbSqlServerDeleteObject(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName tableName, TArgument argumentValue, DeleteOptions options) : base(dataSource, tableName, argumentValue)
	{
		m_Options = options;
	}

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <param name="materializer">The materializer.</param>
	/// <returns>ExecutionToken&lt;TCommand&gt;.</returns>

	public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
	{
		if (materializer == null)
			throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

		var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
		var desiredColumns = materializer.DesiredColumns();
		sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options, desiredColumns == Materializer.NoColumns);
		sqlBuilder.ApplyDesiredColumns(desiredColumns);

		if (KeyColumns.Count > 0)
			sqlBuilder.OverrideKeys(KeyColumns);

		var sql = new StringBuilder();
		string? header;
		string? intoClause;
		string? footer;

		sqlBuilder.UseTableVariable(Table, out header, out intoClause, out footer);
		sql.Append(header);
		sql.Append("DELETE FROM " + Table.Name.ToQuotedString());
		sqlBuilder.BuildSelectClause(sql, " OUTPUT ", "Deleted.", intoClause);
		sqlBuilder.BuildAnonymousWhereClause(sql, " WHERE ", null, true);
		sql.Append(';');
		sql.Append(footer);

		return new OleDbCommandExecutionToken(DataSource, "Delete from " + Table.Name, sql.ToString(), sqlBuilder.GetParameters()).CheckDeleteRowCount(m_Options);
	}
}
