namespace Tortuga.Chain.DataSources;

/// <summary>
/// Used to mark datasources that support the Truncate command.
/// </summary>
public interface ISupportsTruncate
{
	/// <summary>Truncates the specified table.</summary>
	/// <param name="tableName">Name of the table to Truncate.</param>
	/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
	public ILink<int?> Truncate(string tableName);

	/// <summary>Truncates the specified table.</summary>
	/// <typeparam name="TObject">This class used to determine which table to Truncate.</typeparam>
	/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
	public ILink<int?> Truncate<TObject>() where TObject : class;
}
