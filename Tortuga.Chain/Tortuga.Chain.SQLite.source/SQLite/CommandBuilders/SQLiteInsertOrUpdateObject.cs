using System.Linq;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.SQLite.SQLite.CommandBuilders;
using System.Text;
using System;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#endif


namespace Tortuga.Chain.SQLite.CommandBuilders
{
    /// <summary>
    /// Class SQLiteInsertOrUpdateObject
    /// </summary>
    internal sealed class SQLiteInsertOrUpdateObject : SQLiteObjectCommand
    {
        private readonly UpsertOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteInsertOrUpdateObject"/> class.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        public SQLiteInsertOrUpdateObject(SQLiteDataSourceBase dataSource, string tableName, object argumentValue, UpsertOptions options)
            : base(dataSource, tableName, argumentValue)
        {
            m_Options = options;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns><see cref="SQLiteExecutionToken" /></returns>
        public override ExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = Metadata.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(ArgumentValue, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            sqlBuilder.BuildUpdateByKeyStatement(sql, TableName, ";");
            sql.AppendLine();

            sqlBuilder.BuildInsertClause(sql, $"INSERT OR IGNORE INTO {TableName} (", null, ")");
            sqlBuilder.BuildValuesClause(sql, " VALUES (", ");");
            sql.AppendLine();

            if (sqlBuilder.HasReadFields)
            {
                var keys = sqlBuilder.GetKeyColumns().ToList();
                if (keys.Count != 1)
                    throw new NotSupportedException("Cannot return data from a SQLite Upsert unless there is a single primary key.");
                var key = keys[0];

                sqlBuilder.BuildSelectClause(sql, "SELECT ", null, $" FROM {TableName} WHERE {key.QuotedSqlName} = CASE WHEN {key.SqlVariableName} IS NULL OR {key.SqlVariableName} = 0 THEN last_insert_rowid() ELSE {key.SqlVariableName} END;");

            }

            return new SQLiteExecutionToken(DataSource, "Insert or update " + TableName, sql.ToString(), sqlBuilder.GetParameters(), lockType: LockType.Write);

        }

    }
}
