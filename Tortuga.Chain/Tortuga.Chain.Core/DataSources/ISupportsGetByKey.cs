using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources
{

	/// <summary>
	/// Used to mark data sources that support the GetByKey command.
	/// </summary>
	public interface ISupportsGetByKey
	{



		/// <summary>
		/// Gets a record by its primary key.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		/// <remarks>This only works on tables that have a scalar primary key.</remarks>
		ISingleRowDbCommandBuilder GetByKey<TKey>(string tableName, TKey key) where TKey : struct;

		/// <summary>
		/// Gets a record by its primary key.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		/// <remarks>This only works on tables that have a scalar primary key.</remarks>
		ISingleRowDbCommandBuilder GetByKey(string tableName, string key);

	}
}
