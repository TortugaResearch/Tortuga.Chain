using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
class SupportsDeleteByKeyListTrait<TCommand, TParameter, TObjectName, TDbType> : ISupportsDeleteByKeyList, ISupportsDeleteByKey
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
{
	[Container(RegisterInterface = true)]
	internal IUpdateDeleteByKeyHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

	ISingleRowDbCommandBuilder ISupportsDeleteByKey.DeleteByKey<TKey>(string tableName, TKey key, DeleteOptions options) => DeleteByKey(DataSource.DatabaseMetadata.ParseObjectName(tableName), key, options);

	ISingleRowDbCommandBuilder ISupportsDeleteByKey.DeleteByKey(string tableName, string key, DeleteOptions options) => DeleteByKey(DataSource.DatabaseMetadata.ParseObjectName(tableName), key, options);

	ISingleRowDbCommandBuilder<TObject> ISupportsDeleteByKey.DeleteByKey<TObject>(short key, DeleteOptions options)
		where TObject : class
		=> DeleteByKey<TObject>(key, options);

	ISingleRowDbCommandBuilder<TObject> ISupportsDeleteByKey.DeleteByKey<TObject>(int key, DeleteOptions options)
		where TObject : class
		=> DeleteByKey<TObject>(key, options);

	ISingleRowDbCommandBuilder<TObject> ISupportsDeleteByKey.DeleteByKey<TObject>(long key, DeleteOptions options)
		where TObject : class
		=> DeleteByKey<TObject>(key, options);

	ISingleRowDbCommandBuilder<TObject> ISupportsDeleteByKey.DeleteByKey<TObject>(Guid key, DeleteOptions options)
		where TObject : class
		=> DeleteByKey<TObject>(key, options);

	ISingleRowDbCommandBuilder<TObject> ISupportsDeleteByKey.DeleteByKey<TObject>(string key, DeleteOptions options)
		where TObject : class
		=> DeleteByKey<TObject>(key, options);

	/// <summary>
	/// Delete a record by its primary key.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="key">The key.</param>
	/// <param name="options">The options.</param>
	/// <returns>MultipleRowDbCommandBuilder&lt;TCommand, TParameter&gt;.</returns>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter> DeleteByKey<TKey>(TObjectName tableName, TKey key, DeleteOptions options = DeleteOptions.None)
		where TKey : struct
		=> DataSource.OnDeleteByKeyList<TKey>(tableName, new List<TKey> { key }, options);

	/// <summary>
	/// Delete a record by its primary key.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="key">The key.</param>
	/// <param name="options">The options.</param>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter> DeleteByKey(TObjectName tableName, string key, DeleteOptions options = DeleteOptions.None)
		=> DataSource.OnDeleteByKeyList<string>(tableName, new List<string> { key }, options);

	/// <summary>
	/// Delete a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">Used to determine the table name</typeparam>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="key">The key.</param>
	/// <param name="options">The options.</param>
	/// <returns>MultipleRowDbCommandBuilder&lt;TCommand, TParameter&gt;.</returns>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter, TObject> DeleteByKey<TObject, TKey>(TKey key, DeleteOptions options = DeleteOptions.None)
		where TObject : class
		where TKey : struct
		=> new SingleRowDbCommandBuilder<TCommand, TParameter, TObject>(DataSource.OnDeleteByKeyList<TKey>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, new List<TKey> { key }, options));

	/// <summary>
	/// Delete a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">Used to determine the table name</typeparam>
	/// <param name="key">The key.</param>
	/// <param name="options">The options.</param>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter, TObject> DeleteByKey<TObject>(Guid key, DeleteOptions options = DeleteOptions.None)
		where TObject : class
		=> DeleteByKey<TObject, Guid>(key, options);

	/// <summary>
	/// Delete a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">Used to determine the table name</typeparam>
	/// <param name="key">The key.</param>
	/// <param name="options">The options.</param>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter, TObject> DeleteByKey<TObject>(long key, DeleteOptions options = DeleteOptions.None)
		where TObject : class
		=> DeleteByKey<TObject, long>(key, options);

	/// <summary>
	/// Delete a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">Used to determine the table name</typeparam>
	/// <param name="key">The key.</param>
	/// <param name="options">The options.</param>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter, TObject> DeleteByKey<TObject>(short key, DeleteOptions options = DeleteOptions.None)
		where TObject : class
		=> DeleteByKey<TObject, short>(key, options);

	/// <summary>
	/// Delete a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">Used to determine the table name</typeparam>
	/// <param name="key">The key.</param>
	/// <param name="options">The options.</param>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter, TObject> DeleteByKey<TObject>(int key, DeleteOptions options = DeleteOptions.None)
	  where TObject : class
		=> DeleteByKey<TObject, int>(key, options);

	/// <summary>
	/// Delete a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">Used to determine the table name</typeparam>
	/// <param name="key">The key.</param>
	/// <param name="options">The options.</param>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter, TObject> DeleteByKey<TObject>(string key, DeleteOptions options = DeleteOptions.None)
		where TObject : class
		=> new SingleRowDbCommandBuilder<TCommand, TParameter, TObject>(DataSource.OnDeleteByKeyList<string>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, new List<string> { key }, options));

	IMultipleRowDbCommandBuilder ISupportsDeleteByKeyList.DeleteByKeyList<TKey>(string tableName, IEnumerable<TKey> keys, DeleteOptions options) => DeleteByKeyList(DataSource.DatabaseMetadata.ParseObjectName(tableName), keys, options);

	/// <summary>
	/// Delete multiple rows by key.
	/// </summary>
	/// <typeparam name="TKey">The type of the t key.</typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="keys">The keys.</param>
	/// <param name="options">Delete options.</param>
	/// <exception cref="MappingException"></exception>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter> DeleteByKeyList<TKey>(TObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
		=> DataSource.OnDeleteByKeyList(tableName, keys, options);
}
