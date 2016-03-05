using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    /// <summary>
    /// Command object that represents a delete operation.
    /// </summary>
    public class SQLiteDeleteObject : SQLiteObjectCommand
    {
        private readonly DeleteOptions m_Options;

        /// <summary>
        /// Initializes an instance of <see cref="SQLiteDeleteObject" /> for delete operations.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="table"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
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
