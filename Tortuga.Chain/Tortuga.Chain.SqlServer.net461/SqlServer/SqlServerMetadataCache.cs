using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerMetadataCache.
    /// </summary>
    public class SqlServerMetadataCache : DatabaseMetadataCache<SqlServerObjectName>
    {
        private readonly SqlConnectionStringBuilder m_ConnectionBuilder;
        private readonly ConcurrentDictionary<SqlServerObjectName, StoredProcedureMetadata<SqlServerObjectName>> m_StoredProcedures = new ConcurrentDictionary<SqlServerObjectName, StoredProcedureMetadata<SqlServerObjectName>>();
        private readonly ConcurrentDictionary<SqlServerObjectName, TableFunctionMetadata<SqlServerObjectName>> m_TableFunctions = new ConcurrentDictionary<SqlServerObjectName, TableFunctionMetadata<SqlServerObjectName>>();
        private readonly ConcurrentDictionary<SqlServerObjectName, TableOrViewMetadata<SqlServerObjectName>> m_Tables = new ConcurrentDictionary<SqlServerObjectName, TableOrViewMetadata<SqlServerObjectName>>();
        private readonly ConcurrentDictionary<Type, string> m_UdtTypeMap = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerMetadataCache"/> class.
        /// </summary>
        /// <param name="connectionBuilder">The connection builder.</param>
        public SqlServerMetadataCache(SqlConnectionStringBuilder connectionBuilder)
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
        public override StoredProcedureMetadata<SqlServerObjectName> GetStoredProcedure(SqlServerObjectName procedureName)
        {
            return m_StoredProcedures.GetOrAdd(procedureName, GetStoredProcedureInternal);
        }

        /// <summary>
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns>Null if the object could not be found.</returns>
        public override TableFunctionMetadata<SqlServerObjectName> GetTableFunction(SqlServerObjectName tableFunctionName)
        {
            return m_TableFunctions.GetOrAdd(tableFunctionName, GetFunctionInternal);
        }

        /// <summary>
        /// Gets the metadata for a table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>Null if the object could not be found.</returns>
        public override TableOrViewMetadata<SqlServerObjectName> GetTableOrView(SqlServerObjectName tableName)
        {
            return m_Tables.GetOrAdd(tableName, GetTableOrViewInternal);
        }

        /// <summary>
        /// Gets the UDT name of the indicated type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <remarks>You may add custom UDTs to this list using AddUdtTypeName</remarks>
        internal string GetUdtName(Type type)
        {
            string result;
            m_UdtTypeMap.TryGetValue(type, out result);
            return result;
        }
        List<ColumnMetadata> GetColumns(int objectId)
        {
            const string ColumnSql =
                @"WITH    PKS
                          AS ( SELECT   c.name ,
                                        1 AS is_primary_key
                               FROM     sys.indexes i
                                        INNER JOIN sys.index_columns ic ON i.index_id = ic.index_id
                                                                           AND ic.object_id = @ObjectId
                                        INNER JOIN sys.columns c ON ic.column_id = c.column_id
                                                                    AND c.object_id = @ObjectId
                               WHERE    i.is_primary_key = 1
                                        AND ic.is_included_column = 0
                                        AND i.object_id = @ObjectId
                             )
                    SELECT  c.name AS ColumnName ,
                            c.is_computed ,
                            c.is_identity ,
                            c.column_id ,
                            Convert(bit, ISNULL(PKS.is_primary_key, 0)) AS is_primary_key
                    FROM    sys.columns c
                            LEFT JOIN PKS ON c.name = PKS.name
                    WHERE   object_id = @ObjectId;";

            var columns = new List<ColumnMetadata>();
            using (var con = new SqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(ColumnSql, con))
                {
                    cmd.Parameters.AddWithValue("@ObjectId", objectId);
                    using (var reader = cmd.ExecuteReader(/*CommandBehavior.SequentialAccess*/))
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(reader.GetOrdinal("ColumnName"));
                            var computed = reader.GetBoolean(reader.GetOrdinal("is_computed"));
                            var primary = reader.GetBoolean(reader.GetOrdinal("is_primary_key"));
                            var isIdentity = reader.GetBoolean(reader.GetOrdinal("is_identity"));
                            columns.Add(new ColumnMetadata(name, computed, primary, isIdentity));
                        }
                    }
                }
            }
            return columns;
        }

        private TableFunctionMetadata<SqlServerObjectName> GetFunctionInternal(SqlServerObjectName tableFunctionName)
        {
            const string StoredProcedureSql =
                @"SELECT 
                s.name AS SchemaName,
                o.name AS Name,
                o.object_id AS ObjectId
                FROM sys.objects o
                INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
                WHERE o.type = 'TF' AND s.name = @Schema AND o.Name = @Name";


            string actualSchema;
            string actualName;
            int objectId;

            using (var con = new SqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(StoredProcedureSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", tableFunctionName.Schema);
                    cmd.Parameters.AddWithValue("@Name", tableFunctionName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;
                        actualSchema = reader.GetString(reader.GetOrdinal("SchemaName"));
                        actualName = reader.GetString(reader.GetOrdinal("Name"));
                        objectId = reader.GetInt32(reader.GetOrdinal("ObjectId"));
                    }
                }
            }
            var columns = GetColumns(objectId);
            var parameters = GetParameters(objectId);

            return new TableFunctionMetadata<SqlServerObjectName>(new SqlServerObjectName(actualSchema, actualName), parameters, columns);
        }

        List<ParameterMetadata> GetParameters(int objectId)
        {
            const string ParameterSql =
                @"SELECT 
                p.name AS ParameterName
                FROM sys.parameters p 
                WHERE p.object_id = @ObjectId
                ORDER BY parameter_id";

            var parameters = new List<ParameterMetadata>();

            using (var con = new SqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();

                using (var cmd = new SqlCommand(ParameterSql, con))
                {
                    cmd.Parameters.AddWithValue("@ObjectId", objectId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(reader.GetOrdinal("ParameterName"));
                            parameters.Add(new ParameterMetadata(name));
                        }
                    }
                }
            }
            return parameters;
        }

        private StoredProcedureMetadata<SqlServerObjectName> GetStoredProcedureInternal(SqlServerObjectName procedureName)
        {
            const string StoredProcedureSql =
                @"SELECT 
                s.name AS SchemaName,
                sp.name AS Name,
                sp.object_id AS ObjectId
                FROM SYS.procedures sp
                INNER JOIN sys.schemas s ON sp.schema_id = s.schema_id
                WHERE s.name = @Schema AND sp.Name = @Name";


            string actualSchema;
            string actualName;
            int objectId;

            using (var con = new SqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(StoredProcedureSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", procedureName.Schema);
                    cmd.Parameters.AddWithValue("@Name", procedureName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;
                        actualSchema = reader.GetString(reader.GetOrdinal("SchemaName"));
                        actualName = reader.GetString(reader.GetOrdinal("Name"));
                        objectId = reader.GetInt32(reader.GetOrdinal("ObjectId"));
                    }
                }
            }
            var parameters = GetParameters(objectId);

            return new StoredProcedureMetadata<SqlServerObjectName>(new SqlServerObjectName(actualSchema, actualName), parameters);
        }
        TableOrViewMetadata<SqlServerObjectName> GetTableOrViewInternal(SqlServerObjectName tableName)
        {
            const string TableSql =
                @"SELECT 
                s.name AS SchemaName,
                t.name AS Name,
                t.object_id AS ObjectId,
                CONVERT(BIT, 1) AS IsTable 
                FROM SYS.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE s.name = @Schema AND t.Name = @Name

                UNION ALL

                SELECT 
                s.name AS SchemaName,
                t.name AS Name,
                t.object_id AS ObjectId,
                CONVERT(BIT, 0) AS IsTable 
                FROM SYS.views t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE s.name = @Schema AND t.Name = @Name";


            string actualSchema;
            string actualName;
            int objectId;
            bool isTable;

            using (var con = new SqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(TableSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", tableName.Schema);
                    cmd.Parameters.AddWithValue("@Name", tableName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;
                        actualSchema = reader.GetString(reader.GetOrdinal("SchemaName"));
                        actualName = reader.GetString(reader.GetOrdinal("Name"));
                        objectId = reader.GetInt32(reader.GetOrdinal("ObjectId"));
                        isTable = reader.GetBoolean(reader.GetOrdinal("IsTable"));
                    }
                }
            }


            var columns = GetColumns(objectId);

            return new TableOrViewMetadata<SqlServerObjectName>(new SqlServerObjectName(actualSchema, actualName), isTable, columns);
        }
    }

}


