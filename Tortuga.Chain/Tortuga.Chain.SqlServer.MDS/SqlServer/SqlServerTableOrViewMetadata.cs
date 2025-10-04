using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer;

/// <summary>
/// Class SqlServerTableOrViewMetadata.
/// </summary>
/// <typeparam name="TDbType">The type of the t database type.</typeparam>
/// <seealso cref="TableOrViewMetadata{SqlServerObjectName, TDbType}" />
public class SqlServerTableOrViewMetadata<TDbType> : TableOrViewMetadata<SqlServerObjectName, TDbType>
	where TDbType : struct
{
	readonly AbstractSqlServerMetadataCache m_MetadataCache;


	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerTableOrViewMetadata{TDbType}" /> class.
	/// </summary>
	/// <param name="metadataCache">The metadata cache.</param>
	/// <param name="name">The name.</param>
	/// <param name="isTable">if set to <c>true</c> is a table.</param>
	/// <param name="columns">The columns.</param>
	/// <param name="hasTriggers">if set to <c>true</c> has triggers.</param>
	/// <param name="objectId">SQL Server's internal object_id.</param>
	/// <param name="historyTableName">Name of the history table.</param>
	/// <param name="isHistoryTable">if set to true if this is a history table.</param>
	public SqlServerTableOrViewMetadata(AbstractSqlServerMetadataCache metadataCache, SqlServerObjectName name, bool isTable, ColumnMetadataCollection<TDbType> columns, bool hasTriggers, int objectId, SqlServerObjectName? historyTableName, bool isHistoryTable) : base((DatabaseMetadataCache<SqlServerObjectName, TDbType>)(object)metadataCache, name, isTable, columns)
	{
		HasTriggers = hasTriggers;
		ObjectId = objectId;
		m_MetadataCache = metadataCache;
		IsHistoryTable = isHistoryTable;
		HistoryTableName = historyTableName;
	}

	/// <summary>
	/// Gets a value indicating whether the associated table has triggers.
	/// </summary>
	/// <value><c>true</c> if this instance has triggers; otherwise, <c>false</c>.</value>
	/// <remarks>This affects SQL generation.</remarks>
	public bool HasTriggers { get; }

	/// <summary>
	/// SQL Server's internal object_id.
	/// </summary>
	public int ObjectId { get; }


	/// <summary>
	/// If the table is system versioned, this property will not be null.
	/// </summary>
	/// <value>The name of the history table.</value>
	public SqlServerObjectName? HistoryTableName { get; }

	/// <summary>
	/// Gets the history table.
	/// </summary>
	/// <returns>SqlServerTableOrViewMetadata&lt;TDbType&gt;.</returns>
	/// <exception cref="System.InvalidOperationException">Table {Name} does not have a history table.</exception>
	public SqlServerTableOrViewMetadata<TDbType> GetHistoryTable()
	{
		if (HistoryTableName == null)
			throw new InvalidOperationException($"Table {Name} does not have a history table.");

		return (SqlServerTableOrViewMetadata<TDbType>)(object)m_MetadataCache.GetTableOrView(HistoryTableName.Value);
	}

	/// <summary>
	/// True if this is a history table for another table. 
	/// </summary>
	public bool IsHistoryTable { get; }

}