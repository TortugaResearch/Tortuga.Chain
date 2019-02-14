using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.MySql
{
    //Methods in this file should always delegate the same method using MySqlObjectName instead of a string for the table name.

    partial class MySqlDataSourceBase
    {
        /// <summary>
        /// Delete the specified table name.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>ObjectDbCommandBuilder&lt;MySqlCommand, MySqlParameter, TArgument&gt;.</returns>
        public ObjectDbCommandBuilder<MySqlCommand, MySqlParameter, TArgument> Delete<TArgument>(string tableName, TArgument argumentValue, DeleteOptions options = DeleteOptions.None)
        where TArgument : class
        {
            return Delete<TArgument>(new MySqlObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<MySqlCommand, MySqlParameter> DeleteByKey<T>(string tableName, T key, DeleteOptions options = DeleteOptions.None)
            where T : struct
        {
            return DeleteByKey<T>(new MySqlObjectName(tableName), key, options);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<MySqlCommand, MySqlParameter> DeleteByKey(string tableName, string key, DeleteOptions options = DeleteOptions.None)
        {
            return DeleteByKey(new MySqlObjectName(tableName), key, options);
        }

        /// <summary>
        /// Delete multiple rows by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="options">Update options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKey")]
        public MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> DeleteByKeyList<TKey>(string tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
        {
            return DeleteByKeyList<TKey>(new MySqlObjectName(tableName), keys, options);
        }

        /// <summary>
        /// Delete multiple records using a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        public MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> DeleteWithFilter(string tableName, string whereClause)
        {
            return DeleteWithFilter(new MySqlObjectName(tableName), whereClause);
        }

        /// <summary>
        /// Delete multiple records using a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value for the where clause.</param>
        public MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> DeleteWithFilter(string tableName, string whereClause, object argumentValue)
        {
            return DeleteWithFilter(new MySqlObjectName(tableName), whereClause, argumentValue);
        }

        /// <summary>
        /// Delete multiple records using a filter object.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        public MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> DeleteWithFilter(string tableName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            return DeleteWithFilter(new MySqlObjectName(tableName), filterValue, filterOptions);
        }

        /// <summary>
        /// Froms the specified table or view name.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <returns>TableDbCommandBuilder&lt;MySqlCommand, MySqlParameter, MySqlLimitOption&gt;.</returns>
        public TableDbCommandBuilder<MySqlCommand, MySqlParameter, MySqlLimitOption> From(string tableOrViewName)
        {
            return From(new MySqlObjectName(tableOrViewName));
        }

        /// <summary>
        /// Froms the specified table or view name.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <returns>TableDbCommandBuilder&lt;MySqlCommand, MySqlParameter, MySqlLimitOption&gt;.</returns>
        public TableDbCommandBuilder<MySqlCommand, MySqlParameter, MySqlLimitOption> From(string tableOrViewName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            return From(new MySqlObjectName(tableOrViewName), filterValue, filterOptions);
        }

        /// <summary>
        /// Froms the specified table or view name.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <returns>TableDbCommandBuilder&lt;MySqlCommand, MySqlParameter, MySqlLimitOption&gt;.</returns>
        public TableDbCommandBuilder<MySqlCommand, MySqlParameter, MySqlLimitOption> From(string tableOrViewName, string whereClause)
        {
            return From(new MySqlObjectName(tableOrViewName), whereClause);
        }

        /// <summary>
        /// Froms the specified table or view name.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>TableDbCommandBuilder&lt;MySqlCommand, MySqlParameter, MySqlLimitOption&gt;.</returns>
        public TableDbCommandBuilder<MySqlCommand, MySqlParameter, MySqlLimitOption> From(string tableOrViewName, string whereClause, object argumentValue)
        {
            return From(new MySqlObjectName(tableOrViewName), whereClause, argumentValue);
        }

        /// <summary>
        /// Gets a record by its key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<MySqlCommand, MySqlParameter> GetByKey<TKey>(string tableName, TKey key)
            where TKey : struct
        {
            return GetByKey<TKey>(new MySqlObjectName(tableName), key);
        }

        /// <summary>
        /// Gets a record by its key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<MySqlCommand, MySqlParameter> GetByKey(string tableName, string key)
        {
            return GetByKey(new MySqlObjectName(tableName), key);
        }

        /// <summary>Gets a set of records by an unique key.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keyColumn">Name of the key column. This should be a primary or unique key, but that's not enforced.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        public MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> GetByKeyList<T>(string tableName, string keyColumn, IEnumerable<T> keys)
        {
            return GetByKeyList<T>(new MySqlObjectName(tableName), keyColumn, keys);
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
        public MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> GetByKeyList<T>(string tableName, IEnumerable<T> keys)
        {
            return GetByKeyList<T>(new MySqlObjectName(tableName), keys);
        }

        /// <summary>
        /// Inserts the specified table name.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>ObjectDbCommandBuilder&lt;MySqlCommand, MySqlParameter, TArgument&gt;.</returns>
        public ObjectDbCommandBuilder<MySqlCommand, MySqlParameter, TArgument> Insert<TArgument>(string tableName, TArgument argumentValue, InsertOptions options = InsertOptions.None)
        where TArgument : class
        {
            return Insert<TArgument>(new MySqlObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Executes the indicated procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        public MultipleTableDbCommandBuilder<MySqlCommand, MySqlParameter> Procedure(string procedureName)
        {
            return Procedure(new MySqlObjectName(procedureName));
        }

        /// <summary>
        /// Executes the indicated procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        public MultipleTableDbCommandBuilder<MySqlCommand, MySqlParameter> Procedure(string procedureName, object argumentValue)
        {
            return Procedure(new MySqlObjectName(procedureName), argumentValue);
        }

        /// <summary>
        /// This is used to query a scalar function.
        /// </summary>
        /// <param name="scalarFunctionName">Name of the scalar function.</param>
        /// <returns></returns>
        public ScalarDbCommandBuilder<MySqlCommand, MySqlParameter> ScalarFunction(string scalarFunctionName)
        {
            return ScalarFunction(new MySqlObjectName(scalarFunctionName));
        }

        /// <summary>
        /// This is used to query a scalar function.
        /// </summary>
        /// <param name="scalarFunctionName">Name of the scalar function.</param>
        /// <param name="functionArgumentValue">The function arguments.</param>
        /// <returns></returns>
        public ScalarDbCommandBuilder<MySqlCommand, MySqlParameter> ScalarFunction(string scalarFunctionName, object functionArgumentValue)
        {
            return ScalarFunction(new MySqlObjectName(scalarFunctionName), functionArgumentValue);
        }

        /// <summary>
        /// Update the specified table name.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>ObjectDbCommandBuilder&lt;MySqlCommand, MySqlParameter, TArgument&gt;.</returns>
        public ObjectDbCommandBuilder<MySqlCommand, MySqlParameter, TArgument> Update<TArgument>(string tableName, TArgument argumentValue, UpdateOptions options = UpdateOptions.None)
        where TArgument : class
        {
            return Update<TArgument>(new MySqlObjectName(tableName), argumentValue, options);
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
        /// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<MySqlCommand, MySqlParameter> UpdateByKey<TArgument, TKey>(string tableName, TArgument newValues, TKey key, UpdateOptions options = UpdateOptions.None)
            where TKey : struct
        {
            return UpdateByKey<TArgument, TKey>(new MySqlObjectName(tableName), newValues, key, options);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<MySqlCommand, MySqlParameter> UpdateByKey<TArgument>(string tableName, TArgument newValues, string key, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateByKey<TArgument>(new MySqlObjectName(tableName), newValues, key, options);
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
        /// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKeyList")]
        public MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> UpdateByKeyList<TArgument, TKey>(string tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateByKeyList<TArgument, TKey>(new MySqlObjectName(tableName), newValues, keys, options);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="options">The update options.</param>
        public UpdateManyCommandBuilder<MySqlCommand, MySqlParameter> UpdateSet(string tableName, string updateExpression, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateSet(new MySqlObjectName(tableName), updateExpression, options);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="updateArgumentValue">The update argument value.</param>
        /// <param name="options">The update options.</param>
        public UpdateManyCommandBuilder<MySqlCommand, MySqlParameter> UpdateSet(string tableName, string updateExpression, object updateArgumentValue, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateSet(new MySqlObjectName(tableName), updateExpression, updateArgumentValue, options);
        }

        /// <summary>
        /// Update multiple records using an update value.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="options">The options.</param>
        public UpdateManyCommandBuilder<MySqlCommand, MySqlParameter> UpdateSet(string tableName, object newValues, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateSet(new MySqlObjectName(tableName), newValues, options);
        }

        /// <summary>
        /// Upserts the specified table name.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>ObjectDbCommandBuilder&lt;MySqlCommand, MySqlParameter, TArgument&gt;.</returns>
        public ObjectDbCommandBuilder<MySqlCommand, MySqlParameter, TArgument> Upsert<TArgument>(string tableName, TArgument argumentValue, UpsertOptions options = UpsertOptions.None)
        where TArgument : class
        {
            return Upsert<TArgument>(new MySqlObjectName(tableName), argumentValue, options);
        }
    }
}