using System;
using System.Data.SqlClient;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class SqlServerInsertObject.
    /// </summary>
    internal sealed class SqlServerInsertObject<TArgument> : SqlServerObjectCommand<TArgument>
        where TArgument : class
    {
        private readonly InsertOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerInsertObject{TArgument}" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public SqlServerInsertObject(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, TArgument argumentValue, InsertOptions options)
            : base(dataSource, tableName, argumentValue)
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

            var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {Table.Name.ToQuotedString()} (", null, ")");
            sqlBuilder.BuildSelectClause(sql, " OUTPUT ", "Inserted.", null);
            sqlBuilder.BuildValuesClause(sql, " VALUES (", ")");
            sql.Append(";");

            return new SqlServerExecutionToken(DataSource, "Insert into " + Table.Name, sql.ToString(), sqlBuilder.GetParameters());

        }

    }
}



