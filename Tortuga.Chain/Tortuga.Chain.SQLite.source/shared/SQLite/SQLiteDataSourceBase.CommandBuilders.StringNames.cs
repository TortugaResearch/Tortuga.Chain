using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.SQLite.CommandBuilders;

namespace Tortuga.Chain.SQLite
{
    //Methods in this file should always delegate the same method using SQLiteObjectName instead of a string for the table name.
    //In order to support F#, we are not using optional parameters here. See https://github.com/docevaad/Chain/issues/262 for details.

    partial class SQLiteDataSourceBase
    {
        /// <summary>
        /// Creates a <see cref="SQLiteDeleteObject{TArgument}" /> used to perform a delete operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Delete<TArgument>(string tableName, TArgument argumentValue, DeleteOptions options)
        where TArgument : class
        {
            return Delete<TArgument>(new SQLiteObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteDeleteObject{TArgument}" /> used to perform a delete operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Delete<TArgument>(string tableName, TArgument argumentValue)
        where TArgument : class
        {
            return Delete<TArgument>(new SQLiteObjectName(tableName), argumentValue, DeleteOptions.None);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKey<T>(string tableName, T key, DeleteOptions options)
            where T : struct
        {
            return DeleteByKey<T>(new SQLiteObjectName(tableName), key, options);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKey(string tableName, string key, DeleteOptions options)
        {
            return DeleteByKey(new SQLiteObjectName(tableName), key, options);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKey<T>(string tableName, T key)
            where T : struct
        {
            return DeleteByKey<T>(new SQLiteObjectName(tableName), key, DeleteOptions.None);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKey(string tableName, string key)
        {
            return DeleteByKey(new SQLiteObjectName(tableName), key, DeleteOptions.None);
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
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKeyList")]
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKeyList<TKey>(string tableName, IEnumerable<TKey> keys, DeleteOptions options)
        {
            return DeleteByKeyList<TKey>(new SQLiteObjectName(tableName), keys, options);
        }

        /// <summary>
        /// Delete multiple rows by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKeyList")]
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKeyList<TKey>(string tableName, IEnumerable<TKey> keys)
        {
            return DeleteByKeyList<TKey>(new SQLiteObjectName(tableName), keys, DeleteOptions.None);
        }

        /// <summary>
        /// Delete multiple records using a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteWithFilter(string tableName, string whereClause)
        {
            return DeleteWithFilter(new SQLiteObjectName(tableName), whereClause);
        }

        /// <summary>
        /// Delete multiple records using a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value for the where clause.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteWithFilter(string tableName, string whereClause, object argumentValue)
        {
            return DeleteWithFilter(new SQLiteObjectName(tableName), whereClause, argumentValue);
        }

        /// <summary>
        /// Delete multiple records using a filter object.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The options.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteWithFilter(string tableName, object filterValue, FilterOptions filterOptions)
        {
            return DeleteWithFilter(new SQLiteObjectName(tableName), filterValue, filterOptions);
        }

        /// <summary>
        /// Delete multiple records using a filter object.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filterValue">The filter value.</param>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteWithFilter(string tableName, object filterValue)
        {
            return DeleteWithFilter(new SQLiteObjectName(tableName), filterValue, FilterOptions.None);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <returns></returns>
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From(string tableOrViewName)
        {
            return From(new SQLiteObjectName(tableOrViewName));
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From(string tableOrViewName, string whereClause)
        {
            return From(new SQLiteObjectName(tableOrViewName), whereClause);
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
            return From(new SQLiteObjectName(tableOrViewName), whereClause, argumentValue);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <returns>TableDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter, SQLiteLimitOption&gt;.</returns>
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From(string tableOrViewName, object filterValue, FilterOptions filterOptions)
        {
            return From(new SQLiteObjectName(tableOrViewName), filterValue, filterOptions);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <returns>TableDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter, SQLiteLimitOption&gt;.</returns>
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From(string tableOrViewName, object filterValue)
        {
            return From(new SQLiteObjectName(tableOrViewName), filterValue, FilterOptions.None);
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> GetByKey<TKey>(string tableName, TKey key)
            where TKey : struct
        {
            return GetByKey<TKey>(new SQLiteObjectName(tableName), key);
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> GetByKey(string tableName, string key)
        {
            return GetByKey(new SQLiteObjectName(tableName), key);
        }

        /// <summary>Gets a set of records by an unique key.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keyColumn">Name of the key column. This should be a primary or unique key, but that's not enforced.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> GetByKeyList<T>(string tableName, string keyColumn, IEnumerable<T> keys)
        {
            return GetByKeyList<T>(new SQLiteObjectName(tableName), keyColumn, keys);
        }

        /// <summary>
        /// Gets a set of records by their primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetByKeyList")]
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> GetByKeyList<T>(string tableName, IEnumerable<T> keys)
        {
            return GetByKeyList<T>(new SQLiteObjectName(tableName), keys);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteInsertObject{TArgument}" /> used to perform an insert operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Insert<TArgument>(string tableName, TArgument argumentValue, InsertOptions options)
        where TArgument : class
        {
            return Insert<TArgument>(new SQLiteObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteInsertObject{TArgument}" /> used to perform an insert operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Insert<TArgument>(string tableName, TArgument argumentValue)
        where TArgument : class
        {
            return Insert<TArgument>(new SQLiteObjectName(tableName), argumentValue, InsertOptions.None);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteUpdateObject{TArgument}" /> used to perform an update operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Update<TArgument>(string tableName, TArgument argumentValue, UpdateOptions options)
        where TArgument : class
        {
            return Update<TArgument>(new SQLiteObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteUpdateObject{TArgument}" /> used to perform an update operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Update<TArgument>(string tableName, TArgument argumentValue)
        where TArgument : class
        {
            return Update<TArgument>(new SQLiteObjectName(tableName), argumentValue, UpdateOptions.None);
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
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKey<TArgument, TKey>(string tableName, TArgument newValues, TKey key, UpdateOptions options)
            where TKey : struct
        {
            return UpdateByKey<TArgument, TKey>(new SQLiteObjectName(tableName), newValues, key, options);
        }

        /// <summary>
        /// Update a record by its primary key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKey<TArgument>(string tableName, TArgument newValues, string key, UpdateOptions options)
        {
            return UpdateByKey<TArgument>(new SQLiteObjectName(tableName), newValues, key, options);
        }

        /// <summary>
        /// Update a record by its primary key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKey<TArgument, TKey>(string tableName, TArgument newValues, TKey key)
            where TKey : struct
        {
            return UpdateByKey<TArgument, TKey>(new SQLiteObjectName(tableName), newValues, key, UpdateOptions.None);
        }

        /// <summary>
        /// Update a record by its primary key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKey<TArgument>(string tableName, TArgument newValues, string key)
        {
            return UpdateByKey<TArgument>(new SQLiteObjectName(tableName), newValues, key, UpdateOptions.None);
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
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKeyList")]
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKeyList<TArgument, TKey>(string tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options)
        {
            return UpdateByKeyList<TArgument, TKey>(new SQLiteObjectName(tableName), newValues, keys, options);
        }

        /// <summary>
        /// Update multiple rows by key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKeyList")]
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKeyList<TArgument, TKey>(string tableName, TArgument newValues, IEnumerable<TKey> keys)
        {
            return UpdateByKeyList<TArgument, TKey>(new SQLiteObjectName(tableName), newValues, keys, UpdateOptions.None);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="options">The update options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(string tableName, string updateExpression, UpdateOptions options)
        {
            return UpdateSet(new SQLiteObjectName(tableName), updateExpression, options);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="updateArgumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(string tableName, string updateExpression, object updateArgumentValue, UpdateOptions options)
        {
            return UpdateSet(new SQLiteObjectName(tableName), updateExpression, updateArgumentValue, options);
        }

        /// <summary>
        /// Update multiple records using an update value.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="options">The options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(string tableName, object newValues, UpdateOptions options)
        {
            return UpdateSet(new SQLiteObjectName(tableName), newValues, options);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(string tableName, string updateExpression)
        {
            return UpdateSet(new SQLiteObjectName(tableName), updateExpression, UpdateOptions.None);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="updateArgumentValue">The argument value.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(string tableName, string updateExpression, object updateArgumentValue)
        {
            return UpdateSet(new SQLiteObjectName(tableName), updateExpression, updateArgumentValue, UpdateOptions.None);
        }

        /// <summary>
        /// Update multiple records using an update value.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(string tableName, object newValues)
        {
            return UpdateSet(new SQLiteObjectName(tableName), newValues, UpdateOptions.None);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteInsertOrUpdateObject{TArgument}"/> used to perform an "upsert" operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Upsert<TArgument>(string tableName, TArgument argumentValue, UpsertOptions options)
        where TArgument : class
        {
            return Upsert<TArgument>(new SQLiteObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteInsertOrUpdateObject{TArgument}"/> used to perform an "upsert" operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Upsert<TArgument>(string tableName, TArgument argumentValue)
        where TArgument : class
        {
            return Upsert<TArgument>(new SQLiteObjectName(tableName), argumentValue, UpsertOptions.None);
        }
    }
}