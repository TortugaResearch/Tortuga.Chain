using Npgsql;
using System;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;



namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
    /// <summary>
    /// Class PostgreSqlInsertOrUpdateObject
    /// </summary>
    internal sealed class PostgreSqlInsertOrUpdateObject<TArgument> : PostgreSqlObjectCommand<TArgument>
        where TArgument : class
    {
        private readonly UpsertOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlInsertOrUpdateObject"/> class.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        public PostgreSqlInsertOrUpdateObject(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, TArgument argumentValue, UpsertOptions options)
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

            //Looks like ON CONFLICT is useful here http://www.postgresql.org/docs/current/static/sql-insert.html
            //Use RETURNING in place of SQL Servers OUTPUT clause http://www.postgresql.org/docs/current/static/sql-insert.html

            return new PostgreSqlExecutionToken(DataSource, "Insert or update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters());

        }

    }
}
