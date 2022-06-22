using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
class SupportsGetByColumnListTrait<TCommand, TParameter, TObjectName, TDbType> : ISupportsGetByColumnList, ISupportsGetByColumn
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
{
	[Container(RegisterInterface = true)]
	internal IGetByKeyHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

	IMultipleRowDbCommandBuilder ISupportsGetByColumn.GetByColumn<TKey>(string tableName, string columnName, TKey key)
	{
		return GetByColumn(DataSource.DatabaseMetadata.ParseObjectName(tableName), columnName, key);
	}

	IMultipleRowDbCommandBuilder ISupportsGetByColumn.GetByColumn(string tableName, string columnName, string key)
	{
		return GetByColumn(DataSource.DatabaseMetadata.ParseObjectName(tableName), columnName, key);
	}

	IMultipleRowDbCommandBuilder<TObject> ISupportsGetByColumn.GetByColumn<TObject, TKey>(string columnName, TKey key)
		where TObject : class
	{
		return GetByColumn<TObject, TKey>(columnName, key);
	}

	IMultipleRowDbCommandBuilder<TObject> ISupportsGetByColumn.GetByColumn<TObject>(string columnName, string key)
		where TObject : class
	{
		return GetByColumn<TObject>(columnName, key);
	}

	IMultipleRowDbCommandBuilder<TObject> ISupportsGetByColumn.GetByColumn<TObject>(string columnName, int key)
		where TObject : class
	{
		return GetByColumn<TObject>(columnName, key);
	}

	IMultipleRowDbCommandBuilder<TObject> ISupportsGetByColumn.GetByColumn<TObject>(string columnName, long key)
		where TObject : class
	{
		return GetByColumn<TObject>(columnName, key);
	}

	IMultipleRowDbCommandBuilder<TObject> ISupportsGetByColumn.GetByColumn<TObject>(string columnName, Guid key)
		where TObject : class
	{
		return GetByColumn<TObject>(columnName, key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByColumn<TObject>(string columnName, Guid key)
		where TObject : class
	{
		return GetByColumnCore<TObject, Guid>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, columnName, key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByColumn<TObject>(string columnName, long key)
		where TObject : class
	{
		return GetByColumnCore<TObject, long>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, columnName, key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByColumn<TObject>(string columnName, int key)
		where TObject : class
	{
		return GetByColumnCore<TObject, int>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, columnName, key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByColumn<TObject>(string columnName, string key)
		where TObject : class
	{
		return GetByColumnCore<TObject, string>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, columnName, key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByColumn<TObject, TKey>(string columnName, TKey key)
		where TObject : class
	{
		return GetByColumnCore<TObject, TKey>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, columnName, key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter> GetByColumn(TObjectName tableName, string columnName, string key)
	{
		return GetByColumn<string>(tableName, columnName, key);
	}

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="key">The search key.</param>
	[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetByColumn")]
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter> GetByColumn<TKey>(TObjectName tableName, string columnName, TKey key)
	{
		var primaryKeys = DataSource.DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.SqlName.Equals(columnName, StringComparison.OrdinalIgnoreCase)).ToList();

		if (primaryKeys.Count == 0)
			throw new MappingException($"Cannot find a column named {columnName} on table {tableName}.");

		return DataSource.OnGetByKey<object, TKey>(tableName, primaryKeys.Single(), key);
	}

	IMultipleRowDbCommandBuilder ISupportsGetByColumnList.GetByColumnList<TKey>(string tableName, string columnName, IEnumerable<TKey> keys)
	{
		return GetByColumnList(DataSource.DatabaseMetadata.ParseObjectName(tableName), columnName, keys);
	}

	IMultipleRowDbCommandBuilder<TObject> ISupportsGetByColumnList.GetByColumnList<TObject, TKey>(string columnName, IEnumerable<TKey> keys)
	{
		return GetByColumnList<TObject, TKey>(columnName, keys);
	}

	IMultipleRowDbCommandBuilder<TObject> ISupportsGetByColumnList.GetByColumnList<TObject>(string columnName, IEnumerable<int> keys)
	{
		return GetByColumnList<TObject>(columnName, keys);
	}

	IMultipleRowDbCommandBuilder<TObject> ISupportsGetByColumnList.GetByColumnList<TObject>(string columnName, IEnumerable<long> keys)
	{
		return GetByColumnList<TObject>(columnName, keys);
	}

	IMultipleRowDbCommandBuilder<TObject> ISupportsGetByColumnList.GetByColumnList<TObject>(string columnName, IEnumerable<Guid> keys)
	{
		return GetByColumnList<TObject>(columnName, keys);
	}

	/// <summary>
	/// Gets a set of records by their primary key.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="keys">The search keys.</param>
	[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetByColumnList")]
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter> GetByColumnList<TKey>(TObjectName tableName, string columnName, IEnumerable<TKey> keys)
	{
		return GetByColumnListCore<object, TKey>(tableName, columnName, keys);
	}

	/// <summary>
	/// Gets a set of records by a key list.
	/// </summary>
	/// <typeparam name="TObject">The type of the returned object.</typeparam>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="keys">The search keys.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByColumnList<TObject, TKey>(string columnName, IEnumerable<TKey> keys)
		where TObject : class
	{
		return GetByColumnListCore<TObject, TKey>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, columnName, keys);
	}

	/// <summary>
	/// Gets a set of records by a key list.
	/// </summary>
	/// <typeparam name="TObject">The type of the returned object.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="keys">The search keys.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByColumnList<TObject>(string columnName, IEnumerable<Guid> keys)
		where TObject : class
	{
		return GetByColumnList<TObject, Guid>(columnName, keys);
	}

	/// <summary>
	/// Gets a set of records by a key list.
	/// </summary>
	/// <typeparam name="TObject">The type of the returned object.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="keys">The search keys.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByColumnList<TObject>(string columnName, IEnumerable<long> keys)
		where TObject : class
	{
		return GetByColumnList<TObject, long>(columnName, keys);
	}

	/// <summary>
	/// Gets a set of records by a key list.
	/// </summary>
	/// <typeparam name="TObject">The type of the returned object.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="keys">The search keys.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByColumnList<TObject>(string columnName, IEnumerable<int> keys)
		where TObject : class
	{
		return GetByColumnList<TObject, int>(columnName, keys);
	}

	/// <summary>
	/// Gets a set of records by a key list.
	/// </summary>
	/// <typeparam name="TObject">The type of the returned object.</typeparam>
	/// <param name="columnName">The name of the column to search.</param>
	/// <param name="keys">The search keys.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByColumnList<TObject>(string columnName, IEnumerable<string> keys)
		where TObject : class
	{
		return GetByColumnList<TObject, string>(columnName, keys);
	}

	#region Core Functions

	MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByColumnCore<TObject, TKey>(TObjectName tableName, string columnName, TKey key)
where TObject : class
	{
		var primaryKeys = DataSource.DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.SqlName.Equals(columnName, StringComparison.OrdinalIgnoreCase)).ToList();

		if (primaryKeys.Count == 0)
			throw new MappingException($"Cannot find a column named {columnName} on table {tableName}.");

		return new MultipleRowDbCommandBuilder<TCommand, TParameter, TObject>(DataSource.OnGetByKey<TObject, TKey>(tableName, primaryKeys.Single(), key));
	}

	MultipleRowDbCommandBuilder<TCommand, TParameter, TObject> GetByColumnListCore<TObject, TKey>(TObjectName tableName, string columnName, IEnumerable<TKey> keys)
		where TObject : class
	{
		var primaryKeys = DataSource.DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.SqlName.Equals(columnName, StringComparison.OrdinalIgnoreCase)).ToList();

		if (primaryKeys.Count == 0)
			throw new MappingException($"Cannot find a column named {columnName} on table {tableName}.");

		return new MultipleRowDbCommandBuilder<TCommand, TParameter, TObject>(DataSource.OnGetByKeyList<TObject, TKey>(tableName, primaryKeys.Single(), keys));
	}

	#endregion Core Functions
}