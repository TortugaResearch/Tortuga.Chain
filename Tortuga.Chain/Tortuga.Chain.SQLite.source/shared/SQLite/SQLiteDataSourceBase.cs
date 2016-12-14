using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SQLite.CommandBuilders;
using System.Collections.Generic;
using System.Linq;
using Tortuga.Anchor;
using System.Diagnostics.CodeAnalysis;
using Nito.AsyncEx;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
using SQLiteConnection = Microsoft.Data.Sqlite.SqliteConnection;
using SQLiteTransaction = Microsoft.Data.Sqlite.SqliteTransaction;
#endif


namespace Tortuga.Chain.SQLite
{
    /// <summary>
    /// Base class that represents a SQLite Data Source.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public abstract partial class SQLiteDataSourceBase : DataSource<SQLiteConnection, SQLiteTransaction, SQLiteCommand, SQLiteParameter>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDataSourceBase"/> class.
        /// </summary>
        /// <param name="settings">Optional settings object.</param>
        protected SQLiteDataSourceBase(SQLiteDataSourceSettings settings) : base(settings)
        {
            if (settings != null)
            {
                DisableLocks = settings.DisableLocks ?? false;
            }
        }


        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        /// <value>The database metadata.</value>
        public abstract new SQLiteMetadataCache DatabaseMetadata { get; }


        /// <summary>
        /// Normally we use a reader/writer lock to avoid simultaneous writes to a SQlite database. If you disable this locking, you may see extra noise in your tracing output or unexpected exceptions.
        /// </summary>
        public bool DisableLocks { get; }



        /// <summary>
        /// Gets the synchronize lock used during execution of database operations.
        /// </summary>
        /// <value>The synchronize lock.</value>
        internal abstract AsyncReaderWriterLock SyncLock { get; }

        /// <summary>
        /// Creates a <see cref="SQLiteDeleteObject{TArgument}" /> used to perform a delete operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Delete<TArgument>(SQLiteObjectName tableName, TArgument argumentValue, DeleteOptions options = DeleteOptions.None)
        where TArgument : class
        {
            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new SQLiteDeleteObject<TArgument>(this, tableName, argumentValue, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new SQLiteUpdateObject<TArgument>(this, tableName, argumentValue, effectiveOptions);
        }

        /// <summary>
        /// Deletes an object model from the table indicated by the class's Table attribute.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The delete options.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Delete<TArgument>(TArgument argumentValue, DeleteOptions options = DeleteOptions.None) where TArgument : class
        {
            var table = DatabaseMetadata.GetTableOrViewFromClass<TArgument>();

            if (!AuditRules.UseSoftDelete(table))
                return new SQLiteDeleteObject<TArgument>(this, table.Name, argumentValue, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new SQLiteUpdateObject<TArgument>(this, table.Name, argumentValue, effectiveOptions);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <returns></returns>
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From(SQLiteObjectName tableOrViewName)
        {
            return new SQLiteTableOrView(this, tableOrViewName, null, null);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From(SQLiteObjectName tableOrViewName, string whereClause)
        {
            return new SQLiteTableOrView(this, tableOrViewName, whereClause, null);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="whereClause"></param>
        /// <param name="argumentValue"></param>
        /// <returns></returns>
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From(SQLiteObjectName tableOrViewName, string whereClause, object argumentValue)
        {
            return new SQLiteTableOrView(this, tableOrViewName, whereClause, argumentValue);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <returns>TableDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter, SQLiteLimitOption&gt;.</returns>
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From(SQLiteObjectName tableOrViewName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            return new SQLiteTableOrView(this, tableOrViewName, filterValue, filterOptions);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From<TObject>() where TObject : class
        {
            return From(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From<TObject>(string whereClause) where TObject : class
        {
            return From(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, whereClause);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <param name="argumentValue">Optional argument value. Every property in the argument value must have a matching parameter in the WHERE clause</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From<TObject>(string whereClause, object argumentValue) where TObject : class
        {
            return From(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, whereClause, argumentValue);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="filterValue">The filter value is used to generate a simple AND style WHERE clause.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From<TObject>(object filterValue) where TObject : class
        {
            return From(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, filterValue);
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> GetByKey<TKey>(SQLiteObjectName tableName, TKey key)
            where TKey : struct
        {
            return GetByKeyList(tableName, new List<TKey> { key });
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> GetByKey(SQLiteObjectName tableName, string key)
        {
            return GetByKeyList(tableName, new List<string> { key });
        }

        /// <summary>
        /// Gets a set of records by their primary key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> GetByKey<TKey>(SQLiteObjectName tableName, params TKey[] keys)
            where TKey : struct
        {
            return GetByKeyList(tableName, keys);
        }

        /// <summary>
        /// Gets a set of records by their primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> GetByKey(SQLiteObjectName tableName, params string[] keys)
        {
            return GetByKeyList(tableName, keys);
        }

        /// <summary>
        /// Gets a set of records by their primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> GetByKeyList<T>(SQLiteObjectName tableName, IEnumerable<T> keys)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.IsPrimaryKey).ToList();
            if (primaryKeys.Count != 1)
                throw new MappingException($"GetByKey operation isn't allowed on {tableName} because it doesn't have a single primary key. Use DataSource.From instead.");

            var keyList = keys.AsList();
            var columnMetadata = primaryKeys.Single();

            string where;
            if (keys.Count() > 1)
                where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
            else
                where = columnMetadata.SqlName + " = @Param0";

            var parameters = new List<SQLiteParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new SQLiteParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.DbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new SQLiteTableOrView(this, tableName, where, parameters);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteInsertObject{TArgument}" /> used to perform an insert operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Insert<TArgument>(SQLiteObjectName tableName, TArgument argumentValue, InsertOptions options = InsertOptions.None)
        where TArgument : class
        {
            return new SQLiteInsertObject<TArgument>(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Inserts an object into the specified table.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert occurs.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Insert<TArgument>(TArgument argumentValue, InsertOptions options = InsertOptions.None) where TArgument : class
        {
            return Insert(DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
        }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="lockType">Type of the lock.</param>
        /// <returns>SQLiteSqlCall.</returns>
        public MultipleTableDbCommandBuilder<SQLiteCommand, SQLiteParameter> Sql(string sqlStatement, LockType lockType = LockType.Write)
        {
            return new SQLiteSqlCall(this, sqlStatement, null, lockType);
        }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="lockType">Type of the lock.</param>
        /// <returns>SQLiteSqlCall.</returns>
        public MultipleTableDbCommandBuilder<SQLiteCommand, SQLiteParameter> Sql(string sqlStatement, object argumentValue, LockType lockType = LockType.Write)
        {
            return new SQLiteSqlCall(this, sqlStatement, argumentValue, lockType);
        }
        /// <summary>
        /// Creates a <see cref="SQLiteUpdateObject{TArgument}" /> used to perform an update operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Update<TArgument>(SQLiteObjectName tableName, TArgument argumentValue, UpdateOptions options = UpdateOptions.None)
        where TArgument : class
        {
            return new SQLiteUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Updates an object in the specified table.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Update<TArgument>(TArgument argumentValue, UpdateOptions options = UpdateOptions.None) where TArgument : class
        {
            return Update(DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteInsertOrUpdateObject{TArgument}"/> used to perform an "upsert" operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Upsert<TArgument>(SQLiteObjectName tableName, TArgument argumentValue, UpsertOptions options = UpsertOptions.None)
        where TArgument : class
        {
            return new SQLiteInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }
        /// <summary>
        /// Performs an insert or update operation as appropriate.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert/update occurs.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Upsert<TArgument>(TArgument argumentValue, UpsertOptions options = UpsertOptions.None) where TArgument : class
        {
            return Upsert(DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
        }
        /// <summary>
        /// Called when Database.DatabaseMetadata is invoked.
        /// </summary>
        /// <returns></returns>
        protected override IDatabaseMetadataCache OnGetDatabaseMetadata()
        {
            return DatabaseMetadata;
        }


        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKey<TArgument, TKey>(SQLiteObjectName tableName, TArgument newValues, TKey key, UpdateOptions options = UpdateOptions.None)
            where TKey : struct
        {
            return UpdateByKeyList(tableName, newValues, new List<TKey> { key }, options);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKey<TArgument>(SQLiteObjectName tableName, TArgument newValues, string key, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateByKeyList(tableName, newValues, new List<string> { key }, options);
        }

        /// <summary>
        /// Delete multiple rows by key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKey<TArgument, TKey>(SQLiteObjectName tableName, TArgument newValues, params TKey[] keys)
            where TKey : struct
        {
            return UpdateByKeyList(tableName, newValues, keys);
        }

        /// <summary>
        /// Delete multiple rows by key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKey<TArgument>(SQLiteObjectName tableName, TArgument newValues, params string[] keys)
        {
            return UpdateByKeyList(tableName, newValues, keys);
        }


        /// <summary>
        /// Updates multiple rows by key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="options">Update options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKey")]
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKeyList<TArgument, TKey>(SQLiteObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.IsPrimaryKey).ToList();
            if (primaryKeys.Count != 1)
                throw new MappingException($"UpdateByKey operation isn't allowed on {tableName} because it doesn't have a single primary key.");

            var keyList = keys.AsList();
            var columnMetadata = primaryKeys.Single();
            string where;
            if (keys.Count() > 1)
                where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
            else
                where = columnMetadata.SqlName + " = @Param0";

            var parameters = new List<SQLiteParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new SQLiteParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.DbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new SQLiteUpdateSet(this, tableName, newValues, where, parameters, parameters.Count, options);
        }



        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKey<T>(SQLiteObjectName tableName, T key, DeleteOptions options = DeleteOptions.None)
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
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKey(SQLiteObjectName tableName, string key, DeleteOptions options = DeleteOptions.None)
        {
            return DeleteByKeyList(tableName, new List<string> { key }, options);
        }

        /// <summary>
        /// Delete multiple rows by key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKey<T>(SQLiteObjectName tableName, params T[] keys)
            where T : struct
        {
            return DeleteByKeyList(tableName, keys);
        }

        /// <summary>
        /// Delete multiple rows by key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKey(SQLiteObjectName tableName, params string[] keys)
        {
            return DeleteByKeyList(tableName, keys);
        }

        /// <summary>
        /// Delete multiple rows by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="options">Update options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKey")]
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKeyList<TKey>(SQLiteObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.IsPrimaryKey).ToList();
            if (primaryKeys.Count != 1)
                throw new MappingException($"DeleteByKey operation isn't allowed on {tableName} because it doesn't have a single primary key.");

            var keyList = keys.AsList();
            var columnMetadata = primaryKeys.Single();
            string where;
            if (keys.Count() > 1)
                where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
            else
                where = columnMetadata.SqlName + " = @Param0";

            var parameters = new List<SQLiteParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new SQLiteParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.DbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new SQLiteDeleteSet(this, tableName, where, parameters, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new SQLiteUpdateSet(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);

        }

        /// <summary>
        /// Deletes multiple records using a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteSet(SQLiteObjectName tableName, string whereClause)
        {
            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new SQLiteDeleteSet(this, tableName, whereClause, null);

            return new SQLiteUpdateSet(this, tableName, null, whereClause, null, UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected);
        }

        /// <summary>
        /// Deletes multiple records using a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value for the where clause.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteSet(SQLiteObjectName tableName, string whereClause, object argumentValue)
        {
            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new SQLiteDeleteSet(this, tableName, whereClause, argumentValue);

            return new SQLiteUpdateSet(this, tableName, null, whereClause, argumentValue, UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected);
        }

        /// <summary>
        /// Deletes multiple records using a filter object.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The options.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteSet(SQLiteObjectName tableName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new SQLiteDeleteSet(this, tableName, filterValue, filterOptions);

            return new SQLiteUpdateSet(this, tableName, null, filterValue, filterOptions, UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected);
        }


        /// <summary>
        /// Updates multiple records using an update expression and a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteSet(SQLiteObjectName tableName, string updateExpression, string whereClause, object argumentValue, UpdateOptions options = UpdateOptions.None)
        {
            return new SQLiteUpdateSet(this, tableName, updateExpression, whereClause, argumentValue, options);
        }

        /// <summary>
        /// Updates multiple records using an update expression and a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="options">The options.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteSet(SQLiteObjectName tableName, string updateExpression, string whereClause, UpdateOptions options = UpdateOptions.None)
        {
            return new SQLiteUpdateSet(this, tableName, updateExpression, whereClause, null, options);
        }

        /// <summary>
        /// Updates multiple records using an update expression and a filter object.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <param name="options">The update options.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteSet(SQLiteObjectName tableName, string updateExpression, object filterValue, FilterOptions filterOptions = FilterOptions.None, UpdateOptions options = UpdateOptions.None)
        {
            return new SQLiteUpdateSet(this, tableName, updateExpression, filterValue, filterOptions, options);
        }


        /// <summary>
        /// Updates multiple records using an update value and a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteSet(SQLiteObjectName tableName, object newValues, string whereClause, object argumentValue, UpdateOptions options = UpdateOptions.None)
        {
            return new SQLiteUpdateSet(this, tableName, newValues, whereClause, argumentValue, options);
        }


        /// <summary>
        /// Updates multiple records using an update value and a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="options">The update options.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteSet(SQLiteObjectName tableName, object newValues, string whereClause, UpdateOptions options = UpdateOptions.None)
        {
            return new SQLiteUpdateSet(this, tableName, newValues, whereClause, null, options);
        }


        /// <summary>
        /// Updates multiple records using an update value and a filter object.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <param name="options">The update options.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteSet(SQLiteObjectName tableName, object newValues, object filterValue, FilterOptions filterOptions = FilterOptions.None, UpdateOptions options = UpdateOptions.None)
        {
            return new SQLiteUpdateSet(this, tableName, newValues, filterValue, filterOptions, options);
        }

        /// <summary>
        /// Updates multiple records using an update expression and a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(SQLiteObjectName tableName, string updateExpression, string whereClause, object argumentValue, UpdateOptions options = UpdateOptions.None)
        {
            return new SQLiteUpdateSet(this, tableName, updateExpression, whereClause, argumentValue, options);
        }

        /// <summary>
        /// Updates multiple records using an update expression and a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="options">The options.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(SQLiteObjectName tableName, string updateExpression, string whereClause, UpdateOptions options = UpdateOptions.None)
        {
            return new SQLiteUpdateSet(this, tableName, updateExpression, whereClause, null, options);
        }

        /// <summary>
        /// Updates multiple records using an update expression and a filter object.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <param name="options">The update options.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(SQLiteObjectName tableName, string updateExpression, object filterValue, FilterOptions filterOptions = FilterOptions.None, UpdateOptions options = UpdateOptions.None)
        {
            return new SQLiteUpdateSet(this, tableName, updateExpression, filterValue, filterOptions, options);
        }


        /// <summary>
        /// Updates multiple records using an update value and a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(SQLiteObjectName tableName, object newValues, string whereClause, object argumentValue, UpdateOptions options = UpdateOptions.None)
        {
            return new SQLiteUpdateSet(this, tableName, newValues, whereClause, argumentValue, options);
        }


        /// <summary>
        /// Updates multiple records using an update value and a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="options">The update options.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(SQLiteObjectName tableName, object newValues, string whereClause, UpdateOptions options = UpdateOptions.None)
        {
            return new SQLiteUpdateSet(this, tableName, newValues, whereClause, null, options);
        }


        /// <summary>
        /// Updates multiple records using an update value and a filter object.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <param name="options">The update options.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(SQLiteObjectName tableName, object newValues, object filterValue, FilterOptions filterOptions = FilterOptions.None, UpdateOptions options = UpdateOptions.None)
        {
            return new SQLiteUpdateSet(this, tableName, newValues, filterValue, filterOptions, options);
        }
    }


}
