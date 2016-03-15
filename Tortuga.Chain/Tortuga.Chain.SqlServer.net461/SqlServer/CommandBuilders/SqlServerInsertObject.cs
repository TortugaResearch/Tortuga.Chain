using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.Core;
namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class SqlServerInsertObject.
    /// </summary>
    public class SqlServerInsertObject : SqlServerObjectCommand
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerInsertObject"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        public SqlServerInsertObject(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, object argumentValue) : base(dataSource, tableName, argumentValue) { }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>

        public override ExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
        {
            var parameters = new List<SqlParameter>();

            string columns;
            string values;
            ColumnsAndValuesClause(out columns, out values, parameters);
            var output = OutputClause(materializer, false);
            var sql = $"INSERT INTO {TableName.ToQuotedString()} {columns} {output} {values}";

            return new SqlServerExecutionToken(DataSource, "Insert into " + TableName, sql, parameters);
        }

        private void ColumnsAndValuesClause(out string columns, out string values, List<SqlParameter> parameters)
        {
            if (ArgumentDictionary != null)
            {
                var availableColumns = Metadata.GetKeysFor(ArgumentDictionary, GetKeysFilter.ThrowOnNoMatch | GetKeysFilter.MutableColumns).Where(c => !c.IsIdentity && !c.IsComputed).ToList();

                columns = "(" + string.Join(", ", availableColumns.Select(c => c.QuotedSqlName)) + ")";
                values = "VALUES (" + string.Join(", ", availableColumns.Select(c => c.SqlVariableName)) + ")";

                foreach (var item in availableColumns)
                {
                    var value = ArgumentDictionary[item.ClrName] ?? DBNull.Value;
                    var parameter = new SqlParameter(item.SqlVariableName, value);
                    if (item.DbType.HasValue)
                        parameter.SqlDbType = item.DbType.Value;
                    parameters.Add(parameter);
                }
            }
            else
            {
                var availableColumns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), GetPropertiesFilter.ThrowOnNoMatch | GetPropertiesFilter.MutableColumns | GetPropertiesFilter.ForInsert).Where(c => !c.Column.IsIdentity && !c.Column.IsComputed).ToList();

                columns = "(" + string.Join(", ", availableColumns.Select(c => c.Column.QuotedSqlName)) + ")";
                values = "VALUES (" + string.Join(", ", availableColumns.Select(c => c.Column.SqlVariableName)) + ")";

                foreach (var item in availableColumns)
                {
                    var value = item.Property.InvokeGet(ArgumentValue) ?? DBNull.Value;
                    var parameter = new SqlParameter(item.Column.SqlVariableName, value);
                    if (item.Column.DbType.HasValue)
                        parameter.SqlDbType = item.Column.DbType.Value;
                    parameters.Add(parameter);
                }
            }
        }
    }
}



