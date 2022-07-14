using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders;

/// <summary>
/// Class SqlServerDeleteSet.
/// </summary>
internal sealed class SqlServerDeleteSet : DeleteSetDbCommandBuilder<SqlCommand, SqlParameter>
{
	readonly SqlServerTableOrViewMetadata<SqlDbType> m_Table;

	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerDeleteSet" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="parameters">The parameters.</param>
	/// <param name="expectedRowCount">The expected row count.</param>
	/// <param name="options">The options.</param>
	public SqlServerDeleteSet(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, string whereClause, IEnumerable<SqlParameter> parameters, int? expectedRowCount, DeleteOptions options) : base(dataSource, whereClause, parameters, expectedRowCount, options)
	{
		if (options.HasFlag(DeleteOptions.UseKeyAttribute))
			throw new NotSupportedException("Cannot use Key attributes with this operation.");

		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerDeleteSet"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="argumentValue">The argument value.</param>
	public SqlServerDeleteSet(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, string whereClause, object? argumentValue) : base(dataSource, whereClause, argumentValue)
	{
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerDeleteSet"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The options.</param>
	public SqlServerDeleteSet(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, object filterValue, FilterOptions filterOptions) : base(dataSource, filterOptions, filterOptions)
	{
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
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

		var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
		sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

		List<SqlParameter> parameters;
		var sql = new StringBuilder();

		sqlBuilder.UseTableVariable(m_Table, out var header, out var intoClause, out var footer);

		sql.Append(header);
		sql.Append("DELETE FROM " + m_Table.Name.ToQuotedString());
		sqlBuilder.BuildSelectClause(sql, " OUTPUT ", "Deleted.", intoClause);
		if (FilterValue != null)
		{
			sql.Append(" WHERE " + sqlBuilder.ApplyFilterValue(FilterValue, FilterOptions));

			parameters = sqlBuilder.GetParameters();
		}
		else if (!string.IsNullOrWhiteSpace(WhereClause))
		{
			sql.Append(" WHERE " + WhereClause);

			parameters = SqlBuilder.GetParameters<SqlParameter>(ArgumentValue);
			parameters.AddRange(sqlBuilder.GetParameters());
		}
		else
		{
			parameters = sqlBuilder.GetParameters();
		}
		sql.Append(";");
		sql.Append(footer);

		if (Parameters != null)
			parameters.AddRange(Parameters);

		return new SqlServerCommandExecutionToken(DataSource, "Delete from " + m_Table.Name, sql.ToString(), parameters).CheckDeleteRowCount(Options, ExpectedRowCount);
	}

	/// <summary>
	/// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
	/// </summary>
	/// <returns>TableOrViewMetadata.</returns>
	protected override TableOrViewMetadata OnGetTable() => m_Table;
}
