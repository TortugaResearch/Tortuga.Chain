using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Class TableFunctionMetadata.
    /// </summary>
    public abstract class ScalarFunctionMetadata
    {
        /// <summary>
        /// Gets the type used by the database.
        /// </summary>
        public object DbType { get; protected set; }

        /// <summary>
        /// Gets or sets the full name of the type including max length, precsision, and/or scale.
        /// </summary>
        /// <value>
        /// The full name of the type.
        /// </value>
        public string FullTypeName { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column is nullable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this column is nullable; otherwise, <c>false</c>.
        /// </value>
        public bool IsNullable { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum length.
        /// </summary>
        /// <value>
        /// The maximum length.
        /// </value>
        public int? MaxLength { get; protected set; }

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
        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>
        /// The precision.
        /// </value>
        public int? Precision { get; protected set; }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public int? Scale { get; protected set; }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        public string TypeName { get; protected set; }
    }
}
