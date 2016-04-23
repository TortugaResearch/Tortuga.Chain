using System.Threading;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SQLite.CommandBuilders;
using System.Collections.Generic;
using System.Linq;
using Tortuga.Anchor;
using System.Diagnostics.CodeAnalysis;

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
        private readonly ReaderWriterLockSlim m_SyncLock = new ReaderWriterLockSlim(); //Sqlite is single-threaded for writes. It says otherwise, but it spams the trace window with exceptions.

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
        public abstract SQLiteMetadataCache DatabaseMetadata { get; }

        /// <summary>
        /// Normally we use a reader/writer lock to avoid simultaneous writes to a SQlite database. If you disable this locking, you may see extra noise in your tracing output or unexpected exceptions.
        /// </summary>
        public bool DisableLocks { get; }

        IDatabaseMetadataCache IClass1DataSource.DatabaseMetadata
        {
            get { return DatabaseMetadata; }
        }

        /// <summary>
        /// Gets the synchronize lock used during execution of database operations.
        /// </summary>
        /// <value>The synchronize lock.</value>
        protected ReaderWriterLockSlim SyncLock
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

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;
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
    }


}
