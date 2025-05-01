using System.Data.SQLite;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.SQLite.CommandBuilders;

/// <summary>
/// SQLiteTableOrView supports queries against tables and views.
/// </summary>
[UseTrait(typeof(SupportsCount64Trait<SQLiteCommand, SQLiteParameter, SQLiteLimitOption>))]
internal sealed partial class SQLiteTableOrView<TObject> : TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption, TObject>
	where TObject : class
{
	readonly TableOrViewMetadata<SQLiteObjectName, DbType> m_Table;
	object? m_ArgumentValue;
	FilterOptions m_FilterOptions;
	object? m_FilterValue;
	SQLiteLimitOption m_LimitOptions;
	int? m_Skip;
	IEnumerable<SortExpression> m_SortExpressions = Enumerable.Empty<SortExpression>();
	int? m_Take;
	string? m_WhereClause;
	//public object MetadataCache { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="SQLiteTableOrView{TObject}" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableOrViewName">Name of the table or view.</param>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The filter options.</param>
	public SQLiteTableOrView(SQLiteDataSourceBase dataSource, SQLiteObjectName tableOrViewName, object filterValue, FilterOptions filterOptions = FilterOptions.None) :
		base(dataSource)
	{
		m_FilterValue = filterValue;
		m_FilterOptions = filterOptions;
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableOrViewName);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SQLiteTableOrView{TObject}" /> class.
	/// </summary>
	/// <param name="dataSource"></param>
	/// <param name="tableOrViewName"></param>
	/// <param name="whereClause"></param>
	/// <param name="argumentValue"></param>
	public SQLiteTableOrView(SQLiteDataSourceBase dataSource, SQLiteObjectName tableOrViewName, string? whereClause, object? argumentValue) :
		base(dataSource)
	{
		m_ArgumentValue = argumentValue;
		m_WhereClause = whereClause;
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableOrViewName);
	}

	/// <summary>
	/// Gets the columns from the metadata.
	/// </summary>
	/// <value>The columns.</value>
	public override ColumnMetadataCollection Columns => m_Table.Columns.GenericCollection;

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <param name="materializer"></param>
	/// <returns></returns>
	public override CommandExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
	{
		if (materializer == null)
			throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

		var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
		sqlBuilder.ApplyRulesForSelect(DataSource);

		if (AggregateColumns.IsEmpty)
			sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

		//Support check
		if (!Enum.IsDefined(m_LimitOptions))
			throw new NotSupportedException($"SQL Server does not support limit option {(LimitOptions)m_LimitOptions}");

		//Validation
		if (m_Skip < 0)
			throw new InvalidOperationException($"Cannot skip {m_Skip} rows");
		if (m_Skip > 0 && m_LimitOptions != SQLiteLimitOption.Rows)
			throw new InvalidOperationException($"Cannot perform a Skip operation with limit option {m_LimitOptions}");
		if (m_Take <= 0)
			throw new InvalidOperationException($"Cannot take {m_Take} rows");
		if (m_LimitOptions == SQLiteLimitOption.RandomSampleRows && m_SortExpressions.Any())
			throw new InvalidOperationException($"Cannot perform a random sampling when sorting.");

		//SQL Generation
		List<SQLiteParameter> parameters;

		var sql = new StringBuilder();

		if (AggregateColumns.IsEmpty)
			sqlBuilder.BuildSelectClause(sql, "SELECT ", null, null);
		else
			AggregateColumns.BuildSelectClause(sql, "SELECT ", DataSource, null);

		sql.Append(" FROM " + m_Table.Name);

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

			parameters = SqlBuilder.GetParameters<SQLiteParameter>(m_ArgumentValue);
			parameters.AddRange(sqlBuilder.GetParameters());
		}
		else
		{
			sqlBuilder.BuildSoftDeleteClause(sql, " WHERE ", DataSource, null);
			parameters = sqlBuilder.GetParameters();
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
			case SQLiteLimitOption.Rows:
				sql.Append(" LIMIT @fetch_row_count_expression OFFSET @offset_row_count_expression ");
				parameters.Add(new SQLiteParameter("@fetch_row_count_expression", m_Take));
				parameters.Add(new SQLiteParameter("@offset_row_count_expression", m_Skip ?? 0));

				break;

			case SQLiteLimitOption.RandomSampleRows:
				sql.Append(" ORDER BY RANDOM() LIMIT @fetch_row_count_expression ");
				parameters.Add(new SQLiteParameter("@fetch_row_count_expression", m_Take));
				break;
		}

		sql.Append(';');

		return new SQLiteCommandExecutionToken(DataSource, "Query " + m_Table.Name, sql.ToString(), parameters, lockType: LockType.Read);
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
	/// <returns>TableDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter, SQLiteLimitOption&gt;.</returns>
	protected override TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> OnWithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None)
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
	protected override TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> OnWithFilter(string whereClause, object? argumentValue)
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
	protected override TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> OnWithLimits(int? skip, int? take, SQLiteLimitOption limitOptions, int? seed)
	{
		//m_Seed = seed;
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
	protected override TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> OnWithLimits(int? skip, int? take, LimitOptions limitOptions, int? seed)
	{
		//m_Seed = seed;
		m_Skip = skip;
		m_Take = take;
		m_LimitOptions = (SQLiteLimitOption)limitOptions;
		return this;
	}

	/// <summary>
	/// Adds sorting to the command builder.
	/// </summary>
	/// <param name="sortExpressions">The sort expressions.</param>
	/// <returns></returns>
	protected override TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> OnWithSorting(IEnumerable<SortExpression> sortExpressions)
	{
		if (sortExpressions == null)
			throw new ArgumentNullException(nameof(sortExpressions), $"{nameof(sortExpressions)} is null.");

		m_SortExpressions = sortExpressions;
		return this;
	}
}
