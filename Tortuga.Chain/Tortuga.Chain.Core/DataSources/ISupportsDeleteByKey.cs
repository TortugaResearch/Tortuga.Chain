using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources
{

	/// <summary>
	/// Used to mark data sources that support the DeleteByKey command.
	/// </summary>
	public interface ISupportsDeleteByKey
	{
		/// <summary>
		/// Delete by key.
		/// </summary>
		/// <typeparam name="TKey">The type of the t key.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		/// <returns>ISingleRowDbCommandBuilder.</returns>
		ISingleRowDbCommandBuilder DeleteByKey<TKey>(string tableName, TKey key, DeleteOptions options = DeleteOptions.None) where TKey : struct;

		/// <summary>
		/// Delete by key.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		/// <returns>ISingleRowDbCommandBuilder.</returns>
		ISingleRowDbCommandBuilder DeleteByKey(string tableName, string key, DeleteOptions options = DeleteOptions.None);
	}
}
