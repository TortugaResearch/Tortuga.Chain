using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
    /// <summary>
    /// Class PostgreSqlInsertOrUpdateObject
    /// </summary>
    internal sealed class PostgreSqlInsertOrUpdateObject<TArgument> : UpsertDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument>
        where TArgument : class
    {
        readonly UpsertOptions m_Options;
        ImmutableHashSet<string> m_KeyColumns = ImmutableHashSet<string>.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlInsertOrUpdateObject{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public PostgreSqlInsertOrUpdateObject(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, TArgument argumentValue, UpsertOptions options)
            : base(dataSource, argumentValue)
        {
            m_Options = options;
            Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        public TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> Table { get; }

        /// <summary>
        /// Matches the on an alternate column(s). Normally matches need to be on the primary key.
        /// </summary>
        /// <param name="columnNames">The column names that form a unique key.</param>
        /// <returns></returns>
        public override UpsertDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> MatchOn(params string[] columnNames)
        {
            //normalize the column names.
            m_KeyColumns = columnNames.Select(c => Table.Columns[c].SqlName).ToImmutableHashSet();
            return this;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns><see cref="PostgreSqlCommandExecutionToken" /></returns>
        public override CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var identityInsert = m_Options.HasFlag(UpsertOptions.IdentityInsert);
            if (identityInsert)
                throw new NotImplementedException("See issue 256. https://github.com/docevaad/Chain/issues/256");

            var primaryKeyNames = Table.Columns.Where(x => x.IsPrimaryKey).Select(x => x.QuotedSqlName);
            string conflictNames = string.Join(", ", primaryKeyNames);

            var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            if (m_KeyColumns.Count > 0)
                sqlBuilder.OverrideKeys(m_KeyColumns);

            var sql = new StringBuilder();
            List<NpgsqlParameter> keyParameters;
            var isPrimaryKeyIdentity = sqlBuilder.PrimaryKeyIsIdentity(out keyParameters);
            if (isPrimaryKeyIdentity)
            {
                var areKeysNull = keyParameters.Any(c => c.Value == DBNull.Value || c.Value == null) ? true : false;
                if (areKeysNull)
                    sqlBuilder.BuildInsertStatement(sql, Table.Name.ToString(), null);
                else
                    sqlBuilder.BuildUpdateByKeyStatement(sql, Table.Name.ToString(), null);
                sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, ";");
            }
            else
            {
                sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {Table.Name.ToString()} (", null, ")");
                sqlBuilder.BuildValuesClause(sql, " VALUES (", ")");
                sqlBuilder.BuildSetClause(sql, $" ON CONFLICT ({conflictNames}) DO UPDATE SET ", null, null);
                sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, ";");
            }

            //Looks like ON CONFLICT is useful here http://www.postgresql.org/docs/current/static/sql-insert.html
            //Use RETURNING in place of SQL Servers OUTPUT clause http://www.postgresql.org/docs/current/static/sql-insert.html

            return new PostgreSqlCommandExecutionToken(DataSource, "Insert or update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters());
        }

        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <remarks>
        /// If the column name was not found, this will return null
        /// </remarks>
        public override ColumnMetadata TryGetColumn(string columnName) => Table.Columns.TryGetColumn(columnName);

        /// <summary>
        /// Returns a list of columns known to be non-nullable.
        /// </summary>
        /// <returns>
        /// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
        /// </returns>
        /// <remarks>
        /// This is used by materializers to skip IsNull checks.
        /// </remarks>
        public override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => Table.NonNullableColumns;
    }
}
