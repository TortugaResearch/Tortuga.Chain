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
	/// <param name="sourceColumnName">Name of the source column.</param>
	/// <param name="outputColumnName">Name of the output column.</param>
	/// <exception cref="System.ArgumentOutOfRangeException">aggregateType - Cannot use this overload with AggregateType.Custom. Use the one with a SelectExpression.</exception>
	/// <exception cref="System.ArgumentException">columnName is null or empty.</exception>
	/// <exception cref="System.ArgumentException">asColumnName is null or empty.</exception>
	public AggregateColumn(AggregateType aggregateType, string sourceColumnName, string outputColumnName)
	{
		if (!Enum.IsDefined(aggregateType))
			throw new ArgumentOutOfRangeException(nameof(aggregateType), aggregateType, $"{nameof(aggregateType)} is not defined.");

		if (aggregateType == AggregateType.Custom)
			throw new ArgumentOutOfRangeException(nameof(aggregateType), aggregateType, $"AggregateType.Custom cannot be used here. Create a CustomAggregateColumn instead.");

		if (string.IsNullOrEmpty(sourceColumnName))
			throw new ArgumentException($"{nameof(sourceColumnName)} is null or empty.", nameof(sourceColumnName));

		if (string.IsNullOrEmpty(outputColumnName))
			throw new ArgumentException($"{nameof(outputColumnName)} is null or empty.", nameof(outputColumnName));

		AggregateType = aggregateType;
		this.SourceColumnName = sourceColumnName;
		OutputColumnName = outputColumnName;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AggregateColumn"/> class.
	/// </summary>
	/// <remarks>Subclasses are expected to properly set the properties.</remarks>
	protected AggregateColumn()
	{
	}

	/// <summary>
	/// Gets the type of the aggregate to be performed.
	/// </summary>
	/// <value>The type of the aggregate.</value>
	public AggregateType AggregateType { get; protected init; }

	/// <summary>
	/// If true, group by this column.
	/// </summary>
	/// <remarks>This may be set to true by subclasses.</remarks>
	public bool GroupBy { get; protected init; }

	/// <summary>
	/// Gets the column name to be used in the result set.
	/// </summary>
	public string? OutputColumnName { get; protected init; }

	/// <summary>
	/// Gets the custom select expression to be used. Only applicable when AggregateType is Custom.
	/// </summary>
	/// <value>The select expression.</value>
	public string? SelectExpression { get; protected init; }

	/// <summary>
	/// Gets the name of the source column to be provided to the aggregate function.
	/// </summary>
	public string? SourceColumnName { get; protected init; }

	internal string ToGroupBySql(IDatabaseMetadataCache metadataCache)
	{
		if (!GroupBy)
			throw new InvalidOperationException($"Cannot call {nameof(ToGroupBySql)} if {nameof(GroupBy)} is false.");
		if (SourceColumnName == null)
			throw new InvalidOperationException($"{nameof(SourceColumnName)} is null");
		return metadataCache.QuoteColumnName(SourceColumnName!);
	}

	internal string ToSelectSql(IDatabaseMetadataCache metadataCache)
	{
		if (AggregateType != AggregateType.Custom && SourceColumnName == null)
			throw new InvalidOperationException("Non-custom aggregates must have a source column.");

		var asClause = !string.IsNullOrEmpty(OutputColumnName) ? $" AS {metadataCache.QuoteColumnName(OutputColumnName!)}" : null;

		return AggregateType switch
		{
			AggregateType.None => $"{metadataCache.QuoteColumnName(SourceColumnName!)}{asClause}",
			AggregateType.Custom => $"{SelectExpression}{asClause}",
			_ => $"{metadataCache.GetAggregateFunction(AggregateType, SourceColumnName!)}{asClause}",
		};
	}
}
