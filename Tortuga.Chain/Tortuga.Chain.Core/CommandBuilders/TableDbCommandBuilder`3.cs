using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Aggregates;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// This is the base class for table style command builders such as FromTableOrView and FromTableValueFunction.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TParameter">The type of the parameter.</typeparam>
/// <typeparam name="TLimit">The type of the limit option.</typeparam>
/// <seealso cref="CommandBuilders.MultipleRowDbCommandBuilder{TCommand, TParameter}" />
/// <seealso cref="ITableDbCommandBuilder" />
[SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
public abstract class TableDbCommandBuilder<TCommand, TParameter, TLimit> : MultipleRowDbCommandBuilder<TCommand, TParameter>, ITableDbCommandBuilder
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TLimit : struct //really an enum
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TableDbCommandBuilder{TCommand, TParameter, TLimit}"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	protected TableDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource) : base(dataSource)
	{
	}

	/// <summary>
	/// Gets the aggregate columns.
	/// </summary>
	/// <value>The aggregate columns.</value>
	protected AggregateColumnCollection AggregateColumns { get; } = new();

	/// <summary>
	/// Gets the columns from the metadata.
	/// </summary>
	protected abstract ColumnMetadataCollection Columns { get; }

	/// <summary>
	/// Gets the default limit option.
	/// </summary>
	/// <value>
	/// The default limit options.
	/// </value>
	/// <remarks>For most data sources, this will be LimitOptions.Rows. </remarks>
	protected virtual LimitOptions DefaultLimitOption => LimitOptions.Rows;

	/// <summary>
	/// Performs an aggregation on the table using the provided object.
	/// </summary>
	/// <typeparam name="TObject">The type of the t object.</typeparam>
	/// <returns>ObjectMultipleRow&lt;TCommand, TParameter, TObject&gt;.</returns>
	public ObjectMultipleRow<TCommand, TParameter, TObject> AsAggregate<TObject>()
		where TObject : class
	{
		//TODO: Waiting on Anchor 4.1 - JLA
		//TODO: Need to deal with group by order! - JLA
		var properties = MetadataCache.GetMetadata<TObject>().Properties;

		AggregateColumns.Clear();
		foreach (var property in properties)
		{
			var found = 0;
			if (property.MappedColumnName == null)
				continue; //Property has a NotMappedAttribute

			AggregateColumnAttribute? aggregateColumn = property.Attributes.GetAttribute<AggregateColumnAttribute>();
			GroupByColumnAttribute? groupByColumn = property.Attributes.GetAttribute<GroupByColumnAttribute>();
			CustomAggregateColumnAttribute? customAggregateColumn = property.Attributes.GetAttribute<CustomAggregateColumnAttribute>();

			if (aggregateColumn != null)
			{
				found += 1;
				AggregateColumns.Add(new AggregateColumn(aggregateColumn.AggregateType, aggregateColumn.SourceColumnName, property.MappedColumnName));
			}
			if (groupByColumn != null)
			{
				found += 1;
				AggregateColumns.Add(new GroupByColumn(groupByColumn.SourceColumnName ?? property.MappedColumnName, property.MappedColumnName));
			}
			if (customAggregateColumn != null)
			{
				found += 1;
				AggregateColumns.Add(new CustomAggregateColumn(customAggregateColumn.SelectExpression, property.MappedColumnName, customAggregateColumn.GroupBy));
			}

			if (found == 0)
			{
				AggregateColumns.Add(new AggregateColumn(AggregateType.None, property.MappedColumnName, property.MappedColumnName));
				continue;
			}

			if (found > 1)
				throw new MappingException($"Only one of {nameof(AggregateColumnAttribute)}, {nameof(GroupByColumnAttribute)}, or {nameof(CustomAggregateColumnAttribute)} may be on a given property.");
		}

		return new ObjectMultipleRow<TCommand, TParameter, TObject>(this);
	}

	/// <summary>
	/// Performs an aggregation on the table.
	/// </summary>
	/// <param name="aggregateColumns">The aggregate columns.</param>
	public MultipleRowDbCommandBuilder<TCommand, TParameter> AsAggregate(IEnumerable<AggregateColumn> aggregateColumns)
	{
		AggregateColumns.Clear();
		AggregateColumns.AddRange(aggregateColumns);
		return this;
	}

	/// <summary>
	/// Performs an aggregation on the table.
	/// </summary>
	/// <param name="aggregateColumns">The aggregate columns.</param>
	public MultipleRowDbCommandBuilder<TCommand, TParameter> AsAggregate(params AggregateColumn[] aggregateColumns)
	{
		return AsAggregate((IEnumerable<AggregateColumn>)aggregateColumns);
	}

	/// <summary>
	/// Gets the average value for the indicated column.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	public ScalarDbCommandBuilder<TCommand, TParameter> AsAverage(string columnName)
	{
		var column = Columns[columnName];
		AggregateColumns.Add(new AggregateColumn(AggregateType.Average, column.SqlName, column.SqlName));
		return this;
	}

	/// <summary>
	/// Returns the row count using a <c>SELECT Count(*)</c> style query.
	/// </summary>
	/// <returns></returns>
	public ILink<long> AsCount()
	{
		AggregateColumns.Add(new AggregateColumn(AggregateType.Count, "*", "RowCount"));
		return ToInt64();
	}

	/// <summary>
	/// Returns the row count for a given column. <c>SELECT Count(columnName)</c>
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <param name="distinct">if set to <c>true</c> use <c>SELECT COUNT(DISTINCT columnName)</c>.</param>
	/// <returns></returns>
	public ILink<long> AsCount(string columnName, bool distinct = false)
	{
		var column = Columns[columnName];
		AggregateColumns.Add(new AggregateColumn(distinct ? AggregateType.CountDistinct : AggregateType.Count, column.SqlName, column.SqlName));

		return ToInt64();
	}

	/// <summary>
	/// Gets the maximum value for the indicated column.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	public ScalarDbCommandBuilder<TCommand, TParameter> AsMax(string columnName)
	{
		var column = Columns[columnName];
		AggregateColumns.Add(new AggregateColumn(AggregateType.Max, column.SqlName, column.SqlName));
		return this;
	}

	/// <summary>
	/// Gets the minimum value for the indicated column.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	public ScalarDbCommandBuilder<TCommand, TParameter> AsMin(string columnName)
	{
		var column = Columns[columnName];
		AggregateColumns.Add(new AggregateColumn(AggregateType.Min, column.SqlName, column.SqlName));
		return this;
	}

	/// <summary>
	/// Gets the sum of non-null values the indicated column.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <param name="distinct">If true, only include distinct rows.</param>
	public ScalarDbCommandBuilder<TCommand, TParameter> AsSum(string columnName, bool distinct = false)
	{
		var column = Columns[columnName];
		AggregateColumns.Add(new AggregateColumn(distinct ? AggregateType.SumDistinct : AggregateType.Sum, column.SqlName, column.SqlName));
		return this;
	}

	/// <summary>
	/// Adds (or replaces) the filter on this command builder.
	/// </summary>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The filter options.</param>
	public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None)
		=> OnWithFilter(filterValue, filterOptions);

	/// <summary>
	/// Adds (or replaces) the filter on this command builder.
	/// </summary>
	/// <param name="whereClause">The where clause.</param>
	/// <returns></returns>
	public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithFilter(string whereClause)
		=> OnWithFilter(whereClause, null);

	/// <summary>
	/// Adds (or replaces) the filter on this command builder.
	/// </summary>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <returns></returns>
	public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithFilter(string whereClause, object? argumentValue)
		=> OnWithFilter(whereClause, argumentValue);

	ITableDbCommandBuilder ITableDbCommandBuilder.WithFilter(object filterValue, FilterOptions filterOptions) => WithFilter(filterValue, filterOptions);

	ITableDbCommandBuilder ITableDbCommandBuilder.WithFilter(string whereClause) => WithFilter(whereClause);

	ITableDbCommandBuilder ITableDbCommandBuilder.WithFilter(string whereClause, object? argumentValue) => WithFilter(whereClause, argumentValue);

	/// <summary>
	/// Adds limits to the command builder.
	/// </summary>
	/// <param name="skip">The number of rows to skip.</param>
	/// <param name="take">Number of rows to take.</param>
	/// <returns></returns>
	public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithLimits(int? skip, int? take) => OnWithLimits(skip, take, DefaultLimitOption, null);

	/// <summary>
	/// Adds limits to the command builder.
	/// </summary>
	/// <param name="take">Number of rows to take.</param>
	/// <returns></returns>
	public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithLimits(int? take) => OnWithLimits(null, take, DefaultLimitOption, null);

	/// <summary>
	/// Adds limits to the command builder.
	/// </summary>
	/// <param name="take">Number of rows to take.</param>
	/// <param name="limitOptions">The limit options.</param>
	/// <returns></returns>
	public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithLimits(int? take, TLimit limitOptions) => OnWithLimits(null, take, limitOptions, null);

	/// <summary>
	/// Adds limits to the command builder.
	/// </summary>
	/// <param name="take">Number of rows to take.</param>
	/// <param name="limitOptions">The limit options.</param>
	/// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
	/// <returns></returns>
	public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithLimits(int? take, TLimit limitOptions, int seed) => OnWithLimits(null, take, limitOptions, seed);

	ITableDbCommandBuilder ITableDbCommandBuilder.WithLimits(int take, LimitOptions limitOptions, int? seed) => OnWithLimits(null, take, limitOptions, seed);

	ITableDbCommandBuilder ITableDbCommandBuilder.WithLimits(int skip, int take) => OnWithLimits(skip, take, DefaultLimitOption, null);

	/// <summary>
	/// Adds sorting to the command builder.
	/// </summary>
	/// <param name="sortExpressions">The sort expressions.</param>
	/// <returns></returns>
	public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithSorting(IEnumerable<SortExpression> sortExpressions)
		=> OnWithSorting(sortExpressions);

	/// <summary>
	/// Adds sorting to the command builder
	/// </summary>
	/// <param name="sortExpressions">The sort expressions.</param>
	/// <returns></returns>
	public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithSorting(params SortExpression[] sortExpressions) => WithSorting((IEnumerable<SortExpression>)sortExpressions);

	ITableDbCommandBuilder ITableDbCommandBuilder.WithSorting(IEnumerable<SortExpression> sortExpressions) => WithSorting(sortExpressions);

	ITableDbCommandBuilder ITableDbCommandBuilder.WithSorting(params SortExpression[] sortExpressions) => WithSorting((IEnumerable<SortExpression>)sortExpressions);

	/// <summary>
	/// Adds (or replaces) the filter on this command builder.
	/// </summary>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <returns></returns>
	protected abstract TableDbCommandBuilder<TCommand, TParameter, TLimit> OnWithFilter(string whereClause, object? argumentValue);

	/// <summary>
	/// Adds (or replaces) the filter on this command builder.
	/// </summary>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The filter options.</param>
	protected abstract TableDbCommandBuilder<TCommand, TParameter, TLimit> OnWithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None);

	/// <summary>
	/// Adds limits to the command builder.
	/// </summary>
	/// <param name="skip">The number of rows to skip.</param>
	/// <param name="take">Number of rows to take.</param>
	/// <param name="limitOptions">The limit options.</param>
	/// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
	/// <returns></returns>
	protected abstract TableDbCommandBuilder<TCommand, TParameter, TLimit> OnWithLimits(int? skip, int? take, TLimit limitOptions, int? seed);

	/// <summary>
	/// Adds limits to the command builder.
	/// </summary>
	/// <param name="skip">The number of rows to skip.</param>
	/// <param name="take">Number of rows to take.</param>
	/// <param name="limitOptions">The limit options.</param>
	/// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
	/// <returns></returns>
	protected abstract TableDbCommandBuilder<TCommand, TParameter, TLimit> OnWithLimits(int? skip, int? take, LimitOptions limitOptions, int? seed);

	/// <summary>
	/// Adds sorting to the command builder.
	/// </summary>
	/// <param name="sortExpressions">The sort expressions.</param>
	/// <returns></returns>
	protected abstract TableDbCommandBuilder<TCommand, TParameter, TLimit> OnWithSorting(IEnumerable<SortExpression> sortExpressions);
}
