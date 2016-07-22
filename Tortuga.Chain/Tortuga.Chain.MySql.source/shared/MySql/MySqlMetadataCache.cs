using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tortuga.Anchor;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql
{
    /// <summary>
    /// Class MySqlMetadataCache.
    /// </summary>
    public class MySqlMetadataCache : DatabaseMetadataCache<MySqlObjectName, MySqlDbType>
    {
        readonly MySqlConnectionStringBuilder m_ConnectionBuilder;
        readonly ConcurrentDictionary<MySqlObjectName, TableOrViewMetadata<MySqlObjectName, MySqlDbType>> m_Tables = new ConcurrentDictionary<MySqlObjectName, TableOrViewMetadata<MySqlObjectName, MySqlDbType>>();
        readonly ConcurrentDictionary<MySqlObjectName, StoredProcedureMetadata<MySqlObjectName, MySqlDbType>> m_StoredProcedures = new ConcurrentDictionary<MySqlObjectName, StoredProcedureMetadata<MySqlObjectName, MySqlDbType>>();
        readonly ConcurrentDictionary<Type, TableOrViewMetadata<MySqlObjectName, MySqlDbType>> m_TypeTableMap = new ConcurrentDictionary<Type, TableOrViewMetadata<MySqlObjectName, MySqlDbType>>();
        readonly ConcurrentDictionary<MySqlObjectName, TableFunctionMetadata<MySqlObjectName, MySqlDbType>> m_TableFunctions = new ConcurrentDictionary<MySqlObjectName, TableFunctionMetadata<MySqlObjectName, MySqlDbType>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlMetadataCache"/> class.
        /// </summary>
        /// <param name="connectionBuilder">The connection builder.</param>
        public MySqlMetadataCache(MySqlConnectionStringBuilder connectionBuilder)
        {
            m_ConnectionBuilder = connectionBuilder;
        }

        /// <summary>
        /// Gets the stored procedure's metadata.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override StoredProcedureMetadata<MySqlObjectName, MySqlDbType> GetStoredProcedure(MySqlObjectName procedureName)
        {
            return m_StoredProcedures.GetOrAdd(procedureName, GetStoredProcedureInteral);
        }

        /// <summary>
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override TableFunctionMetadata<MySqlObjectName, MySqlDbType> GetTableFunction(MySqlObjectName tableFunctionName)
        {
            return m_TableFunctions.GetOrAdd(tableFunctionName, GetTableFunctionInternal);
        }

        /// <summary>
        /// Gets the metadata for a table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>TableOrViewMetadata&lt;MySqlObjectName, MySqlDbType&gt;.</returns>
        public override TableOrViewMetadata<MySqlObjectName, MySqlDbType> GetTableOrView(MySqlObjectName tableName)
        {
            return m_Tables.GetOrAdd(tableName, GetTableOrViewInternal);
        }

        /// <summary>
        /// Gets the tables and views that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all tables and views were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<TableOrViewMetadata<MySqlObjectName, MySqlDbType>> GetTablesAndViews()
        {
            return m_Tables.GetValues();
        }

        /// <summary>
        /// Preloads all of the metadata for this data source.
        /// </summary>
        public override void Preload()
        {
            PreloadTables();
            PreloadViews();
            PreloadTableFunctions();
            PreloadStoredProcedures();
        }

        /// <summary>
        /// Preloads the metadata for all tables.
        /// </summary>
        public void PreloadTables()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Preloads the metadata for all views.
        /// </summary>
        public void PreloadViews()
        {
            throw new NotImplementedException();
        }

        private TableOrViewMetadata<MySqlObjectName, MySqlDbType> GetTableOrViewInternal(MySqlObjectName tableName)
        {
            throw new NotImplementedException();
        }

        private TableFunctionMetadata<MySqlObjectName, MySqlDbType> GetTableFunctionInternal(MySqlObjectName tableFunctionName)
        {
            throw new NotImplementedException();


        }



        private StoredProcedureMetadata<MySqlObjectName, MySqlDbType> GetStoredProcedureInteral(MySqlObjectName storedProcedureName)
        {
            throw new NotImplementedException();
        }








        /// <summary>
        /// Parse a string and return the database specific representation of the object name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>MySqlObjectName.</returns>
        protected override MySqlObjectName ParseObjectName(string name)
        {
            return new MySqlObjectName(name);
        }

        /// <summary>
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public override TableOrViewMetadata<MySqlObjectName, MySqlDbType> GetTableOrViewFromClass<TObject>()
        {

            throw new NotImplementedException();

        }

        /// <summary>
        /// Types the type of the name to NPG SQL database.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        internal static MySqlDbType? TypeNameToNpgSqlDbType(string typeName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resets the metadata cache, clearing out all cached metadata.
        /// </summary>
        public override void Reset()
        {
            m_StoredProcedures.Clear();
            m_TableFunctions.Clear();
            m_Tables.Clear();
            m_TypeTableMap.Clear();
        }

        /// <summary>
        /// Preloads the table value functions.
        /// </summary>
        public void PreloadStoredProcedures()
        {
            throw new NotImplementedException();

        }

        /// <summary>
        /// Preloads the table value functions.
        /// </summary>
        public void PreloadTableFunctions()
        {
            throw new NotImplementedException();

        }
    }
}
