using Oracle.ManagedDataAccess.Client;
using System;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Oracle.CommandBuilders
{
    /// <summary>
    /// Class OracleSqlCall
    /// </summary>
    public class OracleSqlCall : MultipleTableDbCommandBuilder<OracleCommand, OracleParameter>
    {
        readonly object m_ArgumentValue;
        readonly string m_SqlStatement;

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleSqlCall"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <exception cref="ArgumentException">sqlStatement is null or empty.;sqlStatement</exception>
        public OracleSqlCall(OracleDataSourceBase dataSource, string sqlStatement, object argumentValue) : base(dataSource)
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
        public override CommandExecutionToken<OracleCommand, OracleParameter> Prepare(Materializer<OracleCommand, OracleParameter> materializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <remarks>
        /// If the column name was not found, this will return null
        /// </remarks>
        public override ColumnMetadata TryGetColumn(string columnName)
        {
            return null;
        }
    }
}
