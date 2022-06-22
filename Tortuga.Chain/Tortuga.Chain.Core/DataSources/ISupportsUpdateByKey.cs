using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources
{
	/// <summary>
	/// Used to mark data sources that support the UpdateByKey command.
	/// </summary>
	public interface ISupportsUpdateByKey
	{

		/// <summary>
		/// Update a record by its primary key.
		/// </summary>
		/// <typeparam name="TArgument">The type of the t argument.</typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="newValues">The new values to use.</param>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		ISingleRowDbCommandBuilder UpdateByKey<TArgument, TKey>(string tableName, TArgument newValues, TKey key, UpdateOptions options = UpdateOptions.None)
			where TKey : struct;

		/// <summary>
		/// Update a record by its primary key.
		/// </summary>
		/// <typeparam name="TArgument">The type of the t argument.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="newValues">The new values to use.</param>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		ISingleRowDbCommandBuilder UpdateByKey<TArgument>(string tableName, TArgument newValues, string key, UpdateOptions options = UpdateOptions.None);
	}
}
