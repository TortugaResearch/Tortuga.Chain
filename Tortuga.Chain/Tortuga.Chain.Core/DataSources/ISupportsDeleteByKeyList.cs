using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources
{

	/// <summary>
	/// Used to mark data sources that support the DeleteByKeyList command.
	/// </summary>
	public interface ISupportsDeleteByKeyList
	{
		/// <summary>
		/// Delete by key.
		/// </summary>
		/// <typeparam name="TKey">The type of the t key.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="keys">The keys.</param>
		/// <param name="options">The options.</param>
		/// <returns>IMultipleRowDbCommandBuilder.</returns>
		IMultipleRowDbCommandBuilder DeleteByKeyList<TKey>(string tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None);
	}
}
