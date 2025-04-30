using MySqlConnector;
using System.Globalization;
using System.IO;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql.CommandBuilders;

/// <summary>
/// This class is used to perform bulk inserts
/// </summary>
public sealed class MySqlInsertBulk : DbOperationBuilder<MySqlConnection, MySqlTransaction>
{
	readonly MySqlDataSourceBase m_DataSource;
	readonly IDataReader m_Source;
	readonly TableOrViewMetadata<MySqlObjectName, MySqlDbType> m_Table;
	int? m_BatchSize;
	EventHandler<AbortableOperationEventArgs>? m_EventHandler;
	int? m_NotifyAfter;

	internal MySqlInsertBulk(MySqlDataSourceBase dataSource, MySqlObjectName tableName, DataTable dataTable) : base(dataSource)
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

	internal MySqlInsertBulk(MySqlDataSourceBase dataSource, MySqlObjectName tableName, IDataReader dataReader) : base(dataSource)
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
	public override OperationExecutionToken<MySqlConnection, MySqlTransaction> Prepare()
	{
		return new OperationExecutionToken<MySqlConnection, MySqlTransaction>(m_DataSource, "Bulk Insert into " + m_Table.Name);
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
	/// Returns a list of columns.
	/// </summary>
	/// <returns>If the command builder doesn't know which columns are available, an empty list will be returned.</returns>
	/// <remarks>This is used by materializers to skip exclude columns.</remarks>
	public override IReadOnlyList<ColumnMetadata> TryGetColumns() => m_Table.Columns;

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
	/// Modifies the batch size.
	/// </summary>
	/// <param name="batchSize">Size of the batch.</param>
	/// <returns>SqlServerInsertBulk.</returns>
	public MySqlInsertBulk WithBatchSize(int batchSize)
	{
		m_BatchSize = batchSize;
		return this;
	}

	/// <summary>
	/// After notifyAfter records, the event handler may be fired. This can be used to abort the bulk insert.
	/// </summary>
	/// <param name="eventHandler">The event handler.</param>
	/// <param name="notifyAfter">The notify after. This should be a multiple of the batch size.</param>
	public MySqlInsertBulk WithNotifications(EventHandler<AbortableOperationEventArgs> eventHandler, int notifyAfter)
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
	protected override int? Implementation(MySqlConnection connection, MySqlTransaction? transaction)
	{
		var bl = new MySqlBulkLoader(connection);
		var mappedColumns = SetupBulkCopy(bl);

		var totalCount = 0;
		var rowCount = 0;
		var output = new StringBuilder();
		while (m_Source.Read())
		{
			rowCount += 1;
			WriteRow(mappedColumns, output);
			if (rowCount == m_BatchSize)
			{
				using (var ms = CreateMemoryStream(output))
				{
					bl.FileName = null;
					bl.SourceStream = ms;
					totalCount += bl.Load();

					output.Clear();
				}

				//We only notify after a batch has been posted to the server.
				if (m_NotifyAfter.HasValue && m_EventHandler != null)
				{
					var notificationCount = totalCount % m_NotifyAfter.Value;

					if ((totalCount % m_NotifyAfter) > notificationCount)
					{
						var e = new AbortableOperationEventArgs(totalCount);
						m_EventHandler?.Invoke(this, e);
						if (e.Abort)
							throw new TaskCanceledException("Bulk insert operation aborted.");
					}
				}

				rowCount = 0;
			}
		}

		if (rowCount > 0) //final batch
		{
			using (var ms = CreateMemoryStream(output))
			{
				bl.FileName = null;
				bl.SourceStream = ms;
				totalCount += bl.Load();
			}

			if (m_EventHandler != null)
			{
				var e = new AbortableOperationEventArgs(totalCount);
				m_EventHandler?.Invoke(this, e);
				//can't abort at this point;
			}
		}

		return totalCount;
	}

	/// <summary>
	/// Implementation the specified operation.
	/// </summary>
	/// <param name="connection">The connection.</param>
	/// <param name="transaction">The transaction.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>Task&lt;System.Nullable&lt;System.Int32&gt;&gt;.</returns>
	protected override async Task<int?> ImplementationAsync(MySqlConnection connection, MySqlTransaction? transaction, CancellationToken cancellationToken)
	{
		var bl = new MySqlBulkLoader(connection);
		var mappedColumns = SetupBulkCopy(bl);

		var totalCount = 0;
		var rowCount = 0;
		var output = new StringBuilder();
		while (m_Source.Read())
		{
			rowCount += 1;
			WriteRow(mappedColumns, output);
			if (rowCount == m_BatchSize)
			{
				using (var ms = CreateMemoryStream(output))
				{
					bl.FileName = null;
					bl.SourceStream = ms;
					totalCount += await bl.LoadAsync(cancellationToken).ConfigureAwait(false);

					output.Clear();
				}
				rowCount = 0;
			}
		}

		if (rowCount > 0) //final batch
		{
			using (var ms = CreateMemoryStream(output))
			{
				bl.FileName = null;
				bl.SourceStream = ms;
				totalCount += await bl.LoadAsync(cancellationToken).ConfigureAwait(false);
			}
		}

		return totalCount;
	}

	MemoryStream CreateMemoryStream(StringBuilder output)
	{
		var memoryStreamBytes = Encoding.UTF8.GetBytes(output.ToString());
		return new MemoryStream(memoryStreamBytes, false);
	}

	List<int> SetupBulkCopy(MySqlBulkLoader bl)
	{
		bl.TableName = m_Table.Name.ToString();
		bl.CharacterSet = "UTF8";
		bl.NumberOfLinesToSkip = 0;
		bl.FieldTerminator = ",";
		bl.FieldQuotationCharacter = '"';
		bl.FieldQuotationOptional = false;
		bl.LineTerminator = Environment.NewLine;
		bl.Local = true;

		var mappedColumns = new List<int>(m_Source.FieldCount);
		for (var i = 0; i < m_Source.FieldCount; i++)
		{
			bl.Columns.Add(m_Source.GetName(i));

			if (m_Table.Columns.TryGetColumn(m_Source.GetName(i)) != null)
				mappedColumns.Add(i);
			else if (StrictMode)
				throw new MappingException($"Could not find column on {m_Table.Name.ToString()} that matches property {m_Source.GetName(i)}");
		}
		if (mappedColumns.Count == 0)
			throw new MappingException($"Could not find any properties that map to columns on the table {m_Table.Name.ToString()}");

		return mappedColumns;
	}

	void WriteRow(List<int> mappedColumns, StringBuilder output)
	{
		foreach (var i in mappedColumns)
		{
			var value = m_Source.GetValue(i);
			switch (value)
			{
				case Guid g:
					output.Append("\"" + g.ToString() + "\"");
					break;

				case string s:
					output.Append("\"" +
						s
						.Replace("\"", "\\\"")
						.Replace("\t", @"\t")
						.Replace("\r", @"\r")
						.Replace("\n", @"\n")
						+ "\"");
					break;

				case bool b:
					output.Append(b ? "1" : "0");
					break;

				case DateTime dt:
					output.Append(dt.ToString("yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
					break;

				case DateTimeOffset dto:
					output.Append(dto.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
					break;

				case null:
					output.Append("NULL");
					break;

				default:
					value.ToString(); //this would cover numbers
					break;
			}

			if (i == mappedColumns.Count - 1)
				output.Append(Environment.NewLine);
			else
				output.Append(",");
		}
	}
}
