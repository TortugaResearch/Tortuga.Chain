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
		/// <summary>
		/// Creates a command to perform a delete operation.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="argumentValue"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Delete<TArgument>(AbstractObjectName tableName, TArgument argumentValue, DeleteOptions options = DeleteOptions.None)
		where TArgument : class
		{
			var table = DatabaseMetadata.GetTableOrView(tableName);
			if (!AuditRules.UseSoftDelete(table))
				return OnDeleteObject<TArgument>(tableName, argumentValue, options);

			UpdateOptions effectiveOptions = UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected;
			if (options.HasFlag(DeleteOptions.UseKeyAttribute))
				effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

			return OnUpdateObject<TArgument>(tableName, argumentValue, effectiveOptions);
		}

		/// <summary>
		/// Delete an object model from the table indicated by the class's Table attribute.
		/// </summary>
		/// <typeparam name="TArgument"></typeparam>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The delete options.</param>
		/// <returns></returns>
		public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Delete<TArgument>(TArgument argumentValue, DeleteOptions options = DeleteOptions.None) where TArgument : class
		{
			var table = DatabaseMetadata.GetTableOrViewFromClass<TArgument>();

			if (!AuditRules.UseSoftDelete(table))
				return OnDeleteObject<TArgument>(table.Name, argumentValue, options);

			UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;
			if (options.HasFlag(DeleteOptions.UseKeyAttribute))
				effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

			return OnUpdateObject<TArgument>(table.Name, argumentValue, effectiveOptions);
		}

		/// <summary>
		/// Delete a record by its primary key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteByKey<T>(AbstractObjectName tableName, T key, DeleteOptions options = DeleteOptions.None)
			where T : struct
		{
			return DeleteByKeyList(tableName, new List<T> { key }, options);
		}

		/// <summary>
		/// Delete a record by its primary key.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteByKey(AbstractObjectName tableName, string key, DeleteOptions options = DeleteOptions.None)
		{
			return DeleteByKeyList(tableName, new List<string> { key }, options);
		}

		/// <summary>
		/// Delete a record by its primary key.
		/// </summary>
		/// <typeparam name="TObject">Used to determine the table name</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteByKey<TObject>(Guid key, DeleteOptions options = DeleteOptions.None)
			where TObject : class
		{
			return DeleteByKey<Guid>(DatabaseObjectAsTableOrView<TObject>(OperationType.All).Name, key, options);
		}

		/// <summary>
		/// Delete a record by its primary key.
		/// </summary>
		/// <typeparam name="TObject">Used to determine the table name</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteByKey<TObject>(long key, DeleteOptions options = DeleteOptions.None)
			where TObject : class
		{
			return DeleteByKey<long>(DatabaseObjectAsTableOrView<TObject>(OperationType.All).Name, key, options);
		}

		/// <summary>
		/// Delete a record by its primary key.
		/// </summary>
		/// <typeparam name="TObject">Used to determine the table name</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteByKey<TObject>(int key, DeleteOptions options = DeleteOptions.None)
		  where TObject : class
		{
			return DeleteByKey<int>(DatabaseObjectAsTableOrView<TObject>(OperationType.All).Name, key, options);
		}

		/// <summary>
		/// Delete a record by its primary key.
		/// </summary>
		/// <typeparam name="TObject">Used to determine the table name</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteByKey<TObject>(string key, DeleteOptions options = DeleteOptions.None)
			where TObject : class
		{
			return DeleteByKey(DatabaseObjectAsTableOrView<TObject>(OperationType.All).Name, key, options);
		}

		/// <summary>
		/// Delete multiple records using a where expression.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="whereClause">The where clause.</param>
		public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteWithFilter(AbstractObjectName tableName, string whereClause)
		{
			var table = DatabaseMetadata.GetTableOrView(tableName);
			if (!AuditRules.UseSoftDelete(table))
				return OnDeleteMany(tableName, whereClause, null);

			return OnUpdateMany(tableName, null, UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected).WithFilter(whereClause, null);
		}

		/// <summary>
		/// Delete multiple records using a where expression.
		/// </summary>
		/// <param name="whereClause">The where clause.</param>
		public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteWithFilter<TObject>(string whereClause)
		{
			var tableName = DatabaseObjectAsTableOrView<TObject>(OperationType.All).Name;
			return DeleteWithFilter(tableName, whereClause);
		}

		/// <summary>
		/// Update an object in the specified table.
		/// </summary>
		/// <typeparam name="TArgument"></typeparam>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The update options.</param>
		/// <returns></returns>
		public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Update<TArgument>(TArgument argumentValue, UpdateOptions options = UpdateOptions.None) where TArgument : class
		{
			return Update(DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
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
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> UpdateByKey<TArgument, TKey>(AbstractObjectName tableName, TArgument newValues, TKey key, UpdateOptions options = UpdateOptions.None)
			where TKey : struct
		{
			return UpdateByKeyList(tableName, newValues, new List<TKey> { key }, options);
		}

		/// <summary>
		/// Creates an operation to directly query a table or view
		/// </summary>
		/// <param name="tableOrViewName"></param>
		/// <returns></returns>
		public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> From(AbstractObjectName tableOrViewName)
		{
			return OnFromTableOrView(tableOrViewName, null, null);
		}

		/// <summary>
		/// Creates an operation to directly query a table or view
		/// </summary>
		/// <param name="tableOrViewName"></param>
		/// <param name="whereClause"></param>
		/// <returns></returns>
		public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> From(AbstractObjectName tableOrViewName, string whereClause)
		{
			return OnFromTableOrView(tableOrViewName, whereClause, null);
		}

		/// <summary>
		/// Creates an operation to directly query a table or view
		/// </summary>
		/// <param name="tableOrViewName"></param>
		/// <param name="whereClause"></param>
		/// <param name="argumentValue"></param>
		/// <returns></returns>
		public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> From(AbstractObjectName tableOrViewName, string whereClause, object argumentValue)
		{
			return OnFromTableOrView(tableOrViewName, whereClause, argumentValue);
		}

		/// <summary>
		/// Creates an operation to directly query a table or view
		/// </summary>
		/// <param name="tableOrViewName">Name of the table or view.</param>
		/// <param name="filterValue">The filter value.</param>
		/// <param name="filterOptions">The filter options.</param>
		/// <returns>TableDbCommandBuilder&lt;AbstractCommand, AbstractParameter, AbstractLimitOption&gt;.</returns>
		public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> From(AbstractObjectName tableOrViewName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
		{
			return OnFromTableOrView(tableOrViewName, filterValue, filterOptions);
		}

		/// <summary>
		/// This is used to directly query a table or view.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption, TObject> From<TObject>() where TObject : class
		{
			return OnFromTableOrView<TObject>(DatabaseObjectAsTableOrView<TObject>(OperationType.Select).Name, null, null);
		}

		/// <summary>
		/// This is used to directly query a table or view.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption, TObject> From<TObject>(string whereClause) where TObject : class
		{
			return OnFromTableOrView<TObject>(DatabaseObjectAsTableOrView<TObject>(OperationType.Select).Name, whereClause, null);
		}

		/// <summary>
		/// This is used to directly query a table or view.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
		/// <param name="argumentValue">Optional argument value. Every property in the argument value must have a matching parameter in the WHERE clause</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption, TObject> From<TObject>(string whereClause, object argumentValue) where TObject : class
		{
			return OnFromTableOrView<TObject>(DatabaseObjectAsTableOrView<TObject>(OperationType.Select).Name, whereClause, argumentValue);
		}

		/// <summary>
		/// This is used to directly query a table or view.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="filterValue">The filter value is used to generate a simple AND style WHERE clause.</param>
		/// <param name="filterOptions">The filter options.</param>
		/// <returns>TableDbCommandBuilder&lt;AbstractCommand, AbstractParameter, AbstractLimitOption, TObject&gt;.</returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption, TObject> From<TObject>(object filterValue, FilterOptions filterOptions = FilterOptions.None) where TObject : class
		{
			return OnFromTableOrView<TObject>(DatabaseObjectAsTableOrView<TObject>(OperationType.Select).Name, filterValue, filterOptions);
		}

		/// <summary>
		/// Delete multiple records using a filter object.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="filterValue">The filter value.</param>
		/// <param name="filterOptions">The options.</param>
		public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteWithFilter(AbstractObjectName tableName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
		{
			var table = DatabaseMetadata.GetTableOrView(tableName);
			if (!AuditRules.UseSoftDelete(table))
				return OnDeleteMany(tableName, filterValue, filterOptions);

			return OnUpdateMany(tableName, null, UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected).WithFilter(filterValue, filterOptions);
		}

		/// <summary>
		/// Delete multiple records using a filter object.
		/// </summary>
		/// <param name="filterValue">The filter value.</param>
		/// <param name="filterOptions">The options.</param>
		public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteWithFilter<TObject>(object filterValue, FilterOptions filterOptions = FilterOptions.None)
		{
			var tableName = DatabaseObjectAsTableOrView<TObject>(OperationType.All).Name;
			return DeleteWithFilter(tableName, filterValue, filterOptions);
		}

		/// <summary>
		/// Delete multiple records using a where expression.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="whereClause">The where clause.</param>
		/// <param name="argumentValue">The argument value for the where clause.</param>
		public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteWithFilter(AbstractObjectName tableName, string whereClause, object argumentValue)
		{
			var table = DatabaseMetadata.GetTableOrView(tableName);
			if (!AuditRules.UseSoftDelete(table))
				return OnDeleteMany(tableName, whereClause, argumentValue);

			return OnUpdateMany(tableName, null, UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected).WithFilter(whereClause, argumentValue);
		}

		/// <summary>
		/// Delete multiple records using a where expression.
		/// </summary>
		/// <param name="whereClause">The where clause.</param>
		/// <param name="argumentValue">The argument value for the where clause.</param>
		public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteWithFilter<TObject>(string whereClause, object argumentValue)
		{
			var tableName = DatabaseObjectAsTableOrView<TObject>(OperationType.All).Name;
			return DeleteWithFilter(tableName, whereClause, argumentValue);
		}

		/// <summary>
		/// Update multiple records using an update expression.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="updateExpression">The update expression.</param>
		/// <param name="updateArgumentValue">The argument value.</param>
		/// <param name="options">The update options.</param>
		/// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
		public IUpdateManyDbCommandBuilder<AbstractCommand, AbstractParameter> UpdateSet(AbstractObjectName tableName, string updateExpression, object updateArgumentValue, UpdateOptions options = UpdateOptions.None)
		{
			return OnUpdateMany(tableName, updateExpression, updateArgumentValue, options);
		}

		/// <summary>
		/// Update multiple records using an update value.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="newValues">The new values to use.</param>
		/// <param name="options">The options.</param>
		/// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
		public IUpdateManyDbCommandBuilder<AbstractCommand, AbstractParameter> UpdateSet(AbstractObjectName tableName, object newValues, UpdateOptions options = UpdateOptions.None)
		{
			return OnUpdateMany(tableName, newValues, options);
		}

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
			return GetByKey<TObject, Guid>(DatabaseObjectAsTableOrView<TObject>(OperationType.Select).Name, key);
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
			return GetByKey<TObject, long>(DatabaseObjectAsTableOrView<TObject>(OperationType.Select).Name, key);
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
			return GetByKey<TObject, int>(DatabaseObjectAsTableOrView<TObject>(OperationType.Select).Name, key);
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
			return GetByKey<TObject, string>(DatabaseObjectAsTableOrView<TObject>(OperationType.Select).Name, key);
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

		/// <summary>
		/// Inserts an object into the specified table.
		/// </summary>
		/// <typeparam name="TArgument"></typeparam>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The options for how the insert occurs.</param>
		/// <returns></returns>
		public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Insert<TArgument>(TArgument argumentValue, InsertOptions options = InsertOptions.None) where TArgument : class
		{
			return Insert(DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
		}

		/// <summary>
		/// Creates a operation used to perform an update operation.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="argumentValue"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Update<TArgument>(AbstractObjectName tableName, TArgument argumentValue, UpdateOptions options = UpdateOptions.None)
		where TArgument : class
		{
			return OnUpdateObject<TArgument>(tableName, argumentValue, options);
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
		public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> UpdateByKey<TArgument>(AbstractObjectName tableName, TArgument newValues, string key, UpdateOptions options = UpdateOptions.None)
		{
			return UpdateByKeyList(tableName, newValues, new List<string> { key }, options);
		}

		/// <summary>
		/// Creates an operation used to perform an insert operation.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The options.</param>
		/// <returns></returns>
		public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Insert<TArgument>(AbstractObjectName tableName, TArgument argumentValue, InsertOptions options = InsertOptions.None)
		where TArgument : class
		{
			return OnInsertObject<TArgument>(tableName, argumentValue, options);
		}

		/// <summary>
		/// Update multiple records using an update expression.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="updateExpression">The update expression.</param>
		/// <param name="options">The update options.</param>
		/// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
		public IUpdateManyDbCommandBuilder<AbstractCommand, AbstractParameter> UpdateSet(AbstractObjectName tableName, string updateExpression, UpdateOptions options = UpdateOptions.None)
		{
			return OnUpdateMany(tableName, updateExpression, null, options);
		}

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
			var tableName = DatabaseObjectAsTableOrView<TObject>(OperationType.Select).Name;

			var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;
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

#if !ACCESS

		/// <summary>
		/// Creates a operation used to perform an "upsert" operation.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="argumentValue"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Upsert<TArgument>(AbstractObjectName tableName, TArgument argumentValue, UpsertOptions options = UpsertOptions.None)
		where TArgument : class
		{
			return OnInsertOrUpdateObject<TArgument>(tableName, argumentValue, options);
		}

		/// <summary>
		/// Perform an insert or update operation as appropriate.
		/// </summary>
		/// <typeparam name="TArgument"></typeparam>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The options for how the insert/update occurs.</param>
		/// <returns></returns>
		public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Upsert<TArgument>(TArgument argumentValue, UpsertOptions options = UpsertOptions.None) where TArgument : class
		{
			return OnInsertOrUpdateObject<TArgument>(DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
		}

#endif

		TableOrViewMetadata<AbstractObjectName, AbstractDbType> DatabaseObjectAsTableOrView<TObject>(OperationType operationType)
		{
			var databaseObject = DatabaseMetadata.GetDatabaseObjectFromClass<TObject>(operationType);

			if (databaseObject is TableOrViewMetadata<AbstractObjectName, AbstractDbType> table)
				return table;
			else if (databaseObject is StoredProcedureMetadata<AbstractObjectName, AbstractDbType>)
				throw new ArgumentException($"Table or view expected. Use `Procedure<TObject>(value, OperationType.{operationType})` instead.");
			else if (databaseObject is TableFunctionMetadata<AbstractObjectName, AbstractDbType>)
				throw new ArgumentException($"Table or view expected. Use `TableFunction<TObject>(value, OperationType.{operationType})` instead.");
			else if (databaseObject is ScalarFunctionMetadata<AbstractObjectName, AbstractDbType>)
				throw new ArgumentException($"Table or view expected. Use `ScalarFunction<TObject>(value, OperationType.{operationType})` instead.");
			else
				throw new NotSupportedException($"Unexpected type {databaseObject.GetType()}");
		}

		TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> OnFromTableOrView(AbstractObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
			=> OnFromTableOrView<object>(tableOrViewName, filterValue, filterOptions);

		TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> OnFromTableOrView(AbstractObjectName tableOrViewName, string? whereClause, object? argumentValue)
			=> OnFromTableOrView<object>(tableOrViewName, whereClause, argumentValue);


		/// <summary>Truncates the specified table.</summary>
		/// <param name="tableName">Name of the table to truncate.</param>
		/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
		public partial ILink<int?> Truncate(AbstractObjectName tableName);

		/// <summary>Truncates the specified table.</summary>
		/// <typeparam name="TObject">This class used to determine which table to truncate</typeparam>
		/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
		public ILink<int?> Truncate<TObject>() where TObject : class
		{
			return Truncate(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name);
		}
	}
}
