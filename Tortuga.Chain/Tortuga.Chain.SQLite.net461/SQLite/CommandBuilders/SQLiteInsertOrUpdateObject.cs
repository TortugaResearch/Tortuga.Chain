using System;
using System.Data.SQLite;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.SQLite.SQLite.CommandBuilders;

namespace Tortuga.Chain.SQLite.CommandBuilders
{
    /// <summary>
    /// Class SQLiteInsertOrUpdateObject
    /// </summary>
    public class SQLiteInsertOrUpdateObject : SQLiteObjectCommand
    {
        private readonly InsertOrUpdateOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteInsertOrUpdateObject"/> class.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        public SQLiteInsertOrUpdateObject(SQLiteDataSourceBase dataSource, string tableName, object argumentValue, InsertOrUpdateOptions options)
            :base(dataSource, tableName, argumentValue)
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

            if(m_Options.HasFlag(InsertOrUpdateOptions.))
        }
    }
}
