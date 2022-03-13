using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources
{

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

		/// <summary>Gets a set of records by an unique key.</summary>
		/// <typeparam name="TKey">The type of the t key.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="keyColumn">Name of the key column. This should be a primary or unique key, but that's not enforced.</param>
		/// <param name="keys">The keys.</param>
		/// <returns>IMultipleRowDbCommandBuilder.</returns>
		/// <remarks>This only works on tables that have a scalar primary key.</remarks>
		IMultipleRowDbCommandBuilder GetByKeyList<TKey>(string tableName, string keyColumn, IEnumerable<TKey> keys);
	}
}
