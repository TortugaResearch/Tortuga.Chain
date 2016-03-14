using System;

namespace Tortuga.Chain.Metadata
{

    /// <summary>
    /// An abstract database metadata cache
    /// </summary>
    /// <typeparam name="TName">The type used to represent database object names.</typeparam>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public abstract class DatabaseMetadataCache<TName, TDbType> : IDatabaseMetadataCache
        where TDbType : struct
    {
        /// <summary>
        /// Gets the stored procedure's metadata.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        public virtual StoredProcedureMetadata<TName, TDbType> GetStoredProcedure(TName procedureName)
        {
            throw new NotSupportedException("Stored procedures are not supported by this data source");
        }

        /// <summary>
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        public virtual TableFunctionMetadata<TName, TDbType> GetTableFunction(TName tableFunctionName)
        {
            throw new NotSupportedException("Table value functions are not supported by this data source");
        }

        /// <summary>
        /// Gets the metadata for a table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public abstract TableOrViewMetadata<TName, TDbType> GetTableOrView(TName tableName);

        ITableOrViewMetadata IDatabaseMetadataCache.GetTableOrView(string tableName)
        {
            return GetTableOrView(ParseObjectName(tableName));
        }

        /// <summary>
        /// Parse a string and return the database specific representation of the object name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected abstract TName ParseObjectName(string name);
    }
}
