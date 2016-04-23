using Npgsql;
using System;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
    /// <summary>
    /// Command object that represents an update operation.
    /// </summary>
    internal sealed class PostgreSqlUpdateObject<TArgument> : PostgreSqlObjectCommand<TArgument>
        where TArgument : class
    {
        private readonly UpdateOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlUpdateObject"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public PostgreSqlUpdateObject(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, TArgument argumentValue, UpdateOptions options)
            : base(dataSource, tableName, argumentValue)
        {
            m_Options = options;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns><see cref="PostgreSqlExecutionToken" /></returns>
        public override CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();

            //Use a preceeding SELECT statement for the old values
            //Use RETURNING in place of SQL Servers OUTPUT clause for new values http://www.postgresql.org/docs/current/static/sql-update.html


            return new PostgreSqlExecutionToken(DataSource, "Update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters());
        }

    }
}
