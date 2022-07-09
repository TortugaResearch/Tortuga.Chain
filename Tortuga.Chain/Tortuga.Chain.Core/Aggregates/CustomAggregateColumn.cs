namespace Tortuga.Chain.Aggregates;

/// <summary>
/// This represents an aggregate column with a custom expression.
/// </summary>
public class CustomAggregateColumn : AggregateColumn
{
	/// <summary>
	/// Create a custom aggregate column.
	/// </summary>
	/// <param name="selectExpression">The SQL expression to use.</param>
	/// <param name="outputColumnName">Name of output column.</param>
	/// <param name="groupBy">Indicates whether the expression is also used for grouping.</param>
	/// <exception cref="ArgumentException">asColumnName is null or empty.</exception>
	public CustomAggregateColumn(string selectExpression, string outputColumnName, bool groupBy = false)
	{
		if (string.IsNullOrEmpty(selectExpression))
			throw new ArgumentException($"{nameof(selectExpression)} is null or empty.", nameof(selectExpression));

		if (string.IsNullOrEmpty(outputColumnName))
			throw new ArgumentException($"{nameof(outputColumnName)} is null or empty.", nameof(outputColumnName));

		AggregateType = AggregateType.Custom;
		SelectExpression = selectExpression;
		OutputColumnName = outputColumnName;
		GroupBy = groupBy;
	}
}
