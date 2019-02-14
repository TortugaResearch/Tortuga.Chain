using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer
{
    //Methods in this file should always delegate the same method using SqlServerObjectName instead of a string for the table name.

    partial class SqlServerDataSourceBase
    {
        /// <summary>
        /// Inserts an object model from the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The delete options.</param>
        /// <returns>SqlServerInsert.</returns>
        public ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> Delete<TArgument>(string tableName, TArgument argumentValue, DeleteOptions options = DeleteOptions.None)
        where TArgument : class
        {
            return Delete<TArgument>(new SqlServerObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> DeleteByKey<T>(string tableName, T key, DeleteOptions options = DeleteOptions.None)
            where T : struct
        {
            return DeleteByKey<T>(new SqlServerObjectName(tableName), key, options);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> DeleteByKey(string tableName, string key, DeleteOptions options = DeleteOptions.None)
        {
            return DeleteByKey(new SqlServerObjectName(tableName), key, options);
        }

        /// <summary>
        /// Delete multiple rows by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="options">Update options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKeyList")]
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> DeleteByKeyList<TKey>(string tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
        {
            return DeleteByKeyList<TKey>(new SqlServerObjectName(tableName), keys, options);
        }

        /// <summary>
        /// Delete multiple records using a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> DeleteWithFilter(string tableName, string whereClause)
        {
            return DeleteWithFilter(new SqlServerObjectName(tableName), whereClause);
        }

        /// <summary>
        /// Delete multiple records using a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value for the where clause.</param>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> DeleteWithFilter(string tableName, string whereClause, object argumentValue)
        {
            return DeleteWithFilter(new SqlServerObjectName(tableName), whereClause, argumentValue);
        }

        /// <summary>
        /// Delete multiple records using a filter object.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The options.</param>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> DeleteWithFilter(string tableName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            return DeleteWithFilter(new SqlServerObjectName(tableName), filterValue, filterOptions);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> From(string tableOrViewName)
        {
            return From(new SqlServerObjectName(tableOrViewName));
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <returns>SqlServerTableOrView.</returns>
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> From(string tableOrViewName, string whereClause)
        {
            return From(new SqlServerObjectName(tableOrViewName), whereClause);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <param name="argumentValue">Optional argument value. Every property in the argument value must have a matching parameter in the WHERE clause</param>
        /// <returns>SqlServerTableOrView.</returns>
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> From(string tableOrViewName, string whereClause, object argumentValue)
        {
            return From(new SqlServerObjectName(tableOrViewName), whereClause, argumentValue);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value is used to generate a simple AND style WHERE clause.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <returns>SqlServerTableOrView.</returns>
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> From(string tableOrViewName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            return From(new SqlServerObjectName(tableOrViewName), filterValue, filterOptions);
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> GetByKey<TKey>(string tableName, TKey key)
            where TKey : struct
        {
            return GetByKey<TKey>(new SqlServerObjectName(tableName), key);
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> GetByKey(string tableName, string key)
        {
            return GetByKey(new SqlServerObjectName(tableName), key);
        }

        /// <summary>Gets a set of records by an unique key.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keyColumn">Name of the key column. This should be a primary or unique key, but that's not enforced.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> GetByKeyList<T>(string tableName, string keyColumn, IEnumerable<T> keys)
        {
            return GetByKeyList<T>(new SqlServerObjectName(tableName), keyColumn, keys);
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
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> GetByKeyList<T>(string tableName, IEnumerable<T> keys)
        {
            return GetByKeyList<T>(new SqlServerObjectName(tableName), keys);
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
        public ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> Insert<TArgument>(string tableName, TArgument argumentValue, InsertOptions options = InsertOptions.None)
        where TArgument : class
        {
            return Insert<TArgument>(new SqlServerObjectName(tableName), argumentValue, options);
        }

        /// <summary>
        /// Inserts the batch of records as one operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="dataTable">The data table.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch(string tableName, DataTable dataTable, string tableTypeName, InsertOptions options = InsertOptions.None)
        {
            return InsertBatch(new SqlServerObjectName(tableName), dataTable, new SqlServerObjectName(tableTypeName), options);
        }

        /// <summary>
        /// Inserts the batch of records as one operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch(string tableName, DbDataReader dataReader, string tableTypeName, InsertOptions options = InsertOptions.None)
        {
            return InsertBatch(new SqlServerObjectName(tableName), dataReader, new SqlServerObjectName(tableTypeName), options);
        }

        /// <summary>
        /// Inserts the batch of records as one operation..
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="objects">The objects.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch<TObject>(string tableName, IEnumerable<TObject> objects, string tableTypeName, InsertOptions options = InsertOptions.None)
        {
            return InsertBatch<TObject>(new SqlServerObjectName(tableName), objects, new SqlServerObjectName(tableTypeName), options);
        }

        /// <summary>
        /// Inserts the batch of records as one operation..
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="objects">The objects.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.
        /// </returns>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch<TObject>(IEnumerable<TObject> objects, string tableTypeName, InsertOptions options = InsertOptions.None) where TObject : class
        {
            return InsertBatch<TObject>(objects, new SqlServerObjectName(tableTypeName), options);
        }

        /// <summary>
        /// Inserts the batch of records using bulk insert.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="dataTable">The data table.</param>
        /// <param name="options">The options.</param>
        /// <returns>SqlServerInsertBulk.</returns>
        public SqlServerInsertBulk InsertBulk(string tableName, DataTable dataTable, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
        {
            return InsertBulk(new SqlServerObjectName(tableName), dataTable, options);
        }

        /// <summary>
        /// Inserts the batch of records using bulk insert.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="options">The options.</param>
        /// <returns>SqlServerInsertBulk.</returns>
        public SqlServerInsertBulk InsertBulk(string tableName, IDataReader dataReader, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
        {
            return InsertBulk(new SqlServerObjectName(tableName), dataReader, options);
        }

        /// <summary>
        /// Inserts the batch of records using bulk insert.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="objects">The objects.</param>
        /// <param name="options">The options.</param>
        /// <returns>SqlServerInsertBulk.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public SqlServerInsertBulk InsertBulk<TObject>(string tableName, IEnumerable<TObject> objects, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default) where TObject : class
        {
            return InsertBulk<TObject>(new SqlServerObjectName(tableName), objects, options);
        }

        /// <summary>
        /// Loads a procedure definition
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        public MultipleTableDbCommandBuilder<SqlCommand, SqlParameter> Procedure(string procedureName)
        {
            return Procedure(new SqlServerObjectName(procedureName));
        }

        /// <summary>
        /// Loads a procedure definition and populates it using the parameter object.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        /// <remarks>
        /// The procedure's definition is loaded from the database and used to determine which properties on the parameter object to use.
        /// </remarks>
        public MultipleTableDbCommandBuilder<SqlCommand, SqlParameter> Procedure(string procedureName, object argumentValue)
        {
            return Procedure(new SqlServerObjectName(procedureName), argumentValue);
        }

        /// <summary>
        /// This is used to query a scalar function.
        /// </summary>
        /// <param name="scalarFunctionName">Name of the scalar function.</param>
        /// <returns></returns>
        public ScalarDbCommandBuilder<SqlCommand, SqlParameter> ScalarFunction(string scalarFunctionName)
        {
            return ScalarFunction(new SqlServerObjectName(scalarFunctionName));
        }

        /// <summary>
        /// This is used to query a scalar function.
        /// </summary>
        /// <param name="scalarFunctionName">Name of the scalar function.</param>
        /// <param name="functionArgumentValue">The function argument.</param>
        /// <returns></returns>
        public ScalarDbCommandBuilder<SqlCommand, SqlParameter> ScalarFunction(string scalarFunctionName, object functionArgumentValue)
        {
            return ScalarFunction(new SqlServerObjectName(scalarFunctionName), functionArgumentValue);
        }

        /// <summary>
        /// This is used to query a table valued function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> TableFunction(string tableFunctionName)
        {
            return TableFunction(new SqlServerObjectName(tableFunctionName));
        }

        /// <summary>
        /// This is used to query a table valued function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <param name="functionArgumentValue">The function argument.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> TableFunction(string tableFunctionName, object functionArgumentValue)
        {
            return TableFunction(new SqlServerObjectName(tableFunctionName), functionArgumentValue);
        }

        /// <summary>
        /// Update an object in the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <returns>SqlServerInsert.</returns>
        public ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> Update<TArgument>(string tableName, TArgument argumentValue, UpdateOptions options = UpdateOptions.None)
        where TArgument : class
        {
            return Update<TArgument>(new SqlServerObjectName(tableName), argumentValue, options);
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
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> UpdateByKey<TArgument, TKey>(string tableName, TArgument newValues, TKey key, UpdateOptions options = UpdateOptions.None)
            where TKey : struct
        {
            return UpdateByKey<TArgument, TKey>(new SqlServerObjectName(tableName), newValues, key, options);
        }

        /// <summary>
        /// Update a record by its primary key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> UpdateByKey<TArgument>(string tableName, TArgument newValues, string key, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateByKey<TArgument>(new SqlServerObjectName(tableName), newValues, key, options);
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
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKeyList")]
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> UpdateByKeyList<TArgument, TKey>(string tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateByKeyList<TArgument, TKey>(new SqlServerObjectName(tableName), newValues, keys, options);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="options">The update options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<SqlCommand, SqlParameter> UpdateSet(string tableName, string updateExpression, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateSet(new SqlServerObjectName(tableName), updateExpression, options);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="updateArgumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<SqlCommand, SqlParameter> UpdateSet(string tableName, string updateExpression, object updateArgumentValue, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateSet(new SqlServerObjectName(tableName), updateExpression, updateArgumentValue, options);
        }

        /// <summary>
        /// Update multiple records using an update value.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="options">The options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<SqlCommand, SqlParameter> UpdateSet(string tableName, object newValues, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateSet(new SqlServerObjectName(tableName), newValues, options);
        }

        /// <summary>
        /// Perform an insert or update operation as appropriate.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert/update occurs.</param>
        /// <returns>SqlServerUpdate.</returns>
        public ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> Upsert<TArgument>(string tableName, TArgument argumentValue, UpsertOptions options = UpsertOptions.None)
        where TArgument : class
        {
            return Upsert<TArgument>(new SqlServerObjectName(tableName), argumentValue, options);
        }
    }
}