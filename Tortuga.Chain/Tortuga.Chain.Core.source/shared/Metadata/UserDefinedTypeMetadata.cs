namespace Tortuga.Chain.Metadata
{

    /// <summary>
    /// This interface represents user defined types.
    /// </summary>
    public abstract class UserDefinedTypeMetadata
    {

        /// <summary>
        /// Gets a value indicating whether this instance is table type or a normal UDF.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is a table type; otherwise, <c>false</c>.
        /// </value>
        public bool IsTableType { get; protected set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ColumnMetadataCollection Columns { get; protected set; }

    }

}
