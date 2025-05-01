using Npgsql;
using NpgsqlTypes;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql.CommandBuilders;

/// <summary>
/// Class PostgreSqlDeleteSet.
/// </summary>
internal sealed class PostgreSqlDeleteSet : DeleteSetDbCommandBuilder<NpgsqlCommand, NpgsqlParameter>
{
	readonly TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> m_Table;

	/// <summary>
	/// Initializes a new instance of the <see cref="PostgreSqlDeleteSet" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="parameters">The parameters.</param>
	/// <param name="expectedRowCount">The expected row count.</param>
	/// <param name="options">The options.</param>
	public PostgreSqlDeleteSet(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, string whereClause, IEnumerable<NpgsqlParameter> parameters, int? expectedRowCount, DeleteOptions options) : base(dataSource, whereClause, parameters, expectedRowCount, options)
	{
		if (options.HasFlag(DeleteOptions.UseKeyAttribute))
			throw new NotSupportedException("Cannot use Key attributes with this operation.");

		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PostgreSqlDeleteSet"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="argumentValue">The argument value.</param>
	public PostgreSqlDeleteSet(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, string whereClause, object? argumentValue) : base(dataSource, whereClause, argumentValue)
	{
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PostgreSqlDeleteSet"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The options.</param>
	public PostgreSqlDeleteSet(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, object filterValue, FilterOptions filterOptions) : base(dataSource, filterValue, filterOptions)
	{
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	public override CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
	{
		if (materializer == null)
			throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

		var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
		sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

		List<NpgsqlParameter> parameters;
		var sql = new StringBuilder();
		sql.Append("DELETE FROM " + m_Table.Name.ToQuotedString());
		if (FilterValue != null)
		{
			sql.Append(" WHERE " + sqlBuilder.ApplyFilterValue(FilterValue, FilterOptions));

			parameters = sqlBuilder.GetParameters();
		}
		else if (!string.IsNullOrWhiteSpace(WhereClause))
		{
			sql.Append(" WHERE " + WhereClause);

			parameters = SqlBuilder.GetParameters<NpgsqlParameter>(ArgumentValue);
			parameters.AddRange(sqlBuilder.GetParameters());
		}
		else
		{
			parameters = sqlBuilder.GetParameters();
		}
		sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, null);
		sql.Append(';');

		if (Parameters != null)
			parameters.AddRange(Parameters);

		return new PostgreSqlCommandExecutionToken(DataSource, "Delete from " + m_Table.Name, sql.ToString(), parameters).CheckDeleteRowCount(Options, ExpectedRowCount);
	}

	/// <summary>
	/// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
	/// </summary>
	/// <returns>TableOrViewMetadata.</returns>
	protected override TableOrViewMetadata OnGetTable() => m_Table;
}
