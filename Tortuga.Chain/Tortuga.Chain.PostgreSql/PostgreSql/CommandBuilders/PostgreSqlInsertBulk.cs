using Npgsql;
using NpgsqlTypes;
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
		readonly TableOrViewMetadata<NpgsqlParameter, PostgreSqlObjectName, NpgsqlDbType> m_Table;
		EventHandler<AbortableOperationEventArgs>? m_EventHandler;
		int? m_NotifyAfter;

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
		/// After notifyAfter records, the event handler may be fired. This can be used to abort the bulk insert.
		/// </summary>
		/// <param name="eventHandler">The event handler.</param>
		/// <param name="notifyAfter">The notify after. This should be a multiple of the batch size.</param>
		public PostgreSqlInsertBulk WithNotifications(EventHandler<AbortableOperationEventArgs> eventHandler, int notifyAfter)
		{
			if (eventHandler == null)
				throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");
			if (notifyAfter <= 0)
				throw new ArgumentException($"{nameof(notifyAfter)} must be greater than 0.", nameof(notifyAfter));
			m_EventHandler = eventHandler;
			m_NotifyAfter = notifyAfter;
			return this;
		}

		/// <summary>
		/// Implementation the specified operation.
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <param name="transaction">The transaction.</param>
		/// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
		protected override int? Implementation(NpgsqlConnection connection, NpgsqlTransaction? transaction)
		{
			if (connection == null)
				throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");

			var rowCount = 0;
			var columns = SetupColumns();
			var sql = SetupSql(columns);
			var nextNotification = m_NotifyAfter;

			using var writer = connection.BeginBinaryImport(sql);
			{
				while (m_Source.Read())
				{
					writer.StartRow();

					foreach (var column in columns)
					{
						if (column.Column.DbType.HasValue)
							writer.Write(m_Source.GetValue(column.ColumnIndex), column.Column.DbType.Value);
						else
							writer.Write(m_Source.GetValue(column.ColumnIndex));
					}

					rowCount++;

					if (rowCount == nextNotification)
					{
						var e = new AbortableOperationEventArgs(rowCount);
						m_EventHandler?.Invoke(this, e);

						nextNotification += m_NotifyAfter;

						if (e.Abort)
						{
							writer.Complete();
							throw new TaskCanceledException("Bulk insert operation aborted.");
						}
					}
				}
				writer.Complete();
			}
			return rowCount;
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
			if (connection == null)
				throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");

			var rowCount = 0;
			var columns = SetupColumns();
			var sql = SetupSql(columns);
			var nextNotification = m_NotifyAfter;

			using (var writer = connection.BeginBinaryImport(sql))
			{
				while (m_Source.Read())
				{
					await writer.StartRowAsync(cancellationToken).ConfigureAwait(false);

					foreach (var column in columns)
					{
						if (column.Column.DbType.HasValue)
							await writer.WriteAsync(m_Source.GetValue(column.ColumnIndex), column.Column.DbType.Value, cancellationToken).ConfigureAwait(false);
						else
							await writer.WriteAsync(m_Source.GetValue(column.ColumnIndex), cancellationToken).ConfigureAwait(false);
					}

					rowCount++;

					if (rowCount == nextNotification)
					{
						var e = new AbortableOperationEventArgs(rowCount);
						m_EventHandler?.Invoke(this, e);

						nextNotification += m_NotifyAfter;

						if (e.Abort)
						{
							writer.Complete();
							throw new TaskCanceledException("Bulk insert operation aborted.");
						}
					}
				}
				await writer.CompleteAsync(cancellationToken).ConfigureAwait(false);
			}
			return rowCount;
		}

		List<(int ColumnIndex, ColumnMetadata<NpgsqlDbType> Column)> SetupColumns()
		{
			var mappedColumns = new List<(int ColumnIndex, ColumnMetadata<NpgsqlDbType> Column)>(m_Source.FieldCount);

			for (var i = 0; i < m_Source.FieldCount; i++)
			{
				var column = m_Table.Columns.TryGetColumn(m_Source.GetName(i));

				if (column != null)
				{
					if (!column.IsIdentity) //implicitly skip identity columns
					{
						mappedColumns.Add((i, column));
					}
				}
				else if (StrictMode)
					throw new MappingException($"Could not find column on {m_Table.Name.ToString()} that matches property {m_Source.GetName(i)}");
			}
			if (mappedColumns.Count == 0)
				throw new MappingException($"Could not find any properties that map to columns on the table {m_Table.Name.ToString()}");

			return mappedColumns;
		}

		string SetupSql(List<(int ColumnIndex, ColumnMetadata<NpgsqlDbType> Column)> columns)
		{
			var columnList = string.Join(",", columns.Select(c => c.Column.QuotedSqlName));
			return $"copy {m_Table.Name.ToQuotedString()}({columnList}) from STDIN (FORMAT BINARY)";
		}
	}
}
