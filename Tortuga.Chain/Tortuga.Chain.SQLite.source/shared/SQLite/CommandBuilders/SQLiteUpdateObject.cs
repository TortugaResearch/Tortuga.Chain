using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
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
    /// Command object that represents an update operation.
    /// </summary>
    internal sealed class SQLiteUpdateObject<TArgument> : SQLiteObjectCommand<TArgument>
        where TArgument : class
    {
        readonly UpdateOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteUpdateObject{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public SQLiteUpdateObject(SQLiteDataSourceBase dataSource, string tableName, TArgument argumentValue, UpdateOptions options)
            : base(dataSource, tableName, argumentValue)
        {
            m_Options = options;
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

            var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            if (m_Options.HasFlag(UpdateOptions.ReturnOldValues))
            {
                sqlBuilder.BuildSelectByKeyStatement(sql, Table.Name, ";");
                sql.AppendLine();
            }
            sqlBuilder.BuildUpdateByKeyStatement(sql, Table.Name, ";");
            if (!m_Options.HasFlag(UpdateOptions.ReturnOldValues))
            {
                sql.AppendLine();
                sqlBuilder.BuildSelectByKeyStatement(sql, Table.Name, ";");
            }

            return new SQLiteCommandExecutionToken(DataSource, "Update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters(), lockType: LockType.Write).CheckUpdateRowCount(m_Options);
        }

    }
}
