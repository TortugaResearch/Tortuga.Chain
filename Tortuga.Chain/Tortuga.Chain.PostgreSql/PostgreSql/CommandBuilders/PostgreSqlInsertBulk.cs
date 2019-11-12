using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
    /// <summary>
    /// This class is used to perform bulk inserts
    /// </summary>
    public sealed class PostgreSqlInsertBulk : DbOperationBuilder<NpgsqlConnection, NpgsqlTransaction>
    {
        readonly PostgreSqlDataSourceBase m_DataSource;
        readonly IDataReader m_Source;
        readonly TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> m_Table;
        int? m_BatchSize;

        internal PostgreSqlInsertBulk(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, DataTable dataTable) : base(dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable), $"{nameof(dataTable)} is null.");

            m_DataSource = dataSource;
            m_Source = dataTable.CreateDataReader();
            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            if (!m_Table.IsTable)
                throw new MappingException($"Cannot perform a bulk insert into the view {m_Table.Name}");
        }

        internal PostgreSqlInsertBulk(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, IDataReader dataReader) : base(dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");
            if (dataReader == null)
                throw new ArgumentNullException(nameof(dataReader), $"{nameof(dataReader)} is null.");

            m_DataSource = dataSource;
            m_Source = dataReader;
            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            if (!m_Table.IsTable)
                throw new MappingException($"Cannot perform a bulk insert into the view {m_Table.Name}");
        }

        /// <summary>
        /// Modifies the batch size.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        /// <returns>SqlServerInsertBulk.</returns>
        public PostgreSqlInsertBulk WithBatchSize(int batchSize)
        {
            m_BatchSize = batchSize;
            return this;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
        public override OperationExecutionToken<NpgsqlConnection, NpgsqlTransaction> Prepare()
        {
            return new OperationExecutionToken<NpgsqlConnection, NpgsqlTransaction>(m_DataSource, "Bulk Insert into " + m_Table.Name);
        }

        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <remarks>
        /// If the column name was not found, this will return null
        /// </remarks>
        public override ColumnMetadata? TryGetColumn(string columnName) => m_Table.Columns.TryGetColumn(columnName);

        /// <summary>
        /// Returns a list of columns known to be non-nullable.
        /// </summary>
        /// <returns>
        /// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
        /// </returns>
        /// <remarks>
        /// This is used by materializers to skip IsNull checks.
        /// </remarks>
        public override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => m_Table.NonNullableColumns;

        /// <summary>
        /// Implementation the specified operation.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
        protected override int? Implementation(NpgsqlConnection connection, NpgsqlTransaction? transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation the specified operation.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;System.Nullable&lt;System.Int32&gt;&gt;.</returns>
        protected override async Task<int?> ImplementationAsync(NpgsqlConnection connection, NpgsqlTransaction? transaction, CancellationToken cancellationToken)
        {
            await Task.Delay(0).ConfigureAwait(false);
            throw new NotImplementedException();
        }
    }
}
