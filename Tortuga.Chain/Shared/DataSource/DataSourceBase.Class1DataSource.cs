using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;


#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain.SqlServer
{
	partial class SqlServerDataSourceBase

#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
	partial class OleDbSqlServerDataSourceBase

#elif SQLITE

namespace Tortuga.Chain.SQLite
{
	partial class SQLiteDataSourceBase

#elif MYSQL

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
	partial class PostgreSqlDataSourceBase

#elif ACCESS

namespace Tortuga.Chain.Access
{
	partial class AccessDataSourceBase

#endif
	{







		/************************ ISupportsGetByKey ************************/


		/// <summary>
		/// Gets a record by its primary key.
		/// </summary>
		/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		/// <remarks>This only works on tables that have a scalar primary key.</remarks>
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> GetByKey<TObject>(Guid key)
			where TObject : class
		{
			return GetByKey<TObject, Guid>(DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, key);
		}

		/// <summary>
		/// Gets a record by its primary key.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		/// <remarks>This only works on tables that have a scalar primary key.</remarks>
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> GetByKey<TObject>(long key)
			where TObject : class
		{
			return GetByKey<TObject, long>(DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, key);
		}

		/// <summary>
		/// Gets a record by its primary key.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		/// <remarks>This only works on tables that have a scalar primary key.</remarks>
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> GetByKey<TObject>(int key)
			where TObject : class
		{
			return GetByKey<TObject, int>(DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, key);
		}

		/// <summary>
		/// Gets a record by its primary key.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> GetByKey<TObject>(string key)
			where TObject : class
		{
			return GetByKey<TObject, string>(DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, key);
		}

		/// <summary>
		/// Gets a record by its primary key.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="key">The key.</param>
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> GetByKey(AbstractObjectName tableName, string key)
		{
			return GetByKey<string>(tableName, key);
		}







		/************************ ISupportsGetByKey ************************/

		/// <summary>
		/// Gets a record by its primary key.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="key">The key.</param>
		/// <remarks>This only works on tables that have a scalar primary key.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetByKeyList")]
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> GetByKey<TObject, TKey>(AbstractObjectName tableName, TKey key)
		where TObject : class
		{
			var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;

			if (primaryKeys.Count == 0) //we're looking at a view. Try looking at the underlying table.
			{
				var alternateTableName = DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name;
				primaryKeys = DatabaseMetadata.GetTableOrView(alternateTableName).PrimaryKeyColumns;
			}

			if (primaryKeys.Count != 1)
				throw new MappingException($"{nameof(GetByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key. Use DataSource.From instead.");

			return new SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject>(OnGetByKey<TObject, TKey>(tableName, primaryKeys.Single(), key));
		}

		/// <summary>
		/// Gets a record by its primary key.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="key">The key.</param>
		/// <remarks>This only works on tables that have a scalar primary key.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetByKeyList")]
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> GetByKey<TKey>(AbstractObjectName tableName, TKey key)
		{
			var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;

			if (primaryKeys.Count != 1)
				throw new MappingException($"{nameof(GetByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key. Use DataSource.From instead.");

			return OnGetByKey<object, TKey>(tableName, primaryKeys.Single(), key);
		}

		/************************ ISupportsGetByKeyList ************************/

		/// <summary>
		/// Gets a set of records by their primary key.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="keys">The keys.</param>
		/// <returns></returns>
		/// <remarks>This only works on tables that have a scalar primary key.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetByKeyList")]
		public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> GetByKeyList<TKey>(AbstractObjectName tableName, IEnumerable<TKey> keys)
		{
			var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;
			if (primaryKeys.Count != 1)
				throw new MappingException($"{nameof(GetByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key. Use DataSource.From instead.");

			return OnGetByKeyList<object, TKey>(tableName, primaryKeys.Single(), keys);
		}

		/// <summary>
		/// Gets a set of records by an unique key.
		/// </summary>
		/// <typeparam name="TKey">The type of the keys.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="keyColumn">Name of the key column. This should be a primary or unique key, but that's not enforced.</param>
		/// <param name="keys">The keys.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
		/// <exception cref="MappingException">Cannot find a column named {keyColumn} on table {tableName}.</exception>
		/// <remarks>This only works on tables that have a scalar primary key.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetByKeyList")]
		public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> GetByKeyList<TKey>(AbstractObjectName tableName, string keyColumn, IEnumerable<TKey> keys)
		{
			var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.SqlName.Equals(keyColumn, StringComparison.OrdinalIgnoreCase)).ToList();
			if (primaryKeys.Count == 0)
				throw new MappingException($"Cannot find a column named {keyColumn} on table {tableName}.");

			return OnGetByKeyList<object, TKey>(tableName, primaryKeys.Single(), keys);
		}

		/// <summary>
		/// Gets a set of records by a key list.
		/// </summary>
		/// <typeparam name="TObject">The type of the t object.</typeparam>
		/// <typeparam name="TKey">The type of the t key.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="keyColumn">Name of the key column. This should be a primary or unique key, but that's not enforced.</param>
		/// <param name="keys">The keys.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter, TObject&gt;.</returns>
		/// <exception cref="MappingException">Cannot find a column named {keyColumn} on table {tableName}.</exception>
		/// <remarks>This only works on tables that have a scalar primary key.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetByKeyList")]
		public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> GetByKeyList<TObject, TKey>(AbstractObjectName tableName, string keyColumn, IEnumerable<TKey> keys)
			where TObject : class
		{
			var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.SqlName.Equals(keyColumn, StringComparison.OrdinalIgnoreCase)).ToList();

			if (primaryKeys.Count == 0)
				throw new MappingException($"Cannot find a column named {keyColumn} on table {tableName}.");

			return OnGetByKeyList<TObject, TKey>(tableName, primaryKeys.Single(), keys);
		}

		/// <summary>
		/// Gets a set of records by a key list.
		/// </summary>
		/// <typeparam name="TObject">The type of the returned object.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="keys">The keys.</param>
		public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> GetByKeyList<TObject, TKey>(IEnumerable<TKey> keys)
			where TObject : class
		{
			var tableName = DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name;

			var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;
			if (primaryKeys.Count == 0) //we're looking at a view. Try looking at the underlying table.
			{
				var alternateTableName = DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name;
				primaryKeys = DatabaseMetadata.GetTableOrView(alternateTableName).PrimaryKeyColumns;
			}

			if (primaryKeys.Count != 1)
				throw new MappingException($"{nameof(GetByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key. Use DataSource.From instead.");

			return OnGetByKeyList<TObject, TKey>(tableName, primaryKeys.Single(), keys);
		}

		/// <summary>
		/// Gets a set of records by a key list.
		/// </summary>
		/// <typeparam name="TObject">The type of the returned object.</typeparam>
		/// <param name="keys">The keys.</param>
		public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> GetByKeyList<TObject>(IEnumerable<Guid> keys)
			where TObject : class
		{
			return GetByKeyList<TObject, Guid>(keys);
		}

		/// <summary>
		/// Gets a set of records by a key list.
		/// </summary>
		/// <typeparam name="TObject">The type of the returned object.</typeparam>
		/// <param name="keys">The keys.</param>
		public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> GetByKeyList<TObject>(IEnumerable<long> keys)
			where TObject : class
		{
			return GetByKeyList<TObject, long>(keys);
		}

		/// <summary>
		/// Gets a set of records by a key list.
		/// </summary>
		/// <typeparam name="TObject">The type of the returned object.</typeparam>
		/// <param name="keys">The keys.</param>
		public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> GetByKeyList<TObject>(IEnumerable<int> keys)
			where TObject : class
		{
			return GetByKeyList<TObject, int>(keys);
		}

		/// <summary>
		/// Gets a set of records by a key list.
		/// </summary>
		/// <typeparam name="TObject">The type of the returned object.</typeparam>
		/// <param name="keys">The keys.</param>
		public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> GetByKeyList<TObject>(IEnumerable<string> keys)
			where TObject : class
		{
			return GetByKeyList<TObject, string>(keys);
		}

		/************************ ISupportsFrom ************************/

		TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> OnFromTableOrView(AbstractObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
			=> OnFromTableOrView<object>(tableOrViewName, filterValue, filterOptions);

		TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> OnFromTableOrView(AbstractObjectName tableOrViewName, string? whereClause, object? argumentValue)
			=> OnFromTableOrView<object>(tableOrViewName, whereClause, argumentValue);



	}
}
