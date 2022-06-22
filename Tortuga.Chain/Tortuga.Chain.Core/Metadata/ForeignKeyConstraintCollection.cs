using System.Collections;

namespace Tortuga.Chain.Metadata;

/// <summary>Class ForeignKeyConstraintCollection.
/// Implements the <see cref="IReadOnlyList{ForeignKeyConstraint}"/></summary>
/// <seealso cref="IReadOnlyList{ForeignKeyConstraint}" />
public class ForeignKeyConstraintCollection : IReadOnlyList<ForeignKeyConstraint>
{
	readonly IReadOnlyList<ForeignKeyConstraint> m_Source;

	/// <summary>
	/// Initializes a new instance of the IndexColumnMetadataCollection class that is a read-only wrapper around the specified list.
	/// </summary>
	/// <param name="source">The source.</param>
	internal ForeignKeyConstraintCollection(IReadOnlyList<ForeignKeyConstraint> source)
	{
		m_Source = source;
	}

	/// <summary>Gets the number of elements in the collection.</summary>
	/// <returns>The number of elements in the collection. </returns>
	public int Count => m_Source.Count;

	/// <summary>Gets the element at the specified index in the read-only list.</summary>
	/// <param name="index">The zero-based index of the element to get. </param>
	/// <returns>The element at the specified index in the read-only list.</returns>
	public ForeignKeyConstraint this[int index] => m_Source[index];

	/// <summary>Returns an enumerator that iterates through the collection.</summary>
	/// <returns>An enumerator that can be used to iterate through the collection.</returns>
	public IEnumerator<ForeignKeyConstraint> GetEnumerator()
	{
		return m_Source.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return m_Source.GetEnumerator();
	}
}
