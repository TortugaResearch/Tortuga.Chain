namespace Tortuga.Chain.Aggregates;

/// <summary>
/// This attribute indicates that the propery should be mapped to an aggregate column.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class GroupByColumnAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GroupByColumnAttribute"/> class.
	/// </summary>
	/// <param name="sourceColumnName">Name of the source column. If null, uses the property name.</param>
	/// <param name="groupingOrder">The order to apply the group by columns. If null, uses the order the properties appear in reflection.</param>
	public GroupByColumnAttribute(string? sourceColumnName = null, int? groupingOrder = null)
	{
		SourceColumnName = sourceColumnName;
		GroupingOrder = groupingOrder ?? int.MaxValue;
	}

	/// <summary>
	/// The order to apply the group by columns. If tied, uses the order the properties appear in reflection.
	/// </summary>
	/// <value>The grouping order.</value>
	/// <remarks>Defaults to Int32.MaxValue</remarks>
	public int GroupingOrder { get; }

	/// <summary>
	/// Gets the name of the source column to be provided to the aggregate function.
	/// </summary>
	public string? SourceColumnName { get; }
}
