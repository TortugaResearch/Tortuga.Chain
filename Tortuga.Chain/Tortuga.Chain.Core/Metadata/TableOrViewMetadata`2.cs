using System.Collections.Immutable;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// Metadata for a database table or view.
/// </summary>
/// <typeparam name="TObjectName">The type used to represent database object names.</typeparam>
/// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
public class TableOrViewMetadata<TObjectName, TDbType> : TableOrViewMetadata
	where TObjectName : struct
	where TDbType : struct
{
	readonly SqlBuilder<TDbType> m_Builder;
	readonly ImmutableArray<SortExpression> m_DefaultSortOrder;
	readonly DatabaseMetadataCache<TObjectName, TDbType> m_MetadataCache;
	ForeignKeyConstraintCollection<TObjectName, TDbType>? m_ForeignKeys;
	IndexMetadataCollection<TObjectName, TDbType>? m_Indexes;

	/// <summary>
	/// Initializes a new instance of the <see cref="TableOrViewMetadata{TObjectName, TDbType}"/> class.
	/// </summary>
	/// <param name="metadataCache">The metadata cache.</param>
	/// <param name="name">The name.</param>
	/// <param name="isTable">if set to <c>true</c> [is table].</param>
	/// <param name="columns">The columns.</param>
	public TableOrViewMetadata(DatabaseMetadataCache<TObjectName, TDbType> metadataCache, TObjectName name, bool isTable, ColumnMetadataCollection<TDbType> columns) : base(name.ToString()!, isTable, columns?.GenericCollection!)
	{
		m_MetadataCache = metadataCache ?? throw new ArgumentNullException(nameof(metadataCache), $"{nameof(metadataCache)} is null.");
		Name = name;
		Columns = columns ?? throw new ArgumentNullException(nameof(columns), $"{nameof(columns)} is null.");
		PrimaryKeyColumns = new ColumnMetadataCollection<TDbType>(name.ToString()!, columns.Where(c => c.IsPrimaryKey).ToList());
		m_Builder = new SqlBuilder<TDbType>(Name.ToString()!, Columns);

		if (columns.Count == 0)
			m_DefaultSortOrder = ImmutableArray<SortExpression>.Empty;
		else if (HasPrimaryKey)
			m_DefaultSortOrder = PrimaryKeyColumns.Select(c => new SortExpression(c.SqlName)).ToImmutableArray();
		else
			m_DefaultSortOrder = Columns.Select(c => new SortExpression(c.SqlName)).ToImmutableArray();
	}

	/// <summary>
	/// Gets the columns.
	/// </summary>
	/// <value>
	/// The columns.
	/// </value>
	public new ColumnMetadataCollection<TDbType> Columns { get; }

	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <value>
	/// The name.
	/// </value>
	public new TObjectName Name { get; }

	/// <summary>
	/// Gets the columns that make up the primary key.
	/// </summary>
	/// <value>
	/// The columns.
	/// </value>
	public new ColumnMetadataCollection<TDbType> PrimaryKeyColumns { get; }

	/// <summary>
	/// Creates the SQL builder
	/// </summary>
	/// <returns></returns>
	public SqlBuilder<TDbType> CreateSqlBuilder(bool strictMode) => m_Builder.Clone(strictMode);

	/// <summary>
	/// Gets the default sort order. This will be the primary key(s) if they exist. Otherwise the columns will be used in declared order.
	/// </summary>
	/// <param name="columnsToUse">Sort by [columnsToUse] columns. If not specified, returns all columns in the default sort.</param>
	/// <returns>If no valid sort columns are found, this will return an empty array.</returns>
	public IEnumerable<SortExpression> GetDefaultSortOrder(int columnsToUse = int.MaxValue)
	{
		if (columnsToUse >= m_DefaultSortOrder.Length)
			return m_DefaultSortOrder;
		else
			return m_DefaultSortOrder.Take(columnsToUse);
	}

	/// <summary>
	/// Gets the indexes for this table or view.
	/// </summary>
	/// <returns></returns>
	/// <exception cref="NotSupportedException">Foreign Keys are not supported by this data source</exception>
	public ForeignKeyConstraintCollection<TObjectName, TDbType> GetForeignKeys()
	{
		if (m_ForeignKeys == null)
			m_ForeignKeys = m_MetadataCache.GetForeignKeysForTable(Name);
		return m_ForeignKeys;
	}

	/// <summary>
	/// Gets the indexes for this table or view.
	/// </summary>
	/// <returns></returns>
	public IndexMetadataCollection<TObjectName, TDbType> GetIndexes()
	{
		if (m_Indexes == null)
			m_Indexes = m_MetadataCache.GetIndexesForTable(Name);
		return m_Indexes;
	}
}
