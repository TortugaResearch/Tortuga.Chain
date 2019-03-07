using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.MySql.CommandBuilders;

namespace Tortuga.Chain.MySql
{
    partial class MySqlDataSourceBase
    {
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
        public MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> DeleteByKeyList<TKey>(MySqlObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;
            if (primaryKeys.Count != 1)
                throw new MappingException($"DeleteByKey operation isn't allowed on {tableName} because it doesn't have a single primary key.");

            var keyList = keys.AsList();
            var columnMetadata = primaryKeys.Single();
            string where;
            if (keys.Count() > 1)
                where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
            else
                where = columnMetadata.SqlName + " = @Param0";

            var parameters = new List<MySqlParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new MySqlParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.MySqlDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new MySqlDeleteMany(this, tableName, where, parameters, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new MySqlUpdateMany(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);
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
        public MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> UpdateByKeyList<TArgument, TKey>(MySqlObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
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

            var parameters = new List<MySqlParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new MySqlParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.MySqlDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new MySqlUpdateMany(this, tableName, newValues, where, parameters, parameters.Count, options);
        }

        MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> GetByKeyList<T>(MySqlObjectName tableName, ColumnMetadata<MySqlDbType> columnMetadata, IEnumerable<T> keys)
        {
            var keyList = keys.AsList();

            string where;
            if (keys.Count() > 1)
                where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
            else
                where = columnMetadata.SqlName + " = @Param0";

            var parameters = new List<MySqlParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new MySqlParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.MySqlDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new MySqlTableOrView(this, tableName, where, parameters);
        }

        MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> OnDeleteMany(MySqlObjectName tableName, string whereClause, object argumentValue)
        {
            return new MySqlDeleteMany(this, tableName, whereClause, argumentValue);
        }

        MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> OnDeleteMany(MySqlObjectName tableName, object filterValue, FilterOptions filterOptions)
        {
            return new MySqlDeleteMany(this, tableName, filterValue, filterOptions);
        }

        ObjectDbCommandBuilder<MySqlCommand, MySqlParameter, TArgument> OnDeleteObject<TArgument>(MySqlObjectName tableName, TArgument argumentValue, DeleteOptions options) where TArgument : class
        {
            return new MySqlDeleteObject<TArgument>(this, tableName, argumentValue, options);
        }

        TableDbCommandBuilder<MySqlCommand, MySqlParameter, MySqlLimitOption> OnFromTableOrView(MySqlObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
        {
            return new MySqlTableOrView(this, tableOrViewName, filterValue, filterOptions);
        }

        TableDbCommandBuilder<MySqlCommand, MySqlParameter, MySqlLimitOption> OnFromTableOrView(MySqlObjectName tableOrViewName, string whereClause, object argumentValue)
        {
            return new MySqlTableOrView(this, tableOrViewName, whereClause, argumentValue);
        }

        ObjectDbCommandBuilder<MySqlCommand, MySqlParameter, TArgument> OnInsertObject<TArgument>(MySqlObjectName tableName, TArgument argumentValue, InsertOptions options)
               where TArgument : class
        {
            return new MySqlInsertObject<TArgument>(this, tableName, argumentValue, options);
        }

        ObjectDbCommandBuilder<MySqlCommand, MySqlParameter, TArgument> OnInsertOrUpdateObject<TArgument>(MySqlObjectName tableName, TArgument argumentValue, UpsertOptions options) where TArgument : class
        {
            return new MySqlInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }

        MultipleTableDbCommandBuilder<MySqlCommand, MySqlParameter> OnSql(string sqlStatement, object argumentValue)
        {
            return new MySqlSqlCall(this, sqlStatement, argumentValue);
        }

        IUpdateManyDbCommandBuilder<MySqlCommand, MySqlParameter> OnUpdateMany(MySqlObjectName tableName, string updateExpression, object updateArgumentValue, UpdateOptions options)
        {
            return new MySqlUpdateMany(this, tableName, updateExpression, updateArgumentValue, options);
        }

        IUpdateManyDbCommandBuilder<MySqlCommand, MySqlParameter> OnUpdateMany(MySqlObjectName tableName, object newValues, UpdateOptions options)
        {
            return new MySqlUpdateMany(this, tableName, newValues, options);
        }

        ObjectDbCommandBuilder<MySqlCommand, MySqlParameter, TArgument> OnUpdateObject<TArgument>(MySqlObjectName tableName, TArgument argumentValue, UpdateOptions options) where TArgument : class
        {
            return new MySqlUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }
    }
}
