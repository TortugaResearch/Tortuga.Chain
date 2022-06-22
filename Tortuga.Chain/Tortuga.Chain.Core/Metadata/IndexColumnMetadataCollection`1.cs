using System.Collections.ObjectModel;

namespace Tortuga.Chain.Metadata;

/// <summary>Class IndexColumnMetadataCollection.</summary>
/// <typeparam name="TDbType">The database column type.</typeparam>
public class IndexColumnMetadataCollection<TDbType> : ReadOnlyCollection<IndexColumnMetadata<TDbType>>
	where TDbType : struct
{
	/// <summary>
	/// Initializes a new instance of the IndexColumnMetadataCollection class that is a read-only wrapper around the specified list.
	/// </summary>
	/// <param name="source">The source.</param>
	public IndexColumnMetadataCollection(IEnumerable<IndexColumnMetadata<TDbType>> source) : base(source.ToList())
	{
		GenericCollection = new IndexColumnMetadataCollection(this);
	}

	/// <summary>
	/// Gets the generic version of this collection.
	/// </summary>
	/// <remarks>We can't make this implement IReadOnlyList because it breaks LINQ.</remarks>
	public IndexColumnMetadataCollection GenericCollection { get; }

	/// <summary>
	/// Returns the column associated with the column name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns></returns>
	/// <remarks>If the column name was not found, this will return null</remarks>
	public IndexColumnMetadata<TDbType>? TryGetColumn(string columnName)
	{
		foreach (var item in this)
			if (item.SqlName.Equals(columnName, System.StringComparison.OrdinalIgnoreCase))
				return item;

		return null;
	}
}
