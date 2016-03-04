using System.Collections.Generic;
using System.Data.SQLite;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    public class SQLiteDeleteObject : SQLiteObjectCommand
    {
        private readonly DeleteOptions m_Options;

        public SQLiteDeleteObject(SQLiteDataSourceBase dataSource, string table, object argumentValue, DeleteOptions options)
            : base(dataSource, table, argumentValue)
        {
            m_Options = options;
        }

        public override ExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            var parameters = new List<SQLiteParameter>();

            var where = WhereClause(parameters, m_Options.HasFlag(DeleteOptions.UseKeyAttribute));
            var sql = $"DELETE FROM {TableName} WHERE {where};";

            return new SQLiteExecutionToken(DataSource, "Delete from " + TableName, sql, parameters, lockType: LockType.Write);
        }
    }
}
