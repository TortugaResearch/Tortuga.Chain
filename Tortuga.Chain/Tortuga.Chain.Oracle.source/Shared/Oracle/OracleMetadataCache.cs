using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tortuga.Anchor;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Oracle
{
    /// <summary>
    /// Class OracleMetadataCache.
    /// </summary>
    public class OracleMetadataCache : DatabaseMetadataCache<OracleObjectName, OracleDbType>
    {
        readonly OracleConnectionStringBuilder m_ConnectionBuilder;
        readonly ConcurrentDictionary<OracleObjectName, TableOrViewMetadata<OracleObjectName, OracleDbType>> m_Tables = new ConcurrentDictionary<OracleObjectName, TableOrViewMetadata<OracleObjectName, OracleDbType>>();
        readonly ConcurrentDictionary<OracleObjectName, StoredProcedureMetadata<OracleObjectName, OracleDbType>> m_StoredProcedures = new ConcurrentDictionary<OracleObjectName, StoredProcedureMetadata<OracleObjectName, OracleDbType>>();
        readonly ConcurrentDictionary<Type, TableOrViewMetadata<OracleObjectName, OracleDbType>> m_TypeTableMap = new ConcurrentDictionary<Type, TableOrViewMetadata<OracleObjectName, OracleDbType>>();
        readonly ConcurrentDictionary<OracleObjectName, TableFunctionMetadata<OracleObjectName, OracleDbType>> m_TableFunctions = new ConcurrentDictionary<OracleObjectName, TableFunctionMetadata<OracleObjectName, OracleDbType>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleMetadataCache"/> class.
        /// </summary>
        /// <param name="connectionBuilder">The connection builder.</param>
        public OracleMetadataCache(OracleConnectionStringBuilder connectionBuilder)
        {
            m_ConnectionBuilder = connectionBuilder;
        }

        /// <summary>
        /// Gets the stored procedure's metadata.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override StoredProcedureMetadata<OracleObjectName, OracleDbType> GetStoredProcedure(OracleObjectName procedureName)
        {
            return m_StoredProcedures.GetOrAdd(procedureName, GetStoredProcedureInteral);
        }

        /// <summary>
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override TableFunctionMetadata<OracleObjectName, OracleDbType> GetTableFunction(OracleObjectName tableFunctionName)
        {
            return m_TableFunctions.GetOrAdd(tableFunctionName, GetTableFunctionInternal);
        }

        /// <summary>
        /// Gets the metadata for a table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>TableOrViewMetadata&lt;OracleObjectName, OracleDbType&gt;.</returns>
        public override TableOrViewMetadata<OracleObjectName, OracleDbType> GetTableOrView(OracleObjectName tableName)
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
        public override IReadOnlyCollection<TableOrViewMetadata<OracleObjectName, OracleDbType>> GetTablesAndViews()
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
            const string TableSql =
                @"SELECT
                owner as schemaname,
                table_name as tablename,
                FROM sys.all_tables
                WHERE
                ";

            throw new NotImplementedException();
        }

        private TableOrViewMetadata<OracleObjectName, OracleDbType> GetTableOrViewInternal(OracleObjectName tableName)
        {
            throw new NotImplementedException();
        }

        private TableFunctionMetadata<OracleObjectName, OracleDbType> GetTableFunctionInternal(OracleObjectName tableFunctionName)
        {
            throw new NotImplementedException();


        }



        private StoredProcedureMetadata<OracleObjectName, OracleDbType> GetStoredProcedureInteral(OracleObjectName storedProcedureName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parse a string and return the database specific representation of the object name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>OracleObjectName.</returns>
        protected override OracleObjectName ParseObjectName(string name)
        {
            return new OracleObjectName(name);
        }

        /// <summary>
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public override TableOrViewMetadata<OracleObjectName, OracleDbType> GetTableOrViewFromClass<TObject>()
        {

            throw new NotImplementedException();

        }

        /// <summary>
        /// Types the type of the name to Oracle SQL database.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        internal static OracleDbType? TypeNameToOracleDbType(string typeName)
        {
            switch(typeName)
            {
                case "char": return OracleDbType.Char;
                case "nchar": return OracleDbType.NChar;
                case "nvarchar2": return OracleDbType.NVarchar2;
                case "varchar2": return OracleDbType.Varchar2;
                case "long": return OracleDbType.Long;
                case "raw": return OracleDbType.Raw;
                case "long raw": return OracleDbType.LongRaw;
                case "number": //fall through to Decimal
                case "numeric": //fall through to Decimal
                case "float": //fall through to Decimal
                case "dec": //fall through to Decimal
                case "decimal": return OracleDbType.Decimal;
                case "int": //fall through to integer
                case "integer": return OracleDbType.Int32;
                case "smallint": return OracleDbType.Int16;
                //TODO: finish
            }
            return null;
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
