using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.Core;
using Tortuga.Chain.SqlServer.Materializers;
namespace Tortuga.Chain.SqlServer.CommandBuilders
{

    /// <summary>
    /// Class SqlServerProcedureCall.
    /// </summary>
    public class SqlServerProcedureCall : MultipleTableDbCommandBuilder<SqlCommand, SqlParameter>, ISupportsChangeListener
    {

        private readonly object m_ArgumentValue;
        private readonly StoredProcedureMetadata<SqlServerObjectName, SqlDbType> m_Metadata;
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
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
        public override ExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException("materializer", "materializer is null.");

            var parameters = new List<SqlParameter>();
            var expectedParameters = m_Metadata.Parameters.ToDictionary(p => p.ClrName, StringComparer.OrdinalIgnoreCase);

            if (m_ArgumentValue is IEnumerable<SqlParameter>)
            {
                foreach (var param in (IEnumerable<SqlParameter>)m_ArgumentValue)
                    parameters.Add(param);
            }
            else if (m_ArgumentValue is IReadOnlyDictionary<string, object>)
            {
                foreach (var item in (IReadOnlyDictionary<string, object>)m_ArgumentValue)
                {
                    ParameterMetadata<SqlDbType> paramInfo;
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
                    ParameterMetadata<SqlDbType> paramInfo;
                    if (expectedParameters.TryGetValue(property.MappedColumnName, out paramInfo))
                    {
                        var newSqlParameter = new SqlParameter("@" + property.MappedColumnName, property.InvokeGet(m_ArgumentValue) ?? DBNull.Value);
                        if (paramInfo.SqlDbType.HasValue)
                            newSqlParameter.SqlDbType = paramInfo.SqlDbType.Value;
                        parameters.Add(newSqlParameter);
                    }
                }
            }

            return new SqlServerExecutionToken(DataSource, m_ProcedureName.ToString(), m_ProcedureName.ToQuotedString(), parameters, CommandType.StoredProcedure);
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




