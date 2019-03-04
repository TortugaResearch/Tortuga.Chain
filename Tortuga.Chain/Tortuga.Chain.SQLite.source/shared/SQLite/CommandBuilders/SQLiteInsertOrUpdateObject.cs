using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SQLite.CommandBuilders
{
    /// <summary>
    /// Class SQLiteInsertOrUpdateObject
    /// </summary>
    internal sealed class SQLiteInsertOrUpdateObject<TArgument> : UpsertDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument>
        where TArgument : class
    {
        readonly UpsertOptions m_Options;
        ImmutableHashSet<string> m_KeyColumns = ImmutableHashSet<string>.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteInsertOrUpdateObject{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        public SQLiteInsertOrUpdateObject(SQLiteDataSourceBase dataSource, SQLiteObjectName tableName, TArgument argumentValue, UpsertOptions options)
            : base(dataSource, argumentValue)
        {
            Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_Options = options;
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        public TableOrViewMetadata<SQLiteObjectName, DbType> Table { get; }

        /// <summary>
        /// Matches the on an alternate column(s). Normally matches need to be on the primary key.
        /// </summary>
        /// <param name="columnNames">The column names that form a unique key.</param>
        /// <returns></returns>
        public override UpsertDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> MatchOn(params string[] columnNames)
        {
            //normalize the column names.
            m_KeyColumns = columnNames.Select(c => Table.Columns[c].SqlName).ToImmutableHashSet();
            return this;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns><see cref="SQLiteCommandExecutionToken" /></returns>
        public override CommandExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var identityInsert = m_Options.HasFlag(UpsertOptions.IdentityInsert);

            var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            if (m_KeyColumns.Count > 0)
                sqlBuilder.OverrideKeys(m_KeyColumns);

            var sql = new StringBuilder();
            sqlBuilder.BuildUpdateByKeyStatement(sql, Table.Name.ToQuotedString(), ";");
            sql.AppendLine();

            sqlBuilder.BuildInsertClause(sql, $"INSERT OR IGNORE INTO {Table.Name.ToQuotedString()} (", null, ")", identityInsert);
            sqlBuilder.BuildValuesClause(sql, " VALUES (", ");", identityInsert);
            sql.AppendLine();

            if (sqlBuilder.HasReadFields)
            {
                var keys = sqlBuilder.GetKeyColumns().ToList();
                if (keys.Count != 1)
                    throw new NotSupportedException("Cannot return data from a SQLite Upsert unless there is a single primary key.");
                var key = keys[0];

                sqlBuilder.BuildSelectClause(sql, "SELECT ", null, $" FROM {Table.Name.ToQuotedString()} WHERE {key.QuotedSqlName} = CASE WHEN {key.SqlVariableName} IS NULL OR {key.SqlVariableName} = 0 THEN last_insert_rowid() ELSE {key.SqlVariableName} END;");
            }

            return new SQLiteCommandExecutionToken(DataSource, "Insert or update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters(), lockType: LockType.Write);
        }

        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <remarks>
        /// If the column name was not found, this will return null
        /// </remarks>
        public override ColumnMetadata TryGetColumn(string columnName)
        {
            return Table.Columns.TryGetColumn(columnName);
        }

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
