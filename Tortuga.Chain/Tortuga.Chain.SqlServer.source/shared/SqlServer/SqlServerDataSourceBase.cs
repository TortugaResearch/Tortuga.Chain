using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tortuga.Anchor;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerDataSourceBase.
    /// </summary>
    public abstract partial class SqlServerDataSourceBase : DataSource<SqlConnection, SqlTransaction, SqlCommand, SqlParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDataSourceBase"/> class.
        /// </summary>
        /// <param name="settings">Optional settings value.</param>
        protected SqlServerDataSourceBase(SqlServerDataSourceSettings settings) : base(settings)
        {

        }

        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        /// <value>The database metadata.</value>
        public abstract new SqlServerMetadataCache DatabaseMetadata { get; }

        /// <summary>
        /// Inserts an object model from the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The delete options.</param>
        /// <returns>SqlServerInsert.</returns>
        public ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> Delete<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, DeleteOptions options = DeleteOptions.None)
        where TArgument : class
        {
            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new SqlServerDeleteObject<TArgument>(this, tableName, argumentValue, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new SqlServerUpdateObject<TArgument>(this, tableName, argumentValue, effectiveOptions);
        }

        /// <summary>
        /// Deletes an object model from the table indicated by the class's Table attribute.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The delete options.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> Delete<TArgument>(TArgument argumentValue, DeleteOptions options = DeleteOptions.None) where TArgument : class
        {
            var table = DatabaseMetadata.GetTableOrViewFromClass<TArgument>();

            if (!AuditRules.UseSoftDelete(table))
                return new SqlServerDeleteObject<TArgument>(this, table.Name, argumentValue, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new SqlServerUpdateObject<TArgument>(this, table.Name, argumentValue, effectiveOptions);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> DeleteByKey<T>(SqlServerObjectName tableName, T key, DeleteOptions options = DeleteOptions.None)
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
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> DeleteByKey(SqlServerObjectName tableName, string key, DeleteOptions options = DeleteOptions.None)
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
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> DeleteByKey<T>(SqlServerObjectName tableName, params T[] keys)
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
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> DeleteByKey(SqlServerObjectName tableName, params string[] keys)
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
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKey")]
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> DeleteByKeyList<TKey>(SqlServerObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
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

            var parameters = new List<SqlParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new SqlParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.SqlDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new SqlServerDeleteMany(this, tableName, where, parameters, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new SqlServerUpdateMany(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);

        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> From(SqlServerObjectName tableOrViewName)
        {
            return new SqlServerTableOrView(this, tableOrViewName, null, null);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <returns>SqlServerTableOrView.</returns>
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> From(SqlServerObjectName tableOrViewName, string whereClause)
        {
            return new SqlServerTableOrView(this, tableOrViewName, whereClause, null);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <param name="argumentValue">Optional argument value. Every property in the argument value must have a matching parameter in the WHERE clause</param>
        /// <returns>SqlServerTableOrView.</returns>
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> From(SqlServerObjectName tableOrViewName, string whereClause, object argumentValue)
        {
            return new SqlServerTableOrView(this, tableOrViewName, whereClause, argumentValue);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value is used to generate a simple AND style WHERE clause.</param>
        /// <returns>SqlServerTableOrView.</returns>
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> From(SqlServerObjectName tableOrViewName, object filterValue)
        {
            return new SqlServerTableOrView(this, tableOrViewName, filterValue);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> From<TObject>() where TObject : class
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
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> From<TObject>(string whereClause) where TObject : class
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
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> From<TObject>(string whereClause, object argumentValue) where TObject : class
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
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> From<TObject>(object filterValue) where TObject : class
        {
            return From(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, filterValue);
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> GetByKey<TKey>(SqlServerObjectName tableName, TKey key)
            where TKey : struct
        {
            return GetByKeyList(tableName, new List<TKey> { key });
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> GetByKey(SqlServerObjectName tableName, string key)
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
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> GetByKey<TKey>(SqlServerObjectName tableName, params TKey[] keys)
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
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> GetByKey(SqlServerObjectName tableName, params string[] keys)
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
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> GetByKeyList<T>(SqlServerObjectName tableName, IEnumerable<T> keys)
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

            var parameters = new List<SqlParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new SqlParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.SqlDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new SqlServerTableOrView(this, tableName, where, parameters);
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
        public ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> Insert<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, InsertOptions options = InsertOptions.None)
        where TArgument : class
        {
            return new SqlServerInsertObject<TArgument>(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Inserts an object into the specified table.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert occurs.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> Insert<TArgument>(TArgument argumentValue, InsertOptions options = InsertOptions.None) where TArgument : class
        {
            return Insert(DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
        }

        /// <summary>
        /// Inserts the batch of records as one operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="dataTable">The data table.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch(SqlServerObjectName tableName, SqlServerObjectName tableTypeName, DataTable dataTable, InsertOptions options = InsertOptions.None)
        {
            return new SqlServerInsertBatch(this, tableName, tableTypeName, dataTable, options);
        }

        /// <summary>
        /// Inserts the batch of records as one operation.
        /// </summary>
        /// <typeparam name="TObject">The type of the t object.</typeparam>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="dataTable">The data table.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch<TObject>(SqlServerObjectName tableTypeName, DataTable dataTable, InsertOptions options = InsertOptions.None) where TObject : class
        {
            return InsertBatch(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, tableTypeName, dataTable, options);
        }

        /// <summary>
        /// Inserts the batch of records as one operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch(SqlServerObjectName tableName, SqlServerObjectName tableTypeName, DbDataReader dataReader, InsertOptions options = InsertOptions.None)
        {
            return new SqlServerInsertBatch(this, tableName, tableTypeName, dataReader, options);
        }

        /// <summary>
        /// Inserts the batch of records as one operation.
        /// </summary>
        /// <typeparam name="TObject">The type of the t object.</typeparam>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch<TObject>(SqlServerObjectName tableTypeName, DbDataReader dataReader, InsertOptions options = InsertOptions.None) where TObject : class
        {
            return InsertBatch(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, tableTypeName, dataReader, options);
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
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch<TObject>(SqlServerObjectName tableName, SqlServerObjectName tableTypeName, IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None)
        {
            var tableType = DatabaseMetadata.GetUserDefinedType(tableTypeName);
            return new SqlServerInsertBatch(this, tableName, tableTypeName, new ObjectDataReader<TObject>(tableType, objects), options);
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
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch<TObject>(SqlServerObjectName tableTypeName, IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None) where TObject : class
        {
            return InsertBatch(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, tableTypeName, objects, options);
        }

        /// <summary>
        /// Inserts the batch of records using bulk insert.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="dataTable">The data table.</param>
        /// <param name="options">The options.</param>
        /// <returns>SqlServerInsertBulk.</returns>
        public SqlServerInsertBulk InsertBulk(SqlServerObjectName tableName, DataTable dataTable, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
        {
            return new SqlServerInsertBulk(this, tableName, dataTable, options);
        }

        /// <summary>
        /// Inserts the batch of records using bulk insert.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="options">The options.</param>
        /// <returns>SqlServerInsertBulk.</returns>
        public SqlServerInsertBulk InsertBulk(SqlServerObjectName tableName, IDataReader dataReader, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
        {
            return new SqlServerInsertBulk(this, tableName, dataReader, options);
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
        public SqlServerInsertBulk InsertBulk<TObject>(SqlServerObjectName tableName, IEnumerable<TObject> objects, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default) where TObject : class
        {
            var tableType = DatabaseMetadata.GetTableOrView(tableName);
            return new SqlServerInsertBulk(this, tableName, new ObjectDataReader<TObject>(tableType, objects, OperationTypes.Insert), options);
        }

        /// <summary>
        /// Inserts the batch of records using bulk insert.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="dataTable">The data table.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// SqlServerInsertBulk.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public SqlServerInsertBulk InsertBulk<TObject>(DataTable dataTable, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default) where TObject : class
        {
            return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, dataTable, options);
        }

        /// <summary>
        /// Inserts the batch of records using bulk insert.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// SqlServerInsertBulk.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public SqlServerInsertBulk InsertBulk<TObject>(IDataReader dataReader, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default) where TObject : class
        {
            return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, dataReader, options);
        }

        /// <summary>
        /// Inserts the batch of records using bulk insert.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="objects">The objects.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// SqlServerInsertBulk.
        /// </returns>
        public SqlServerInsertBulk InsertBulk<TObject>(IEnumerable<TObject> objects, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default) where TObject : class
        {
            return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, objects, options);
        }

        /// <summary>
        /// Loads a procedure definition
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        public MultipleTableDbCommandBuilder<SqlCommand, SqlParameter> Procedure(SqlServerObjectName procedureName)
        {
            return new SqlServerProcedureCall(this, procedureName, null);
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
        public MultipleTableDbCommandBuilder<SqlCommand, SqlParameter> Procedure(SqlServerObjectName procedureName, object argumentValue)
        {
            return new SqlServerProcedureCall(this, procedureName, argumentValue);
        }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <returns></returns>
        public MultipleTableDbCommandBuilder<SqlCommand, SqlParameter> Sql(string sqlStatement)
        {
            return new SqlServerSqlCall(this, sqlStatement, null);
        }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>SqlServerSqlCall.</returns>
        public MultipleTableDbCommandBuilder<SqlCommand, SqlParameter> Sql(string sqlStatement, object argumentValue)
        {
            return new SqlServerSqlCall(this, sqlStatement, argumentValue);
        }

        /// <summary>
        /// This is used to query a table valued function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> TableFunction(SqlServerObjectName tableFunctionName)
        {
            return new SqlServerTableFunction(this, tableFunctionName, null);
        }

        /// <summary>
        /// This is used to query a table valued function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <param name="functionArgumentValue">The function argument.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> TableFunction(SqlServerObjectName tableFunctionName, object functionArgumentValue)
        {
            return new SqlServerTableFunction(this, tableFunctionName, functionArgumentValue);
        }
        /// <summary>
        /// Updates an object in the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <returns>SqlServerInsert.</returns>
        public ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> Update<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, UpdateOptions options = UpdateOptions.None)
        where TArgument : class
        {
            return new SqlServerUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Updates an object in the specified table.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> Update<TArgument>(TArgument argumentValue, UpdateOptions options = UpdateOptions.None) where TArgument : class
        {
            return Update(DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
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
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> UpdateByKey<TArgument, TKey>(SqlServerObjectName tableName, TArgument newValues, TKey key, UpdateOptions options = UpdateOptions.None)
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
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> UpdateByKey<TArgument>(SqlServerObjectName tableName, TArgument newValues, string key, UpdateOptions options = UpdateOptions.None)
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
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> UpdateByKey<TArgument, TKey>(SqlServerObjectName tableName, TArgument newValues, params TKey[] keys)
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
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> UpdateByKey<TArgument>(SqlServerObjectName tableName, TArgument newValues, params string[] keys)
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
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKey")]
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> UpdateByKeyList<TArgument, TKey>(SqlServerObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
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

            var parameters = new List<SqlParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new SqlParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.SqlDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new SqlServerUpdateMany(this, tableName, newValues, where, parameters, parameters.Count, options);
        }
        /// <summary>
        /// Performs an insert or update operation as appropriate.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert/update occurs.</param>
        /// <returns>SqlServerUpdate.</returns>
        public ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> Upsert<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, UpsertOptions options = UpsertOptions.None)
        where TArgument : class
        {
            return new SqlServerInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }
        /// <summary>
        /// Performs an insert or update operation as appropriate.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert/update occurs.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> Upsert<TArgument>(TArgument argumentValue, UpsertOptions options = UpsertOptions.None) where TArgument : class
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

    }
}

