using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Class TableFunctionMetadata.
    /// </summary>
    public abstract class TableFunctionMetadata
    {
        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ColumnMetadataCollection Columns { get; protected set; }

        /// <summary>
        /// Gets the columns known to be nullable.
        /// </summary>
        /// <value>
        /// The nullable columns.
        /// </value>
        /// <remarks>This is used to improve the performance of materializers by avoiding is null checks.</remarks>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ColumnMetadataCollection NullableColumns { get; protected set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ParameterMetadataCollection Parameters { get; protected set; }


    }
}
