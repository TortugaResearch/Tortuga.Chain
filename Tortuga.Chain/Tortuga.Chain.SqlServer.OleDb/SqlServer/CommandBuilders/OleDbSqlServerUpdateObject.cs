using System.Data.OleDb;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SqlServer.CommandBuilders;

/// <summary>
/// Class OleDbSqlServerUpdateObject.
/// </summary>
internal sealed class OleDbSqlServerUpdateObject<TArgument> : OleDbSqlServerObjectCommand<TArgument>
	where TArgument : class
{
	readonly UpdateOptions m_Options;

	/// <summary>
	/// Initializes a new instance of the <see cref="OleDbSqlServerUpdateObject{TArgument}" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="options">The options.</param>
	public OleDbSqlServerUpdateObject(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName tableName, TArgument argumentValue, UpdateOptions options) : base(dataSource, tableName, argumentValue)
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

		if (!Table.HasPrimaryKey && !m_Options.HasFlag(UpdateOptions.UseKeyAttribute) && KeyColumns.Count == 0)
			throw new MappingException($"Cannot perform an update operation on {Table.Name} unless UpdateOptions.UseKeyAttribute or .WithKeys() is specified.");

		var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
		var desiredColumns = materializer.DesiredColumns();
		sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options, desiredColumns == Materializer.NoColumns);
		sqlBuilder.ApplyDesiredColumns(desiredColumns);

		if (KeyColumns.Count > 0)
			sqlBuilder.OverrideKeys(KeyColumns);

		var prefix = m_Options.HasFlag(UpdateOptions.ReturnOldValues) ? "Deleted." : "Inserted.";

		var sql = new StringBuilder();
		string? header;
		string? intoClause;
		string? footer;

		sqlBuilder.UseTableVariable(Table, out header, out intoClause, out footer);

		sql.Append(header);
		sql.Append($"UPDATE {Table.Name.ToQuotedString()}");
		sqlBuilder.BuildAnonymousSetClause(sql, " SET ", null, null);
		sqlBuilder.BuildSelectClause(sql, " OUTPUT ", prefix, intoClause);
		sqlBuilder.BuildAnonymousWhereClause(sql, " WHERE ", null, false); //second pass parameters
		sql.Append(";");
		sql.Append(footer);

		return new OleDbCommandExecutionToken(DataSource, "Update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters()).CheckUpdateRowCount(m_Options);
	}
}
