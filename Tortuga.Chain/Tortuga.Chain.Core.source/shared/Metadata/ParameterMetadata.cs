namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Metadata for a stored procedure parameter
    /// </summary>
    public abstract class ParameterMetadata
    {
        /// <summary>
        /// Gets the name used by CLR objects.
        /// </summary>
        public string ClrName { get; protected set; }

        /// <summary>
        /// Gets the name used by the database.
        /// </summary>
        public string SqlParameterName { get; protected set; }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public string TypeName { get; protected set; }

        /// <summary>
        /// Gets the type used by the database.
        /// </summary>
        public object DbType { get; protected set; }

    }
}
