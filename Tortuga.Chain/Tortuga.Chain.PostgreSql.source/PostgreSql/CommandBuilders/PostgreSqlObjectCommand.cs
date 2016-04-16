using Npgsql;
using NpgsqlTypes;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;



namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
    /// <summary>
    /// Base class that describes a PostgreSql database command.
    /// </summary>
    internal abstract class PostgreSqlObjectCommand : SingleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter>
    {
        private readonly PostgreSqlObjectName m_TableName;
        private readonly object m_ArgumentValue;
        private readonly TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> m_Metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlObjectCommand" /> class
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        protected PostgreSqlObjectCommand(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, object argumentValue)
            : base(dataSource)
        {
            m_TableName = tableName;
            m_ArgumentValue = argumentValue;
            m_Metadata = ((PostgreSqlDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(m_TableName);
        }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        protected PostgreSqlObjectName TableName
        {
            get { return m_TableName; }
        }

        /// <summary>
        /// Gets the argument value.
        /// </summary>
        protected object ArgumentValue
        {
            get { return m_ArgumentValue; }
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        public TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> Metadata
        {
            get { return m_Metadata; }
        }

    }
}
