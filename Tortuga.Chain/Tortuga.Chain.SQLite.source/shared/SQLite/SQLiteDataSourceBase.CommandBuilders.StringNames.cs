using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.SQLite.CommandBuilders;

namespace Tortuga.Chain.SQLite
{
    //Methods in this file should always delegate the same method using SQLiteObjectName instead of a string for the table name.

    partial class SQLiteDataSourceBase
    {
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
            return Delete<TArgument>(new SQLiteObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKey<T>(string tableName, T key, DeleteOptions options = DeleteOptions.None)
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
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKey(string tableName, string key, DeleteOptions options = DeleteOptions.None)
        {
            return DeleteByKey(new SQLiteObjectName(tableName), key, options);
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
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKeyList<TKey>(string tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
        {
            return DeleteByKeyList<TKey>(new SQLiteObjectName(tableName), keys, options);
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
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteWithFilter(string tableName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            return DeleteWithFilter(new SQLiteObjectName(tableName), filterValue, filterOptions);
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
        public TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> From(string tableOrViewName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            return From(new SQLiteObjectName(tableOrViewName), filterValue, filterOptions);
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
        public ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> Insert<TArgument>(string tableName, TArgument argumentValue, InsertOptions options = InsertOptions.None)
        where TArgument : class
        {
            return Insert<TArgument>(new SQLiteObjectName(tableName), argumentValue, options);
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
            return Update<TArgument>(new SQLiteObjectName(tableName), argumentValue, options);
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
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKey<TArgument, TKey>(string tableName, TArgument newValues, TKey key, UpdateOptions options = UpdateOptions.None)
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
        public SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKey<TArgument>(string tableName, TArgument newValues, string key, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateByKey<TArgument>(new SQLiteObjectName(tableName), newValues, key, options);
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
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKeyList<TArgument, TKey>(string tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateByKeyList<TArgument, TKey>(new SQLiteObjectName(tableName), newValues, keys, options);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="options">The update options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(string tableName, string updateExpression, UpdateOptions options = UpdateOptions.None)
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
        public IUpdateManyCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(string tableName, string updateExpression, object updateArgumentValue, UpdateOptions options = UpdateOptions.None)
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
        public IUpdateManyCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateSet(string tableName, object newValues, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateSet(new SQLiteObjectName(tableName), newValues, options);
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
            return Upsert<TArgument>(new SQLiteObjectName(tableName), argumentValue, options);
        }
    }
}
