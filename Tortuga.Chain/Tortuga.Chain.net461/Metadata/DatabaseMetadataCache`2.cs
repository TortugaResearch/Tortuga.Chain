namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// An abstract database metadata cache
    /// </summary>
    /// <typeparam name="TName">The type used to represent database object names.</typeparam>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public abstract class DatabaseMetadataCache<TName, TDbType>
        where TDbType : struct
    {
        /// <summary>
        /// Gets the stored procedure's metadata.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        public abstract StoredProcedureMetadata<TName, TDbType> GetStoredProcedure(TName procedureName);

        /// <summary>
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        public abstract TableFunctionMetadata<TName, TDbType> GetTableFunction(TName tableFunctionName);

        /// <summary>
        /// Gets the metadata for a table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public abstract TableOrViewMetadata<TName, TDbType> GetTableOrView(TName tableName);
    }
}
