namespace Tortuga.Chain.Metadata;

/// <summary>Class IndexColumnMetadata.
/// Implements the <see cref="Tortuga.Chain.Metadata.IndexColumnMetadata"/></summary>
/// <typeparam name="TDbType">The type of the t database type.</typeparam>
/// <seealso cref="Tortuga.Chain.Metadata.IndexColumnMetadata" />
public class IndexColumnMetadata<TDbType> : IndexColumnMetadata
	where TDbType : struct
{
	/// <summary>
	/// Initializes a new instance of the IndexColumnMetadata class.
	/// </summary>
	/// <param name="column">The underlying column details.</param>
	/// <param name="isDescending">Indicates the column is indexed in descending order.</param>
	/// <param name="isIncluded">Indicates the column is an unindexed, included column.</param>
	public IndexColumnMetadata(ColumnMetadata<TDbType> column, bool? isDescending, bool isIncluded) : base(column, isDescending, isIncluded)
	{
		Details = column;
	}

	/// <summary>
	/// Gets the underlying column.
	/// </summary>
	public new ColumnMetadata<TDbType> Details { get; }
}
