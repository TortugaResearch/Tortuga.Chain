namespace Tortuga.Chain.Aggregates;

/// <summary>
/// Gets the type of the aggregate to be performed.
/// </summary>
public enum AggregateType
{
	/// <summary>
	/// No aggregate. Usually used for GroupBy columns.
	/// </summary>
	None = 0,

	/// <summary>
	/// Custom aggregate. Uses the SelectExpression function.
	/// </summary>
	Custom,

	/// <summary>
	/// Gets the minimum value for the indicated column.
	/// </summary>
	Min,

	/// <summary>
	/// Gets the maximum value for the indicated column.
	/// </summary>
	Max,

	/// <summary>
	/// Gets the average value for the indicated column.
	/// </summary>
	Average,

	/// <summary>
	/// Gets the count of non-null values the indicated column. May provide '*'.
	/// </summary>
	Count,

	/// <summary>
	/// Gets the count of distinct, non-null values the indicated column. May provide '*'.
	/// </summary>
	CountDistinct,

	/// <summary>
	/// Gets the sum of non-null values the indicated column.
	/// </summary>
	Sum,

	/// <summary>
	/// Gets the sum of distinct, non-null values the indicated column.
	/// </summary>
	SumDistinct
}
