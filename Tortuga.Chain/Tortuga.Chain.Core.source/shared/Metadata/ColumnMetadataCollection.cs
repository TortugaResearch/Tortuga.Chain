using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Class ColumnMetadataCollection.
    /// </summary>
    public class ColumnMetadataCollection : ReadOnlyCollection<ColumnMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnMetadataCollection" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public ColumnMetadataCollection(IEnumerable<ColumnMetadata> source) : base(source.ToList())
        {
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
