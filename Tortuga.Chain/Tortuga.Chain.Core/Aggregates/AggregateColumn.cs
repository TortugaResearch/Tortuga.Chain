using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Aggregates;

/// <summary>
/// An AggregateColumn is used to generate aggregates in the SQL generation.
/// </summary>
public class AggregateColumn
{
	/// <summary>
	/// Create a non-custom aggregate column.
	/// </summary>
	/// <param name="aggregateType">Type of the aggregate. Cannot be Custom.</param>
	/// <param name="columnName">Name of the column.</param>
	/// <param name="asColumnName">Name of as column.</param>
	/// <exception cref="System.ArgumentOutOfRangeException">aggregateType - Cannot use this overload with AggregateType.Custom. Use the one with a SelectExpression.</exception>
	/// <exception cref="System.ArgumentException">columnName is null or empty.</exception>
	/// <exception cref="System.ArgumentException">asColumnName is null or empty.</exception>
	public AggregateColumn(AggregateType aggregateType, string columnName, string asColumnName)
	{
		if (!Enum.IsDefined(typeof(AggregateType), aggregateType))
			throw new ArgumentOutOfRangeException(nameof(aggregateType), aggregateType, $"{nameof(aggregateType)} is not defined.");

		if (aggregateType == AggregateType.Custom)
			throw new ArgumentOutOfRangeException(nameof(aggregateType), aggregateType, $"Cannot use this overload with AggregateType.Custom. Use the one with a SelectExpression.");

		if (string.IsNullOrEmpty(columnName))
			throw new ArgumentException($"{nameof(columnName)} is null or empty.", nameof(columnName));

		if (string.IsNullOrEmpty(asColumnName))
			throw new ArgumentException($"{nameof(asColumnName)} is null or empty.", nameof(asColumnName));

		AggregateType = aggregateType;
		ColumnName = columnName;
		AsColumnName = asColumnName;
	}

	/// <summary>
	/// Create a group by column.
	/// </summary>
	/// <param name="columnName">Name of the column to group by.</param>
	/// <exception cref="System.ArgumentException">columnName is null or empty.</exception>
	public AggregateColumn(string columnName)
	{
		if (string.IsNullOrEmpty(columnName))
			throw new ArgumentException($"{nameof(columnName)} is null or empty.", nameof(columnName));

		AggregateType = AggregateType.None;
		ColumnName = columnName;
		AsColumnName = columnName;
		GroupBy = true;
	}

	/// <summary>
	/// Create a custom aggregate column.
	/// </summary>
	/// <param name="selectExpression">The SQL expression to use.</param>
	/// <param name="asColumnName">Name of as column.</param>
	/// <exception cref="System.ArgumentException">asColumnName is null or empty.</exception>
	public AggregateColumn(string selectExpression, string asColumnName)
	{
		if (string.IsNullOrEmpty(selectExpression))
			throw new ArgumentException($"{nameof(selectExpression)} is null or empty.", nameof(selectExpression));

		if (string.IsNullOrEmpty(asColumnName))
			throw new ArgumentException($"{nameof(asColumnName)} is null or empty.", nameof(asColumnName));

		AggregateType = AggregateType.Custom;
		SelectExpression = selectExpression;
		AsColumnName = asColumnName;
	}

	/// <summary>
	/// Gets the type of the aggregate to be performed.
	/// </summary>
	/// <value>The type of the aggregate.</value>
	public AggregateType AggregateType { get; }

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
	/// Gets the custom select expression to be used. Only applicable when AggregateType is Custom.
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
		return AggregateType switch
		{
			AggregateType.None => $"{metadataCache.QuoteColumnName(ColumnName!)} AS {metadataCache.QuoteColumnName(AsColumnName)}",
			AggregateType.Custom => $"{SelectExpression} AS {metadataCache.QuoteColumnName(AsColumnName)}",
			_ => $"{metadataCache.GetAggregateFunction(AggregateType, ColumnName!)} AS {metadataCache.QuoteColumnName(AsColumnName)}",
		};
	}
}
