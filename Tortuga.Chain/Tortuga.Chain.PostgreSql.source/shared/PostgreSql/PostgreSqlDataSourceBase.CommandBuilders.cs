using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.PostgreSql.CommandBuilders;

namespace Tortuga.Chain.PostgreSql
{
    partial class PostgreSqlDataSourceBase
    {
        /// <summary>
        /// Delete the specified table name.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>ObjectDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, TArgument&gt;.</returns>
        public ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> Delete<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, DeleteOptions options = DeleteOptions.None)
        where TArgument : class
        {
            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new PostgreSqlDeleteObject<TArgument>(this, tableName, argumentValue, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new PostgreSqlUpdateObject<TArgument>(this, tableName, argumentValue, effectiveOptions);
        }

        /// <summary>
        /// Delete an object model from the table indicated by the class's Table attribute.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The delete options.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> Delete<TArgument>(TArgument argumentValue, DeleteOptions options = DeleteOptions.None) where TArgument : class
        {
            var table = DatabaseMetadata.GetTableOrViewFromClass<TArgument>();

            if (!AuditRules.UseSoftDelete(table))
                return new PostgreSqlDeleteObject<TArgument>(this, table.Name, argumentValue, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new PostgreSqlUpdateObject<TArgument>(this, table.Name, argumentValue, effectiveOptions);
        }

        /// <summary>
        /// Delete a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> DeleteByKey<T>(PostgreSqlObjectName tableName, T key, DeleteOptions options = DeleteOptions.None)
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
        /// <returns>MultipleRowDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> DeleteByKey(PostgreSqlObjectName tableName, string key, DeleteOptions options = DeleteOptions.None)
        {
            return DeleteByKeyList(tableName, new List<string> { key }, options);
        }

        /// <summary>
        /// Delete multiple rows by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="options">Update options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKeyList")]
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> DeleteByKeyList<TKey>(PostgreSqlObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.IsPrimaryKey).ToList();
            if (primaryKeys.Count != 1)
                throw new MappingException($"{nameof(DeleteByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key.");

            var keyList = keys.AsList();
            var columnMetadata = primaryKeys.Single();
            string where;
            if (keys.Count() > 1)
                where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
            else
                where = columnMetadata.SqlName + " = @Param0";

            var parameters = new List<NpgsqlParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new NpgsqlParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.NpgsqlDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new PostgreSqlDeleteMany(this, tableName, where, parameters, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new PostgreSqlUpdateMany(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);
        }

        /// <summary>
        /// Delete multiple records using a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> DeleteWithFilter(PostgreSqlObjectName tableName, string whereClause)
        {
            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new PostgreSqlDeleteMany(this, tableName, whereClause, null);

            return new PostgreSqlUpdateMany(this, tableName, null, UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected).WithFilter(whereClause, null);
        }

        /// <summary>
        /// Delete multiple records using a where expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value for the where clause.</param>
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> DeleteWithFilter(PostgreSqlObjectName tableName, string whereClause, object argumentValue)
        {
            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new PostgreSqlDeleteMany(this, tableName, whereClause, argumentValue);

            return new PostgreSqlUpdateMany(this, tableName, null, UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected).WithFilter(whereClause, argumentValue);
        }

        /// <summary>
        /// Delete multiple records using a filter object.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The options.</param>
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> DeleteWithFilter(PostgreSqlObjectName tableName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new PostgreSqlDeleteMany(this, tableName, filterValue, filterOptions);

            return new PostgreSqlUpdateMany(this, tableName, null, UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected).WithFilter(filterValue, filterOptions);
        }

        /// <summary>
        /// Froms the specified table or view name.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> From(PostgreSqlObjectName tableOrViewName)
        {
            return new PostgreSqlTableOrView(this, tableOrViewName);
        }

        /// <summary>
        /// Froms the specified table or view name.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> From(PostgreSqlObjectName tableOrViewName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            return new PostgreSqlTableOrView(this, tableOrViewName, filterValue, filterOptions);
        }

        /// <summary>
        /// Froms the specified table or view name.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> From(PostgreSqlObjectName tableOrViewName, string whereClause)
        {
            return new PostgreSqlTableOrView(this, tableOrViewName, whereClause);
        }

        /// <summary>
        /// Froms the specified table or view name.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> From(PostgreSqlObjectName tableOrViewName, string whereClause, object argumentValue)
        {
            return new PostgreSqlTableOrView(this, tableOrViewName, whereClause, argumentValue);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> From<TObject>() where TObject : class
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
        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> From<TObject>(string whereClause) where TObject : class
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
        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> From<TObject>(string whereClause, object argumentValue) where TObject : class
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
        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> From<TObject>(object filterValue) where TObject : class
        {
            return From(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, filterValue);
        }

        /// <summary>
        /// Gets a record by its key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> GetByKey<TKey>(PostgreSqlObjectName tableName, TKey key)
            where TKey : struct
        {
            return GetByKeyList(tableName, new List<TKey> { key });
        }

        /// <summary>
        /// Gets a record by its key.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> GetByKey(PostgreSqlObjectName tableName, string key)
        {
            return GetByKeyList(tableName, new List<string> { key });
        }

        /// <summary>Gets a set of records by an unique key.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keyColumn">Name of the key column. This should be a primary or unique key, but that's not enforced.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> GetByKeyList<T>(PostgreSqlObjectName tableName, string keyColumn, IEnumerable<T> keys)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.SqlName.Equals(keyColumn, System.StringComparison.OrdinalIgnoreCase)).ToList();
            if (primaryKeys.Count == 0)
                throw new MappingException($"Cannot find a column named {keyColumn} on table {tableName}.");

            return GetByKeyList<T>(tableName, primaryKeys.Single(), keys);
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
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> GetByKeyList<T>(PostgreSqlObjectName tableName, IEnumerable<T> keys)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.IsPrimaryKey).ToList();
            if (primaryKeys.Count != 1)
                throw new MappingException($"{nameof(GetByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key. Use DataSource.From instead.");

            return GetByKeyList<T>(tableName, primaryKeys.Single(), keys);
        }

        /// <summary>
        /// Inserts the specified table name.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>ObjectDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, TArgument&gt;.</returns>
        public ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> Insert<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, InsertOptions options = InsertOptions.None)
        where TArgument : class
        {
            return new PostgreSqlInsertObject<TArgument>(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Inserts an object into the specified table.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert occurs.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> Insert<TArgument>(TArgument argumentValue, InsertOptions options = InsertOptions.None) where TArgument : class
        {
            return Insert(DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
        }

        /// <summary>
        /// Executes the indicated procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        public MultipleTableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> Procedure(PostgreSqlObjectName procedureName)
        {
            return new PostgreSqlProcedureCall(this, procedureName);
        }

        /// <summary>
        /// Executes the indicated procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        public MultipleTableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> Procedure(PostgreSqlObjectName procedureName, object argumentValue)
        {
            return new PostgreSqlProcedureCall(this, procedureName, argumentValue);
        }

        /// <summary>
        /// This is used to query a scalar function.
        /// </summary>
        /// <param name="scalarFunctionName">Name of the scalar function.</param>
        /// <param name="functionArgumentValue">The function argument.</param>
        /// <returns></returns>
        public ScalarDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> ScalarFunction(PostgreSqlObjectName scalarFunctionName, object functionArgumentValue)
        {
            return new PostgreSqlScalarFunction(this, scalarFunctionName, functionArgumentValue);
        }

        /// <summary>
        /// This is used to query a scalar function.
        /// </summary>
        /// <param name="scalarFunctionName">Name of the scalar function.</param>
        /// <returns></returns>
        public ScalarDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> ScalarFunction(PostgreSqlObjectName scalarFunctionName)
        {
            return new PostgreSqlScalarFunction(this, scalarFunctionName, null);
        }

        /// <summary>
        /// This is used to query a table valued function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> TableFunction(PostgreSqlObjectName tableFunctionName)
        {
            return new PostgreSqlTableFunction(this, tableFunctionName, null);
        }

        /// <summary>
        /// This is used to query a table valued function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <param name="functionArgumentValue">The function argument.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> TableFunction(PostgreSqlObjectName tableFunctionName, object functionArgumentValue)
        {
            return new PostgreSqlTableFunction(this, tableFunctionName, functionArgumentValue);
        }

        /// <summary>
        /// Update the specified table name.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>ObjectDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, TArgument&gt;.</returns>
        public ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> Update<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, UpdateOptions options = UpdateOptions.None)
        where TArgument : class
        {
            return new PostgreSqlUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Update an object in the specified table.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> Update<TArgument>(TArgument argumentValue, UpdateOptions options = UpdateOptions.None) where TArgument : class
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
        /// <returns>MultipleRowDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> UpdateByKey<TArgument, TKey>(PostgreSqlObjectName tableName, TArgument newValues, TKey key, UpdateOptions options = UpdateOptions.None)
            where TKey : struct
        {
            return UpdateByKeyList(tableName, newValues, new List<TKey> { key }, options);
        }

        /// <summary>
        /// Update a record by its primary key.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> UpdateByKey<TArgument>(PostgreSqlObjectName tableName, TArgument newValues, string key, UpdateOptions options = UpdateOptions.None)
        {
            return UpdateByKeyList(tableName, newValues, new List<string> { key }, options);
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
        /// <returns>MultipleRowDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKeyList")]
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> UpdateByKeyList<TArgument, TKey>(PostgreSqlObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.IsPrimaryKey).ToList();
            if (primaryKeys.Count != 1)
                throw new MappingException($"{nameof(UpdateByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key.");

            var keyList = keys.AsList();
            var columnMetadata = primaryKeys.Single();
            string where;
            if (keys.Count() > 1)
                where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
            else
                where = columnMetadata.SqlName + " = @Param0";

            var parameters = new List<NpgsqlParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new NpgsqlParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.NpgsqlDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new PostgreSqlUpdateMany(this, tableName, newValues, where, parameters, parameters.Count, options);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="options">The update options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<NpgsqlCommand, NpgsqlParameter> UpdateSet(PostgreSqlObjectName tableName, string updateExpression, UpdateOptions options = UpdateOptions.None)
        {
            return new PostgreSqlUpdateMany(this, tableName, updateExpression, null, options);
        }

        /// <summary>
        /// Update multiple records using an update expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="updateArgumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<NpgsqlCommand, NpgsqlParameter> UpdateSet(PostgreSqlObjectName tableName, string updateExpression, object updateArgumentValue, UpdateOptions options = UpdateOptions.None)
        {
            return new PostgreSqlUpdateMany(this, tableName, updateExpression, updateArgumentValue, options);
        }

        /// <summary>
        /// Update multiple records using an update value.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values to use.</param>
        /// <param name="options">The options.</param>
        /// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
        public IUpdateManyCommandBuilder<NpgsqlCommand, NpgsqlParameter> UpdateSet(PostgreSqlObjectName tableName, object newValues, UpdateOptions options = UpdateOptions.None)
        {
            return new PostgreSqlUpdateMany(this, tableName, newValues, options);
        }

        /// <summary>
        /// Upserts the specified table name.
        /// </summary>
        /// <typeparam name="TArgument">The type of the t argument.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>ObjectDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, TArgument&gt;.</returns>
        public ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> Upsert<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, UpsertOptions options = UpsertOptions.None)
        where TArgument : class
        {
            return new PostgreSqlInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Perform an insert or update operation as appropriate.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert/update occurs.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> Upsert<TArgument>(TArgument argumentValue, UpsertOptions options = UpsertOptions.None) where TArgument : class
        {
            return Upsert(DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
        }

        MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> GetByKeyList<T>(PostgreSqlObjectName tableName, ColumnMetadata<NpgsqlDbType> columnMetadata, IEnumerable<T> keys)
        {
            var keyList = keys.AsList();
            string where;
            if (keys.Count() > 1)
                where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
            else
                where = columnMetadata.SqlName + " = @Param0";

            var parameters = new List<NpgsqlParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new NpgsqlParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.NpgsqlDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new PostgreSqlTableOrView(this, tableName, where, parameters);
        }

        MultipleTableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> OnSql(string sqlStatement, object argumentValue)
        {
            return new PostgreSqlSqlCall(this, sqlStatement, argumentValue);
        }
    }
}