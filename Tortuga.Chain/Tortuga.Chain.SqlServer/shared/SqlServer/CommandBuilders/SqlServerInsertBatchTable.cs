using System.Data.Common;
using System.Text;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders;

/// <summary>
/// Class SqlServerInsertBatchTable is when a table-type parameter is provided.
/// </summary>
internal class SqlServerInsertBatchTable : MultipleRowDbCommandBuilder<SqlCommand, SqlParameter>
{
	readonly InsertOptions m_Options;

	readonly object m_Source;
	readonly TableOrViewMetadata<SqlServerObjectName, SqlDbType> m_Table;
	readonly UserDefinedTableTypeMetadata<SqlServerObjectName, SqlDbType> m_TableType;

	public SqlServerInsertBatchTable(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, DataTable dataTable, SqlServerObjectName tableTypeName, InsertOptions options) : base(dataSource)
	{
		m_Source = dataTable;
		m_Options = options;
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
		m_TableType = dataSource.DatabaseMetadata.GetUserDefinedTableType(tableTypeName);
	}

	public SqlServerInsertBatchTable(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, DbDataReader dataReader, SqlServerObjectName tableTypeName, InsertOptions options) : base(dataSource)
	{
		m_Source = dataReader;
		m_Options = options;
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
		m_TableType = dataSource.DatabaseMetadata.GetUserDefinedTableType(tableTypeName);
	}

	public override CommandExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
	{
		if (materializer == null)
			throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

		var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
		sqlBuilder.ApplyTableType(DataSource, OperationTypes.Insert, m_TableType.Columns);
		sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

		var sql = new StringBuilder();

		bool identityInsert = m_Options.HasFlag(InsertOptions.IdentityInsert);
		if (identityInsert)
			sql.AppendLine($"SET IDENTITY_INSERT {m_Table.Name.ToQuotedString()} ON;");

		sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {m_Table.Name.ToQuotedString()} (", null, ")", identityInsert);
		sqlBuilder.BuildSelectClause(sql, " OUTPUT ", "Inserted.", null);
		sqlBuilder.BuildSelectTvpForInsertClause(sql, " SELECT ", null, " FROM @ValuesParameter ", identityInsert);
		sql.Append(";");

		if (identityInsert)
			sql.AppendLine($"SET IDENTITY_INSERT {m_Table.Name.ToQuotedString()} OFF;");

		var parameters = sqlBuilder.GetParameters();
		parameters.Add(new SqlParameter()
		{
			ParameterName = "@ValuesParameter",
			Value = m_Source,
			SqlDbType = SqlDbType.Structured,
			TypeName = m_TableType.Name.ToQuotedString()
		});
		return new SqlServerCommandExecutionToken(DataSource, "Insert batch into " + m_Table.Name, sql.ToString(), parameters);
	}

	/// <summary>
	/// Returns the column associated with the column name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns></returns>
	/// <remarks>
	/// If the column name was not found, this will return null
	/// </remarks>
	public override ColumnMetadata? TryGetColumn(string columnName)
	{
		return m_Table.Columns.TryGetColumn(columnName);
	}

	/// <summary>
	/// Returns a list of columns known to be non-nullable.
	/// </summary>
	/// <returns>
	/// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
	/// </returns>
	/// <remarks>
	/// This is used by materializers to skip IsNull checks.
	/// </remarks>
	public override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => m_Table.NonNullableColumns;
}