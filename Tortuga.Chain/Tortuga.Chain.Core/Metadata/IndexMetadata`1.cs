namespace Tortuga.Chain.Metadata
{
	/// <summary>
	/// Metadata for an index.
	/// </summary>
	/// <typeparam name="TName">The type of the name.</typeparam>
	/// <typeparam name="TDbType">The database column type.</typeparam>
	/// <seealso cref="IndexMetadata" />
	public class IndexMetadata<TName, TDbType> : IndexMetadata
		where TName : struct
		where TDbType : struct
	{
		/// <summary>Initializes a new instance of the <see cref="IndexMetadata{TName, TDbType}"/> class.</summary>
		/// <param name="tableName">Name of the table (or view).</param>
		/// <param name="name">The name.</param>
		/// <param name="isPrimaryKey">if set to <c>true</c> is a primary key.</param>
		/// <param name="isUnique">if set to <c>true</c> is a unique index.</param>
		/// <param name="isUniqueConstraint">if set to <c>true</c> is a unique constraint.</param>
		/// <param name="columns">The columns.</param>
		/// <param name="indexSizeKB">Approximate index size in KB</param>
		/// <param name="rowCount">Approximate row count</param>
		/// <param name="indexType">Type of the index.</param>
		/// <exception cref="ArgumentNullException">columns</exception>
		public IndexMetadata(TName tableName, string? name, bool isPrimaryKey, bool isUnique, bool isUniqueConstraint, IndexColumnMetadataCollection<TDbType> columns, long? indexSizeKB, long? rowCount, IndexType indexType) : base(tableName.ToString()!, name, isPrimaryKey, isUnique, isUniqueConstraint, columns?.GenericCollection!, indexSizeKB, rowCount, indexType)
		{
			TableName = tableName;
			Columns = columns ?? throw new ArgumentNullException(nameof(columns), $"{nameof(columns)} is null.");
		}

		/// <summary>
		/// Gets the columns.
		/// </summary>
		public new IndexColumnMetadataCollection<TDbType> Columns { get; }

		/// <summary>
		/// Gets the name of the table (or view) the index applies to.
		/// </summary>
		/// <value>
		/// The name of the table.
		/// </value>
		public new TName TableName { get; }
	}
}
