namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Abstract version of the database metadata cache.
    /// </summary>
    public interface IDatabaseMetadataCache
    {
        /// <summary>
        /// Gets the metadata for a table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        ITableOrViewMetadata GetTableOrView(string tableName);
    }
}
