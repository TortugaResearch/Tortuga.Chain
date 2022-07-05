using System.ComponentModel;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources;

/// <summary>
/// Used to mark data sources that support the GetByKeyList command.
/// </summary>
public interface ISupportsGetByKeyList
{
	/// <summary>
	/// Gets a set of records by their primary key.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="keys">The keys.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	IMultipleRowDbCommandBuilder GetByKeyList<TKey>(string tableName, IEnumerable<TKey> keys);

	/// <summary>
	/// Gets a set of records by their primary key.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="keys">The keys.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	IMultipleRowDbCommandBuilder<TObject> GetByKeyList<TObject, TKey>(IEnumerable<TKey> keys)
		where TObject : class;

	/// <summary>
	/// Gets a set of records by their primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="keys">The keys.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	IMultipleRowDbCommandBuilder<TObject> GetByKeyList<TObject>(IEnumerable<short> keys)
		where TObject : class;

	/// <summary>
	/// Gets a set of records by their primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="keys">The keys.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	IMultipleRowDbCommandBuilder<TObject> GetByKeyList<TObject>(IEnumerable<int> keys)
		where TObject : class;

	/// <summary>
	/// Gets a set of records by their primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="keys">The keys.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	IMultipleRowDbCommandBuilder<TObject> GetByKeyList<TObject>(IEnumerable<long> keys)
		where TObject : class;

	/// <summary>
	/// Gets a set of records by their primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="keys">The keys.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	IMultipleRowDbCommandBuilder<TObject> GetByKeyList<TObject>(IEnumerable<Guid> keys)
		where TObject : class;

	/// <summary>Gets a set of records by an unique key.</summary>
	/// <typeparam name="TKey">The type of the t key.</typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="keyColumn">Name of the key column. This should be a primary or unique key, but that's not enforced.</param>
	/// <param name="keys">The keys.</param>
	/// <returns>IMultipleRowDbCommandBuilder.</returns>
	[Obsolete("This will be replaced by GetByColumn")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	IMultipleRowDbCommandBuilder GetByKeyList<TKey>(string tableName, string keyColumn, IEnumerable<TKey> keys);
}