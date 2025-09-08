﻿using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SqlServer.CommandBuilders;

/// <summary>
/// Class SqlServerDeleteObject.
/// </summary>
internal sealed class SqlServerDeleteObject<TArgument> : SqlServerObjectCommand<TArgument>
	where TArgument : class
{
	readonly DeleteOptions m_Options;

	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerDeleteObject{TArgument}"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="options">The options.</param>
	public SqlServerDeleteObject(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, TArgument argumentValue, DeleteOptions options) : base(dataSource, tableName, argumentValue)
	{
		m_Options = options;
	}

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <param name="materializer">The materializer.</param>
	/// <returns>ExecutionToken&lt;TCommand&gt;.</returns>

	public override CommandExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
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

		sqlBuilder.UseTableVariable(Table, out var header, out var intoClause, out var footer);
		sql.Append(header);
		sql.Append("DELETE FROM " + Table.Name.ToQuotedString());
		sqlBuilder.BuildSelectClause(sql, " OUTPUT ", "Deleted.", intoClause);
		sqlBuilder.BuildWhereClause(sql, " WHERE ", null);
		sql.Append(';');
		sql.Append(footer);

		return new SqlServerCommandExecutionToken(DataSource, "Delete from " + Table.Name, sql.ToString(), sqlBuilder.GetParameters()).CheckDeleteRowCount(m_Options);
	}
}