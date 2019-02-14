using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Access.CommandBuilders;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Access
{
    //Methods in this file should always delegate the same method using AccessObjectName instead of a string for the table name.

    partial class AccessDataSourceBase
    {
        /// <summary>
        /// Creates a <see cref="AccessDeleteObject{TArgument}" /> used to perform a delete operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument> Delete<TArgument>(string tableName, TArgument argumentValue, DeleteOptions options = DeleteOptions.None)
        where TArgument : class
        {
            return Delete<TArgument>(new AccessObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<OleDbCommand, OleDbParameter> DeleteByKey<T>(string tableName, T key, DeleteOptions options = DeleteOptions.None)
            where T : struct
        {
            return DeleteByKey<T>(new AccessObjectName(tableName), key, options);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<OleDbCommand, OleDbParameter> DeleteByKey(string tableName, string key, DeleteOptions options = DeleteOptions.None)
        {
            return DeleteByKey(new AccessObjectName(tableName), key, options);
        }

        /// <summary>
        /// Delete multiple rows by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="options">Update options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKeyList")]
        public MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> DeleteByKeyList<TKey>(string tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
        {
            return DeleteByKeyList<TKey>(new AccessObjectName(tableName), keys, options);
        }

        /// <summary>
        /// Delete multiple records using a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        public MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> DeleteWithFilter(string tableName, string whereClause)
        {
            return DeleteWithFilter(new AccessObjectName(tableName), whereClause);
        }

        /// <summary>
        /// Delete multiple records using a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value for the where clause.</param>
        public MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> DeleteWithFilter(string tableName, string whereClause, object argumentValue)
        {
            return DeleteWithFilter(new AccessObjectName(tableName), whereClause, argumentValue);
        }

        /// <summary>
        /// Delete multiple records using a filter object.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The options.</param>
        public MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> DeleteWithFilter(string tableName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            return DeleteWithFilter(new AccessObjectName(tableName), filterValue, filterOptions);
        }

        /// <summary>
        /// Creates a <see cref="AccessTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <returns></returns>
        public TableDbCommandBuilder<OleDbCommand, OleDbParameter, AccessLimitOption> From(string tableOrViewName)
        {
            return From(new AccessObjectName(tableOrViewName));
        }

        /// <summary>
        /// Creates a <see cref="AccessTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public TableDbCommandBuilder<OleDbCommand, OleDbParameter, AccessLimitOption> From(string tableOrViewName, string whereClause)
        {
            return From(new AccessObjectName(tableOrViewName), whereClause);
        }

        /// <summary>
        /// Creates a <see cref="AccessTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="whereClause"></param>
        /// <param name="argumentValue"></param>
        /// <returns></returns>
        public TableDbCommandBuilder<OleDbCommand, OleDbParameter, AccessLimitOption> From(string tableOrViewName, string whereClause, object argumentValue)
        {
            return From(new AccessObjectName(tableOrViewName), whereClause, argumentValue);
        }

        /// <summary>
        /// Creates a <see cref="AccessTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <returns>TableDbCommandBuilder&lt;OleDbCommand, OleDbParameter, AccessLimitOption&gt;.</returns>
        public TableDbCommandBuilder<OleDbCommand, OleDbParameter, AccessLimitOption> From(string tableOrViewName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            return From(new AccessObjectName(tableOrViewName), filterValue, filterOptions);
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<OleDbCommand, OleDbParameter> GetByKey<TKey>(string tableName, TKey key)
            where TKey : struct
        {
            return GetByKey<TKey>(new AccessObjectName(tableName), key);
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<OleDbCommand, OleDbParameter> GetByKey(string tableName, string key)
        {
            return GetByKey(new AccessObjectName(tableName), key);
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
        public MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> GetByKeyList<T>(string tableName, IEnumerable<T> keys)
        {
            return GetByKeyList<T>(new AccessObjectName(tableName), keys);
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
        public MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> GetByKeyList<T>(string tableName, string keyColumn, IEnumerable<T> keys)
        {
            return GetByKeyList<T>(new AccessObjectName(tableName), keyColumn, keys);
        }

        /// <summary>
        /// Creates a <see cref="AccessInsertObject{TArgument}" /> used to perform an insert operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument> Insert<TArgument>(string tableName, TArgument argumentValue, InsertOptions options = InsertOptions.None)
        where TArgument : class
        {
            return Insert<TArgument>(new AccessObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Creates a <see cref="AccessUpdateObject{TArgument}" /> used to perform an update operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument> Update<TArgument>(string tableName, TArgument argumentValue, UpdateOptions options = UpdateOptions.None)
        where TArgument : class
        {
            return Update<TArgument>(new AccessObjectName(tableName), argumentValue, options);
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
        /// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<OleDbCommand, OleDbParameter> UpdateByKey<TArgument, TKey>(string tableName, TArgument newValues, TKey key, UpdateOptions options = UpdateOptions.None)
            where TKey : struct
        {
            return UpdateByKey<TArgument, TKey>(new AccessObjectName(tableName), newValues, key, options);
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
        public SingleRowDbCommandBuilder<OleDbCommand, OleDbParameter> UpdateByKey<TArgument>(string tableName, TArgument newValues, string key, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateByKey<TArgument>(new AccessObjectName(tableName), newValues, key, options);
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
        /// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKeyList")]
        public MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> UpdateByKeyList<TArgument, TKey>(string tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateByKeyList<TArgument, TKey>(new AccessObjectName(tableName), newValues, keys, options);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="options">The update options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<OleDbCommand, OleDbParameter> UpdateSet(string tableName, string updateExpression, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateSet(new AccessObjectName(tableName), updateExpression, options);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="updateArgumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<OleDbCommand, OleDbParameter> UpdateSet(string tableName, string updateExpression, object updateArgumentValue, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateSet(new AccessObjectName(tableName), updateExpression, updateArgumentValue, options);
        }

        /// <summary>
        /// Update multiple records using an update value.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="options">The options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<OleDbCommand, OleDbParameter> UpdateSet(string tableName, object newValues, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateSet(new AccessObjectName(tableName), newValues, options);
        }
    }
}