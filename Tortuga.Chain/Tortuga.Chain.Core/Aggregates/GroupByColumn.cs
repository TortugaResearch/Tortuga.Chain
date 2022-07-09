namespace Tortuga.Chain.Aggregates;

/// <summary>
/// This represents a grouping column.
/// </summary>
public class GroupByColumn : AggregateColumn
{
	/// <summary>
	/// Create a group by column.
	/// </summary>
	/// <param name="columnName">Name of the column to group by.</param>
	/// <param name="asColumnName"></param>
	/// <exception cref="ArgumentException">columnName is null or empty.</exception>
	public GroupByColumn(string columnName, string? asColumnName = "")
	{
		if (string.IsNullOrEmpty(columnName))
			throw new ArgumentException($"{nameof(columnName)} is null or empty.", nameof(columnName));

		AggregateType = AggregateType.None;
		ColumnName = columnName;
		OutputColumnName = asColumnName ?? columnName;
		GroupBy = true;
	}
}
