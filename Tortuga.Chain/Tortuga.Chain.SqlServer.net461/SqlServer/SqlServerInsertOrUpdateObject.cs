using System;
using System.Data.SqlClient;
using Tortuga.Chain.Formatters;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerInsertOrUpdateObject.
    /// </summary>
    public class SqlServerInsertOrUpdateObject : SqlServerObjectCommand
    {
        private readonly InsertOrUpdateOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerInsertOrUpdateObject"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public SqlServerInsertOrUpdateObject(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, object argumentValue, InsertOrUpdateOptions options) : base(dataSource, tableName, argumentValue)
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
            throw new NotImplementedException();
        }
    }
}