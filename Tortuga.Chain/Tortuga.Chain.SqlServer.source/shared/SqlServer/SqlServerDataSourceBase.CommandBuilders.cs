using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tortuga.Anchor;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer
{
    partial class SqlServerDataSourceBase
    {
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
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> DeleteByKeyList<TKey>(SqlServerObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;
            if (primaryKeys.Count != 1)
                throw new MappingException($"{nameof(DeleteByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key.");

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
        /// Inserts the batch of records as one operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="dataTable">The data table.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use InsertBatch(SqlServerObjectName, DataTable, SqlServerObjectName, InsertOptions) instead. This overload will be removed in a future version.")]
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch(SqlServerObjectName tableName, SqlServerObjectName tableTypeName, DataTable dataTable, InsertOptions options = InsertOptions.None)
        {
            return new SqlServerInsertBatch(this, tableName, dataTable, tableTypeName, options);
        }

        /// <summary>
        /// Inserts the batch of records as one operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="dataTable">The data table.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch(SqlServerObjectName tableName, DataTable dataTable, SqlServerObjectName tableTypeName, InsertOptions options = InsertOptions.None)
        {
            return new SqlServerInsertBatch(this, tableName, dataTable, tableTypeName, options);
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use InsertBatch(SqlServerObjectName, DataTable, SqlServerObjectName, InsertOptions) instead. This overload will be removed in a future version.")]
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use InsertBatch(SqlServerObjectName, DbDataReader, SqlServerObjectName, InsertOptions) instead. This overload will be removed in a future version.")]
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch(SqlServerObjectName tableName, SqlServerObjectName tableTypeName, DbDataReader dataReader, InsertOptions options = InsertOptions.None)
        {
            return new SqlServerInsertBatch(this, tableName, dataReader, tableTypeName, options);
        }

        /// <summary>
        /// Inserts the batch of records as one operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch(SqlServerObjectName tableName, DbDataReader dataReader, SqlServerObjectName tableTypeName, InsertOptions options = InsertOptions.None)
        {
            return new SqlServerInsertBatch(this, tableName, dataReader, tableTypeName, options);
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use InsertBatch(SqlServerObjectName, DbDataReader, SqlServerObjectName, InsertOptions) instead. This overload will be removed in a future version.")]
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use InsertBatch(SqlServerObjectName, IEnumerable<TObject>, SqlServerObjectName, InsertOptions) instead. This overload will be removed in a future version.")]
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch<TObject>(SqlServerObjectName tableName, SqlServerObjectName tableTypeName, IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None)
        {
            var tableType = DatabaseMetadata.GetUserDefinedType(tableTypeName);
            return new SqlServerInsertBatch(this, tableName, new ObjectDataReader<TObject>(tableType, objects), tableTypeName, options);
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
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch<TObject>(SqlServerObjectName tableName, IEnumerable<TObject> objects, SqlServerObjectName tableTypeName, InsertOptions options = InsertOptions.None)
        {
            var tableType = DatabaseMetadata.GetUserDefinedType(tableTypeName);
            return new SqlServerInsertBatch(this, tableName, new ObjectDataReader<TObject>(tableType, objects), tableTypeName, options);
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use InsertBatch(IEnumerable<TObject>, SqlServerObjectName, InsertOptions) instead. This overload will be removed in a future version.")]
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch<TObject>(SqlServerObjectName tableTypeName, IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None) where TObject : class
        {
            return InsertBatch(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, tableTypeName, objects, options);
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
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch<TObject>(IEnumerable<TObject> objects, SqlServerObjectName tableTypeName, InsertOptions options = InsertOptions.None) where TObject : class
        {
            return InsertBatch(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, objects, tableTypeName, options);
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
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> UpdateByKeyList<TArgument, TKey>(SqlServerObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;
            if (primaryKeys.Count != 1)
                throw new MappingException($"{nameof(UpdateByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key.");

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

        MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> GetByKeyList<T>(SqlServerObjectName tableName, ColumnMetadata<SqlDbType> columnMetadata, IEnumerable<T> keys)
        {
            var keyList = keys.AsList();
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

        MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> OnDeleteMany(SqlServerObjectName tableName, string whereClause, object argumentValue)
        {
            return new SqlServerDeleteMany(this, tableName, whereClause, argumentValue);
        }

        MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> OnDeleteMany(SqlServerObjectName tableName, object filterValue, FilterOptions filterOptions)
        {
            return new SqlServerDeleteMany(this, tableName, filterValue, filterOptions);
        }

        ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> OnDeleteObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, DeleteOptions options) where TArgument : class
        {
            return new SqlServerDeleteObject<TArgument>(this, tableName, argumentValue, options);
        }

        TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> OnFromTableOrView(SqlServerObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
        {
            return new SqlServerTableOrView(this, tableOrViewName, filterValue, filterOptions);
        }

        TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> OnFromTableOrView(SqlServerObjectName tableOrViewName, string whereClause, object argumentValue)
        {
            return new SqlServerTableOrView(this, tableOrViewName, whereClause, argumentValue);
        }

        ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> OnInsertObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, InsertOptions options)
               where TArgument : class
        {
            return new SqlServerInsertObject<TArgument>(this, tableName, argumentValue, options);
        }

        ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> OnInsertOrUpdateObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, UpsertOptions options) where TArgument : class
        {
            return new SqlServerInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }

        MultipleTableDbCommandBuilder<SqlCommand, SqlParameter> OnSql(string sqlStatement, object argumentValue)
        {
            return new SqlServerSqlCall(this, sqlStatement, argumentValue);
        }

        IUpdateManyDbCommandBuilder<SqlCommand, SqlParameter> OnUpdateMany(SqlServerObjectName tableName, string updateExpression, object updateArgumentValue, UpdateOptions options)
        {
            return new SqlServerUpdateMany(this, tableName, updateExpression, updateArgumentValue, options);
        }

        IUpdateManyDbCommandBuilder<SqlCommand, SqlParameter> OnUpdateMany(SqlServerObjectName tableName, object newValues, UpdateOptions options)
        {
            return new SqlServerUpdateMany(this, tableName, newValues, options);
        }

        ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> OnUpdateObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, UpdateOptions options) where TArgument : class
        {
            return new SqlServerUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }
    }
}
