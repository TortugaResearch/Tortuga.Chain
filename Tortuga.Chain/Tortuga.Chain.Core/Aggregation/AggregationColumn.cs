using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Aggregation;

/// <summary>
/// An AggregationColumn is used to generate aggregations in the SQL generation.
/// </summary>
public class AggregationColumn
{
	/// <summary>
	/// Create a non-custom aggregation column.
	/// </summary>
	/// <param name="aggregationType">Type of the aggregation. Cannot be Custom.</param>
	/// <param name="columnName">Name of the column.</param>
	/// <param name="asColumnName">Name of as column.</param>
	/// <exception cref="System.ArgumentOutOfRangeException">aggregationType - Cannot use this overload with AggregationType.Custom. Use the one with a SelectExpression.</exception>
	/// <exception cref="System.ArgumentException">columnName is null or empty.</exception>
	/// <exception cref="System.ArgumentException">asColumnName is null or empty.</exception>
	public AggregationColumn(AggregationType aggregationType, string columnName, string asColumnName)
	{
		if (!Enum.IsDefined(typeof(AggregationType), aggregationType))
			throw new ArgumentOutOfRangeException(nameof(aggregationType), aggregationType, $"{nameof(aggregationType)} is not defined.");

		if (aggregationType == AggregationType.Custom)
			throw new ArgumentOutOfRangeException(nameof(aggregationType), aggregationType, $"Cannot use this overload with AggregationType.Custom. Use the one with a SelectExpression.");

		if (string.IsNullOrEmpty(columnName))
			throw new ArgumentException($"{nameof(columnName)} is null or empty.", nameof(columnName));

		if (string.IsNullOrEmpty(asColumnName))
			throw new ArgumentException($"{nameof(asColumnName)} is null or empty.", nameof(asColumnName));

		AggregationType = aggregationType;
		ColumnName = columnName;
		AsColumnName = asColumnName;
	}

	/// <summary>
	/// Create a group by column.
	/// </summary>
	/// <param name="columnName">Name of the column to group by.</param>
	/// <exception cref="System.ArgumentException">columnName is null or empty.</exception>
	public AggregationColumn(string columnName)
	{
		if (string.IsNullOrEmpty(columnName))
			throw new ArgumentException($"{nameof(columnName)} is null or empty.", nameof(columnName));

		AggregationType = AggregationType.None;
		ColumnName = columnName;
		AsColumnName = columnName;
		GroupBy = true;
	}

	/// <summary>
	/// Create a non-custom aggregation column.
	/// </summary>
	/// <param name="selectExpression">The SQL expression to use.</param>
	/// <param name="asColumnName">Name of as column.</param>
	/// <exception cref="System.ArgumentException">asColumnName is null or empty.</exception>
	public AggregationColumn(string selectExpression, string asColumnName)
	{
		if (string.IsNullOrEmpty(selectExpression))
			throw new ArgumentException($"{nameof(selectExpression)} is null or empty.", nameof(selectExpression));

		if (string.IsNullOrEmpty(asColumnName))
			throw new ArgumentException($"{nameof(asColumnName)} is null or empty.", nameof(asColumnName));

		AggregationType = AggregationType.Custom;
		SelectExpression = selectExpression;
		AsColumnName = asColumnName;
	}

	/// <summary>
	/// Gets the type of the aggregation to be performed.
	/// </summary>
	/// <value>The type of the aggregation.</value>
	public AggregationType AggregationType { get; }

	/// <summary>
	/// Gets the column name to be used in the result set.
	/// </summary>
	public string AsColumnName { get; }

	/// <summary>
	/// Gets the name of the source column to be provided to the aggregate function.
	/// </summary>
	public string? ColumnName { get; }

	/// <summary>
	/// If true, group by this column.
	/// </summary>
	public bool GroupBy { get; }

	/// <summary>
	/// Gets the custom select expression to be used. Only applicable when AggregationType is Custom.
	/// </summary>
	/// <value>The select expression.</value>
	public string? SelectExpression { get; }

	internal string ToGroupBySql(IDatabaseMetadataCache metadataCache)
	{
		if (!GroupBy)
			throw new InvalidOperationException($"Cannot call {nameof(ToGroupBySql)} if {nameof(GroupBy)} is false.");
		if (ColumnName == null)
			throw new InvalidOperationException($"{nameof(ColumnName)} is null");
		return metadataCache.QuoteColumnName(ColumnName!);
	}

	internal string ToSelectSql(IDatabaseMetadataCache metadataCache)
	{
		return AggregationType switch
		{
			AggregationType.None => $"{metadataCache.QuoteColumnName(ColumnName!)} AS {metadataCache.QuoteColumnName(AsColumnName)}",
			AggregationType.Custom => $"{SelectExpression} AS {metadataCache.QuoteColumnName(AsColumnName)}",
			_ => $"{metadataCache.GetAggregationFunction(AggregationType, ColumnName!)} AS {metadataCache.QuoteColumnName(AsColumnName)}",
		};
	}
}
