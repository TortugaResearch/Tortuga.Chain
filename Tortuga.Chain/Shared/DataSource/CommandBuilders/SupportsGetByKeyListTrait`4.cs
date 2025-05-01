using System.ComponentModel;
using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
[SuppressMessage("Performance", "CA1812")]
sealed class SupportsGetByKeyListTrait<TCommand, TParameter, TObjectName, TDbType> : ISupportsGetByKeyList, ISupportsGetByKey
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
{
	[Container(RegisterInterface = true)]
	internal IGetByKeyHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

	ISingleRowDbCommandBuilder ISupportsGetByKey.GetByKey<TKey>(string tableName, TKey key)
	{
		return GetByKey(DataSource.DatabaseMetadata.ParseObjectName(tableName), key);
	}

	ISingleRowDbCommandBuilder ISupportsGetByKey.GetByKey(string tableName, string key)
	{
		return GetByKey(DataSource.DatabaseMetadata.ParseObjectName(tableName), key);
	}

	ISingleRowDbCommandBuilder<TObject> ISupportsGetByKey.GetByKey<TObject, TKey>(TKey key)
		where TObject : class
	{
		return GetByKey<TObject, TKey>(key);
	}

	ISingleRowDbCommandBuilder<TObject> ISupportsGetByKey.GetByKey<TObject>(string key)
		where TObject : class
	{
		return GetByKey<TObject>(key);
	}

	ISingleRowDbCommandBuilder<TObject> ISupportsGetByKey.GetByKey<TObject>(short key)
	where TObject : class
	{
		return GetByKey<TObject>(key);
	}

	ISingleRowDbCommandBuilder<TObject> ISupportsGetByKey.GetByKey<TObject>(int key)
		where TObject : class
	{
		return GetByKey<TObject>(key);
	}

	ISingleRowDbCommandBuilder<TObject> ISupportsGetByKey.GetByKey<TObject>(long key)
		where TObject : class
	{
		return GetByKey<TObject>(key);
	}

	ISingleRowDbCommandBuilder<TObject> ISupportsGetByKey.GetByKey<TObject>(Guid key)
		where TObject : class
	{
		return GetByKey<TObject>(key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="key">The key.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKey<TObject>(Guid key)
		where TObject : class
	{
		return GetByKeyCore<TObject, Guid>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="key">The key.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKey<TObject>(long key)
		where TObject : class
	{
		return GetByKeyCore<TObject, long>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="key">The key.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKey<TObject>(short key)
		where TObject : class
	{
		return GetByKeyCore<TObject, short>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="key">The key.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKey<TObject>(int key)
		where TObject : class
	{
		return GetByKeyCore<TObject, int>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="key">The key.</param>
	/// <returns></returns>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKey<TObject>(string key)
		where TObject : class
	{
		return GetByKeyCore<TObject, string>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="key">The key.</param>
	/// <returns></returns>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKey<TObject, TKey>(TKey key)
		where TObject : class
	{
		return GetByKeyCore<TObject, TKey>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="key">The key.</param>
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter> GetByKey(TObjectName tableName, string key)
	{
		return GetByKey<string>(tableName, key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="key">The key.</param>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetByKeyList")]
	[Expose]
	public SingleRowDbCommandBuilder<TCommand, TParameter> GetByKey<TKey>(TObjectName tableName, TKey key)
	{
		var primaryKeys = DataSource.DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;

		if (primaryKeys.Count != 1)
			throw new MappingException($"{nameof(GetByKey)} operation isn't allowed on {tableName} because it doesn't have a single primary key. Use DataSource.From instead.");

		return DataSource.OnGetByKey<object, TKey>(tableName, primaryKeys.Single(), key);
	}

	IMultipleRowDbCommandBuilder ISupportsGetByKeyList.GetByKeyList<TKey>(string tableName, IEnumerable<TKey> keys)
	{
		return GetByKeyList(DataSource.DatabaseMetadata.ParseObjectName(tableName), keys);
	}

	IMultipleRowDbCommandBuilder<TObject> ISupportsGetByKeyList.GetByKeyList<TObject, TKey>(IEnumerable<TKey> keys)
	{
		return GetByKeyList<TObject, TKey>(keys);
	}

	IMultipleRowDbCommandBuilder<TObject> ISupportsGetByKeyList.GetByKeyList<TObject>(IEnumerable<short> keys)
	{
		return GetByKeyList<TObject>(keys);
	}

	IMultipleRowDbCommandBuilder<TObject> ISupportsGetByKeyList.GetByKeyList<TObject>(IEnumerable<int> keys)
	{
		return GetByKeyList<TObject>(keys);
	}

	IMultipleRowDbCommandBuilder<TObject> ISupportsGetByKeyList.GetByKeyList<TObject>(IEnumerable<long> keys)
	{
		return GetByKeyList<TObject>(keys);
	}

	IMultipleRowDbCommandBuilder<TObject> ISupportsGetByKeyList.GetByKeyList<TObject>(IEnumerable<Guid> keys)
	{
		return GetByKeyList<TObject>(keys);
	}

	/// <summary>
	/// Gets a set of records by their primary key.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="keys">The keys.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetByKeyList")]
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter> GetByKeyList<TKey>(TObjectName tableName, IEnumerable<TKey> keys)
	{
		return GetByKeyListCore<object, TKey>(tableName, keys);
	}

	/// <summary>
	/// Gets a set of records by a key list.
	/// </summary>
	/// <typeparam name="TObject">The type of the returned object.</typeparam>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="keys">The keys.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKeyList<TObject, TKey>(IEnumerable<TKey> keys)
		where TObject : class
	{
		return GetByKeyListCore<TObject, TKey>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, keys);
	}

	/// <summary>
	/// Gets a set of records by a key list.
	/// </summary>
	/// <typeparam name="TObject">The type of the returned object.</typeparam>
	/// <param name="keys">The keys.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKeyList<TObject>(IEnumerable<Guid> keys)
		where TObject : class
	{
		return GetByKeyList<TObject, Guid>(keys);
	}

	/// <summary>
	/// Gets a set of records by a key list.
	/// </summary>
	/// <typeparam name="TObject">The type of the returned object.</typeparam>
	/// <param name="keys">The keys.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKeyList<TObject>(IEnumerable<long> keys)
		where TObject : class
	{
		return GetByKeyList<TObject, long>(keys);
	}

	/// <summary>
	/// Gets a set of records by a key list.
	/// </summary>
	/// <typeparam name="TObject">The type of the returned object.</typeparam>
	/// <param name="keys">The keys.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKeyList<TObject>(IEnumerable<short> keys)
		where TObject : class
	{
		return GetByKeyList<TObject, short>(keys);
	}

	/// <summary>
	/// Gets a set of records by a key list.
	/// </summary>
	/// <typeparam name="TObject">The type of the returned object.</typeparam>
	/// <param name="keys">The keys.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKeyList<TObject>(IEnumerable<int> keys)
		where TObject : class
	{
		return GetByKeyList<TObject, int>(keys);
	}

	/// <summary>
	/// Gets a set of records by a key list.
	/// </summary>
	/// <typeparam name="TObject">The type of the returned object.</typeparam>
	/// <param name="keys">The keys.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKeyList<TObject>(IEnumerable<string> keys)
		where TObject : class
	{
		return GetByKeyList<TObject, string>(keys);
	}

	#region Core Functions

	IMultipleRowDbCommandBuilder ISupportsGetByKeyList.GetByKeyList<TKey>(string tableName, string keyColumn, IEnumerable<TKey> keys)
	{
		return GetByKeyListCore<object, TKey>(DataSource.DatabaseMetadata.ParseObjectName(tableName), keyColumn, keys);
	}

	/// <summary>Gets a set of records by an unique key.</summary>
	/// <typeparam name="TKey">The type of the t key.</typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="keyColumn">Name of the key column. This should be a primary or unique key, but that's not enforced.</param>
	/// <param name="keys">The keys.</param>
	/// <returns>IMultipleRowDbCommandBuilder.</returns>
	[Obsolete("This will be replaced by GetByColumn")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter> GetByKeyList<TKey>(string tableName, string keyColumn, IEnumerable<TKey> keys)
	{
		return GetByKeyListCore<object, TKey>(DataSource.DatabaseMetadata.ParseObjectName(tableName), keyColumn, keys);
	}

	SingleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKeyCore<TObject, TKey>(TObjectName tableName, TKey key)
			where TObject : class
	{
		var primaryKeys = DataSource.DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;

		if (primaryKeys.Count == 0) //we're looking at a view. Try looking at the underlying table.
		{
			var alternateTableName = DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name;
			primaryKeys = DataSource.DatabaseMetadata.GetTableOrView(alternateTableName).PrimaryKeyColumns;
		}

		if (primaryKeys.Count != 1)
			throw new MappingException($"{nameof(GetByKey)} operation isn't allowed on {tableName} because it doesn't have a single primary key. Use DataSource.From instead.");

		return new SingleRowDbCommandBuilder<TCommand, TParameter, TObject>(DataSource.OnGetByKey<TObject, TKey>(tableName, primaryKeys.Single(), key));
	}

	/*
	//TODO-441 GetByColumn
	MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKeyCore<TObject, TKey>(TObjectName tableName, string keyColumn, TKey key)
where TObject : class
	{
		var primaryKeys = DataSource.DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.SqlName.Equals(keyColumn, StringComparison.OrdinalIgnoreCase)).ToList();

		if (primaryKeys.Count == 0)
			throw new MappingException($"Cannot find a column named {keyColumn} on table {tableName}.");

		return new SingleRowDbCommandBuilder<TCommand, TParameter, TObject>(DataSource.OnGetByKey<TObject, TKey>(tableName, primaryKeys.Single(), key));
	}
	*/

	MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKeyListCore<TObject, TKey>(TObjectName tableName, IEnumerable<TKey> keys)
where TObject : class
	{
		var primaryKeys = DataSource.DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;

		if (primaryKeys.Count == 0) //we're looking at a view. Try looking at the underlying table.
		{
			var alternateTableName = DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name;
			primaryKeys = DataSource.DatabaseMetadata.GetTableOrView(alternateTableName).PrimaryKeyColumns;
		}

		if (primaryKeys.Count != 1)
			throw new MappingException($"{nameof(GetByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key. Use DataSource.From instead.");

		return new MultipleRowDbCommandBuilder<TCommand, TParameter, TObject>(DataSource.OnGetByKeyList<TObject, TKey>(tableName, primaryKeys.Single(), keys));
	}

	//TODO-441 GetByColumn
	MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByKeyListCore<TObject, TKey>(TObjectName tableName, string keyColumn, IEnumerable<TKey> keys)
		where TObject : class
	{
		var primaryKeys = DataSource.DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.SqlName.Equals(keyColumn, StringComparison.OrdinalIgnoreCase)).ToList();

		if (primaryKeys.Count == 0)
			throw new MappingException($"Cannot find a column named {keyColumn} on table {tableName}.");

		return new MultipleRowDbCommandBuilder<TCommand, TParameter, TObject>(DataSource.OnGetByKeyList<TObject, TKey>(tableName, primaryKeys.Single(), keys));
	}

	#endregion Core Functions
}
