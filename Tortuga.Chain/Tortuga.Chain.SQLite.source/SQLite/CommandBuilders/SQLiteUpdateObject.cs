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
    /// Command object that represents an update operation.
    /// </summary>
    internal sealed class SQLiteUpdateObject : SQLiteObjectCommand
    {
        private readonly UpdateOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteUpdateObject"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public SQLiteUpdateObject(SQLiteDataSourceBase dataSource, string tableName, object argumentValue, UpdateOptions options)
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

            var sqlBuilder = Metadata.CreateSqlBuilder();
            sqlBuilder.ApplyArgumentValue(ArgumentValue, m_Options.HasFlag(UpdateOptions.UseKeyAttribute), DataSource.StrictMode);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns(), DataSource.StrictMode);

            var sql = new StringBuilder();
            if (m_Options.HasFlag(UpdateOptions.ReturnOldValues))
            {
                sqlBuilder.BuildSelectByKeyStatment(sql, TableName, ";");
                sql.AppendLine();
            }
            sqlBuilder.BuildUpdateByKeyStatment(sql, TableName, ";");
            if (!m_Options.HasFlag(UpdateOptions.ReturnOldValues))
            {
                sql.AppendLine();
                sqlBuilder.BuildSelectByKeyStatment(sql, TableName, ";");
            }

            return new SQLiteExecutionToken(DataSource, "Update " + TableName, sql.ToString(), sqlBuilder.GetParameters(), lockType: LockType.Write);
        }

    }
}
