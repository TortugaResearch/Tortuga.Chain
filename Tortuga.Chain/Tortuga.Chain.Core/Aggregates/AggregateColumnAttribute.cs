namespace Tortuga.Chain.Aggregates;

/// <summary>
/// This attribute indicates that the propery should be mapped to an aggregate column.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class AggregateColumnAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AggregateColumnAttribute"/> class.
	/// </summary>
	/// <param name="aggregateType">Type of the aggregate.</param>
	/// <param name="sourceColumnName">Name of the source column.</param>
	public AggregateColumnAttribute(AggregateType aggregateType, string sourceColumnName)
	{
		AggregateType = AggregateType;
		SourceColumnName = sourceColumnName;
	}

	/// <summary>
	/// Gets the type of the aggregate to be performed.
	/// </summary>
	/// <value>The type of the aggregate.</value>
	public AggregateType AggregateType { get; }

	/// <summary>
	/// Gets the name of the source column to be provided to the aggregate function.
	/// </summary>
	public string SourceColumnName { get; }
}
