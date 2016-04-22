using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    public sealed class SqlServerInsertBulk : DbOperationBuilder<SqlConnection, SqlTransaction>
    {
        private readonly IDataReader m_Source;
        private readonly SqlServerDataSourceBase m_DataSource;
        private readonly SqlBulkCopyOptions m_Options;
        private readonly TableOrViewMetadata<SqlServerObjectName, SqlDbType> m_Table;
        private readonly int? m_BatchSize;
        private bool m_EnableStreaming;

        internal SqlServerInsertBulk(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, DataTable dataTable, SqlBulkCopyOptions options, int? batchSize) : base(dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable), $"{nameof(dataTable)} is null.");

            m_DataSource = dataSource;
            m_Source = dataTable.CreateDataReader();
            m_Options = options;
            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            if (!m_Table.IsTable)
                throw new MappingException($"Cannot perform a bulk insert into the view {m_Table.Name}");
            m_BatchSize = batchSize;
        }

        internal SqlServerInsertBulk(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, IDataReader dataReader, SqlBulkCopyOptions options, int? batchSize) : base(dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");
            if (dataReader == null)
                throw new ArgumentNullException(nameof(dataReader), $"{nameof(dataReader)} is null.");

            m_DataSource = dataSource;
            m_Source = dataReader;
            m_Options = options;
            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            if (!m_Table.IsTable)
                throw new MappingException($"Cannot perform a bulk insert into the view {m_Table.Name}");
            m_BatchSize = batchSize;
        }

        public override OperationExecutionToken<SqlConnection, SqlTransaction> Prepare()
        {
            return new OperationExecutionToken<SqlConnection, SqlTransaction>(m_DataSource, "Bulk Insert into " + m_Table.Name);
        }


        protected override int? Implementation(SqlConnection connection, SqlTransaction transaction)
        {
            var bcp = new SqlBulkCopy(connection, m_Options, transaction);
            bcp.DestinationTableName = m_Table.Name.ToQuotedString();
            if (m_BatchSize.HasValue)
                bcp.BatchSize = m_BatchSize.Value;
            bcp.EnableStreaming = m_EnableStreaming;

            bcp.WriteToServer(m_Source);
            return m_Source.RecordsAffected;
        }

        protected override async Task<int?> ImplementationAsync(SqlConnection connection, SqlTransaction transaction, CancellationToken cancellationToken)
        {
            var bcp = new SqlBulkCopy(connection, m_Options, transaction);
            bcp.DestinationTableName = m_Table.Name.ToQuotedString();
            if (m_BatchSize.HasValue)
                bcp.BatchSize = m_BatchSize.Value;
            bcp.EnableStreaming = m_EnableStreaming;

            await bcp.WriteToServerAsync(m_Source, cancellationToken);
            return m_Source.RecordsAffected;
        }

        /// <summary>
        /// Enables streaming.
        /// </summary>
        /// <returns>SqlServerInsertBulk.</returns>
        /// <remarks>When EnableStreaming is true, SqlBulkCopy reads from an IDataReader object using SequentialAccess, optimizing memory usage by using the IDataReader streaming capabilities. When it’s set to false, the SqlBulkCopy class loads all the data returned by the IDataReader object into memory before sending it to SQL Server or SQL Azure.</remarks>
        public SqlServerInsertBulk WithStreaming()
        {
            m_EnableStreaming = true;
            return this;
        }
    }
}
