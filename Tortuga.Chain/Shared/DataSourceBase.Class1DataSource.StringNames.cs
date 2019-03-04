using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.CommandBuilders;

#if SQL_SERVER

using AbstractCommand = System.Data.SqlClient.SqlCommand;
using AbstractObjectName = Tortuga.Chain.SqlServer.SqlServerObjectName;
using AbstractParameter = System.Data.SqlClient.SqlParameter;
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
using AbstractParameter = System.Data.OleDb.OleDbParameter;
using AbstractObjectName = Tortuga.Chain.Access.AccessObjectName;
using AbstractLimitOption = Tortuga.Chain.AccessLimitOption;

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
        /// Inserts an object model from the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The delete options.</param>
        /// <returns>SqlServerInsert.</returns>
        public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Delete<TArgument>(string tableName, TArgument argumentValue, DeleteOptions options)
        where TArgument : class
        {
            return Delete<TArgument>(new AbstractObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Inserts an object model from the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>SqlServerInsert.</returns>
        public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Delete<TArgument>(string tableName, TArgument argumentValue)
        where TArgument : class
        {
            return Delete<TArgument>(new AbstractObjectName(tableName), argumentValue, DeleteOptions.None);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteByKey<T>(string tableName, T key, DeleteOptions options)
            where T : struct
        {
            return DeleteByKey<T>(new AbstractObjectName(tableName), key, options);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteByKey(string tableName, string key, DeleteOptions options)
        {
            return DeleteByKey(new AbstractObjectName(tableName), key, options);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteByKey<T>(string tableName, T key)
            where T : struct
        {
            return DeleteByKey<T>(new AbstractObjectName(tableName), key, DeleteOptions.None);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteByKey(string tableName, string key)
        {
            return DeleteByKey(new AbstractObjectName(tableName), key, DeleteOptions.None);
        }

        /// <summary>
        /// Delete multiple rows by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="options">Update options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKeyList")]
        public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteByKeyList<TKey>(string tableName, IEnumerable<TKey> keys, DeleteOptions options)
        {
            return DeleteByKeyList<TKey>(new AbstractObjectName(tableName), keys, options);
        }

        /// <summary>
        /// Delete multiple rows by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKeyList")]
        public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteByKeyList<TKey>(string tableName, IEnumerable<TKey> keys)
        {
            return DeleteByKeyList<TKey>(new AbstractObjectName(tableName), keys, DeleteOptions.None);
        }

        /// <summary>
        /// Delete multiple records using a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteWithFilter(string tableName, string whereClause)
        {
            return DeleteWithFilter(new AbstractObjectName(tableName), whereClause);
        }

        /// <summary>
        /// Delete multiple records using a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value for the where clause.</param>
        public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteWithFilter(string tableName, string whereClause, object argumentValue)
        {
            return DeleteWithFilter(new AbstractObjectName(tableName), whereClause, argumentValue);
        }

        /// <summary>
        /// Delete multiple records using a filter object.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The options.</param>
        public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteWithFilter(string tableName, object filterValue, FilterOptions filterOptions)
        {
            return DeleteWithFilter(new AbstractObjectName(tableName), filterValue, filterOptions);
        }

        /// <summary>
        /// Delete multiple records using a filter object.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filterValue">The filter value.</param>
        public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> DeleteWithFilter(string tableName, object filterValue)
        {
            return DeleteWithFilter(new AbstractObjectName(tableName), filterValue, FilterOptions.None);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> From(string tableOrViewName)
        {
            return From(new AbstractObjectName(tableOrViewName));
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <returns>SqlServerTableOrView.</returns>
        public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> From(string tableOrViewName, string whereClause)
        {
            return From(new AbstractObjectName(tableOrViewName), whereClause);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <param name="argumentValue">Optional argument value. Every property in the argument value must have a matching parameter in the WHERE clause</param>
        /// <returns>SqlServerTableOrView.</returns>
        public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> From(string tableOrViewName, string whereClause, object argumentValue)
        {
            return From(new AbstractObjectName(tableOrViewName), whereClause, argumentValue);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value is used to generate a simple AND style WHERE clause.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <returns>SqlServerTableOrView.</returns>
        public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> From(string tableOrViewName, object filterValue, FilterOptions filterOptions)
        {
            return From(new AbstractObjectName(tableOrViewName), filterValue, filterOptions);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value is used to generate a simple AND style WHERE clause.</param>
        /// <returns>SqlServerTableOrView.</returns>
        public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> From(string tableOrViewName, object filterValue)
        {
            return From(new AbstractObjectName(tableOrViewName), filterValue, FilterOptions.None);
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> GetByKey<TKey>(string tableName, TKey key)
            where TKey : struct
        {
            return GetByKey<TKey>(new AbstractObjectName(tableName), key);
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> GetByKey(string tableName, string key)
        {
            return GetByKey(new AbstractObjectName(tableName), key);
        }

        /// <summary>Gets a set of records by an unique key.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keyColumn">Name of the key column. This should be a primary or unique key, but that's not enforced.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;MyAbstractCommand, MyAbstractParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> GetByKeyList<T>(string tableName, string keyColumn, IEnumerable<T> keys)
        {
            return GetByKeyList<T>(new AbstractObjectName(tableName), keyColumn, keys);
        }

        /// <summary>
        /// Gets a set of records by their primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;MyAbstractCommand, MyAbstractParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetByKeyList")]
        public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> GetByKeyList<T>(string tableName, IEnumerable<T> keys)
        {
            return GetByKeyList<T>(new AbstractObjectName(tableName), keys);
        }

        /// <summary>
        /// Inserts an object into the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// SqlServerInsert.
        /// </returns>
        public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Insert<TArgument>(string tableName, TArgument argumentValue, InsertOptions options)
        where TArgument : class
        {
            return Insert<TArgument>(new AbstractObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Inserts an object into the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>
        /// SqlServerInsert.
        /// </returns>
        public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Insert<TArgument>(string tableName, TArgument argumentValue)
        where TArgument : class
        {
            return Insert<TArgument>(new AbstractObjectName(tableName), argumentValue, InsertOptions.None);
        }

        /// <summary>
        /// Update an object in the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <returns>SqlServerInsert.</returns>
        public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Update<TArgument>(string tableName, TArgument argumentValue, UpdateOptions options)
        where TArgument : class
        {
            return Update<TArgument>(new AbstractObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Update an object in the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>SqlServerInsert.</returns>
        public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Update<TArgument>(string tableName, TArgument argumentValue)
        where TArgument : class
        {
            return Update<TArgument>(new AbstractObjectName(tableName), argumentValue, UpdateOptions.None);
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
        public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> UpdateByKey<TArgument, TKey>(string tableName, TArgument newValues, TKey key, UpdateOptions options)
            where TKey : struct
        {
            return UpdateByKey<TArgument, TKey>(new AbstractObjectName(tableName), newValues, key, options);
        }

        /// <summary>
        /// Update a record by its primary key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> UpdateByKey<TArgument>(string tableName, TArgument newValues, string key, UpdateOptions options)
        {
            return UpdateByKey<TArgument>(new AbstractObjectName(tableName), newValues, key, options);
        }

        /// <summary>
        /// Update a record by its primary key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> UpdateByKey<TArgument, TKey>(string tableName, TArgument newValues, TKey key)
            where TKey : struct
        {
            return UpdateByKey<TArgument, TKey>(new AbstractObjectName(tableName), newValues, key, UpdateOptions.None);
        }

        /// <summary>
        /// Update a record by its primary key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> UpdateByKey<TArgument>(string tableName, TArgument newValues, string key)
        {
            return UpdateByKey<TArgument>(new AbstractObjectName(tableName), newValues, key, UpdateOptions.None);
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
        /// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKeyList")]
        public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> UpdateByKeyList<TArgument, TKey>(string tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options)
        {
            return UpdateByKeyList<TArgument, TKey>(new AbstractObjectName(tableName), newValues, keys, options);
        }

        /// <summary>
        /// Update multiple rows by key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKeyList")]
        public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> UpdateByKeyList<TArgument, TKey>(string tableName, TArgument newValues, IEnumerable<TKey> keys)
        {
            return UpdateByKeyList<TArgument, TKey>(new AbstractObjectName(tableName), newValues, keys, UpdateOptions.None);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="options">The update options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<AbstractCommand, AbstractParameter> UpdateSet(string tableName, string updateExpression, UpdateOptions options)
        {
            return UpdateSet(new AbstractObjectName(tableName), updateExpression, options);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="updateArgumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<AbstractCommand, AbstractParameter> UpdateSet(string tableName, string updateExpression, object updateArgumentValue, UpdateOptions options)
        {
            return UpdateSet(new AbstractObjectName(tableName), updateExpression, updateArgumentValue, options);
        }

        /// <summary>
        /// Update multiple records using an update value.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="options">The options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<AbstractCommand, AbstractParameter> UpdateSet(string tableName, object newValues, UpdateOptions options)
        {
            return UpdateSet(new AbstractObjectName(tableName), newValues, options);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<AbstractCommand, AbstractParameter> UpdateSet(string tableName, string updateExpression)
        {
            return UpdateSet(new AbstractObjectName(tableName), updateExpression, UpdateOptions.None);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="updateArgumentValue">The argument value.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<AbstractCommand, AbstractParameter> UpdateSet(string tableName, string updateExpression, object updateArgumentValue)
        {
            return UpdateSet(new AbstractObjectName(tableName), updateExpression, updateArgumentValue, UpdateOptions.None);
        }

        /// <summary>
        /// Update multiple records using an update value.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<AbstractCommand, AbstractParameter> UpdateSet(string tableName, object newValues)
        {
            return UpdateSet(new AbstractObjectName(tableName), newValues, UpdateOptions.None);
        }

#if !ACCESS

        /// <summary>
        /// Perform an insert or update operation as appropriate.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert/update occurs.</param>
        /// <returns>SqlServerUpdate.</returns>
        public UpsertDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Upsert<TArgument>(string tableName, TArgument argumentValue, UpsertOptions options)
        where TArgument : class
        {
            return Upsert<TArgument>(new AbstractObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Perform an insert or update operation as appropriate.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>SqlServerUpdate.</returns>
        public UpsertDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Upsert<TArgument>(string tableName, TArgument argumentValue)
        where TArgument : class
        {
            return Upsert<TArgument>(new AbstractObjectName(tableName), argumentValue, UpsertOptions.None);
        }

#endif
    }
}
