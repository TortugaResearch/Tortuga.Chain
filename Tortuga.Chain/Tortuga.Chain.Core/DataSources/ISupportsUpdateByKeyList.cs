using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources
{

	/// <summary>
	/// Used to mark data sources that support the UpdateByKey command.
	/// </summary>
	public interface ISupportsUpdateByKeyList
	{
		/// <summary>
		/// Update multiple rows by key.
		/// </summary>
		/// <typeparam name="TArgument">The type of the t argument.</typeparam>
		/// <typeparam name="TKey">The type of the t key.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="newValues">The new values to use.</param>
		/// <param name="keys">The keys.</param>
		/// <param name="options">Update options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		/// <exception cref="MappingException"></exception>
		IMultipleRowDbCommandBuilder UpdateByKeyList<TArgument, TKey>(string tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None);
	}
}
