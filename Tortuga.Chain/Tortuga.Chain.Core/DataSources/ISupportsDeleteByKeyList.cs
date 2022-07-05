using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources;

/// <summary>
/// Used to mark data sources that support the DeleteByKeyList command.
/// </summary>
public interface ISupportsDeleteByKeyList
{
	/// <summary>
	/// Deletes a set of records by their primary key.
	/// </summary>
	/// <typeparam name="TKey">The type of the t key.</typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="keys">The keys.</param>
	/// <param name="options">The options.</param>
	/// <returns>IMultipleRowDbCommandBuilder.</returns>
	IMultipleRowDbCommandBuilder DeleteByKeyList<TKey>(string tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None);

	/// <summary>
	/// Deletes a set of records by their primary key.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be modified.</typeparam>
	/// <param name="keys">The keys.</param>
	/// <param name="options">The options.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	IMultipleRowDbCommandBuilder<TObject> DeleteByKeyList<TObject, TKey>(IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
		where TObject : class
		where TKey : struct;

	/// <summary>
	/// Deletes a set of records by their primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be modified.</typeparam>
	/// <param name="keys">The keys.</param>
	/// <param name="options">The options.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	IMultipleRowDbCommandBuilder<TObject> DeleteByKeyList<TObject>(IEnumerable<short> keys, DeleteOptions options = DeleteOptions.None)
		where TObject : class;

	/// <summary>
	/// Deletes a set of records by their primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be modified.</typeparam>
	/// <param name="keys">The keys.</param>
	/// <param name="options">The options.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	IMultipleRowDbCommandBuilder<TObject> DeleteByKeyList<TObject>(IEnumerable<int> keys, DeleteOptions options = DeleteOptions.None)
		where TObject : class;

	/// <summary>
	/// Deletes a set of records by their primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be modified.</typeparam>
	/// <param name="keys">The keys.</param>
	/// <param name="options">The options.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	IMultipleRowDbCommandBuilder<TObject> DeleteByKeyList<TObject>(IEnumerable<long> keys, DeleteOptions options = DeleteOptions.None)
		where TObject : class;

	/// <summary>
	/// Deletes a set of records by their primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be modified.</typeparam>
	/// <param name="keys">The keys.</param>
	/// <param name="options">The options.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	IMultipleRowDbCommandBuilder<TObject> DeleteByKeyList<TObject>(IEnumerable<Guid> keys, DeleteOptions options = DeleteOptions.None)
		where TObject : class;
}
