using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tortuga.Chain.Formatters;
using Tortuga.Chain.Metadata;

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
        /// <param name="formatter">The formatter.</param>
        /// <returns>ExecutionToken&lt;TCommandType&gt;.</returns>

        public override ExecutionToken<SqlCommand, SqlParameter> Prepare(Formatter<SqlCommand, SqlParameter> formatter)
        {
            var parameters = new List<SqlParameter>();

            string columns;
            string values;
            ColumnsAndValuesClause(out columns, out values, parameters);
            var output = OutputClause(formatter, false);
            var sql = $"INSERT INTO {TableName.ToQuotedString()} {columns} {output} {values}";

            return new ExecutionToken<SqlCommand, SqlParameter>(DataSource, "Insert into " + TableName, sql, parameters);
        }

        private void ColumnsAndValuesClause(out string columns, out string values, List<SqlParameter> parameters)
        {
            var availableColumns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), GetPropertiesFilter.ThrowOnNoMatch | GetPropertiesFilter.UpdatableOnly).Where(c => !c.Column.IsIdentity && !c.Column.IsComputed).ToList();

            columns = "(" + string.Join(", ", availableColumns.Select(c => c.Column.QuotedSqlName)) + ")";
            values = "VALUES (" + string.Join(", ", availableColumns.Select(c => c.Column.SqlVariableName)) + ")";

            foreach (var item in availableColumns)
            {
                var value = item.Property.InvokeGet(ArgumentValue) ?? DBNull.Value;
                var parameter = new SqlParameter(item.Column.SqlVariableName, value);
                if (item.Column.SqlDbType.HasValue)
                    parameter.SqlDbType = item.Column.SqlDbType.Value;
                parameters.Add(parameter);
            }
        }
    }
}



