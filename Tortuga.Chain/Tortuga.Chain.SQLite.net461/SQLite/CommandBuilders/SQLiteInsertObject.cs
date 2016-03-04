using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    /// <summary>
    /// Class that represents a SQLite Insert.
    /// </summary>
    public class SQLiteInsertObject : SQLiteObjectCommand
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SQLiteInsertObject"/> class.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="tableName"></param>
        public SQLiteInsertObject(SQLiteDataSourceBase dataSource, string tableName, object argumentValue)
            : base(dataSource, tableName, argumentValue)
        {
        }

        public override ExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            var parameters = new List<SQLiteParameter>();

            string columns;
            string values;
            ColumnsAndValuesClause(out columns, out values, parameters);
            var output = OutputClause(materializer);
            var sql = $"INSERT INTO {TableName} {columns} {values}; {output};";

            return new SQLiteExecutionToken(DataSource, "Insert into " + TableName, sql, parameters, lockType: LockType.Write);
        }

        private void ColumnsAndValuesClause(out string columns, out string values, List<SQLiteParameter> parameters)
        {
            var availableColumns = Metadata.GetPropertiesFor(ArgumentValue.GetType(),
                 GetPropertiesFilter.ThrowOnNoMatch | GetPropertiesFilter.UpdatableOnly | GetPropertiesFilter.ForInsert);

            columns = "(" + string.Join(", ", availableColumns.Select(c => c.Column.QuotedSqlName)) + ")";
            values = "VALUES (" + string.Join(", ", availableColumns.Select(c => c.Column.SqlVariableName)) + ")";
            LoadParameters(availableColumns, parameters);
        }

        private string OutputClause(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            if (materializer is NonQueryMaterializer<SQLiteCommand, SQLiteParameter>)
                return null;

            var desiredColumns = materializer.DesiredColumns().ToLookup(c => c);
            if (desiredColumns.Count > 0)
            {
                var availableColumns = Metadata.Columns.Where(c => desiredColumns.Contains(c.ClrName)).ToList();
                if (availableColumns.Count == 0)
                    throw new MappingException($"None of the requested columns[{string.Join(", ", desiredColumns)}] were not found on {TableName}");
                return $"SELECT {string.Join(", ", availableColumns.Select(c => c.QuotedSqlName))} FROM {TableName} WHERE ROWID = last_insert_rowid()";
            }
            else if (Metadata.Columns.Any(c => c.IsPrimaryKey))
            {
                var availableColumns = Metadata.Columns.Where(c => c.IsPrimaryKey).Select(c => c.QuotedSqlName).ToList();
                return $"SELECT {string.Join(", ", availableColumns)} FROM {TableName} WHERE ROWID = last_insert_rowid()";
            }
            else
            {
                return "SELECT last_insert_rowid()";
            }
        }
    }
}
