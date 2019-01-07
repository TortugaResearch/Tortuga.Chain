#if !OleDb_Missing
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{

    /// <summary>
    /// Class OleDbSqlServerProcedureCall.
    /// </summary>
    internal sealed partial class OleDbSqlServerProcedureCall : MultipleTableDbCommandBuilder<OleDbCommand, OleDbParameter>
    {

        readonly object m_ArgumentValue;
        readonly StoredProcedureMetadata<SqlServerObjectName, OleDbType> m_Procedure;

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbSqlServerProcedureCall"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="argumentValue">The argument value.</param>
        internal OleDbSqlServerProcedureCall(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName procedureName, object argumentValue = null) : base(dataSource)
        {
            if (procedureName == SqlServerObjectName.Empty)
                throw new ArgumentException($"{nameof(procedureName)} is empty", nameof(procedureName));

            m_ArgumentValue = argumentValue;
            m_Procedure = DataSource.DatabaseMetadata.GetStoredProcedure(procedureName);
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
        public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException("materializer", "materializer is null.");

            List<OleDbParameter> parameters;

            if (m_ArgumentValue is IEnumerable<OleDbParameter>)
            {
                parameters = ((IEnumerable<OleDbParameter>)m_ArgumentValue).ToList();
            }
            else
            {
                var sqlBuilder = m_Procedure.CreateSqlBuilder(StrictMode);
                sqlBuilder.ApplyArgumentValue(DataSource, OperationTypes.None, m_ArgumentValue);
                parameters = sqlBuilder.GetParameters();
            }

            return new OleDbCommandExecutionToken(DataSource, m_Procedure.Name.ToString(), m_Procedure.Name.ToQuotedString(), parameters, CommandType.StoredProcedure);

        }


        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public new OleDbSqlServerDataSourceBase DataSource
        {
            get { return (OleDbSqlServerDataSourceBase)base.DataSource; }
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


}




#endif