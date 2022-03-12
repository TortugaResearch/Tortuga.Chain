using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
	/// <summary>
	/// Class SqlServerDeleteWithFilter.
	/// </summary>
	internal sealed class SqlServerDeleteMany : MultipleRowDbCommandBuilder<SqlCommand, SqlParameter>
	{
		//readonly DeleteOptions m_Options;
		readonly IEnumerable<SqlParameter>? m_Parameters;

		readonly SqlServerTableOrViewMetadata<SqlDbType> m_Table;
		readonly string? m_WhereClause;
		readonly object? m_ArgumentValue;
		readonly object? m_FilterValue;
		readonly FilterOptions m_FilterOptions;
		readonly DeleteOptions m_Options;
		readonly int? m_ExpectedRowCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlServerDeleteMany" /> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="whereClause">The where clause.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="expectedRowCount">The expected row count.</param>
		/// <param name="options">The options.</param>
		public SqlServerDeleteMany(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, string whereClause, IEnumerable<SqlParameter> parameters, int? expectedRowCount, DeleteOptions options) : base(dataSource)
		{
			if (options.HasFlag(DeleteOptions.UseKeyAttribute))
				throw new NotSupportedException("Cannot use Key attributes with this operation.");

			m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
			m_WhereClause = whereClause;
			m_Parameters = parameters;
			m_Options = options;
			m_ExpectedRowCount = expectedRowCount;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlServerDeleteMany"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="whereClause">The where clause.</param>
		/// <param name="argumentValue">The argument value.</param>
		public SqlServerDeleteMany(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, string whereClause, object? argumentValue) : base(dataSource)
		{
			m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
			m_WhereClause = whereClause;
			m_ArgumentValue = argumentValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlServerDeleteMany"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="filterValue">The filter value.</param>
		/// <param name="filterOptions">The options.</param>
		public SqlServerDeleteMany(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, object filterValue, FilterOptions filterOptions) : base(dataSource)
		{
			m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
			m_FilterValue = filterValue;
			m_FilterOptions = filterOptions;
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
			if (m_FilterValue != null)
			{
				sql.Append(" WHERE " + sqlBuilder.ApplyFilterValue(m_FilterValue, m_FilterOptions));

				parameters = sqlBuilder.GetParameters();
			}
			else if (!string.IsNullOrWhiteSpace(m_WhereClause))
			{
				sql.Append(" WHERE " + m_WhereClause);

				parameters = SqlBuilder.GetParameters<SqlParameter>(m_ArgumentValue);
				parameters.AddRange(sqlBuilder.GetParameters());
			}
			else
			{
				parameters = sqlBuilder.GetParameters();
			}
			sql.Append(";");
			sql.Append(footer);

			if (m_Parameters != null)
				parameters.AddRange(m_Parameters);

			return new SqlServerCommandExecutionToken(DataSource, "Delete from " + m_Table.Name, sql.ToString(), parameters).CheckDeleteRowCount(m_Options, m_ExpectedRowCount);
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
}
