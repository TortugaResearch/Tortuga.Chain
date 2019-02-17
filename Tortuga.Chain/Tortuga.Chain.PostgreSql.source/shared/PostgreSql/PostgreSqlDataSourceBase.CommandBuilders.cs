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

        MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> OnDeleteMany(PostgreSqlObjectName tableName, string whereClause, object argumentValue)
        {
            return new PostgreSqlDeleteMany(this, tableName, whereClause, argumentValue);
        }

        MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> OnDeleteMany(PostgreSqlObjectName tableName, object filterValue, FilterOptions filterOptions)
        {
            return new PostgreSqlDeleteMany(this, tableName, filterValue, filterOptions);
        }

        ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> OnDeleteObject<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, DeleteOptions options) where TArgument : class
        {
            return new PostgreSqlDeleteObject<TArgument>(this, tableName, argumentValue, options);
        }

        TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> OnFromTableOrView(PostgreSqlObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
        {
            return new PostgreSqlTableOrView(this, tableOrViewName, filterValue, filterOptions);
        }

        TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> OnFromTableOrView(PostgreSqlObjectName tableOrViewName, string whereClause, object argumentValue)
        {
            return new PostgreSqlTableOrView(this, tableOrViewName, whereClause, argumentValue);
        }

        ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> OnInsertObject<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, InsertOptions options)
               where TArgument : class
        {
            return new PostgreSqlInsertObject<TArgument>(this, tableName, argumentValue, options);
        }

        ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> OnInsertOrUpdateObject<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, UpsertOptions options) where TArgument : class
        {
            return new PostgreSqlInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }

        MultipleTableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> OnSql(string sqlStatement, object argumentValue)
        {
            return new PostgreSqlSqlCall(this, sqlStatement, argumentValue);
        }

        IUpdateManyCommandBuilder<NpgsqlCommand, NpgsqlParameter> OnUpdateMany(PostgreSqlObjectName tableName, string updateExpression, object updateArgumentValue, UpdateOptions options)
        {
            return new PostgreSqlUpdateMany(this, tableName, updateExpression, updateArgumentValue, options);
        }

        IUpdateManyCommandBuilder<NpgsqlCommand, NpgsqlParameter> OnUpdateMany(PostgreSqlObjectName tableName, object newValues, UpdateOptions options)
        {
            return new PostgreSqlUpdateMany(this, tableName, newValues, options);
        }

        ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> OnUpdateObject<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, UpdateOptions options) where TArgument : class
        {
            return new PostgreSqlUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }
    }
}