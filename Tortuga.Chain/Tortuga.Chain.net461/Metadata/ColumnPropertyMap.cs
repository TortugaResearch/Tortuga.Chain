using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// This maps database columns (tables and views) to class properties.
    /// </summary>
    public class ColumnPropertyMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnPropertyMap"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="property">The property.</param>
        public ColumnPropertyMap(ColumnMetadata column, PropertyMetadata property)
        {
            Column = column;
            Property = property;
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>The column.</value>
        public ColumnMetadata Column { get; private set; }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        public PropertyMetadata Property { get; private set; }
    }
}
