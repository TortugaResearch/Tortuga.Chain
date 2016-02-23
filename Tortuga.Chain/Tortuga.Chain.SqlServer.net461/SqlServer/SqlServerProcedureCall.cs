using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Formatters;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.SqlServer
{

    /// <summary>
    /// Class SqlServerProcedureCall.
    /// </summary>
    public class SqlServerProcedureCall : MultipleTableDbCommandBuilder<SqlCommand, SqlParameter>
    {

        private readonly object m_ArgumentValue;
        private readonly StoredProcedureMetadata<SqlServerObjectName> m_Metadata;
        private readonly SqlServerObjectName m_ProcedureName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerProcedureCall"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="argumentValue">The argument value.</param>
        internal SqlServerProcedureCall(SqlServerDataSourceBase dataSource, SqlServerObjectName procedureName, object argumentValue = null) : base(dataSource)
        {
            if (procedureName == SqlServerObjectName.Empty)
                throw new ArgumentException($"{nameof(procedureName)} is empty", nameof(procedureName));

            m_ArgumentValue = argumentValue;
            m_ProcedureName = procedureName;
            m_Metadata = ((SqlServerDataSourceBase)DataSource).DatabaseMetadata.GetStoredProcedure(procedureName);
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <returns>ExecutionToken&lt;TCommandType&gt;.</returns>
        public override ExecutionToken<SqlCommand, SqlParameter> Prepare(Formatter<SqlCommand, SqlParameter> formatter)
        {
            var parameters = new List<SqlParameter>();
            var expectedParameters = m_Metadata.Parameters.ToDictionary(p => p.ClrName, StringComparer.InvariantCultureIgnoreCase);

            if (m_ArgumentValue is IEnumerable<SqlParameter>)
            {
                foreach (var param in (IEnumerable<SqlParameter>)m_ArgumentValue)
                    parameters.Add(param);
            }
            else if (m_ArgumentValue is IReadOnlyDictionary<string, object>)
            {
                foreach (var item in (IReadOnlyDictionary<string, object>)m_ArgumentValue)
                {
                    ParameterMetadata paramInfo;
                    if (expectedParameters.TryGetValue(item.Key, out paramInfo))
                    {
                        var newSqlParameter = new SqlParameter("@" + item.Key, item.Value ?? DBNull.Value);
                        if (paramInfo.SqlDbType.HasValue)
                            newSqlParameter.SqlDbType = paramInfo.SqlDbType.Value;
                        parameters.Add(newSqlParameter);
                    }
                }
            }
            else if (m_ArgumentValue != null)
            {
                foreach (var property in MetadataCache.GetMetadata(m_ArgumentValue.GetType()).Properties)
                {
                    ParameterMetadata paramInfo;
                    if (expectedParameters.TryGetValue(property.MappedColumnName, out paramInfo))
                    {
                        var newSqlParameter = new SqlParameter("@" + property.MappedColumnName, property.InvokeGet(m_ArgumentValue) ?? DBNull.Value);
                        if (paramInfo.SqlDbType.HasValue)
                            newSqlParameter.SqlDbType = paramInfo.SqlDbType.Value;
                        parameters.Add(newSqlParameter);
                    }
                }
            }

            return new ExecutionToken<SqlCommand, SqlParameter>(DataSource, m_ProcedureName.ToString(), m_ProcedureName.ToQuotedString(), parameters, CommandType.StoredProcedure);
        }
    }

}




