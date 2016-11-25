using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.PostgreSql.CommandBuilders;
using System;

namespace Tortuga.Chain.PostgreSql
{
    /// <summary>
    /// Class PostgreSqlDataSourceBase.
    /// </summary>
    public abstract partial class PostgreSqlDataSourceBase : DataSource<NpgsqlConnection, NpgsqlTransaction, NpgsqlCommand, NpgsqlParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDataSourceBase"/> class.
        /// </summary>
        /// <param name="settings">Optional settings object.</param>
        protected PostgreSqlDataSourceBase(DataSourceSettings settings) : base(settings)
        {
        }

        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        public abstract new PostgreSqlMetadataCache DatabaseMetadata { get; }


        /// <summary>
        /// Deletes the specified table name.
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
        /// Deletes an object model from the table indicated by the class's Table attribute.
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
            return new PostgreSqlTableOrView(this, tableOrViewName, filterValue, filterOptions );
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> From<TObject>(object filterValue) where TObject : class
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
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> GetByKey<TKey>(PostgreSqlObjectName tableName, params TKey[] keys)
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
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> GetByKey(PostgreSqlObjectName tableName, params string[] keys)
        {
            return GetByKeyList(tableName, keys);
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

        /// <summary>
        /// Gets the by key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> GetByKeyList<T>(PostgreSqlObjectName tableName, IEnumerable<T> keys)
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
        /// SQLs the specified SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <returns>MultipleTableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        public MultipleTableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> Sql(string sqlStatement)
        {
            return new PostgreSqlSqlCall(this, sqlStatement, null);
        }

        /// <summary>
        /// SQLs the specified SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>MultipleTableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        public MultipleTableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> Sql(string sqlStatement, object argumentValue)
        {
            return new PostgreSqlSqlCall(this, sqlStatement, argumentValue);
        }

        /// <summary>
        /// Updates the specified table name.
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
        /// Updates an object in the specified table.
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
        /// Performs an insert or update operation as appropriate.
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert/update occurs.</param>
        /// <returns></returns>
        public ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> Upsert<TArgument>(TArgument argumentValue, UpsertOptions options = UpsertOptions.None) where TArgument : class
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
        /// <returns>MultipleRowDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> UpdateByKey<TArgument, TKey>(PostgreSqlObjectName tableName, TArgument newValues, TKey key, UpdateOptions options = UpdateOptions.None)
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
        /// <returns>MultipleRowDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        public SingleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> UpdateByKey<TArgument>(PostgreSqlObjectName tableName, TArgument newValues, string key, UpdateOptions options = UpdateOptions.None)
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
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> UpdateByKey<TArgument, TKey>(PostgreSqlObjectName tableName, TArgument newValues, params TKey[] keys)
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
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> UpdateByKey<TArgument>(PostgreSqlObjectName tableName, TArgument newValues, params string[] keys)
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
        /// <returns>MultipleRowDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKey")]
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> UpdateByKeyList<TArgument, TKey>(PostgreSqlObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
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
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> DeleteByKey<T>(PostgreSqlObjectName tableName, params T[] keys)
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
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> DeleteByKey(PostgreSqlObjectName tableName, params string[] keys)
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
        /// <returns>MultipleRowDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKey")]
        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> DeleteByKeyList<TKey>(PostgreSqlObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
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
        /// Executes the indicated procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        public MultipleTableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> Procedure(string procedureName)
        {
            return new PostgreSqlProcedureCall(this, procedureName);
        }

        /// <summary>
        /// Executes the indicated procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        public MultipleTableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> Procedure(string procedureName, object argumentValue)
        {
            return new PostgreSqlProcedureCall(this, procedureName, argumentValue);
        }

        /// <summary>
        /// Dereferences cursors returned by a stored procedure.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="implementation">The implementation.</param>
        /// <returns>System.String.</returns>
        protected static int? DereferenceCursors(NpgsqlCommand cmd, CommandImplementation<NpgsqlCommand> implementation)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd", "cmd is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var closeTransaction = false;
            try
            {
                if (cmd.Transaction == null)
                {
                    cmd.Transaction = cmd.Connection.BeginTransaction();
                    closeTransaction = true;
                }

                var sql = new StringBuilder();
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    while (reader.Read())
                        sql.AppendLine($"FETCH ALL IN \"{ reader.GetString(0) }\";");

                using (var cmd2 = new NpgsqlCommand())
                {
                    cmd2.Connection = cmd.Connection;
                    cmd2.Transaction = cmd.Transaction;
                    cmd2.CommandTimeout = cmd.CommandTimeout;
                    cmd2.CommandText = sql.ToString();
                    cmd2.CommandType = CommandType.Text;
                    return implementation(cmd2);
                }
            }
            finally
            {
                if (closeTransaction)
                    cmd.Transaction.Commit();
            }
        }

        /// <summary>
        /// Dereferences cursors returned by a stored procedure.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="implementation">The implementation.</param>
        /// <returns>System.String.</returns>
        protected static async Task<int?> DereferenceCursorsAsync(NpgsqlCommand cmd, CommandImplementationAsync<NpgsqlCommand> implementation)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd", "cmd is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var closeTransaction = false;
            try
            {
                if (cmd.Transaction == null)
                {
                    cmd.Transaction = cmd.Connection.BeginTransaction();
                    closeTransaction = true;
                }

                var sql = new StringBuilder();
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess).ConfigureAwait(false))
                    while (await reader.ReadAsync().ConfigureAwait(false))
                        sql.AppendLine($"FETCH ALL IN \"{ reader.GetString(0) }\";");

                using (var cmd2 = new NpgsqlCommand())
                {
                    cmd2.Connection = cmd.Connection;
                    cmd2.Transaction = cmd.Transaction;
                    cmd2.CommandTimeout = cmd.CommandTimeout;
                    cmd2.CommandText = sql.ToString();
                    cmd2.CommandType = CommandType.Text;
                    return await implementation(cmd2).ConfigureAwait(false);
                }
            }
            finally
            {
                if (closeTransaction)
                    await cmd.Transaction.CommitAsync().ConfigureAwait(false);
            }
        }
    }
}
