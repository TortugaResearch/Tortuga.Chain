using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class AbstractSqlServerMetadataCache.
    /// </summary>
    /// <typeparam name="TDbType">The type of the t database type.</typeparam>
    /// <seealso cref="DatabaseMetadataCache{SqlServerObjectName, TDbType}" />
    public abstract class AbstractSqlServerMetadataCache<TDbType> : DatabaseMetadataCache<SqlServerObjectName, TDbType>
                where TDbType : struct
    {
        internal readonly DbConnectionStringBuilder m_ConnectionBuilder;
        internal readonly ConcurrentDictionary<SqlServerObjectName, StoredProcedureMetadata<SqlServerObjectName, TDbType>> m_StoredProcedures = new ConcurrentDictionary<SqlServerObjectName, StoredProcedureMetadata<SqlServerObjectName, TDbType>>();

        internal readonly ConcurrentDictionary<SqlServerObjectName, TableFunctionMetadata<SqlServerObjectName, TDbType>> m_TableFunctions = new ConcurrentDictionary<SqlServerObjectName, TableFunctionMetadata<SqlServerObjectName, TDbType>>();

        internal readonly ConcurrentDictionary<SqlServerObjectName, SqlServerTableOrViewMetadata<TDbType>> m_Tables = new ConcurrentDictionary<SqlServerObjectName, SqlServerTableOrViewMetadata<TDbType>>();

        internal readonly ConcurrentDictionary<Type, TableOrViewMetadata<SqlServerObjectName, TDbType>> m_TypeTableMap = new ConcurrentDictionary<Type, TableOrViewMetadata<SqlServerObjectName, TDbType>>();

        internal readonly ConcurrentDictionary<Type, string> m_UdtTypeMap = new ConcurrentDictionary<Type, string>();
        internal readonly ConcurrentDictionary<SqlServerObjectName, UserDefinedTypeMetadata<SqlServerObjectName, TDbType>> m_UserDefinedTypes = new ConcurrentDictionary<SqlServerObjectName, UserDefinedTypeMetadata<SqlServerObjectName, TDbType>>();
        internal string m_DefaultSchema;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractSqlServerMetadataCache{TDbType}"/> class.
        /// </summary>
        /// <param name="connectionBuilder">The connection builder.</param>
        internal AbstractSqlServerMetadataCache(DbConnectionStringBuilder connectionBuilder)
        {
            m_ConnectionBuilder = connectionBuilder;
        }

        /// <summary>
        /// It is necessary to map some types to their corresponding UDT Names in Sql Server.
        /// </summary>
        /// <param name="type">The type to be mapped</param>
        /// <param name="udtName">The name that SQL server sees</param>
        /// <remarks>The types SqlGeometry and SqlGeography are automatically included in the map.</remarks>
        public void AddUdtTypeName(Type type, string udtName)
        {
            m_UdtTypeMap[type] = udtName;
        }

        /// <summary>
        /// Gets the stored procedure's metadata.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>Null if the object could not be found.</returns>
        public override StoredProcedureMetadata<SqlServerObjectName, TDbType> GetStoredProcedure(SqlServerObjectName procedureName)
        {
            return m_StoredProcedures.GetOrAdd(procedureName, GetStoredProcedureInternal);
        }

        /// <summary>
        /// Gets the stored procedures that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all stored procedures were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<StoredProcedureMetadata<SqlServerObjectName, TDbType>> GetStoredProcedures()
        {
            return m_StoredProcedures.GetValues();
        }

        /// <summary>
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns>Null if the object could not be found.</returns>
        public override TableFunctionMetadata<SqlServerObjectName, TDbType> GetTableFunction(SqlServerObjectName tableFunctionName)
        {
            return m_TableFunctions.GetOrAdd(tableFunctionName, GetTableFunctionInternal);
        }

        /// <summary>
        /// Gets the table-valued functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all table-valued functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<TableFunctionMetadata<SqlServerObjectName, TDbType>> GetTableFunctions()
        {
            return m_TableFunctions.GetValues();
        }

        /// <summary>
        /// Gets the metadata for a table or view.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>Null if the object could not be found.</returns>
        public override TableOrViewMetadata<SqlServerObjectName, TDbType> GetTableOrView(SqlServerObjectName tableName)
        {
            return m_Tables.GetOrAdd(tableName, GetTableOrViewInternal);
        }

        ///// <summary>
        ///// Gets the UDT name of the indicated type.
        ///// </summary>
        ///// <param name="type">The type.</param>
        ///// <returns></returns>
        ///// <remarks>You may add custom UDTs to this list using AddUdtTypeName</remarks>
        //internal string GetUdtName(Type type)
        //{
        //    string result;
        //    m_UdtTypeMap.TryGetValue(type, out result);
        //    return result;
        //}

        /// <summary>
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public override TableOrViewMetadata<SqlServerObjectName, TDbType> GetTableOrViewFromClass<TObject>()
        {

            var type = typeof(TObject);
            TableOrViewMetadata<SqlServerObjectName, TDbType> result;
            if (m_TypeTableMap.TryGetValue(type, out result))
                return result;

            var typeInfo = MetadataCache.GetMetadata(type);
            if (!string.IsNullOrEmpty(typeInfo.MappedTableName))
            {
                if (string.IsNullOrEmpty(typeInfo.MappedSchemaName))
                    result = GetTableOrView(new SqlServerObjectName(typeInfo.MappedTableName));
                else
                    result = GetTableOrView(new SqlServerObjectName(typeInfo.MappedSchemaName, typeInfo.MappedTableName));
                m_TypeTableMap[type] = result;
                return result;
            }

            //infer schema from namespace
            var schema = type.Namespace;
            if (schema?.Contains(".") ?? false)
                schema = schema.Substring(schema.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) + 1);
            var name = type.Name;

            try
            {
                result = GetTableOrView(new SqlServerObjectName(schema, name));
                m_TypeTableMap[type] = result;
                return result;
            }
            catch (MissingObjectException) { }


            //that didn't work, so try the default schema
            result = GetTableOrView(new SqlServerObjectName(null, name));
            m_TypeTableMap[type] = result;
            return result;

        }

        /// <summary>
        /// Gets the tables and views that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all tables and views were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<TableOrViewMetadata<SqlServerObjectName, TDbType>> GetTablesAndViews()
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
            PreloadStoredProcedures();
            PreloadTableFunctions();
            PreloadUserDefinedTypes();
        }

        /// <summary>
        /// Preloads the stored procedures.
        /// </summary>
        public abstract void PreloadStoredProcedures();

        /// <summary>
        /// Preloads the table value functions.
        /// </summary>
        public abstract void PreloadTableFunctions();

        /// <summary>
        /// Preloads metadata for all tables.
        /// </summary>
        /// <remarks>This is normally used only for testing. By default, metadata is loaded as needed.</remarks>
        public abstract void PreloadTables();

        /// <summary>
        /// Preloads the user defined types.
        /// </summary>
        /// <remarks>This is normally used only for testing. By default, metadata is loaded as needed.</remarks>
        public abstract void PreloadUserDefinedTypes();

        /// <summary>
        /// Preloads metadata for all views.
        /// </summary>
        /// <remarks>This is normally used only for testing. By default, metadata is loaded as needed.</remarks>
        public abstract void PreloadViews();

        /// <summary>
        /// Resets the metadata cache, clearing out all cached metadata.
        /// </summary>
        public override void Reset()
        {
            m_StoredProcedures.Clear();
            m_TableFunctions.Clear();
            m_Tables.Clear();
            m_TypeTableMap.Clear();
            m_UdtTypeMap.Clear();
            m_UserDefinedTypes.Clear();
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal static void AdjustTypeDetails(string typeName, ref int? maxLength, ref int? precision, ref int? scale, out string fullTypeName)
        {
            switch (typeName)
            {
                case "bigint":
                case "bit":
                case "date":
                case "datetime":
                case "timestamp":
                case "tinyint":
                case "uniqueidentifier":
                case "smallint":
                case "sql_variant":
                case "float":
                case "int":
                    maxLength = null;
                    precision = null;
                    scale = null;
                    fullTypeName = typeName;
                    break;
                case "binary":
                case "char":
                    precision = null;
                    scale = null;
                    fullTypeName = $"{typeName}({maxLength})";
                    break;
                case "datetime2":
                case "datetimeoffset":
                case "time":
                    maxLength = null;
                    precision = null;
                    fullTypeName = $"{typeName}({scale})";
                    break;
                case "numeric":
                case "decimal":
                    fullTypeName = $"{typeName}({precision},{scale})";
                    break;
                case "nchar":
                    maxLength = maxLength / 2;
                    precision = null;
                    scale = null;
                    fullTypeName = $"nchar({maxLength})";
                    break;
                case "nvarchar":
                    maxLength = maxLength / 2;
                    precision = null;
                    scale = null;
                    if (maxLength > 0)
                        fullTypeName = $"nvarchar({maxLength})";
                    else
                        fullTypeName = $"nvarchar(max)";
                    break;
                case "varbinary":
                case "varchar":
                    precision = null;
                    scale = null;
                    if (maxLength > 0)
                        fullTypeName = $"{typeName}({maxLength})";
                    else
                        fullTypeName = $"{typeName}(max)";
                    break;
                default:
                    if (maxLength <= 0)
                        maxLength = 0;
                    if (precision <= 0)
                        precision = 0;
                    if (scale <= 0)
                        scale = 0;
                    fullTypeName = typeName;
                    break;
            }
        }

        internal abstract StoredProcedureMetadata<SqlServerObjectName, TDbType> GetStoredProcedureInternal(SqlServerObjectName procedureName);
        internal abstract TableFunctionMetadata<SqlServerObjectName, TDbType> GetTableFunctionInternal(SqlServerObjectName tableFunctionName);

        internal abstract SqlServerTableOrViewMetadata<TDbType> GetTableOrViewInternal(SqlServerObjectName tableName);
        /// <summary>
        /// Parse a string and return the database specific representation of the object name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override SqlServerObjectName ParseObjectName(string name)
        {
            return new SqlServerObjectName(name);
        }

        /// <summary>
        /// Gets the table-valued functions that were loaded by this cache.
        /// </summary>
        /// <returns>ICollection&lt;UserDefinedTypeMetadata&lt;SqlServerObjectName, SqlDbType&gt;&gt;.</returns>
        /// <remarks>Call Preload before invoking this method to ensure that all table-valued functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        public override IReadOnlyCollection<UserDefinedTypeMetadata<SqlServerObjectName, TDbType>> GetUserDefinedTypes()
        {
            return m_UserDefinedTypes.GetValues();
        }


        /// <summary>
        /// Gets the metadata for a user defined type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>UserDefinedTypeMetadata&lt;SqlServerObjectName, SqlDbType&gt;.</returns>
        public override UserDefinedTypeMetadata<SqlServerObjectName, TDbType> GetUserDefinedType(SqlServerObjectName typeName)
        {
            return m_UserDefinedTypes.GetOrAdd(typeName, GetUserDefinedTypeInternal);
        }

        internal abstract UserDefinedTypeMetadata<SqlServerObjectName, TDbType> GetUserDefinedTypeInternal(SqlServerObjectName typeName);
    }
}
