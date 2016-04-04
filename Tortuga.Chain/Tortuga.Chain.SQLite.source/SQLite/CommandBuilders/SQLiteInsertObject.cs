using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using System.Text;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#endif

namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    /// <summary>
    /// Class that represents a SQLite Insert.
    /// </summary>
    internal sealed class SQLiteInsertObject : SQLiteObjectCommand
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SQLiteInsertObject" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        public SQLiteInsertObject(SQLiteDataSourceBase dataSource, string tableName, object argumentValue)
            : base(dataSource, tableName, argumentValue)
        {
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns><see cref="SQLiteExecutionToken" /></returns>
        public override ExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            var sqlBuilder = Metadata.CreateSqlBuilder();
            sqlBuilder.ApplyArgumentValue(ArgumentValue, false, DataSource.StrictMode);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns(), DataSource.StrictMode);

            var sql = new StringBuilder();
            sqlBuilder.BuildInsertStatment(sql, TableName, ";");
            sql.AppendLine();
            sqlBuilder.BuildSelectClause(sql, "SELECT ", null, $" FROM {TableName} WHERE ROWID=last_insert_rowid();");

            return new SQLiteExecutionToken(DataSource, "Insert into " + TableName, sql.ToString(), sqlBuilder.GetParameters(), lockType: LockType.Write);
        }

    }
}

