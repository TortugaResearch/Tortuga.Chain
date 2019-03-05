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
        readonly string m_Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnMetadataCollection" /> class.
        /// </summary>
        /// <param name="name">The name of the parent object.</param>
        /// <param name="source">The source.</param>
        public ColumnMetadataCollection(string name, IEnumerable<ColumnMetadata> source) : base(source.ToList())
        {
            m_Name = name;
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

        /// <summary>
        /// Gets the <see cref="ColumnMetadata"/> with the specified column name.
        /// </summary>
        /// <value>
        /// The <see cref="ColumnMetadata"/>.
        /// </value>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public ColumnMetadata this[string columnName]
        {
            get
            {
                foreach (var item in this)
                    if (item.SqlName.Equals(columnName, System.StringComparison.OrdinalIgnoreCase))
                        return item;

#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
                throw new KeyNotFoundException($"Could not find column named {columnName} in object {m_Name}");
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
            }
        }
    }
}
