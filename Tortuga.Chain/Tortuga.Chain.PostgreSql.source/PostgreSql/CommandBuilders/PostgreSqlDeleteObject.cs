using Npgsql;
using System;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
    /// <summary>
    /// Command object that represents a delete operation.
    /// </summary>
    internal sealed class PostgreSqlDeleteObject<TArgument> : PostgreSqlObjectCommand<TArgument>
        where TArgument : class
    {
        private readonly DeleteOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDeleteObject"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="table">The table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public PostgreSqlDeleteObject(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName table, TArgument argumentValue, DeleteOptions options)
            : base(dataSource, table, argumentValue)
        {
            m_Options = options;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns><see cref="PostgreSqlExecutionToken" /></returns>
        public override ExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = Metadata.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            //Use the RETURNING clause for output similar to SQL Server's OUTPUT clause: http://www.postgresql.org/docs/current/static/sql-delete.html

            return new PostgreSqlExecutionToken(DataSource, "Delete from " + TableName, sql.ToString(), sqlBuilder.GetParameters());
        }
    }
}
