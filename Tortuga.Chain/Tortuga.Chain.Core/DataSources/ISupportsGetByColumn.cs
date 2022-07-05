using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources;

/// <summary>
/// Used to mark data sources that support the GetByColumn command.
/// </summary>
public interface ISupportsGetByColumn
{
	/// <summary>
	/// Gets one or more records by an arbitrary column.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	IMultipleRowDbCommandBuilder GetByColumn<TKey>(string tableName, string columnName, TKey key)
		where TKey : struct;

	/// <summary>
	/// Gets one or more records by an arbitrary column.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	IMultipleRowDbCommandBuilder GetByColumn(string tableName, string columnName, string key);

	/// <summary>
	/// Gets one or more records by an arbitrary column.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	IMultipleRowDbCommandBuilder<TObject> GetByColumn<TObject, TKey>(string columnName, TKey key)
		where TObject : class
		where TKey : struct;

	/// <summary>
	/// Gets one or more records by an arbitrary column.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	IMultipleRowDbCommandBuilder<TObject> GetByColumn<TObject>(string columnName, string key)
		where TObject : class;

	/// <summary>
	/// Gets one or more records by an arbitrary column.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	IMultipleRowDbCommandBuilder<TObject> GetByColumn<TObject>(string columnName, short key)
		where TObject : class;

	/// <summary>
	/// Gets one or more records by an arbitrary column.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	IMultipleRowDbCommandBuilder<TObject> GetByColumn<TObject>(string columnName, int key)
		where TObject : class;

	/// <summary>
	/// Gets one or more records by an arbitrary column.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	IMultipleRowDbCommandBuilder<TObject> GetByColumn<TObject>(string columnName, long key)
		where TObject : class;

	/// <summary>
	/// Gets one or more records by an arbitrary column.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	IMultipleRowDbCommandBuilder<TObject> GetByColumn<TObject>(string columnName, Guid key)
		where TObject : class;
}