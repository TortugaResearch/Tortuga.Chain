using System.Collections;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// Class IndexMetadataCollection.
/// </summary>
public class IndexMetadataCollection : IReadOnlyList<IndexMetadata>
{
	readonly IReadOnlyList<IndexMetadata> m_Source;

	/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.Metadata.IndexMetadataCollection"/> class.</summary>
	/// <param name="source">The source.</param>
	public IndexMetadataCollection(IReadOnlyList<IndexMetadata> source)
	{
		m_Source = source;
	}

	/// <summary>
	/// Gets the number of elements in the collection.
	/// </summary>
	public int Count => m_Source.Count;

	/// <summary>
	/// Gets the <see cref="IndexMetadata"/> at the specified index.
	/// </summary>
	/// <value>
	/// The <see cref="IndexMetadata"/>.
	/// </value>
	/// <param name="index">The index.</param>
	/// <returns></returns>
	public IndexMetadata this[int index] => m_Source[index];

	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>
	/// An enumerator that can be used to iterate through the collection.
	/// </returns>
	public IEnumerator<IndexMetadata> GetEnumerator()
	{
		return m_Source.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return m_Source.GetEnumerator();
	}
}
