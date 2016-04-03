using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class SqlServerObjectCommand.
    /// </summary>
    internal abstract class SqlServerObjectCommand : SingleRowDbCommandBuilder<SqlCommand, SqlParameter>
    {
        private readonly IReadOnlyDictionary<string, object> m_ArgumentDictionary;
        private readonly object m_ArgumentValue;
        private readonly TableOrViewMetadata<SqlServerObjectName, SqlDbType> m_Metadata;
        private readonly SqlServerObjectName m_TableName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerObjectCommand" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        protected SqlServerObjectCommand(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, object argumentValue)
            : base(dataSource)
        {
            m_ArgumentValue = argumentValue;
            m_ArgumentDictionary = ArgumentValue as IReadOnlyDictionary<string, object>;
            m_TableName = tableName;
            m_Metadata = DataSource.DatabaseMetadata.GetTableOrView(m_TableName);
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        protected SqlServerObjectName TableName
        {
            get { return m_TableName; }
        }

        /// <summary>
        /// Gets the argument value.
        /// </summary>
        /// <value>The argument value.</value>
        public object ArgumentValue
        {
            get { return m_ArgumentValue; }
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public TableOrViewMetadata<SqlServerObjectName, SqlDbType> Metadata
        {
            get { return m_Metadata; }
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public new SqlServerDataSourceBase DataSource
        {
            get { return (SqlServerDataSourceBase)base.DataSource; }
        }


    }
}
