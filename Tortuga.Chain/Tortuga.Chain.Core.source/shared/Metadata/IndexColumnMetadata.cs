namespace Tortuga.Chain.Metadata
{
    /// <summary>
    ///
    /// </summary>
    public class IndexColumnMetadata
    {
        /// <summary>
        /// Initializes a new instance of the IndexColumnMetadata class.
        /// </summary>
        /// <param name="column">The underlying column details.</param>
        /// <param name="isDescending">Indicates the column is indexed in descending order.</param>
        /// <param name="isIncluded">Indicates the column is an unindexed, included column.</param>
        public IndexColumnMetadata(ColumnMetadata column, bool isDescending, bool isIncluded)
        {
            Details = column;
            IsDescending = isDescending;
            IsIncluded = isIncluded;
        }

        /// <summary>
        /// Gets the underlying column.
        /// </summary>
        public ColumnMetadata Details { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is descending.
        /// </summary>
        public bool IsDescending { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is an included column.
        /// </summary>
        public bool IsIncluded { get; }

        /// <summary>
        /// Gets the name used by the database.
        /// </summary>
        public string SqlName => Details.SqlName;
    }
}
