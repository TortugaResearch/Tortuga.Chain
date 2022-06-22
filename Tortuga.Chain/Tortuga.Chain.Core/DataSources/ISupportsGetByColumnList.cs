using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources;

/// <summary>
/// Used to mark data sources that support the GetByColumnList command.
/// </summary>
public interface ISupportsGetByColumnList
{
	/// <summary>
	/// Gets one or more records by an arbitrary column.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="keys">The search keys.</param>
	IMultipleRowDbCommandBuilder GetByColumnList<TKey>(string tableName, string columnName, IEnumerable<TKey> keys);

	/// <summary>
	/// Gets one or more records by an arbitrary column.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="keys">The search keys.</param>
	IMultipleRowDbCommandBuilder<TObject> GetByColumnList<TObject, TKey>(string columnName, IEnumerable<TKey> keys)
		where TObject : class;

	/// <summary>
	/// Gets one or more records by an arbitrary column.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="keys">The search keys.</param>
	IMultipleRowDbCommandBuilder<TObject> GetByColumnList<TObject>(string columnName, IEnumerable<short> keys)
		where TObject : class;

	/// <summary>
	/// Gets one or more records by an arbitrary column.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="keys">The search keys.</param>
	IMultipleRowDbCommandBuilder<TObject> GetByColumnList<TObject>(string columnName, IEnumerable<int> keys)
		where TObject : class;

	/// <summary>
	/// Gets one or more records by an arbitrary column.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="keys">The search keys.</param>
	IMultipleRowDbCommandBuilder<TObject> GetByColumnList<TObject>(string columnName, IEnumerable<long> keys)
		where TObject : class;

	/// <summary>
	/// Gets one or more records by an arbitrary column.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="keys">The search keys.</param>
	IMultipleRowDbCommandBuilder<TObject> GetByColumnList<TObject>(string columnName, IEnumerable<Guid> keys)
		where TObject : class;
}