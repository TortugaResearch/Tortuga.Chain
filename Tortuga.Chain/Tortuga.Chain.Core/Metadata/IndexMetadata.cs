namespace Tortuga.Chain.Metadata;

/// <summary>
/// Metadata for an index.
/// </summary>
public abstract class IndexMetadata
{
	/// <summary>Initializes a new instance of the <see cref="IndexMetadata{TObjectName, TDbType}"/> class.</summary>
	/// <param name="tableName">Name of the table (or view).</param>
	/// <param name="name">The name.</param>
	/// <param name="isPrimaryKey">if set to <c>true</c> is a primary key.</param>
	/// <param name="isUnique">if set to <c>true</c> is a unique index.</param>
	/// <param name="isUniqueConstraint">if set to <c>true</c> is a unique constraint.</param>
	/// <param name="columns">The columns.</param>
	/// <param name="indexSizeKB">Approximate index size in KB</param>
	/// <param name="rowCount">Approximate row count</param>
	/// <param name="indexType">Type of the index.</param>
	protected IndexMetadata(string tableName, string? name, bool isPrimaryKey, bool isUnique, bool isUniqueConstraint, IndexColumnMetadataCollection columns, long? indexSizeKB, long? rowCount, IndexType indexType)
	{
		TableName = tableName;
		Name = name;
		IsUnique = isUnique;
		IsUniqueConstraint = isUniqueConstraint;
		IsPrimaryKey = isPrimaryKey;
		Columns = columns;
		IndexSizeKB = indexSizeKB;
		RowCount = rowCount;
		IndexType = indexType;
	}

	/// <summary>
	/// Gets the columns.
	/// </summary>
	public IndexColumnMetadataCollection Columns { get; }

	/// <summary>
	/// Gets the approximate index size in kilobytes.
	/// </summary>
	public long? IndexSizeKB { get; }

	/// <summary>Gets the type of the index.</summary>
	public IndexType IndexType { get; }

	/// <summary>
	/// Gets a value indicating whether this index is a primary key.
	/// </summary>
	/// <value>
	///   <c>true</c> if this instance is primary key; otherwise, <c>false</c>.
	/// </value>
	public bool IsPrimaryKey { get; }

	/// <summary>
	/// Gets a value indicating whether this is a unique index.
	/// </summary>
	/// <value>
	///   <c>true</c> if this instance is unique; otherwise, <c>false</c>.
	/// </value>
	public bool IsUnique { get; }

	/// <summary>
	/// Gets a value indicating whether this is a unique constraint.
	/// </summary>
	/// <value>
	///   <c>true</c> if this instance is unique; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>In some database, a unique index is not necessary a unique constraint.</remarks>
	public bool IsUniqueConstraint { get; }

	/// <summary>
	/// Gets the name of the index.
	/// </summary>
	/// <value>
	/// The name.
	/// </value>
	public string? Name { get; }

	/// <summary>
	/// Gets the approximate row count.
	/// </summary>
	public long? RowCount { get; }

	/// <summary>
	/// Gets the name of the table (or view) the index applies to.
	/// </summary>
	/// <value>
	/// The name of the table.
	/// </value>
	public string TableName { get; }
}
