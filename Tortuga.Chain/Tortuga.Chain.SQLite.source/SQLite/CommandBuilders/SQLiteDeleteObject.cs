using System;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using System.Text;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#endif

namespace Tortuga.Chain.SQLite.CommandBuilders
{
    /// <summary>
    /// Command object that represents a delete operation.
    /// </summary>
    internal sealed class SQLiteDeleteObject<TArgument> : SQLiteObjectCommand<TArgument>
        where TArgument : class
    {
        private readonly DeleteOptions m_Options;


        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDeleteObject{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="table">The table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public SQLiteDeleteObject(SQLiteDataSourceBase dataSource, string table, TArgument argumentValue, DeleteOptions options)
            : base(dataSource, table, argumentValue)
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

            var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            sqlBuilder.BuildSelectByKeyStatement(sql, Table.Name, ";");
            sql.AppendLine();
            sqlBuilder.BuildDeleteStatement(sql, Table.Name, ";");

            return new SQLiteExecutionToken(DataSource, "Delete from " + Table.Name, sql.ToString(), sqlBuilder.GetParameters(), lockType: LockType.Write);
        }
    }
}
