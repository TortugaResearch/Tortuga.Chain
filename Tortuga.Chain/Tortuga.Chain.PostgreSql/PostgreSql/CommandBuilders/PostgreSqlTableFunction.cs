using Npgsql;
using NpgsqlTypes;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql.CommandBuilders;

/// <summary>
/// Class PostgreSqlTableFunction.
/// </summary>
public class PostgreSqlTableFunction : TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption>
{
	readonly object? m_FunctionArgumentValue;
	readonly TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType> m_Table;
	object? m_ArgumentValue;
	FilterOptions m_FilterOptions;
	object? m_FilterValue;
	PostgreSqlLimitOption m_LimitOptions;

	int? m_Skip;
	IEnumerable<SortExpression> m_SortExpressions = Enumerable.Empty<SortExpression>();
	int? m_Take;
	string? m_WhereClause;
	bool m_Distinct;

	/// <summary>
	/// Initializes a new instance of the <see cref="PostgreSqlTableFunction" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableFunctionName">Name of the table function.</param>
	/// <param name="functionArgumentValue">The function argument.</param>
	public PostgreSqlTableFunction(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableFunctionName, object? functionArgumentValue) : base(dataSource)
	{
		if (dataSource == null)
			throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");

		m_Table = dataSource.DatabaseMetadata.GetTableFunction(tableFunctionName);
		m_FunctionArgumentValue = functionArgumentValue;
	}

	/// <summary>
	/// Gets the columns from the metadata.
	/// </summary>
	/// <value>The columns.</value>
	public override ColumnMetadataCollection Columns => m_Table.Columns.GenericCollection;

	/// <summary>
	/// Gets the data source.
	/// </summary>
	/// <value>The data source.</value>
	public new PostgreSqlDataSourceBase DataSource => (PostgreSqlDataSourceBase)base.DataSource;

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <param name="materializer">The materializer.</param>
	/// <returns>
	/// ExecutionToken&lt;TCommand&gt;.
	/// </returns>
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	public override CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
	{
		if (materializer == null)
			throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

		var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
		sqlBuilder.ApplyRulesForSelect(DataSource);

		if (m_FunctionArgumentValue != null)
			sqlBuilder.ApplyArgumentValue(DataSource, m_FunctionArgumentValue);
		if (AggregateColumns.IsEmpty)
		{
			var desired = materializer.DesiredColumns();
			if (desired == Materializer.AutoSelectDesiredColumns)
				desired = Materializer.AllColumns;
			sqlBuilder.ApplyDesiredColumns(desired);
		}

		//Support check
		if (!Enum.IsDefined(m_LimitOptions))
			throw new NotSupportedException($"PostgreSQL does not support limit option {(LimitOptions)m_LimitOptions}");

		//Validation
		if (m_Skip < 0)
			throw new InvalidOperationException($"Cannot skip {m_Skip} rows");

		if (m_Skip > 0 && m_LimitOptions != PostgreSqlLimitOption.Rows)
			throw new InvalidOperationException($"Cannot perform a Skip operation with limit option {m_LimitOptions}");

		if (m_Take <= 0)
			throw new InvalidOperationException($"Cannot take {m_Take} rows");

		//SQL Generation
		List<NpgsqlParameter> parameters;
		var sql = new StringBuilder();

		var distinctClause = m_Distinct ? "DISTINCT " : "";

		if (AggregateColumns.IsEmpty)
			sqlBuilder.BuildSelectClause(sql, "SELECT " + distinctClause, null, null);
		else
			AggregateColumns.BuildSelectClause(sql, "SELECT " + distinctClause, DataSource, null);

		sqlBuilder.BuildFromFunctionClause(sql, $" FROM {m_Table.Name.ToQuotedString()} (", " ) ");

		if (m_FilterValue != null)
		{
			sql.Append(" WHERE (" + sqlBuilder.ApplyFilterValue(m_FilterValue, m_FilterOptions) + ")");
			sqlBuilder.BuildSoftDeleteClause(sql, " AND (", DataSource, ") ");

			parameters = sqlBuilder.GetParameters();
		}
		else if (!string.IsNullOrWhiteSpace(m_WhereClause))
		{
			sql.Append(" WHERE (" + m_WhereClause + ")");
			sqlBuilder.BuildSoftDeleteClause(sql, " AND (", DataSource, ") ");

			parameters = SqlBuilder.GetParameters<NpgsqlParameter>(m_ArgumentValue);
			parameters.AddRange(sqlBuilder.GetParameters());
		}
		else
		{
			sqlBuilder.BuildSoftDeleteClause(sql, " WHERE ", DataSource, null);
			parameters = sqlBuilder.GetParameters();
		}

		if (AggregateColumns.HasGroupBy)
			AggregateColumns.BuildGroupByClause(sql, " GROUP BY ", DataSource, null);

		if (m_LimitOptions.RequiresSorting() && !m_SortExpressions.Any() && StrictMode)
			throw new InvalidOperationException("Limits were requested without a sort order. Use WithSorting to supply a sort order or disable strict mode.");

		sqlBuilder.BuildOrderByClause(sql, " ORDER BY ", m_SortExpressions, null);

		switch (m_LimitOptions)
		{
			case PostgreSqlLimitOption.Rows:

				sql.Append(" OFFSET @offset_row_count_expression");
				parameters.Add(new NpgsqlParameter("@offset_row_count_expression", m_Skip ?? 0));

				if (m_Take.HasValue)
				{
					sql.Append(" LIMIT @limit_row_count_expression");
					parameters.Add(new NpgsqlParameter("@limit_row_count_expression", m_Take));
				}

				break;
		}

		sql.Append(';');

		return new PostgreSqlCommandExecutionToken(DataSource, "Query Function " + m_Table.Name, sql.ToString(), parameters);
	}

	/// <summary>
	/// Returns the column associated with the column name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns></returns>
	/// <remarks>
	/// If the column name was not found, this will return null
	/// </remarks>
	public override ColumnMetadata? TryGetColumn(string columnName) => m_Table.Columns.TryGetColumn(columnName);

	/// <summary>
	/// Returns a list of columns.
	/// </summary>
	/// <returns>If the command builder doesn't know which columns are available, an empty list will be returned.</returns>
	/// <remarks>This is used by materializers to skip exclude columns.</remarks>
	public override IReadOnlyList<ColumnMetadata> TryGetColumns() => m_Table.Columns;

	/// <summary>
	/// Returns a list of columns known to be non-nullable.
	/// </summary>
	/// <returns>
	/// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
	/// </returns>
	/// <remarks>
	/// This is used by materializers to skip IsNull checks.
	/// </remarks>
	public override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => m_Table.NullableColumns;

	/// <summary>
	/// Adds (or replaces) the filter on this command builder.
	/// </summary>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The filter options.</param>
	/// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
	protected override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> OnWithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None)
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
	protected override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> OnWithFilter(string whereClause, object? argumentValue)
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
	protected override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> OnWithLimits(int? skip, int? take, PostgreSqlLimitOption limitOptions, int? seed)
	{
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
	protected override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> OnWithLimits(int? skip, int? take, LimitOptions limitOptions, int? seed)
	{
		m_Skip = skip;
		m_Take = take;
		m_LimitOptions = (PostgreSqlLimitOption)limitOptions;
		return this;
	}

	/// <summary>
	/// Adds sorting to the command builder.
	/// </summary>
	/// <param name="sortExpressions">The sort expressions.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	protected override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> OnWithSorting(IEnumerable<SortExpression> sortExpressions)
	{
		if (sortExpressions == null)
			throw new ArgumentNullException(nameof(sortExpressions), $"{nameof(sortExpressions)} is null.");

		m_SortExpressions = sortExpressions;
		return this;
	}

	/// <summary>
	/// Adds DISTINCT to the command builder.
	/// </summary>
	/// <returns>TableDbCommandBuilder&lt;AbstractCommand, AbstractParameter, AbstractLimitOption&gt;.</returns>
	protected override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> OnWithDistinct()
	{
		m_Distinct = true;
		return this;
	}
}
