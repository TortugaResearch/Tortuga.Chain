using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.Oracle.CommandBuilders;

namespace Tortuga.Chain.Oracle
{
    /// <summary>
    /// Class OracleDataSourceBase.
    /// </summary>
    public abstract partial class OracleDataSourceBase : DataSource<OracleConnection, OracleTransaction, OracleCommand, OracleParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDataSourceBase"/> class.
        /// </summary>
        /// <param name="settings">Optional settings object.</param>
        protected OracleDataSourceBase(DataSourceSettings settings) : base(settings)
        {
        }

        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        public abstract new OracleMetadataCache DatabaseMetadata { get; }


        /// <summary>
        /// Deletes the specified table name.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>ObjectDbCommandBuilder&lt;OracleCommand, OracleParameter, TArgument&gt;.</returns>
        public ObjectDbCommandBuilder<OracleCommand, OracleParameter, TArgument> Delete<TArgument>(OracleObjectName tableName, TArgument argumentValue, DeleteOptions options = DeleteOptions.None)
        where TArgument : class
        {
            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new OracleDeleteObject<TArgument>(this, tableName, argumentValue, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new OracleUpdateObject<TArgument>(this, tableName, argumentValue, effectiveOptions);
        }

        /// <summary>
        /// Deletes an object model from the table indicated by the class's Table attribute.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The delete options.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<OracleCommand, OracleParameter, TArgument> Delete<TArgument>(TArgument argumentValue, DeleteOptions options = DeleteOptions.None) where TArgument : class
        {
            var table = DatabaseMetadata.GetTableOrViewFromClass<TArgument>();

            if (!AuditRules.UseSoftDelete(table))
                return new OracleDeleteObject<TArgument>(this, table.Name, argumentValue, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new OracleUpdateObject<TArgument>(this, table.Name, argumentValue, effectiveOptions);
        }

        /// <summary>
        /// Froms the specified table or view name.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <returns>TableDbCommandBuilder&lt;OracleCommand, OracleParameter, OracleLimitOption&gt;.</returns>
        public TableDbCommandBuilder<OracleCommand, OracleParameter, OracleLimitOption> From(OracleObjectName tableOrViewName)
        {
            return new OracleTableOrView(this, tableOrViewName);
        }

        /// <summary>
        /// Froms the specified table or view name.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <returns>TableDbCommandBuilder&lt;OracleCommand, OracleParameter, OracleLimitOption&gt;.</returns>
        public TableDbCommandBuilder<OracleCommand, OracleParameter, OracleLimitOption> From(OracleObjectName tableOrViewName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            return new OracleTableOrView(this, tableOrViewName, filterValue, filterOptions);
        }

        /// <summary>
        /// Froms the specified table or view name.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <returns>TableDbCommandBuilder&lt;OracleCommand, OracleParameter, OracleLimitOption&gt;.</returns>
        public TableDbCommandBuilder<OracleCommand, OracleParameter, OracleLimitOption> From(OracleObjectName tableOrViewName, string whereClause)
        {
            return new OracleTableOrView(this, tableOrViewName, whereClause);
        }

        /// <summary>
        /// Froms the specified table or view name.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>TableDbCommandBuilder&lt;OracleCommand, OracleParameter, OracleLimitOption&gt;.</returns>
        public TableDbCommandBuilder<OracleCommand, OracleParameter, OracleLimitOption> From(OracleObjectName tableOrViewName, string whereClause, object argumentValue)
        {
            return new OracleTableOrView(this, tableOrViewName, whereClause, argumentValue);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TableDbCommandBuilder<OracleCommand, OracleParameter, OracleLimitOption> From<TObject>() where TObject : class
        {
            return From(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TableDbCommandBuilder<OracleCommand, OracleParameter, OracleLimitOption> From<TObject>(string whereClause) where TObject : class
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TableDbCommandBuilder<OracleCommand, OracleParameter, OracleLimitOption> From<TObject>(string whereClause, object argumentValue) where TObject : class
        {
            return From(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, whereClause, argumentValue);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="filterValue">The filter value is used to generate a simple AND style WHERE clause.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TableDbCommandBuilder<OracleCommand, OracleParameter, OracleLimitOption> From<TObject>(object filterValue) where TObject : class
        {
            return From(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, filterValue);
        }

        /// <summary>
        /// Gets a set of records by their primary key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public MultipleRowDbCommandBuilder<OracleCommand, OracleParameter> GetByKey<TKey>(OracleObjectName tableName, params TKey[] keys)
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
        public MultipleRowDbCommandBuilder<OracleCommand, OracleParameter> GetByKey(OracleObjectName tableName, params string[] keys)
        {
            return GetByKeyList(tableName, keys);
        }

        /// <summary>
        /// Gets a record by its key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;OracleCommand, OracleParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<OracleCommand, OracleParameter> GetByKey<TKey>(OracleObjectName tableName, TKey key)
            where TKey : struct
        {
            return GetByKeyList(tableName, new List<TKey> { key });
        }

        /// <summary>
        /// Gets a record by its key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;OracleCommand, OracleParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<OracleCommand, OracleParameter> GetByKey(OracleObjectName tableName, string key)
        {
            return GetByKeyList(tableName, new List<string> { key });
        }

        /// <summary>
        /// Gets the by key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;OracleCommand, OracleParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        public MultipleRowDbCommandBuilder<OracleCommand, OracleParameter> GetByKeyList<T>(OracleObjectName tableName, IEnumerable<T> keys)
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

            var parameters = new List<OracleParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new OracleParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.OracleDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new OracleTableOrView(this, tableName, where, parameters);
        }



        /// <summary>
        /// Inserts the specified table name.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>ObjectDbCommandBuilder&lt;OracleCommand, OracleParameter, TArgument&gt;.</returns>
        public ObjectDbCommandBuilder<OracleCommand, OracleParameter, TArgument> Insert<TArgument>(OracleObjectName tableName, TArgument argumentValue, InsertOptions options = InsertOptions.None)
        where TArgument : class
        {
            return new OracleInsertObject<TArgument>(this, tableName, argumentValue, options);
        }
        /// <summary>
        /// Inserts an object into the specified table.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert occurs.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<OracleCommand, OracleParameter, TArgument> Insert<TArgument>(TArgument argumentValue, InsertOptions options = InsertOptions.None) where TArgument : class
        {
            return Insert(DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
        }

        /// <summary>
        /// SQLs the specified SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <returns>MultipleTableDbCommandBuilder&lt;OracleCommand, OracleParameter&gt;.</returns>
        public MultipleTableDbCommandBuilder<OracleCommand, OracleParameter> Sql(string sqlStatement)
        {
            return new OracleSqlCall(this, sqlStatement, null);
        }

        /// <summary>
        /// SQLs the specified SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>MultipleTableDbCommandBuilder&lt;OracleCommand, OracleParameter&gt;.</returns>
        public MultipleTableDbCommandBuilder<OracleCommand, OracleParameter> Sql(string sqlStatement, object argumentValue)
        {
            return new OracleSqlCall(this, sqlStatement, argumentValue);
        }

        /// <summary>
        /// Updates the specified table name.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>ObjectDbCommandBuilder&lt;OracleCommand, OracleParameter, TArgument&gt;.</returns>
        public ObjectDbCommandBuilder<OracleCommand, OracleParameter, TArgument> Update<TArgument>(OracleObjectName tableName, TArgument argumentValue, UpdateOptions options = UpdateOptions.None)
        where TArgument : class
        {
            return new OracleUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }
        /// <summary>
        /// Updates an object in the specified table.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<OracleCommand, OracleParameter, TArgument> Update<TArgument>(TArgument argumentValue, UpdateOptions options = UpdateOptions.None) where TArgument : class
        {
            return Update(DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
        }

        /// <summary>
        /// Upserts the specified table name.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>ObjectDbCommandBuilder&lt;OracleCommand, OracleParameter, TArgument&gt;.</returns>
        public ObjectDbCommandBuilder<OracleCommand, OracleParameter, TArgument> Upsert<TArgument>(OracleObjectName tableName, TArgument argumentValue, UpsertOptions options = UpsertOptions.None)
        where TArgument : class
        {
            return new OracleInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }
        /// <summary>
        /// Performs an insert or update operation as appropriate.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert/update occurs.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<OracleCommand, OracleParameter, TArgument> Upsert<TArgument>(TArgument argumentValue, UpsertOptions options = UpsertOptions.None) where TArgument : class
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

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;OracleCommand, OracleParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<OracleCommand, OracleParameter> UpdateByKey<TArgument, TKey>(OracleObjectName tableName, TArgument newValues, TKey key, UpdateOptions options = UpdateOptions.None)
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
        /// <returns>MultipleRowDbCommandBuilder&lt;OracleCommand, OracleParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<OracleCommand, OracleParameter> UpdateByKey<TArgument>(OracleObjectName tableName, TArgument newValues, string key, UpdateOptions options = UpdateOptions.None)
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
        public MultipleRowDbCommandBuilder<OracleCommand, OracleParameter> UpdateByKey<TArgument, TKey>(OracleObjectName tableName, TArgument newValues, params TKey[] keys)
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
        public MultipleRowDbCommandBuilder<OracleCommand, OracleParameter> UpdateByKey<TArgument>(OracleObjectName tableName, TArgument newValues, params string[] keys)
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
        /// <returns>MultipleRowDbCommandBuilder&lt;OracleCommand, OracleParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKey")]
        public MultipleRowDbCommandBuilder<OracleCommand, OracleParameter> UpdateByKeyList<TArgument, TKey>(OracleObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
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

            var parameters = new List<OracleParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new OracleParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.OracleDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new OracleUpdateMany(this, tableName, newValues, where, parameters, parameters.Count, options);
        }



        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;OracleCommand, OracleParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<OracleCommand, OracleParameter> DeleteByKey<T>(OracleObjectName tableName, T key, DeleteOptions options = DeleteOptions.None)
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
        /// <returns>MultipleRowDbCommandBuilder&lt;OracleCommand, OracleParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<OracleCommand, OracleParameter> DeleteByKey(OracleObjectName tableName, string key, DeleteOptions options = DeleteOptions.None)
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
        public MultipleRowDbCommandBuilder<OracleCommand, OracleParameter> DeleteByKey<T>(OracleObjectName tableName, params T[] keys)
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
        public MultipleRowDbCommandBuilder<OracleCommand, OracleParameter> DeleteByKey(OracleObjectName tableName, params string[] keys)
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
        /// <returns>MultipleRowDbCommandBuilder&lt;OracleCommand, OracleParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKey")]
        public MultipleRowDbCommandBuilder<OracleCommand, OracleParameter> DeleteByKeyList<TKey>(OracleObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
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

            var parameters = new List<OracleParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new OracleParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.OracleDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new OracleDeleteMany(this, tableName, where, parameters, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new OracleUpdateMany(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);

        }


        /// <summary>
        /// This is used to query a table valued function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<OracleCommand, OracleParameter, OracleLimitOption> TableFunction(OracleObjectName tableFunctionName)
        {
            return new OracleTableFunction(this, tableFunctionName, null);
        }

        /// <summary>
        /// This is used to query a table valued function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <param name="functionArgumentValue">The function argument.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<OracleCommand, OracleParameter, OracleLimitOption> TableFunction(OracleObjectName tableFunctionName, object functionArgumentValue)
        {
            return new OracleTableFunction(this, tableFunctionName, functionArgumentValue);
        }

        /// <summary>
        /// Executes the indicated procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        public MultipleTableDbCommandBuilder<OracleCommand, OracleParameter> Procedure(string procedureName)
        {
            return new OracleProcedureCall(this, procedureName);
        }

        /// <summary>
        /// Executes the indicated procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        public MultipleTableDbCommandBuilder<OracleCommand, OracleParameter> Procedure(string procedureName, object argumentValue)
        {
            return new OracleProcedureCall(this, procedureName, argumentValue);
        }




    }
}
