using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

#if !SqlDependency_Missing
using Tortuga.Chain.SqlServer.Materializers;
#endif

namespace Tortuga.Chain.SqlServer.CommandBuilders
{

    /// <summary>
    /// Class SqlServerSqlCall.
    /// </summary>
    internal sealed partial class SqlServerSqlCall : MultipleTableDbCommandBuilder<SqlCommand, SqlParameter>
    {
        readonly object m_ArgumentValue;
        readonly string m_SqlStatement;

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
            return new SqlServerCommandExecutionToken(DataSource, "Raw SQL call", m_SqlStatement, SqlBuilder.GetParameters<SqlParameter>(m_ArgumentValue));
        }


        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <remarks>
        /// If the column name was not found, this will return null
        /// </remarks>
        public override ColumnMetadata TryGetColumn(string columnName) => null;

        /// <summary>
        /// Returns a list of columns known to be non-nullable.
        /// </summary>
        /// <returns>
        /// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
        /// </returns>
        /// <remarks>
        /// This is used by materializers to skip IsNull checks.
        /// </remarks>
        public override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => ImmutableList<ColumnMetadata>.Empty;

    }


#if !SqlDependency_Missing
    partial class SqlServerSqlCall : ISupportsChangeListener
    {
        SqlServerCommandExecutionToken ISupportsChangeListener.Prepare(Materializer<SqlCommand, SqlParameter> materializer)
        {
            return (SqlServerCommandExecutionToken)Prepare(materializer);
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
    }
#endif

}

