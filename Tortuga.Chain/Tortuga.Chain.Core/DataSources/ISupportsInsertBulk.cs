namespace Tortuga.Chain.DataSources;

/// <summary>
/// Used to mark data sources that supports bulk inserts.
/// </summary>
public interface ISupportsInsertBulk
{
	/// <summary>
	/// Inserts the batch of records using bulk insert.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="dataTable">The data table.</param>
	/// <returns>MySqlInsertBulk.</returns>
	public ILink<int> InsertBulk(string tableName, DataTable dataTable);

	/// <summary>
	/// Inserts the batch of records using bulk insert.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="dataReader">The data reader.</param>
	/// <returns>MySqlInsertBulk.</returns>
	public ILink<int> InsertBulk(string tableName, IDataReader dataReader);

	/// <summary>
	/// Inserts the batch of records using bulk insert.
	/// </summary>
	/// <typeparam name="TObject"></typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="objects">The objects.</param>
	/// <returns>MySqlInsertBulk.</returns>
	public ILink<int> InsertBulk<TObject>(string tableName, IEnumerable<TObject> objects) where TObject : class;

	/// <summary>
	/// Inserts the batch of records using bulk insert.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="dataTable">The data table.</param>
	/// <returns>
	/// MySqlInsertBulk.
	/// </returns>
	public ILink<int> InsertBulk<TObject>(DataTable dataTable) where TObject : class;

	/// <summary>
	/// Inserts the batch of records using bulk insert.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="dataReader">The data reader.</param>
	/// <returns>
	/// MySqlInsertBulk.
	/// </returns>
	public ILink<int> InsertBulk<TObject>(IDataReader dataReader) where TObject : class;

	/// <summary>
	/// Inserts the batch of records using bulk insert.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="objects">The objects.</param>
	/// <returns>
	/// MySqlInsertBulk.
	/// </returns>
	public ILink<int> InsertBulk<TObject>(IEnumerable<TObject> objects) where TObject : class;
}
