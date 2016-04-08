using System.Data.SqlClient;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.SqlServer.Core;
using System;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class SqlServerInsertObject.
    /// </summary>
    internal sealed class SqlServerInsertObject : SqlServerObjectCommand
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
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = Metadata.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(ArgumentValue, false);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {TableName.ToQuotedString()} (", null, ")");
            sqlBuilder.BuildSelectClause(sql, " OUTPUT ", "Inserted.", null);
            sqlBuilder.BuildValuesClause(sql, " VALUES (", ")");
            sql.Append(";");

            return new SqlServerExecutionToken(DataSource, "Insert into " + TableName, sql.ToString(), sqlBuilder.GetParameters());

        }

    }
}



