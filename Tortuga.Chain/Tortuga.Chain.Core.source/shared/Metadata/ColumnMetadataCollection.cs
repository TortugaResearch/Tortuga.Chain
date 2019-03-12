using System.Collections;
using System.Collections.Generic;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Class ColumnMetadataCollection.
    /// </summary>
    public class ColumnMetadataCollection : IReadOnlyList<ColumnMetadata>
    {
        IReadOnlyList<ColumnMetadata> m_Source;

        /// <summary>
        /// Initializes a new instance of the IndexColumnMetadataCollection class that is a read-only wrapper around the specified list.
        /// </summary>
        /// <param name="source">The source.</param>
        internal ColumnMetadataCollection(IReadOnlyList<ColumnMetadata> source)
        {
            m_Source = source;
        }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection. </returns>
        public int Count => m_Source.Count;

        /// <summary>Gets the element at the specified index in the read-only list.</summary>
        /// <param name="index">The zero-based index of the element to get. </param>
        /// <returns>The element at the specified index in the read-only list.</returns>
        public ColumnMetadata this[int index] => m_Source[index];

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ColumnMetadata> GetEnumerator()
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
        public ColumnMetadata TryGetColumn(string columnName)
        {
            foreach (var item in this)
                if (item.SqlName.Equals(columnName, System.StringComparison.OrdinalIgnoreCase))
                    return item;

            return null;
        }
    }
}
