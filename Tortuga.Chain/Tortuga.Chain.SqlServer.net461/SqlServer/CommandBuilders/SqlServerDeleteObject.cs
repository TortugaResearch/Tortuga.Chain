using System.Collections.Generic;
using System.Data.SqlClient;
using Tortuga.Chain.Formatters;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class SqlServerDeleteObject.
    /// </summary>
    public class SqlServerDeleteObject : SqlServerObjectCommand
    {
        private readonly DeleteOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDeleteObject"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public SqlServerDeleteObject(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, object argumentValue, DeleteOptions options) : base(dataSource, tableName, argumentValue)
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

            var where = WhereClause(parameters, m_Options.HasFlag(DeleteOptions.UseKeyAttribute));
            var output = OutputClause(formatter, true);
            var sql = $"DELETE FROM {TableName.ToQuotedString()} {output} WHERE {where}";

            return new ExecutionToken<SqlCommand, SqlParameter>(DataSource, "Delete from " + TableName, sql, parameters);
        }


    }
}

