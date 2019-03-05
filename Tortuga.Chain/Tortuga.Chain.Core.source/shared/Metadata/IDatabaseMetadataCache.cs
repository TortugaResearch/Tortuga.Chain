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
        /// Gets the stored procedure's metadata.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        StoredProcedureMetadata GetStoredProcedure(string procedureName);

        /// <summary>
        /// Gets the stored procedures that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all stored procedures were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IReadOnlyCollection<StoredProcedureMetadata> GetStoredProcedures();

        /// <summary>
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        TableFunctionMetadata GetTableFunction(string tableFunctionName);

        /// <summary>
        /// Gets the table-valued functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all table-valued functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IReadOnlyCollection<TableFunctionMetadata> GetTableFunctions();

        /// <summary>
        /// Gets the metadata for a table or view.
        /// </summary>
        /// <param name="tableName">Name of the table or view.</param>
        /// <returns></returns>
        TableOrViewMetadata GetTableOrView(string tableName);

        /// <summary>
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        TableOrViewMetadata GetTableOrViewFromClass<TObject>() where TObject : class;

        /// <summary>
        /// Gets the tables and views that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all tables and views were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IReadOnlyCollection<TableOrViewMetadata> GetTablesAndViews();

        /// <summary>
        /// Gets the metadata for a user defined type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        UserDefinedTypeMetadata GetUserDefinedType(string typeName);

        /// <summary>
        /// Gets the table-valued functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all table-valued functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IReadOnlyCollection<UserDefinedTypeMetadata> GetUserDefinedTypes();

        /// <summary>
        /// Preloads all of the metadata for this data source.
        /// </summary>
        void Preload();

        /// <summary>
        /// Resets the metadata cache, clearing out all cached metadata.
        /// </summary>
        void Reset();

        /// <summary>
        /// Tries to get the stored procedure's metadata.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="storedProcedure">The stored procedure.</param>
        /// <returns></returns>
        bool TryGetStoredProcedure(string procedureName, out StoredProcedureMetadata storedProcedure);

        /// <summary>
        /// Tries to get the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <param name="tableFunction">The table function.</param>
        /// <returns></returns>
        bool TryGetTableFunction(string tableFunctionName, out TableFunctionMetadata tableFunction);

        /// <summary>
        /// Tries to get the metadata for a table or view.
        /// </summary>
        /// <param name="tableName">Name of the table or view.</param>
        /// <param name="tableOrView">The table or view.</param>
        /// <returns></returns>
        bool TryGetTableOrView(string tableName, out TableOrViewMetadata tableOrView);

        /// <summary>
        /// Try to get the metadata for a user defined type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="userDefinedType">Type of the user defined.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        bool TryGetUserDefinedType(string typeName, out UserDefinedTypeMetadata userDefinedType);
    }
}
