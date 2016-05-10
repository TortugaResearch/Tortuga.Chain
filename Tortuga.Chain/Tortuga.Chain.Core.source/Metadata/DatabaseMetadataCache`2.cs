using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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

        /// <summary>
        /// Preloads all of the metadata for this data source.
        /// </summary>
        public abstract void Preload();

        /// <summary>
        /// Gets the tables and views that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all tables and views were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public abstract IReadOnlyCollection<TableOrViewMetadata<TName, TDbType>> GetTablesAndViews();

        /// <summary>
        /// Gets the stored procedures that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all stored procedures were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual IReadOnlyCollection<StoredProcedureMetadata<TName, TDbType>> GetStoredProcedures()
        {
            throw new NotSupportedException("Stored procedures are not supported by this data source");
        }

        /// <summary>
        /// Gets the table-valued functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all table-valued functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual IReadOnlyCollection<TableFunctionMetadata<TName, TDbType>> GetTableFunctions()
        {
            throw new NotSupportedException("Table value functions are not supported by this data source");
        }

        /// <summary>
        /// Gets the table-valued functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all table-valued functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual IReadOnlyCollection<UserDefinedTypeMetadata<TName, TDbType>> GetUserDefinedTypes()
        {
            throw new NotSupportedException("Table value functions are not supported by this data source");
        }

        /// <summary>
        /// Gets the metadata for a user defined type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual UserDefinedTypeMetadata<TName, TDbType> GetUserDefinedType(TName typeName)
        {
            throw new NotSupportedException("User defined types are not supported by this data source");
        }

        /// <summary>
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public abstract TableOrViewMetadata<TName, TDbType> GetTableOrViewFromClass<TObject>() where TObject : class;

        /// <summary>
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        ITableOrViewMetadata IDatabaseMetadataCache.GetTableOrViewFromClass<TObject>() 
        {
            return GetTableOrViewFromClass<TObject>();
        }

        /// <summary>
        /// Resets the metadata cache, clearing out all cached metadata.
        /// </summary>
        public abstract void Reset();


        IStoredProcedureMetadata IDatabaseMetadataCache.GetStoredProcedure(string procedureName)
        {
            return GetStoredProcedure(ParseObjectName(procedureName));
        }

        ITableFunctionMetadata IDatabaseMetadataCache.GetTableFunction(string tableFunctionName)
        {
            return GetTableFunction(ParseObjectName(tableFunctionName));
        }

        IReadOnlyCollection<ITableOrViewMetadata> IDatabaseMetadataCache.GetTablesAndViews()
        {
            return GetTablesAndViews();
        }

        IReadOnlyCollection<IStoredProcedureMetadata> IDatabaseMetadataCache.GetStoredProcedures()
        {
            return GetStoredProcedures();
        }

        IReadOnlyCollection<ITableFunctionMetadata> IDatabaseMetadataCache.GetTableFunctions()
        {
            return GetTableFunctions();
        }

        IReadOnlyCollection<IUserDefinedTypeMetadata> IDatabaseMetadataCache.GetUserDefinedTypes()
        {
            return GetUserDefinedTypes();
        }

        IUserDefinedTypeMetadata IDatabaseMetadataCache.GetUserDefinedType(string typeName)
        {
            return GetUserDefinedType(ParseObjectName(typeName));
        }
    }
}
