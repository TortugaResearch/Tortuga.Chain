using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDbType">The type of the database type.</typeparam>
    public class ColumnMetadataCollection<TDbType> : ReadOnlyCollection<ColumnMetadata<TDbType>>
        where TDbType : struct
    {
        private readonly string m_Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnMetadataCollection{TDbType}" /> class.
        /// </summary>
        /// <param name="name">The name of the parent object.</param>
        /// <param name="list">The list to wrap.</param>
        public ColumnMetadataCollection(string name, IList<ColumnMetadata<TDbType>> list) : base(list)
        {
            m_Name = name;
        }

        /// <summary>
        /// Gets the <see cref="ColumnMetadata{TDbType}"/> with the specified column name.
        /// </summary>
        /// <value>
        /// The <see cref="ColumnMetadata{TDbType}"/>.
        /// </value>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public ColumnMetadata<TDbType> this[string columnName]
        {
            get
            {
                foreach (var item in this)
                    if (item.SqlName.Equals(columnName, System.StringComparison.OrdinalIgnoreCase))
                        return item;

                throw new KeyNotFoundException($"Could not find column named {columnName} in object {m_Name}");
            }
        }
    }
}
