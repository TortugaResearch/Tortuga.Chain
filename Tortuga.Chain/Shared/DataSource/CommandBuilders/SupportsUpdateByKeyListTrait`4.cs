using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
[SuppressMessage("Performance", "CA1812")]
sealed class SupportsUpdateByKeyListTrait<TCommand, TParameter, TObjectName, TDbType> : ISupportsUpdateByKey, ISupportsUpdateByKeyList
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
{
	[Container(RegisterInterface = true)]
	internal IUpdateDeleteByKeyHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

	ISingleRowDbCommandBuilder ISupportsUpdateByKey.UpdateByKey<TArgument, TKey>(string tableName, TArgument newValues, TKey key, UpdateOptions options)
	{
		return DataSource.OnUpdateByKeyList(DataSource.DatabaseMetadata.ParseObjectName(tableName), newValues, new List<TKey> { key }, options);
	}

	ISingleRowDbCommandBuilder ISupportsUpdateByKey.UpdateByKey<TArgument>(string tableName, TArgument newValues, string key, UpdateOptions options)
	{
		return DataSource.OnUpdateByKeyList(DataSource.DatabaseMetadata.ParseObjectName(tableName), newValues, new List<string> { key }, options);
	}

	/// <summary>
	/// Update a record by its primary key.
	/// </summary>
	/// <typeparam name="TArgument">The type of the t argument.</typeparam>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="newValues">The new values to use.</param>
	/// <param name="key">The key.</param>
	/// <param name="options">The options.</param>
	/// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter> UpdateByKey<TArgument, TKey>(TObjectName tableName, TArgument newValues, TKey key, UpdateOptions options = UpdateOptions.None)
		where TKey : struct
	{
		return DataSource.OnUpdateByKeyList(tableName, newValues, new List<TKey> { key }, options);
	}

	/// <summary>
	/// Update a record by its primary key.
	/// </summary>
	/// <typeparam name="TArgument">The type of the t argument.</typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="newValues">The new values to use.</param>
	/// <param name="key">The key.</param>
	/// <param name="options">The options.</param>
	/// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter> UpdateByKey<TArgument>(TObjectName tableName, TArgument newValues, string key, UpdateOptions options = UpdateOptions.None)
	{
		return DataSource.OnUpdateByKeyList(tableName, newValues, new List<string> { key }, options);
	}

	IMultipleRowDbCommandBuilder ISupportsUpdateByKeyList.UpdateByKeyList<TArgument, TKey>(string tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options)
	{
		return DataSource.OnUpdateByKeyList(DataSource.DatabaseMetadata.ParseObjectName(tableName), newValues, keys, options);
	}

	/// <summary>
	/// Update multiple rows by key.
	/// </summary>
	/// <typeparam name="TArgument">The type of the t argument.</typeparam>
	/// <typeparam name="TKey">The type of the t key.</typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="newValues">The new values to use.</param>
	/// <param name="keys">The keys.</param>
	/// <param name="options">Update options.</param>
	/// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
	/// <exception cref="MappingException"></exception>
	[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKeyList")]
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter> UpdateByKeyList<TArgument, TKey>(TObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
	{
		return DataSource.OnUpdateByKeyList(tableName, newValues, keys, options);
	}
}
