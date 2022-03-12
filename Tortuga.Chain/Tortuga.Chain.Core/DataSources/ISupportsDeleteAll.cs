namespace Tortuga.Chain.DataSources;

/// <summary>
/// Used to mark datasources that support the DeleteAll command.
/// </summary>
public interface ISupportsDeleteAll
{
	/// <summary>Deletes all records in the specified table.</summary>
	/// <param name="tableName">Name of the table to clear.</param>
	/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
	public ILink<int?> DeleteAll(string tableName);

	/// <summary>Deletes all records in the specified table.</summary>
	/// <typeparam name="TObject">This class used to determine which table to clear.</typeparam>
	/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
	public ILink<int?> DeleteAll<TObject>() where TObject : class;
}
