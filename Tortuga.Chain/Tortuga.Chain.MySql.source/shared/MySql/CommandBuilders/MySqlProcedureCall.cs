using MySql.Data.MySqlClient;
using System;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql.CommandBuilders
{

    /// <summary>
    /// Class MySqlProcedureCall.
    /// </summary>
    internal sealed class MySqlProcedureCall : MultipleTableDbCommandBuilder<MySqlCommand, MySqlParameter>
    {

        readonly object m_ArgumentValue;
        readonly StoredProcedureMetadata<MySqlObjectName, MySqlDbType> m_Procedure;
        //readonly MySqlObjectName m_ProcedureName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlProcedureCall"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="argumentValue">The argument value.</param>
        internal MySqlProcedureCall(MySqlDataSourceBase dataSource, MySqlObjectName procedureName, object argumentValue = null) : base(dataSource)
        {
            if (procedureName == MySqlObjectName.Empty)
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
        public override CommandExecutionToken<MySqlCommand, MySqlParameter> Prepare(Materializer<MySqlCommand, MySqlParameter> materializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public new MySqlDataSourceBase DataSource
        {
            get { return (MySqlDataSourceBase)base.DataSource; }
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




