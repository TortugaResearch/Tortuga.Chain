using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer
{
    partial class OleDbSqlServerDataSourceBase
    {
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
        public MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> DeleteByKeyList<TKey>(SqlServerObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;
            if (primaryKeys.Count != 1)
                throw new MappingException($"{nameof(DeleteByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key.");

            var keyList = keys.AsList();
            var columnMetadata = primaryKeys.Single();
            string where;
            if (keys.Count() > 1)
                where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "?")) + ")";
            else
                where = columnMetadata.SqlName + " = ?";

            var parameters = new List<OleDbParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new OleDbParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.OleDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new OleDbSqlServerDeleteMany(this, tableName, where, parameters);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new OleDbSqlServerUpdateMany(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);
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
        public MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> UpdateByKeyList<TArgument, TKey>(SqlServerObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;
            if (primaryKeys.Count != 1)
                throw new MappingException($"{nameof(UpdateByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key.");

            var keyList = keys.AsList();
            var columnMetadata = primaryKeys.Single();
            string where;
            if (keys.Count() > 1)
                where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "?")) + ")";
            else
                where = columnMetadata.SqlName + " = ?";

            var parameters = new List<OleDbParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new OleDbParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.OleDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new OleDbSqlServerUpdateMany(this, tableName, newValues, where, parameters, parameters.Count, options);
        }

        MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> GetByKeyList<T>(SqlServerObjectName tableName, ColumnMetadata<OleDbType> columnMetadata, IEnumerable<T> keys)
        {
            var keyList = keys.AsList();
            string where;
            if (keys.Count() > 1)
                where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "?")) + ")";
            else
                where = columnMetadata.SqlName + " = ?";

            var parameters = new List<OleDbParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new OleDbParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.OleDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new OleDbSqlServerTableOrView(this, tableName, where, parameters);
        }

        MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> OnDeleteMany(SqlServerObjectName tableName, string whereClause, object argumentValue)
        {
            return new OleDbSqlServerDeleteMany(this, tableName, whereClause, argumentValue);
        }

        MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> OnDeleteMany(SqlServerObjectName tableName, object filterValue, FilterOptions filterOptions)
        {
            return new OleDbSqlServerDeleteMany(this, tableName, filterValue, filterOptions);
        }

        ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument> OnDeleteObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, DeleteOptions options) where TArgument : class
        {
            return new OleDbSqlServerDeleteObject<TArgument>(this, tableName, argumentValue, options);
        }

        TableDbCommandBuilder<OleDbCommand, OleDbParameter, SqlServerLimitOption> OnFromTableOrView(SqlServerObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
        {
            return new OleDbSqlServerTableOrView(this, tableOrViewName, filterValue, filterOptions);
        }

        TableDbCommandBuilder<OleDbCommand, OleDbParameter, SqlServerLimitOption> OnFromTableOrView(SqlServerObjectName tableOrViewName, string whereClause, object argumentValue)
        {
            return new OleDbSqlServerTableOrView(this, tableOrViewName, whereClause, argumentValue);
        }

        ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument> OnInsertObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, InsertOptions options)
               where TArgument : class
        {
            return new OleDbSqlServerInsertObject<TArgument>(this, tableName, argumentValue, options);
        }

        ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument> OnInsertOrUpdateObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, UpsertOptions options) where TArgument : class
        {
            return new OleDbSqlServerInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }

        MultipleTableDbCommandBuilder<OleDbCommand, OleDbParameter> OnSql(string sqlStatement, object argumentValue)
        {
            return new OleDbSqlServerSqlCall(this, sqlStatement, argumentValue);
        }

        IUpdateManyDbCommandBuilder<OleDbCommand, OleDbParameter> OnUpdateMany(SqlServerObjectName tableName, string updateExpression, object updateArgumentValue, UpdateOptions options)
        {
            return new OleDbSqlServerUpdateMany(this, tableName, updateExpression, updateArgumentValue, options);
        }

        IUpdateManyDbCommandBuilder<OleDbCommand, OleDbParameter> OnUpdateMany(SqlServerObjectName tableName, object newValues, UpdateOptions options)
        {
            return new OleDbSqlServerUpdateMany(this, tableName, newValues, options);
        }

        ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument> OnUpdateObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, UpdateOptions options) where TArgument : class
        {
            return new OleDbSqlServerUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }
    }
}
