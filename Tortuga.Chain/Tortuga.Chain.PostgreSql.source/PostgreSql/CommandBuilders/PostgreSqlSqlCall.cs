using Npgsql;
using System;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
    /// <summary>
    /// Class PostgreSqlSqlCall
    /// </summary>
    public class PostgreSqlSqlCall : MultipleTableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter>
    {
        private readonly object m_ArgumentValue;
        private readonly string m_SqlStatement;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlSqlCall"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <exception cref="ArgumentException">sqlStatement is null or empty.;sqlStatement</exception>
        public PostgreSqlSqlCall(PostgreSqlDataSourceBase dataSource, string sqlStatement, object argumentValue) : base(dataSource)
        {
            if (string.IsNullOrEmpty(sqlStatement))
                throw new ArgumentException("sqlStatement is null or empty.", "sqlStatement");

            m_SqlStatement = sqlStatement;
            m_ArgumentValue = argumentValue;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>
        /// ExecutionToken&lt;TCommand&gt;.
        /// </returns>
        public override ExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
        {
            return new ExecutionToken<NpgsqlCommand, NpgsqlParameter>(DataSource, "Raw SQL Call", m_SqlStatement, SqlBuilder.GetParameters<NpgsqlParameter>(m_ArgumentValue));
        }
    }
}
