using Oracle.ManagedDataAccess.Client;
using System;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Oracle.CommandBuilders
{

    /// <summary>
    /// Class OracleProcedureCall.
    /// </summary>
    internal sealed class OracleProcedureCall : MultipleTableDbCommandBuilder<OracleCommand, OracleParameter>
    {

        readonly object m_ArgumentValue;
        readonly StoredProcedureMetadata<OracleObjectName, OracleDbType> m_Procedure;
        //readonly OracleObjectName m_ProcedureName;

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleProcedureCall"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="argumentValue">The argument value.</param>
        internal OracleProcedureCall(OracleDataSourceBase dataSource, OracleObjectName procedureName, object argumentValue = null) : base(dataSource)
        {
            if (procedureName == OracleObjectName.Empty)
                throw new ArgumentException($"{nameof(procedureName)} is empty", nameof(procedureName));

            m_ArgumentValue = argumentValue;
            //m_ProcedureName = procedureName;
            m_Procedure = DataSource.DatabaseMetadata.GetStoredProcedure(procedureName);
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
        public override CommandExecutionToken<OracleCommand, OracleParameter> Prepare(Materializer<OracleCommand, OracleParameter> materializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public new OracleDataSourceBase DataSource
        {
            get { return (OracleDataSourceBase)base.DataSource; }
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




