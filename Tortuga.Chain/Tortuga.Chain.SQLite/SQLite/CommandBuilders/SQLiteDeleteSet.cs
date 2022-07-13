using System.Data.SQLite;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SQLite.CommandBuilders;

/// <summary>
/// Class SQLiteDeleteSet.
/// </summary>
internal sealed class SQLiteDeleteSet : DeleteSetDbCommandBuilder<SQLiteCommand, SQLiteParameter>
{
	readonly TableOrViewMetadata<SQLiteObjectName, DbType> m_Table;

	/// <summary>
	/// Initializes a new instance of the <see cref="SQLiteDeleteSet" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="parameters">The parameters.</param>
	/// <param name="expectedRowCount">The expected row count.</param>
	/// <param name="options">The options.</param>
	public SQLiteDeleteSet(SQLiteDataSourceBase dataSource, SQLiteObjectName tableName, string whereClause, IEnumerable<SQLiteParameter> parameters, int? expectedRowCount, DeleteOptions options) : base(dataSource, whereClause, parameters, expectedRowCount, options)
	{
		if (options.HasFlag(DeleteOptions.UseKeyAttribute))
			throw new NotSupportedException("Cannot use Key attributes with this operation.");

		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SQLiteDeleteSet"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="argumentValue">The argument value.</param>
	public SQLiteDeleteSet(SQLiteDataSourceBase dataSource, SQLiteObjectName tableName, string whereClause, object? argumentValue) : base(dataSource, whereClause, argumentValue)
	{
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SQLiteDeleteSet"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The options.</param>
	public SQLiteDeleteSet(SQLiteDataSourceBase dataSource, SQLiteObjectName tableName, object filterValue, FilterOptions filterOptions) : base(dataSource, filterOptions, filterOptions)
	{
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	public override CommandExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
	{
		if (materializer == null)
			throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

		var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
		sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

		List<SQLiteParameter> parameters;
		var sql = new StringBuilder();

		if (sqlBuilder.HasReadFields)
		{
			sqlBuilder.BuildSelectClause(sql, "SELECT ", null, " FROM " + m_Table.Name.ToQuotedString());
			if (FilterValue != null)
				sql.Append(" WHERE " + sqlBuilder.ApplyFilterValue(FilterValue, FilterOptions));
			else if (!string.IsNullOrWhiteSpace(WhereClause))
				sql.Append(" WHERE " + WhereClause);
			sql.AppendLine(";");
		}

		sql.Append("DELETE FROM " + m_Table.Name.ToQuotedString());
		if (FilterValue != null)
		{
			sql.Append(" WHERE " + sqlBuilder.ApplyFilterValue(FilterValue, FilterOptions));
			parameters = sqlBuilder.GetParameters();
		}
		else if (!string.IsNullOrWhiteSpace(WhereClause))
		{
			sql.Append(" WHERE " + WhereClause);
			parameters = SqlBuilder.GetParameters<SQLiteParameter>(ArgumentValue);
			parameters.AddRange(sqlBuilder.GetParameters());
		}
		else
		{
			parameters = sqlBuilder.GetParameters();
		}
		sql.Append(";");

		if (Parameters != null)
			parameters.AddRange(Parameters);

		return new SQLiteCommandExecutionToken(DataSource, "Delete from " + m_Table.Name, sql.ToString(), parameters, lockType: LockType.Write).CheckDeleteRowCount(Options, ExpectedRowCount);
	}

	/// <summary>
	/// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
	/// </summary>
	/// <returns>TableOrViewMetadata.</returns>
	protected override TableOrViewMetadata OnGetTable() => m_Table;
}
