using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    public class SQLiteUpdateObject : SQLiteObjectCommand
    {
        private readonly UpdateOptions m_Options;

        public SQLiteUpdateObject(SQLiteDataSourceBase dataSource, string tableName, object argumentValue, UpdateOptions options)
            : base(dataSource, tableName, argumentValue)
        {
            m_Options = options;
        }

        public override ExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            var parameters = new List<SQLiteParameter>();

            var set = SetClause(parameters);
            var where = WhereClause(parameters, m_Options.HasFlag(UpdateOptions.UseKeyAttribute));
            var output = OutputClause(materializer, where);
            string sql;

            if (m_Options.HasFlag(UpdateOptions.ReturnOldValues))
                sql = $"{output}; UPDATE {TableName} {set} WHERE {where};";
            else
                sql = $"UPDATE {TableName} {set} WHERE {where}; {output};";

            return new SQLiteExecutionToken(DataSource, "Update " + TableName, sql, parameters, lockType: LockType.Write);
        }

        private string SetClause(List<SQLiteParameter> parameters)
        {
            var filter = GetPropertiesFilter.ThrowOnNoMatch | GetPropertiesFilter.UpdatableOnly;

            if (m_Options.HasFlag(UpdateOptions.UseKeyAttribute))
                filter |= GetPropertiesFilter.ObjectDefinedNonKey;
            else
                filter |= GetPropertiesFilter.NonPrimaryKey;

            if (DataSource.StrictMode)
                filter |= GetPropertiesFilter.ThrowOnMissingColumns;

            var availableColumns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), filter);

            var set = "SET " + string.Join(", ", availableColumns.Select(c => $"{c.Column.QuotedSqlName} = {c.Column.SqlVariableName}"));
            LoadParameters(availableColumns, parameters);
            return set;
        }
    }
}
