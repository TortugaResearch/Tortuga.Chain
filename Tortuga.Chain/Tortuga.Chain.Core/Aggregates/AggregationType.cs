using System.ComponentModel;

namespace Tortuga.Chain.Aggregates;

/// <summary>
/// Gets the type of the aggregate to be performed.
/// </summary>
public enum AggregateType
{
	/// <summary>
	/// Custom aggregate. Uses the SelectExpression function.
	/// </summary>
	/// <remarks>This is for internal use only.</remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	Custom = -1,

	/// <summary>
	/// No aggregate. Usually used for GroupBy columns.
	/// </summary>
	None = 0,

	/// <summary>
	/// Gets the minimum value for the indicated column.
	/// </summary>
	Min = 1,

	/// <summary>
	/// Gets the maximum value for the indicated column.
	/// </summary>
	Max = 2,

	/// <summary>
	/// Gets the average value for the indicated column.
	/// </summary>
	Average = 3,

	/// <summary>
	/// Gets the count of non-null values the indicated column. May provide '*'.
	/// </summary>
	Count = 4,

	/// <summary>
	/// Gets the count of distinct, non-null values the indicated column. May provide '*'.
	/// </summary>
	CountDistinct = 5,

	/// <summary>
	/// Gets the sum of non-null values the indicated column.
	/// </summary>
	Sum = 6,

	/// <summary>
	/// Gets the sum of distinct, non-null values the indicated column.
	/// </summary>
	SumDistinct = 7,
}
