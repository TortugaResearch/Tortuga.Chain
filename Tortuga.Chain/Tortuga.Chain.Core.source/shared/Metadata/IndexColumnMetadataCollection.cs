using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    ///
    /// </summary>
    public class IndexColumnMetadataCollection : ReadOnlyCollection<IndexColumnMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the IndexColumnMetadataCollection class that is a read-only wrapper around the specified list.
        /// </summary>
        /// <param name="source">The source.</param>
        public IndexColumnMetadataCollection(IEnumerable<IndexColumnMetadata> source) : base(source.ToList())
        {
        }

        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <remarks>If the column name was not found, this will return null</remarks>
        public IndexColumnMetadata TryGetColumn(string columnName)
        {
            foreach (var item in this)
                if (item.SqlName.Equals(columnName, System.StringComparison.OrdinalIgnoreCase))
                    return item;

            return null;
        }
    }
}
