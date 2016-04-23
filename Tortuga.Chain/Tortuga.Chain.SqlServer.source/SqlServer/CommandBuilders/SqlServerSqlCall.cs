using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.SqlServer.Materializers;
namespace Tortuga.Chain.SqlServer.CommandBuilders
{

    /// <summary>
    /// Class SqlServerSqlCall.
    /// </summary>
    internal sealed class SqlServerSqlCall : MultipleTableDbCommandBuilder<SqlCommand, SqlParameter>, ISupportsChangeListener
    {
        private readonly object m_ArgumentValue;
        private readonly string m_SqlStatement;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerSqlCall" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <exception cref="ArgumentException">sqlStatement is null or empty.;sqlStatement</exception>
        public SqlServerSqlCall(SqlServerDataSourceBase dataSource, string sqlStatement, object argumentValue) : base(dataSource)
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
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
        public override CommandExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
        {
            return new SqlServerExecutionToken(DataSource, "Raw SQL call", m_SqlStatement, SqlBuilder.GetParameters<SqlParameter>(m_ArgumentValue));
        }

        /// <summary>
        /// Waits for change in the data that is returned by this operation.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns>Task that can be waited for.</returns>
        /// <remarks>This requires the use of SQL Dependency</remarks>
        public Task WaitForChange(CancellationToken cancellationToken, object state = null)
        {
            return WaitForChangeMaterializer.GenerateTask(this, cancellationToken, state);
        }

        SqlServerExecutionToken ISupportsChangeListener.Prepare(Materializer<SqlCommand, SqlParameter> materializer)
        {
            return (SqlServerExecutionToken)Prepare(materializer);
        }
    }
}

