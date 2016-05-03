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

        /// <summary>
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ITableOrViewMetadata GetTableOrViewFromClass<T>() where T : class;

    }
}
