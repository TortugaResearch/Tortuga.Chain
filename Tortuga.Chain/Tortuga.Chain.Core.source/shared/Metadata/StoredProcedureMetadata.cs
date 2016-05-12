
namespace Tortuga.Chain.Metadata
{

    /// <summary>
    /// Class StoredProcedureMetadata.
    /// </summary>
    public abstract class StoredProcedureMetadata
    {

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ParameterMetadataCollection Parameters { get; protected set; }
    }
}
