using System.Collections;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// Class IndexColumnMetadataCollection.
/// Implements the <see cref="IReadOnlyList{IndexColumnMetadata}" />
/// </summary>
/// <seealso cref="IReadOnlyList{IndexColumnMetadata}" />
public class IndexColumnMetadataCollection : IReadOnlyList<IndexColumnMetadata>
{
	readonly IReadOnlyList<IndexColumnMetadata> m_Source;

	/// <summary>
	/// Initializes a new instance of the IndexColumnMetadataCollection class that is a read-only wrapper around the specified list.
	/// </summary>
	/// <param name="source">The source.</param>
	internal IndexColumnMetadataCollection(IReadOnlyList<IndexColumnMetadata> source)
	{
		m_Source = source;
	}

	/// <summary>Gets the number of elements in the collection.</summary>
	/// <returns>The number of elements in the collection. </returns>
	public int Count => m_Source.Count;

	/// <summary>Gets the element at the specified index in the read-only list.</summary>
	/// <param name="index">The zero-based index of the element to get. </param>
	/// <returns>The element at the specified index in the read-only list.</returns>
	public IndexColumnMetadata this[int index] => m_Source[index];

	/// <summary>Returns an enumerator that iterates through the collection.</summary>
	/// <returns>An enumerator that can be used to iterate through the collection.</returns>
	public IEnumerator<IndexColumnMetadata> GetEnumerator()
	{
		return m_Source.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return m_Source.GetEnumerator();
	}

	/// <summary>
	/// Returns the column associated with the column name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns></returns>
	/// <remarks>If the column name was not found, this will return null</remarks>
	public IndexColumnMetadata? TryGetColumn(string columnName)
	{
		foreach (var item in this)
			if (item.SqlName.Equals(columnName, System.StringComparison.OrdinalIgnoreCase))
				return item;

		return null;
	}
}
