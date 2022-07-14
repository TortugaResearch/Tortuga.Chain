namespace Tortuga.Chain.Aggregates;

/// <summary>
/// This represents a grouping column.
/// </summary>
public class GroupByColumn : AggregateColumn
{
	/// <summary>
	/// Create a group by column.
	/// </summary>
	/// <param name="sourceColumnName">Name of the column to group by.</param>
	/// <param name="outputColumnName">If null, the sourceColumnName will be used.</param>
	/// <exception cref="ArgumentException">columnName is null or empty.</exception>
	public GroupByColumn(string sourceColumnName, string? outputColumnName)
	{
		if (string.IsNullOrEmpty(sourceColumnName))
			throw new ArgumentException($"{nameof(sourceColumnName)} is null or empty.", nameof(sourceColumnName));

		AggregateType = AggregateType.None;
		SourceColumnName = sourceColumnName;
		OutputColumnName = outputColumnName ?? sourceColumnName;
		GroupBy = true;
	}
}
