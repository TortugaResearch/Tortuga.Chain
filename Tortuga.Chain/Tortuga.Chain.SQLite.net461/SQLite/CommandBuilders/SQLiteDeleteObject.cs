using System.Collections.Generic;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#endif

namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    /// <summary>
    /// Command object that represents a delete operation.
    /// </summary>
    public class SQLiteDeleteObject : SQLiteObjectCommand
    {
        private readonly DeleteOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDeleteObject"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="table">The table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public SQLiteDeleteObject(SQLiteDataSourceBase dataSource, string table, object argumentValue, DeleteOptions options)
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
            var parameters = new List<SQLiteParameter>();

            var where = WhereClause(parameters, m_Options.HasFlag(DeleteOptions.UseKeyAttribute));
            var output = OutputClause(materializer, where);
            var sql = $"{output}; DELETE FROM {TableName} WHERE {where};";

            return new SQLiteExecutionToken(DataSource, "Delete from " + TableName, sql, parameters, lockType: LockType.Write);
        }
    }
}
