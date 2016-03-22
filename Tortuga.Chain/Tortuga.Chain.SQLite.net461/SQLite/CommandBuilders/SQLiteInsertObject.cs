using System.Collections.Generic;
using System.Linq;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#endif

namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    /// <summary>
    /// Class that represents a SQLite Insert.
    /// </summary>
    public class SQLiteInsertObject : SQLiteObjectCommand
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SQLiteInsertObject" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        public SQLiteInsertObject(SQLiteDataSourceBase dataSource, string tableName, object argumentValue)
            : base(dataSource, tableName, argumentValue)
        {
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns><see cref="SQLiteExecutionToken" /></returns>
        public override ExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            var parameters = new List<SQLiteParameter>();

            string columns;
            string values;
            ColumnsAndValuesClause(out columns, out values, parameters);
            var output = OutputClause(materializer, "ROWID=last_insert_rowid()");
            var sql = $"INSERT INTO {TableName} {columns} {values}; {output};";

            return new SQLiteExecutionToken(DataSource, "Insert into " + TableName, sql, parameters, lockType: LockType.Write);
        }

        private void ColumnsAndValuesClause(out string columns, out string values, List<SQLiteParameter> parameters)
        {
            if (ArgumentDictionary != null)
            {
                var availableColumns = Metadata.GetKeysFor(ArgumentDictionary, GetKeysFilter.ThrowOnNoMatch | GetKeysFilter.MutableColumns);

                columns = "(" + string.Join(", ", availableColumns.Select(c => c.QuotedSqlName)) + ")";
                values = "VALUES (" + string.Join(", ", availableColumns.Select(c => c.SqlVariableName)) + ")";
                LoadDictionaryParameters(availableColumns, parameters);
            }
            else
            {
                var availableColumns = Metadata.GetPropertiesFor(ArgumentValue.GetType(),
                     GetPropertiesFilter.ThrowOnNoMatch | GetPropertiesFilter.MutableColumns | GetPropertiesFilter.ForInsert);

                columns = "(" + string.Join(", ", availableColumns.Select(c => c.Column.QuotedSqlName)) + ")";
                values = "VALUES (" + string.Join(", ", availableColumns.Select(c => c.Column.SqlVariableName)) + ")";
                LoadParameters(availableColumns, parameters);
            }
        }
    }
}
