using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tortuga.Chain.CommandBuilders;

#if SQL_SERVER

using AbstractCommand = System.Data.SqlClient.SqlCommand;
using AbstractParameter = System.Data.SqlClient.SqlParameter;
using AbstractObjectName = Tortuga.Chain.SqlServer.SqlServerObjectName;
using AbstractLimitOption = Tortuga.Chain.SqlServerLimitOption;

#elif SQL_SERVER_OLEDB

using AbstractCommand = System.Data.OleDb.OleDbCommand;
using AbstractLimitOption = Tortuga.Chain.SqlServerLimitOption;
using AbstractObjectName = Tortuga.Chain.SqlServer.SqlServerObjectName;
using AbstractParameter = System.Data.OleDb.OleDbParameter;

#elif SQLITE

using AbstractCommand = System.Data.SQLite.SQLiteCommand;
using AbstractParameter = System.Data.SQLite.SQLiteParameter;
using AbstractObjectName = Tortuga.Chain.SQLite.SQLiteObjectName;
using AbstractLimitOption = Tortuga.Chain.SQLiteLimitOption;

#elif MYSQL

using AbstractCommand = MySql.Data.MySqlClient.MySqlCommand;
using AbstractParameter = MySql.Data.MySqlClient.MySqlParameter;
using AbstractObjectName = Tortuga.Chain.MySql.MySqlObjectName;
using AbstractLimitOption = Tortuga.Chain.MySqlLimitOption;

#elif POSTGRESQL

using AbstractCommand = Npgsql.NpgsqlCommand;
using AbstractParameter = Npgsql.NpgsqlParameter;
using AbstractObjectName = Tortuga.Chain.PostgreSql.PostgreSqlObjectName;
using AbstractLimitOption = Tortuga.Chain.PostgreSqlLimitOption;

#elif ACCESS

using AbstractCommand = System.Data.OleDb.OleDbCommand;
using AbstractLimitOption = Tortuga.Chain.AccessLimitOption;
using AbstractObjectName = Tortuga.Chain.Access.AccessObjectName;
using AbstractParameter = System.Data.OleDb.OleDbParameter;

#endif

#if SQL_SERVER

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
        /// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteByKey(AbstractObjectName tableName, string key, DeleteOptions options = DeleteOptions.None)
        {
            return DeleteByKeyList(tableName, new List<string> { key }, options);
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
        public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> From<TObject>() where TObject : class
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
        public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> From<TObject>(string whereClause) where TObject : class
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
        public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> From<TObject>(string whereClause, object argumentValue) where TObject : class
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
        public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> From<TObject>(object filterValue) where TObject : class
        {
            return From(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, filterValue);
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
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="updateArgumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<AbstractCommand, AbstractParameter> UpdateSet(AbstractObjectName tableName, string updateExpression, object updateArgumentValue, UpdateOptions options = UpdateOptions.None)
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
        public IUpdateManyCommandBuilder<AbstractCommand, AbstractParameter> UpdateSet(AbstractObjectName tableName, object newValues, UpdateOptions options = UpdateOptions.None)
        {
            return OnUpdateMany(tableName, newValues, options);
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> GetByKey<TKey>(AbstractObjectName tableName, TKey key)
            where TKey : struct
        {
            return GetByKeyList(tableName, keys: (IEnumerable<TKey>)new List<TKey> { key });
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> GetByKey(AbstractObjectName tableName, string key)
        {
            return GetByKeyList(tableName, keys: (IEnumerable<string>)new List<string> { key });
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
        public IUpdateManyCommandBuilder<AbstractCommand, AbstractParameter> UpdateSet(AbstractObjectName tableName, string updateExpression, UpdateOptions options = UpdateOptions.None)
        {
            return OnUpdateMany(tableName, updateExpression, null, options);
        }

        /// <summary>
        /// Gets a set of records by their primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetByKeyList")]
        public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> GetByKeyList<T>(AbstractObjectName tableName, IEnumerable<T> keys)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.IsPrimaryKey).ToList();
            if (primaryKeys.Count != 1)
                throw new MappingException($"{nameof(GetByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key. Use DataSource.From instead.");

            return GetByKeyList<T>(tableName, primaryKeys.Single(), keys);
        }

        /// <summary>
        /// Gets a set of records by an unique key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keyColumn">Name of the key column. This should be a primary or unique key, but that's not enforced.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetByKeyList")]
        public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> GetByKeyList<T>(AbstractObjectName tableName, string keyColumn, IEnumerable<T> keys)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.SqlName.Equals(keyColumn, System.StringComparison.OrdinalIgnoreCase)).ToList();
            if (primaryKeys.Count == 0)
                throw new MappingException($"Cannot find a column named {keyColumn} on table {tableName}.");

            return GetByKeyList<T>(tableName, primaryKeys.Single(), keys);
        }

#if !ACCESS

        /// <summary>
        /// Creates a operation used to perform an "upsert" operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public UpsertDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Upsert<TArgument>(AbstractObjectName tableName, TArgument argumentValue, UpsertOptions options = UpsertOptions.None)
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
        public UpsertDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Upsert<TArgument>(TArgument argumentValue, UpsertOptions options = UpsertOptions.None) where TArgument : class
        {
            return OnInsertOrUpdateObject<TArgument>(DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
        }

#endif
    }
}
