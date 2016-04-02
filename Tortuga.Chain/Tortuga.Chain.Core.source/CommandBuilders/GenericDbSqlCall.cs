using System;
using System.Collections.Generic;
using System.Data.Common;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders
{
    class GenericDbSqlCall : MultipleTableDbCommandBuilder<DbCommand, DbParameter>
    {
        private readonly object m_ArgumentValue;
        private readonly GenericDbDataSource m_DataSource;
        private readonly string m_SqlStatement;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDbSqlCall"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <exception cref="ArgumentException">sqlStatement is null or empty.;sqlStatement</exception>
        /// <exception cref="ArgumentException">sqlStatement is null or empty.;sqlStatement</exception>
        public GenericDbSqlCall(GenericDbDataSource dataSource, string sqlStatement, object argumentValue) : base(dataSource)
        {
            if (string.IsNullOrEmpty(sqlStatement))
                throw new ArgumentException("sqlStatement is null or empty.", "sqlStatement");

            m_SqlStatement = sqlStatement;
            m_ArgumentValue = argumentValue;
            m_DataSource = dataSource;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
        public override ExecutionToken<DbCommand, DbParameter> Prepare(Materializer<DbCommand, DbParameter> materializer)
        {
            var parameters = new List<DbParameter>();

            if (m_ArgumentValue is IEnumerable<DbParameter>)
                foreach (var param in (IEnumerable<DbParameter>)m_ArgumentValue)
                    parameters.Add(param);
            else if (m_ArgumentValue is IReadOnlyDictionary<string, object>)
                foreach (var item in (IReadOnlyDictionary<string, object>)m_ArgumentValue)
                {
                    var param = m_DataSource.CreateParameter();
                    param.ParameterName = item.Key.StartsWith("@", StringComparison.OrdinalIgnoreCase) ? item.Key : "@" + item.Key;
                    param.Value = item.Value ?? DBNull.Value;
                    parameters.Add(param);
                }
            else if (m_ArgumentValue != null)
                foreach (var property in MetadataCache.GetMetadata(m_ArgumentValue.GetType()).Properties)
                {
                    var param = m_DataSource.CreateParameter();
                    param.ParameterName = "@" + property.MappedColumnName;
                    param.Value = property.InvokeGet(m_ArgumentValue) ?? DBNull.Value;
                    parameters.Add(param);
                }

            return new ExecutionToken<DbCommand, DbParameter>(DataSource, "Raw SQL call", m_SqlStatement, parameters);
        }



    }
}
