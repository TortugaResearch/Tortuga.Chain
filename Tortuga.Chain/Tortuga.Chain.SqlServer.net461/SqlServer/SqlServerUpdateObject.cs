using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Tortuga.Chain.Formatters;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerUpdateObject.
    /// </summary>
    public class SqlServerUpdateObject : SqlServerObjectCommand
    {
        private readonly UpdateOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerUpdateObject"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public SqlServerUpdateObject(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, object argumentValue, UpdateOptions options) : base(dataSource, tableName, argumentValue)
        {
            m_Options = options;

        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <returns>ExecutionToken&lt;TCommandType&gt;.</returns>

        public override ExecutionToken<SqlCommand, SqlParameter> Prepare(Formatter<SqlCommand, SqlParameter> formatter)
        {
            var parameters = new List<SqlParameter>();

            var set = SetClause(parameters);
            var output = OutputClause(formatter, m_Options.HasFlag(UpdateOptions.ReturnOldValues));
            var where = WhereClause(parameters, m_Options.HasFlag(UpdateOptions.UseKeyAttribute));

            var sql = $"UPDATE {TableName.ToQuotedString()} {set} {output} WHERE {where}";

            return new ExecutionToken<SqlCommand, SqlParameter>(DataSource, "Update " + TableName, sql, parameters);
        }

        private string SetClause(List<SqlParameter> parameters)
        {
            var filter = GetPropertiesFilter.ThrowOnNoMatch | GetPropertiesFilter.UpdatableOnly;

            if (m_Options.HasFlag(UpdateOptions.UseKeyAttribute))
                filter = filter | GetPropertiesFilter.ObjectDefinedNonKey;
            else
                filter = filter | GetPropertiesFilter.NonPrimaryKey;

            if (DataSource.StrictMode)
                filter = filter | GetPropertiesFilter.ThrowOnMissingColumns;

            var availableColumns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), filter);

            var result = "SET " + string.Join(", ", availableColumns.Select(c => $"{c.Column.QuotedSqlName} = {c.Column.SqlVariableName}"));

            foreach (var item in availableColumns)
            {
                var value = item.Property.InvokeGet(ArgumentValue) ?? DBNull.Value;
                var parameter = new SqlParameter(item.Column.SqlVariableName, value);
                if (item.Column.SqlDbType.HasValue)
                    parameter.SqlDbType = item.Column.SqlDbType.Value;
                parameters.Add(parameter);
            }

            return result;
        }
    }
}

