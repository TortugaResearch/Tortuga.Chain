namespace Tortuga.Chain.DataSources;

/// <summary>
/// Used to mark datasources that support enabling and disabling indexes on a table.
/// </summary>
public interface ISupportsDisableIndexes
{
	/// <summary>
	/// Disables all of the indexes on the indicated table..
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <remarks>For SQL Server, this will not disable the clustered index.</remarks>
	public ILink<int?> DisableIndexes(string tableName);

	/// <summary>
	/// Enables all of the indexes on the indicated table.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>

	public ILink<int?> EnableIndexes(string tableName);

	/// <summary>
	/// Disables all of the indexes on the indicated table..
	/// </summary>
	/// <remarks>For SQL Server, this will not disable the clustered index.</remarks>
	public ILink<int?> DisableIndexes<TObject>() where TObject : class;

	/// <summary>
	/// Enables all of the indexes on the indicated table.
	/// </summary>
	public ILink<int?> EnableIndexes<TObject>() where TObject : class;
}
