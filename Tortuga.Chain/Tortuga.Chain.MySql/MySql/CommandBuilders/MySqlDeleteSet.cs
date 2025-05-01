﻿using MySqlConnector;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql.CommandBuilders;

/// <summary>
/// Class MySqlDeleteSet.
/// </summary>
internal sealed class MySqlDeleteSet : DeleteSetDbCommandBuilder<MySqlCommand, MySqlParameter>
{
	readonly TableOrViewMetadata<MySqlObjectName, MySqlDbType> m_Table;

	/// <summary>
	/// Initializes a new instance of the <see cref="MySqlDeleteSet" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="parameters">The parameters.</param>
	/// <param name="expectedRowCount">The expected row count.</param>
	/// <param name="options">The options.</param>
	public MySqlDeleteSet(MySqlDataSourceBase dataSource, MySqlObjectName tableName, string whereClause, IEnumerable<MySqlParameter> parameters, int? expectedRowCount, DeleteOptions options) : base(dataSource, whereClause, parameters, expectedRowCount, options)
	{
		if (options.HasFlag(DeleteOptions.UseKeyAttribute))
			throw new NotSupportedException("Cannot use Key attributes with this operation.");

		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MySqlDeleteSet"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="argumentValue">The argument value.</param>
	public MySqlDeleteSet(MySqlDataSourceBase dataSource, MySqlObjectName tableName, string whereClause, object? argumentValue) : base(dataSource, whereClause, argumentValue)
	{
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MySqlDeleteSet"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The options.</param>
	public MySqlDeleteSet(MySqlDataSourceBase dataSource, MySqlObjectName tableName, object filterValue, FilterOptions filterOptions) : base(dataSource, filterValue, filterOptions)
	{
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	public override CommandExecutionToken<MySqlCommand, MySqlParameter> Prepare(Materializer<MySqlCommand, MySqlParameter> materializer)
	{
		if (materializer == null)
			throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

		var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
		sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

		List<MySqlParameter> parameters;
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
			parameters = SqlBuilder.GetParameters<MySqlParameter>(ArgumentValue);
			parameters.AddRange(sqlBuilder.GetParameters());
		}
		else
		{
			parameters = sqlBuilder.GetParameters();
		}
		sql.Append(';');

		if (Parameters != null)
			parameters.AddRange(Parameters);

		return new MySqlCommandExecutionToken(DataSource, "Delete from " + m_Table.Name, sql.ToString(), parameters).CheckDeleteRowCount(Options, ExpectedRowCount);
	}

	/// <summary>
	/// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
	/// </summary>
	/// <returns>TableOrViewMetadata.</returns>
	protected override TableOrViewMetadata OnGetTable() => m_Table;
}
