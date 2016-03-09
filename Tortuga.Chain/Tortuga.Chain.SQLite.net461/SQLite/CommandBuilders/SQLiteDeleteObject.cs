using System.Collections.Generic;
using System.Data.SQLite;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    /// <summary>
    /// Class SQLiteDeleteObject.
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
        /// Prepares the specified materializer.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public override Tortuga.Chain.Core.ExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            var parameters = new List<SQLiteParameter>();

            var where = WhereClause(parameters, m_Options.HasFlag(DeleteOptions.UseKeyAttribute));
            var sql = $"DELETE FROM {TableName} WHERE {where};";

            return new SQLiteExecutionToken(DataSource, "Delete from " + TableName, sql, parameters, lockType: LockType.Write);
        }
    }
}
