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
#endif


namespace Tortuga.Chain.SQLite
{
    /// <summary>
    /// Base class that represents a SQLite Data Source.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public abstract class SQLiteDataSourceBase : DataSource<SQLiteConnection, SQLiteTransaction, SQLiteCommand, SQLiteParameter>, IClass1DataSource
    {
        readonly AsyncReaderWriterLock m_SyncLock = new AsyncReaderWriterLock(); //Sqlite is single-threaded for writes. It says otherwise, but it spams the trace window with exceptions.

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
        /// Gets the database metadata.
        /// </summary>
        /// <value>
        /// The database metadata.
        /// </value>
        /// <remarks>
        /// Data sources are expected to shadow this with their specific version.
        /// </remarks>
        IDatabaseMetadataCache IClass1DataSource.DatabaseMetadata
        {
            get { return DatabaseMetadata; }
        }

        /// <summary>
        /// Gets the synchronize lock used during execution of database operations.
        /// </summary>
        /// <value>The synchronize lock.</value>
        protected AsyncReaderWriterLock SyncLock
        {
            get { return m_SyncLock; }
        }

        /// <summary>
        /// Creates a <see cref="SQLiteDeleteObject{TArgument}" /> used to perform a delete operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Delete<TArgument>(string tableName, TArgument argumentValue, DeleteOptions options = DeleteOptions.None)
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
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <returns></returns>
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From(string tableOrViewName)
        {
            return new SQLiteTableOrView(this, tableOrViewName, null, null);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From(string tableOrViewName, string whereClause)
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
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From(string tableOrViewName, string whereClause, object argumentValue)
        {
            return new SQLiteTableOrView(this, tableOrViewName, whereClause, argumentValue);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From(string tableOrViewName, object filterValue)
        {
            return new SQLiteTableOrView(this, tableOrViewName, filterValue);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Delete<TArgument>(string tableName, TArgument argumentValue, DeleteOptions options)
        {
            return Delete(tableName, argumentValue, options);
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName)
        {
            return From(tableOrViewName);
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName, string whereClause)
        {
            return From(tableOrViewName, whereClause);
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName, string whereClause, object argumentValue)
        {
            return From(tableOrViewName, whereClause, argumentValue);
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName, object filterValue)
        {
            return From(tableOrViewName, filterValue);
        }
        IMultipleTableDbCommandBuilder IClass0DataSource.Sql(string sqlStatement, object argumentValue)
        {
            return Sql(sqlStatement, argumentValue);
        }
        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Update<TArgument>(string tableName, TArgument argumentValue, UpdateOptions options)
        {
            return Update(tableName, argumentValue, options);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Upsert<TArgument>(string tableName, TArgument argumentValue, UpsertOptions options)
        {
            return Upsert(tableName, argumentValue, options);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Insert<TArgument>(string tableName, TArgument argumentValue, InsertOptions options)
        {
            return Insert(tableName, argumentValue, options);
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
        /// Creates a <see cref="SQLiteInsertObject{TArgument}" /> used to perform an insert operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Insert<TArgument>(string tableName, TArgument argumentValue, InsertOptions options = InsertOptions.None)
        where TArgument : class
        {
            return new SQLiteInsertObject<TArgument>(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteInsertOrUpdateObject{TArgument}"/> used to perform an "upsert" operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Upsert<TArgument>(string tableName, TArgument argumentValue, UpsertOptions options = UpsertOptions.None)
        where TArgument : class
        {
            return new SQLiteInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteUpdateObject{TArgument}" /> used to perform an update operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Update<TArgument>(string tableName, TArgument argumentValue, UpdateOptions options = UpdateOptions.None)
        where TArgument : class
        {
            return new SQLiteUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }


        ISingleRowDbCommandBuilder IClass1DataSource.GetByKey<T>(string tableName, T key)
        {
            return GetByKey(tableName, key);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.GetByKey<T>(string tableName, IEnumerable<T> keys)
        {
            return GetByKey(tableName, keys);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.GetByKey<T>(string tableName, params T[] keys)
        {
            return GetByKey(tableName, (IEnumerable<T>)keys);
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> GetByKey<T>(string tableName, T key)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.IsPrimaryKey).ToList();
            if (primaryKeys.Count != 1)
                throw new MappingException($"GetByKey operation isn't allowed on {tableName} because it doesn't have a single primary key. Use DataSource.From instead.");

            var columnMetadata = primaryKeys.Single();
            var where = columnMetadata.SqlName + " = " + columnMetadata.SqlVariableName;

            var parameters = new List<SQLiteParameter>();

            var param = new SQLiteParameter(columnMetadata.SqlVariableName, key);
            if (columnMetadata.DbType.HasValue)
                param.DbType = columnMetadata.DbType.Value;
            parameters.Add(param);


            return new SQLiteTableOrView(this, tableName, where, parameters);
        }

        /// <summary>
        /// Gets a set of records by their primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> GetByKey<T>(string tableName, params T[] keys)
        {
            return GetByKey(tableName, (IEnumerable<T>)keys);
        }

        /// <summary>
        /// Gets a set of records by their primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> GetByKey<T>(string tableName, IEnumerable<T> keys)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.IsPrimaryKey).ToList();
            if (primaryKeys.Count != 1)
                throw new MappingException($"GetByKey operation isn't allowed on {tableName} because it doesn't have a single primary key. Use DataSource.From instead.");

            var keyList = keys.AsList();
            var columnMetadata = primaryKeys.Single();
            var where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";

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

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Delete<TArgument>(TArgument argumentValue, DeleteOptions options)
        {
            return Delete(argumentValue, options);
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

        ITableDbCommandBuilder IClass1DataSource.From<TObject>()
        {
            return From<TObject>();
        }

        ITableDbCommandBuilder IClass1DataSource.From<TObject>(string whereClause)
        {
            return From<TObject>(whereClause);
        }

        ITableDbCommandBuilder IClass1DataSource.From<TObject>(string whereClause, object argumentValue)
        {
            return From<TObject>(whereClause, argumentValue);
        }

        ITableDbCommandBuilder IClass1DataSource.From<TObject>(object filterValue)
        {
            return From<TObject>(filterValue);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Insert<TArgument>(TArgument argumentValue, InsertOptions options)
        {
            return Insert(argumentValue, options);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Upsert<TArgument>(TArgument argumentValue, UpsertOptions options)
        {
            return Upsert(argumentValue, options);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Update<TArgument>(TArgument argumentValue, UpdateOptions options)
        {
            return Update(argumentValue, options);
        }


        /// <summary>
        /// Called when Database.DatabaseMetadata is invoked.
        /// </summary>
        /// <returns></returns>
        protected override IDatabaseMetadataCache OnGetDatabaseMetadata()
        {
            return DatabaseMetadata;
        }

    }


}
