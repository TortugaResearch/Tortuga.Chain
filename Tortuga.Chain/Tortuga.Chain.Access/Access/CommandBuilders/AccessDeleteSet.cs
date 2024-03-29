﻿using System.Data.OleDb;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Access.CommandBuilders;

/// <summary>
/// Class AccessDeleteSet.
/// </summary>
internal sealed class AccessDeleteSet : DeleteSetDbCommandBuilder<OleDbCommand, OleDbParameter>
{
	readonly TableOrViewMetadata<AccessObjectName, OleDbType> m_Table;

	/// <summary>
	/// Initializes a new instance of the <see cref="AccessDeleteSet" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="parameters">The parameters.</param>
	/// <param name="expectedRowCount">The expected row count.</param>
	/// <param name="options">The options.</param>
	public AccessDeleteSet(AccessDataSourceBase dataSource, AccessObjectName tableName, string whereClause, IEnumerable<OleDbParameter> parameters, int? expectedRowCount, DeleteOptions options) : base(dataSource, whereClause, parameters, expectedRowCount, options)
	{
		if (options.HasFlag(DeleteOptions.UseKeyAttribute))
			throw new NotSupportedException("Cannot use Key attributes with this operation.");

		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AccessDeleteSet"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="argumentValue">The argument value.</param>
	public AccessDeleteSet(AccessDataSourceBase dataSource, AccessObjectName tableName, string whereClause, object? argumentValue) : base(dataSource, whereClause, argumentValue)
	{
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AccessDeleteSet"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The options.</param>
	public AccessDeleteSet(AccessDataSourceBase dataSource, AccessObjectName tableName, object filterValue, FilterOptions filterOptions) : base(dataSource, filterValue, filterOptions)
	{
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
	{
		if (materializer == null)
			throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

		var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
		sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

		List<OleDbParameter> parameters;
		var sql = new StringBuilder();

		sql.Append("DELETE FROM " + m_Table.Name.ToQuotedString());
		if (FilterValue != null)
		{
			sql.Append(" WHERE " + sqlBuilder.ApplyFilterValue(FilterValue, FilterOptions));
			parameters = sqlBuilder.GetParameters(DataSource);
		}
		else if (!string.IsNullOrWhiteSpace(WhereClause))
		{
			sql.Append(" WHERE " + WhereClause);
			parameters = SqlBuilder.GetParameters<OleDbParameter>(ArgumentValue);
			parameters.AddRange(sqlBuilder.GetParameters(DataSource));
		}
		else
		{
			parameters = sqlBuilder.GetParameters(DataSource);
		}
		sql.Append(";");
		if (Parameters != null)
			parameters.AddRange(Parameters);

		var deleteCommand = new AccessCommandExecutionToken(DataSource, "Delete from " + m_Table.Name, sql.ToString(), parameters).CheckDeleteRowCount(Options, ExpectedRowCount);

		var desiredColumns = materializer.DesiredColumns();
		if (desiredColumns == Materializer.NoColumns)
			return deleteCommand;

		var result = PrepareRead(desiredColumns);
		result.NextCommand = deleteCommand;
		return result;
	}

	/// <summary>
	/// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
	/// </summary>
	/// <returns>TableOrViewMetadata.</returns>
	protected override TableOrViewMetadata OnGetTable() => m_Table;

	AccessCommandExecutionToken PrepareRead(IReadOnlyList<string> desiredColumns)
	{
		var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
		sqlBuilder.ApplyDesiredColumns(desiredColumns);

		List<OleDbParameter> parameters;
		var sql = new StringBuilder();
		sqlBuilder.BuildSelectClause(sql, "SELECT ", null, null);
		sql.Append(" FROM " + m_Table.Name.ToQuotedString());
		if (FilterValue != null)
		{
			sql.Append(" WHERE " + sqlBuilder.ApplyFilterValue(FilterValue, FilterOptions));
			parameters = sqlBuilder.GetParameters(DataSource);
		}
		else if (!string.IsNullOrWhiteSpace(WhereClause))
		{
			sql.Append(" WHERE " + WhereClause);
			parameters = SqlBuilder.GetParameters<OleDbParameter>(ArgumentValue);
			parameters.AddRange(sqlBuilder.GetParameters(DataSource));
		}
		else
		{
			parameters = sqlBuilder.GetParameters(DataSource);
		}
		sql.Append(";");

		if (Parameters != null)
			parameters.AddRange(Parameters.Select(p => p.Clone()));

		return new AccessCommandExecutionToken(DataSource, "Query after deleting " + m_Table.Name, sql.ToString(), parameters);
	}
}
