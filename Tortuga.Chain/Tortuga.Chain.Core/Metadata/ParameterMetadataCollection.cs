using System.Collections;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// Class ParameterMetadataCollection.
/// </summary>
public class ParameterMetadataCollection : IReadOnlyList<ParameterMetadata>
{
	readonly IReadOnlyList<ParameterMetadata> m_Source;

	/// <summary>
	/// Initializes a new instance of the IndexColumnMetadataCollection class that is a read-only wrapper around the specified list.
	/// </summary>
	/// <param name="source">The source.</param>
	internal ParameterMetadataCollection(IReadOnlyList<ParameterMetadata> source)
	{
		m_Source = source;
	}

	/// <summary>
	/// Gets the number of elements in the collection.
	/// </summary>
	public int Count => m_Source.Count;

	/// <summary>
	/// Gets the <see cref="ParameterMetadata"/> at the specified index.
	/// </summary>
	/// <value>
	/// The <see cref="ParameterMetadata"/>.
	/// </value>
	/// <param name="index">The index.</param>
	/// <returns></returns>
	public ParameterMetadata this[int index] => m_Source[index];

	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>
	/// An enumerator that can be used to iterate through the collection.
	/// </returns>
	public IEnumerator<ParameterMetadata> GetEnumerator()
	{
		return m_Source.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return m_Source.GetEnumerator();
	}

	/// <summary>
	/// Returns the parameter associated with the parameter name.
	/// </summary>
	/// <param name="parameterName">Name of the parameter.</param>
	/// <returns></returns>
	/// <remarks>If the parameter name was not found, this will return null</remarks>
	public ParameterMetadata? TryGetParameter(string parameterName)
	{
		foreach (var item in this)
			if (item.SqlParameterName.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
				return item;

		return null;
	}
}
