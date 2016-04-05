using System.Data.SqlClient;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.SqlServer.Core;
using System;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class SqlServerDeleteObject.
    /// </summary>
    internal sealed class SqlServerDeleteObject : SqlServerObjectCommand
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
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>

        public override ExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = Metadata.CreateSqlBuilder();
            sqlBuilder.ApplyArgumentValue(ArgumentValue, m_Options.HasFlag(DeleteOptions.UseKeyAttribute), DataSource.StrictMode);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns(), DataSource.StrictMode);

            var sql = new StringBuilder();
            sql.Append("DELETE FROM " + Metadata.Name.ToQuotedString());
            sqlBuilder.BuildSelectClause(sql, " OUTPUT ", "Deleted.", null);
            sqlBuilder.BuildWhereClause(sql, " WHERE ", null);
            sql.Append(";");

            return new SqlServerExecutionToken(DataSource, "Delete from " + TableName, sql.ToString(), sqlBuilder.GetParameters());
        }


    }
}

