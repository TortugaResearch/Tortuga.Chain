using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tortuga.Chain.Formatters;

namespace Tortuga.Chain.SqlServer
{

    /// <summary>
    /// Class SqlServerProcedureCall.
    /// </summary>
    public class SqlServerProcedureCall : SqlServerDbCommandBuilder
    {

        private readonly object m_ArgumentValue;
        private readonly SqlServerObjectName m_ProcedureName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerProcedureCall"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        internal SqlServerProcedureCall(SqlServerDataSourceBase dataSource, SqlServerObjectName procedureName) : base(dataSource)
        {
            m_ProcedureName = procedureName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerProcedureCall"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="argumentValue">The argument value.</param>
        internal SqlServerProcedureCall(SqlServerDataSourceBase dataSource, SqlServerObjectName procedureName, object argumentValue) : base(dataSource)
        {
            if (procedureName == SqlServerObjectName.Empty)
                throw new ArgumentException($"{nameof(procedureName)} is empty", nameof(procedureName));

            m_ArgumentValue = argumentValue;
            m_ProcedureName = procedureName;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <returns>ExecutionToken&lt;TCommandType&gt;.</returns>
        public override ExecutionToken<SqlCommand, SqlParameter> Prepare(Formatter<SqlCommand, SqlParameter> formatter)
        {
            var parameters = new List<SqlParameter>();

            if (m_ArgumentValue != null)
            {
                if (m_ArgumentValue is IEnumerable<SqlParameter>)
                {
                    foreach (var param in (IEnumerable<SqlParameter>)m_ArgumentValue)
                        parameters.Add(param);
                }
                else if (m_ArgumentValue is IReadOnlyDictionary<string, object>)
                {
                    foreach (var item in (IReadOnlyDictionary<string, object>)m_ArgumentValue)
                        parameters.Add(new SqlParameter(item.Key, item.Value));
                }
                else
                {
                    //TODO: Create parameters from argumentValue;
                    throw new NotImplementedException();
                }
            }

            return new ExecutionToken<SqlCommand, SqlParameter>(DataSource, m_ProcedureName.ToString(), m_ProcedureName.ToQuotedString(), parameters, CommandType.StoredProcedure);
        }
    }

}




