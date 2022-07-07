using System.Data.OleDb;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SqlServer.CommandBuilders;

/// <summary>
/// Class OleDbSqlServerInsertOrUpdateObject.
/// </summary>
internal sealed class OleDbSqlServerInsertOrUpdateObject<TArgument> : OleDbSqlServerObjectCommand<TArgument>
		where TArgument : class
{
	readonly UpsertOptions m_Options;

	/// <summary>
	/// Initializes a new instance of the <see cref="OleDbSqlServerInsertOrUpdateObject{TArgument}"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="options">The options.</param>
	public OleDbSqlServerInsertOrUpdateObject(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName tableName, TArgument argumentValue, UpsertOptions options) : base(dataSource, tableName, argumentValue)
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
		sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
		sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

		if (KeyColumns.Count > 0)
			sqlBuilder.OverrideKeys(KeyColumns);

		var availableColumns = sqlBuilder.GetParameterizedColumns().ToList();

		var sql = new StringBuilder();
		string? header;
		string? intoClause;
		string? footer;

		sqlBuilder.UseTableVariable(Table, out header, out intoClause, out footer);

		sql.Append(header);
		sql.Append($"MERGE INTO {Table.Name.ToQuotedString()} WITH ( UPDLOCK, SERIALIZABLE ) target USING ");
		sql.Append("(VALUES (" + string.Join(", ", availableColumns.Select(c => "?")) + ")) AS source (" + string.Join(", ", availableColumns.Select(c => c.QuotedSqlName)) + ")");
		sql.Append(" ON ");
		sql.Append(string.Join(" AND ", sqlBuilder.GetKeyColumns().ToList().Select(c => $"target.{c.QuotedSqlName} = source.{c.QuotedSqlName}")));

		sql.Append(" WHEN MATCHED THEN UPDATE SET ");
		sql.Append(string.Join(", ", sqlBuilder.GetUpdateColumns().Select(x => $"{x.QuotedSqlName} = source.{x.QuotedSqlName}")));

		var insertColumns = sqlBuilder.GetInsertColumns();
		sql.Append(" WHEN NOT MATCHED THEN INSERT (");
		sql.Append(string.Join(", ", insertColumns.Select(x => x.QuotedSqlName)));
		sql.Append(") VALUES (");
		sql.Append(string.Join(", ", insertColumns.Select(x => "source." + x.QuotedSqlName)));
		sql.Append(" )");
		sqlBuilder.BuildSelectClause(sql, " OUTPUT ", "Inserted.", intoClause);
		sql.Append(";");
		sql.Append(footer);

		return new OleDbCommandExecutionToken(DataSource, "Insert or update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters());
	}
}
