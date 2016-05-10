using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ITableOrViewMetadata GetTableOrViewFromClass<TObject>() where TObject : class;

        /// <summary>
        /// Resets the metadata cache, clearing out all cached metadata.
        /// </summary>
        void Reset();

        /// <summary>
        /// Preloads all of the metadata for this data source.
        /// </summary>
        void Preload();

        /// <summary>
        /// Gets the stored procedure's metadata.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        IStoredProcedureMetadata GetStoredProcedure(string procedureName);

        /// <summary>
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        ITableFunctionMetadata GetTableFunction(string tableFunctionName);

        /// <summary>
        /// Gets the tables and views that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all tables and views were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IReadOnlyCollection<ITableOrViewMetadata> GetTablesAndViews();

        /// <summary>
        /// Gets the stored procedures that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all stored procedures were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IReadOnlyCollection<IStoredProcedureMetadata> GetStoredProcedures();

        /// <summary>
        /// Gets the table-valued functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all table-valued functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IReadOnlyCollection<ITableFunctionMetadata> GetTableFunctions();

        /// <summary>
        /// Gets the table-valued functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all table-valued functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IReadOnlyCollection<IUserDefinedTypeMetadata> GetUserDefinedTypes();

        /// <summary>
        /// Gets the metadata for a user defined type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IUserDefinedTypeMetadata GetUserDefinedType(string typeName);

    }
}
