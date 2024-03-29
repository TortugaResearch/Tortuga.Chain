﻿using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders;

/// <summary>
/// OleDbSqlServerTableOrView supports queries against tables and views.
/// </summary>
internal sealed partial class OleDbSqlServerTableOrView<TObject> : TableDbCommandBuilder<OleDbCommand, OleDbParameter, SqlServerLimitOption, TObject>
        where TObject : class
{
    readonly TableOrViewMetadata<SqlServerObjectName, OleDbType> m_Table;
    object? m_ArgumentValue;
    FilterOptions m_FilterOptions;
    object? m_FilterValue;
    SqlServerLimitOption m_LimitOptions;
    int? m_Seed;

    int? m_Skip;
    IEnumerable<SortExpression> m_SortExpressions = Enumerable.Empty<SortExpression>();
    int? m_Take;
    string? m_WhereClause;

    /// <summary>
    /// Initializes a new instance of the <see cref="OleDbSqlServerTableOrView{TObject}" /> class.
    /// </summary>
    /// <param name="dataSource">The data source.</param>
    /// <param name="tableOrViewName">Name of the table or view.</param>
    /// <param name="filterValue">The filter value.</param>
    /// <param name="filterOptions">The filter options.</param>
    /// <exception cref="System.ArgumentException"></exception>
    public OleDbSqlServerTableOrView(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName tableOrViewName, object filterValue, FilterOptions filterOptions = FilterOptions.None) : base(dataSource)
    {
        if (tableOrViewName == SqlServerObjectName.Empty)
            throw new ArgumentException($"{nameof(tableOrViewName)} is empty", nameof(tableOrViewName));

        m_FilterValue = filterValue;
        m_FilterOptions = filterOptions;
        m_Table = DataSource.DatabaseMetadata.GetTableOrView(tableOrViewName);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OleDbSqlServerTableOrView{TObject}"/> class.
    /// </summary>
    /// <param name="dataSource">The data source.</param>
    /// <param name="tableOrViewName">Name of the table or view.</param>
    /// <param name="whereClause">The where clause.</param>
    /// <param name="argumentValue">The argument value.</param>
    public OleDbSqlServerTableOrView(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName tableOrViewName, string? whereClause, object? argumentValue) : base(dataSource)
    {
        if (tableOrViewName == SqlServerObjectName.Empty)
            throw new ArgumentException($"{nameof(tableOrViewName)} is empty", nameof(tableOrViewName));

        m_ArgumentValue = argumentValue;
        m_WhereClause = whereClause;
        m_Table = DataSource.DatabaseMetadata.GetTableOrView(tableOrViewName);
    }

    /// <summary>
    /// Gets the data source.
    /// </summary>
    /// <value>The data source.</value>
    public new OleDbSqlServerDataSourceBase DataSource => (OleDbSqlServerDataSourceBase)base.DataSource;

    /// <summary>
    /// Gets the columns from the metadata.
    /// </summary>
    /// <value>The columns.</value>
    protected override ColumnMetadataCollection Columns => m_Table.Columns.GenericCollection;

    /// <summary>
    /// Prepares the command for execution by generating any necessary SQL.
    /// </summary>
    /// <param name="materializer">The materializer.</param>
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
    {
        if (materializer == null)
            throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

        var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
        sqlBuilder.ApplyRulesForSelect(DataSource);

        if (AggregateColumns.IsEmpty)
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

        //Support check
        if (!Enum.IsDefined(typeof(SqlServerLimitOption), m_LimitOptions))
            throw new NotSupportedException($"SQL Server does not support limit option {(LimitOptions)m_LimitOptions}");

        //Validation
        if (m_Skip < 0)
            throw new InvalidOperationException($"Cannot skip {m_Skip} rows");

        if (m_Skip > 0 && !m_SortExpressions.Any())
            throw new InvalidOperationException($"Cannot perform a Skip operation with out a sort expression.");

        if (m_Skip > 0 && m_LimitOptions != SqlServerLimitOption.Rows)
            throw new InvalidOperationException($"Cannot perform a Skip operation with limit option {m_LimitOptions}");

        if (m_Take <= 0)
            throw new InvalidOperationException($"Cannot take {m_Take} rows");

        if ((m_LimitOptions == SqlServerLimitOption.TableSampleSystemRows || m_LimitOptions == SqlServerLimitOption.TableSampleSystemPercentage) && m_SortExpressions.Any())
            throw new InvalidOperationException($"Cannot perform random sampling when sorting.");

        if ((m_LimitOptions == SqlServerLimitOption.RowsWithTies || m_LimitOptions == SqlServerLimitOption.PercentageWithTies) && !m_SortExpressions.Any())
            throw new InvalidOperationException($"Cannot perform a WITH TIES operation without sorting.");

        //SQL Generation
        var parameters = new List<OleDbParameter>();
        var sql = new StringBuilder();

        string? topClause = null;
        switch (m_LimitOptions)
        {
            case SqlServerLimitOption.Rows:
                if (!m_SortExpressions.Any())
                    topClause = $"TOP ({m_Take}) ";
                break;

            case SqlServerLimitOption.Percentage:
                topClause = $"TOP ({m_Take}) PERCENT ";
                break;

            case SqlServerLimitOption.PercentageWithTies:
                topClause = $"TOP ({m_Take}) PERCENT WITH TIES ";
                break;

            case SqlServerLimitOption.RowsWithTies:
                topClause = $"TOP ({m_Take}) WITH TIES ";
                break;
        }

        if (AggregateColumns.IsEmpty)
            sqlBuilder.BuildSelectClause(sql, "SELECT " + topClause, null, null);
        else
            AggregateColumns.BuildSelectClause(sql, "SELECT ", DataSource, null);

        sql.Append(" FROM " + m_Table.Name.ToQuotedString());

        switch (m_LimitOptions)
        {
            case SqlServerLimitOption.TableSampleSystemRows:
                sql.Append($" TABLESAMPLE SYSTEM ({m_Take} ROWS) ");
                if (m_Seed.HasValue)
                    sql.Append($"REPEATABLE ({m_Seed}) ");
                break;

            case SqlServerLimitOption.TableSampleSystemPercentage:
                sql.Append($" TABLESAMPLE SYSTEM ({m_Take} PERCENT) ");
                if (m_Seed.HasValue)
                    sql.Append($"REPEATABLE ({m_Seed}) ");
                break;
        }

        if (m_FilterValue != null)
        {
            sql.Append(" WHERE (" + sqlBuilder.ApplyAnonymousFilterValue(m_FilterValue, m_FilterOptions) + ")");
            sqlBuilder.BuildAnonymousSoftDeleteClause(sql, " AND (", DataSource, ") ");

            parameters.AddRange(sqlBuilder.GetParameters());
        }
        else if (!string.IsNullOrWhiteSpace(m_WhereClause))
        {
            sql.Append(" WHERE (" + m_WhereClause + ")");
            sqlBuilder.BuildAnonymousSoftDeleteClause(sql, " AND (", DataSource, ") ");

            parameters = SqlBuilder.GetParameters<OleDbParameter>(m_ArgumentValue);
            parameters.AddRange(sqlBuilder.GetParameters());
        }
        else
        {
            sqlBuilder.BuildAnonymousSoftDeleteClause(sql, " WHERE ", DataSource, null);
            parameters.AddRange(sqlBuilder.GetParameters());
        }

        if (AggregateColumns.HasGroupBy)
            AggregateColumns.BuildGroupByClause(sql, " GROUP BY ", DataSource, null);

        if (m_LimitOptions.RequiresSorting() && !m_SortExpressions.Any())
        {
            if (m_Table.HasPrimaryKey)
                sqlBuilder.BuildOrderByClause(sql, " ORDER BY ", m_Table.PrimaryKeyColumns.Select(x => new SortExpression(x.SqlName)), null);
            else if (StrictMode)
                throw new InvalidOperationException("Limits were requested, but no primary keys were detected. Use WithSorting to supply a sort order or disable strict mode.");
        }
        else
        {
            sqlBuilder.BuildOrderByClause(sql, " ORDER BY ", m_SortExpressions, null);
        }

        switch (m_LimitOptions)
        {
            case SqlServerLimitOption.Rows:

                if (m_SortExpressions.Any())
                {
                    sql.Append(" OFFSET ? ROWS ");
                    parameters.Add(new OleDbParameter("@offset_row_count_expression", m_Skip ?? 0));

                    if (m_Take.HasValue)
                    {
                        sql.Append(" FETCH NEXT ? ROWS ONLY");
                        parameters.Add(new OleDbParameter("@fetch_row_count_expression", m_Take));
                    }
                }
                //else
                //{
                //    parameters.Add(new OleDbParameter("@fetch_row_count_expression", m_Take));
                //}
                break;

            case SqlServerLimitOption.Percentage:
            case SqlServerLimitOption.PercentageWithTies:
            case SqlServerLimitOption.RowsWithTies:
                break;
        }

        sql.Append(";");

        return new OleDbCommandExecutionToken(DataSource, "Query " + m_Table.Name, sql.ToString(), parameters);
    }

	/// <summary>
	/// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
	/// </summary>
	/// <returns>TableOrViewMetadata.</returns>
	protected override TableOrViewMetadata OnGetTable() => m_Table;

    /// <summary>
    /// Adds (or replaces) the filter on this command builder.
    /// </summary>
    /// <param name="filterValue">The filter value.</param>
    /// <param name="filterOptions">The filter options.</param>
    /// <returns>TableDbCommandBuilder&lt;OleDbCommand, OleDbParameter, SqlServerLimitOption&gt;.</returns>
    protected override TableDbCommandBuilder<OleDbCommand, OleDbParameter, SqlServerLimitOption> OnWithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None)
    {
        m_FilterValue = filterValue;
        m_WhereClause = null;
        m_ArgumentValue = null;
        m_FilterOptions = filterOptions;
        return this;
    }

    /// <summary>
    /// Adds (or replaces) the filter on this command builder.
    /// </summary>
    /// <param name="whereClause">The where clause.</param>
    /// <param name="argumentValue">The argument value.</param>
    /// <returns></returns>
    protected override TableDbCommandBuilder<OleDbCommand, OleDbParameter, SqlServerLimitOption> OnWithFilter(string whereClause, object? argumentValue)
    {
        m_FilterValue = null;
        m_WhereClause = whereClause;
        m_ArgumentValue = argumentValue;
        return this;
    }

    /// <summary>
    /// Adds limits to the command builder.
    /// </summary>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="take">Number of rows to take.</param>
    /// <param name="limitOptions">The limit options.</param>
    /// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
    /// <returns></returns>
    protected override TableDbCommandBuilder<OleDbCommand, OleDbParameter, SqlServerLimitOption> OnWithLimits(int? skip, int? take, SqlServerLimitOption limitOptions, int? seed)
    {
        m_Seed = seed;
        m_Skip = skip;
        m_Take = take;
        m_LimitOptions = limitOptions;
        return this;
    }

    /// <summary>
    /// Adds limits to the command builder.
    /// </summary>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="take">Number of rows to take.</param>
    /// <param name="limitOptions">The limit options.</param>
    /// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
    /// <returns></returns>
    protected override TableDbCommandBuilder<OleDbCommand, OleDbParameter, SqlServerLimitOption> OnWithLimits(int? skip, int? take, LimitOptions limitOptions, int? seed)
    {
        m_Seed = seed;
        m_Skip = skip;
        m_Take = take;
        m_LimitOptions = (SqlServerLimitOption)limitOptions;
        return this;
    }

    /// <summary>
    /// Adds sorting to the command builder.
    /// </summary>
    /// <param name="sortExpressions">The sort expressions.</param>
    /// <returns></returns>
    protected override TableDbCommandBuilder<OleDbCommand, OleDbParameter, SqlServerLimitOption> OnWithSorting(IEnumerable<SortExpression> sortExpressions)
    {
        if (sortExpressions == null)
            throw new ArgumentNullException(nameof(sortExpressions), $"{nameof(sortExpressions)} is null.");

		m_SortExpressions = sortExpressions;
		return this;
	}
}
