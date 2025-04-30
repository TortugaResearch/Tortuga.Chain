using System.ComponentModel;

namespace Tortuga.Chain.Aggregates;

/// <summary>
/// Base class for Aggregate attribute. Not to be used directly.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class BaseAggregateAttribute : Attribute
{
	/// <summary>
	/// If true, group by this column.
	/// </summary>
	public bool GroupBy { get; init; }

	/// <summary>
	/// The order is important for group by columns. If tied or null, uses the order the properties appear in reflection.
	/// </summary>
	/// <value>The grouping order.</value>
	public int? Order { get; init; }
}
