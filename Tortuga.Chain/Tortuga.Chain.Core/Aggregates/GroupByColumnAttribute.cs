namespace Tortuga.Chain.Aggregates;

/// <summary>
/// This attribute indicates that the propery should be mapped to an aggregate column.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class GroupByColumnAttribute : BaseAggregateAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GroupByColumnAttribute"/> class.
	/// </summary>
	/// <param name="sourceColumnName">Name of the source column. If null, uses the property name.</param>
	public GroupByColumnAttribute(string? sourceColumnName = null)
	{
		SourceColumnName = sourceColumnName;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="GroupByColumnAttribute"/> class.
	/// </summary>
	/// <param name="sourceColumnName">Name of the source column. If null, uses the property name.</param>
	/// <param name="order">The order to apply the group by columns. If null, uses the order the properties appear in reflection.</param>
	public GroupByColumnAttribute(string? sourceColumnName, int order)
	{
		SourceColumnName = sourceColumnName;
		Order = order;
		GroupBy = true;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="GroupByColumnAttribute"/> class.
	/// </summary>
	/// <param name="order">The order to apply the group by columns. If null, uses the order the properties appear in reflection.</param>
	public GroupByColumnAttribute(int order)
	{
		Order = order;
		GroupBy = true;
	}

	/// <summary>
	/// Gets the name of the source column to be provided to the aggregate function.
	/// </summary>
	public string? SourceColumnName { get; }
}
