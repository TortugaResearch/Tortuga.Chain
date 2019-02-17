using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SQLite.CommandBuilders;

namespace Tortuga.Chain.SQLite
{
    partial class SQLiteDataSourceBase
    {
        /// <summary>
        /// Delete multiple rows by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="options">Update options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKeyList")]
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> DeleteByKeyList<TKey>(SQLiteObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
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

            var parameters = new List<SQLiteParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new SQLiteParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.DbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new SQLiteDeleteMany(this, tableName, where, parameters, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new SQLiteUpdateMany(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);
        }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="lockType">Type of the lock.</param>
        /// <returns>SQLiteSqlCall.</returns>
        public MultipleTableDbCommandBuilder<SQLiteCommand, SQLiteParameter> Sql(string sqlStatement, LockType lockType)
        {
            return new SQLiteSqlCall(this, sqlStatement, null, lockType);
        }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="lockType">Type of the lock.</param>
        /// <returns>SQLiteSqlCall.</returns>
        public MultipleTableDbCommandBuilder<SQLiteCommand, SQLiteParameter> Sql(string sqlStatement, object argumentValue, LockType lockType)
        {
            return new SQLiteSqlCall(this, sqlStatement, argumentValue, lockType);
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
        /// <returns>MultipleRowDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKeyList")]
        public MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> UpdateByKeyList<TArgument, TKey>(SQLiteObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
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

            var parameters = new List<SQLiteParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new SQLiteParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.DbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new SQLiteUpdateMany(this, tableName, newValues, where, parameters, parameters.Count, options);
        }

        MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> GetByKeyList<T>(SQLiteObjectName tableName, ColumnMetadata<DbType> columnMetadata, IEnumerable<T> keys)
        {
            var keyList = keys.AsList();

            string where;
            if (keys.Count() > 1)
                where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
            else
                where = columnMetadata.SqlName + " = @Param0";

            var parameters = new List<SQLiteParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new SQLiteParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.DbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new SQLiteTableOrView(this, tableName, where, parameters);
        }

        MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> OnDeleteMany(SQLiteObjectName tableName, string whereClause, object argumentValue)
        {
            return new SQLiteDeleteMany(this, tableName, whereClause, argumentValue);
        }

        MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> OnDeleteMany(SQLiteObjectName tableName, object filterValue, FilterOptions filterOptions)
        {
            return new SQLiteDeleteMany(this, tableName, filterValue, filterOptions);
        }

        ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> OnDeleteObject<TArgument>(SQLiteObjectName tableName, TArgument argumentValue, DeleteOptions options) where TArgument : class
        {
            return new SQLiteDeleteObject<TArgument>(this, tableName, argumentValue, options);
        }

        TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> OnFromTableOrView(SQLiteObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
        {
            return new SQLiteTableOrView(this, tableOrViewName, filterValue, filterOptions);
        }

        TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> OnFromTableOrView(SQLiteObjectName tableOrViewName, string whereClause, object argumentValue)
        {
            return new SQLiteTableOrView(this, tableOrViewName, whereClause, argumentValue);
        }

        ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> OnInsertObject<TArgument>(SQLiteObjectName tableName, TArgument argumentValue, InsertOptions options)
               where TArgument : class
        {
            return new SQLiteInsertObject<TArgument>(this, tableName, argumentValue, options);
        }

        ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> OnInsertOrUpdateObject<TArgument>(SQLiteObjectName tableName, TArgument argumentValue, UpsertOptions options) where TArgument : class
        {
            return new SQLiteInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }

        MultipleTableDbCommandBuilder<SQLiteCommand, SQLiteParameter> OnSql(string sqlStatement, object argumentValue)
        {
            return new SQLiteSqlCall(this, sqlStatement, argumentValue, LockType.Write);
        }

        IUpdateManyCommandBuilder<SQLiteCommand, SQLiteParameter> OnUpdateMany(SQLiteObjectName tableName, string updateExpression, object updateArgumentValue, UpdateOptions options)
        {
            return new SQLiteUpdateMany(this, tableName, updateExpression, updateArgumentValue, options);
        }

        IUpdateManyCommandBuilder<SQLiteCommand, SQLiteParameter> OnUpdateMany(SQLiteObjectName tableName, object newValues, UpdateOptions options)
        {
            return new SQLiteUpdateMany(this, tableName, newValues, options);
        }

        ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> OnUpdateObject<TArgument>(SQLiteObjectName tableName, TArgument argumentValue, UpdateOptions options) where TArgument : class
        {
            return new SQLiteUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }
    }
}