using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerMetadataCache.
    /// </summary>
    public class SqlServerMetadataCache : DatabaseMetadataCache<SqlServerObjectName, SqlDbType>
    {
        private readonly SqlConnectionStringBuilder m_ConnectionBuilder;
        private readonly ConcurrentDictionary<SqlServerObjectName, StoredProcedureMetadata<SqlServerObjectName, SqlDbType>> m_StoredProcedures = new ConcurrentDictionary<SqlServerObjectName, StoredProcedureMetadata<SqlServerObjectName, SqlDbType>>();
        private readonly ConcurrentDictionary<SqlServerObjectName, TableFunctionMetadata<SqlServerObjectName, SqlDbType>> m_TableFunctions = new ConcurrentDictionary<SqlServerObjectName, TableFunctionMetadata<SqlServerObjectName, SqlDbType>>();
        private readonly ConcurrentDictionary<SqlServerObjectName, TableOrViewMetadata<SqlServerObjectName, SqlDbType>> m_Tables = new ConcurrentDictionary<SqlServerObjectName, TableOrViewMetadata<SqlServerObjectName, SqlDbType>>();
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
        public override StoredProcedureMetadata<SqlServerObjectName, SqlDbType> GetStoredProcedure(SqlServerObjectName procedureName)
        {
            return m_StoredProcedures.GetOrAdd(procedureName, GetStoredProcedureInternal);
        }

        /// <summary>
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns>Null if the object could not be found.</returns>
        public override TableFunctionMetadata<SqlServerObjectName, SqlDbType> GetTableFunction(SqlServerObjectName tableFunctionName)
        {
            return m_TableFunctions.GetOrAdd(tableFunctionName, GetFunctionInternal);
        }

        /// <summary>
        /// Gets the metadata for a table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>Null if the object could not be found.</returns>
        public override TableOrViewMetadata<SqlServerObjectName, SqlDbType> GetTableOrView(SqlServerObjectName tableName)
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
        List<ColumnMetadata<SqlDbType>> GetColumns(int objectId)
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
							Convert(bit, ISNULL(PKS.is_primary_key, 0)) AS is_primary_key,
							t.name as TypeName
					FROM    sys.columns c
							LEFT JOIN PKS ON c.name = PKS.name
							LEFT JOIN sys.types t on c.system_type_id = t.user_type_id
							WHERE   object_id = @ObjectId;";

            var columns = new List<ColumnMetadata<SqlDbType>>();
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
                            var typeName = reader.IsDBNull(reader.GetOrdinal("TypeName")) ? null : reader.GetString(reader.GetOrdinal("TypeName"));
                            columns.Add(new ColumnMetadata<SqlDbType>(name, computed, primary, isIdentity, typeName, TypeNameToSqlDbType(typeName)));
                        }
                    }
                }
            }
            return columns;
        }

        private TableFunctionMetadata<SqlServerObjectName, SqlDbType> GetFunctionInternal(SqlServerObjectName tableFunctionName)
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

            return new TableFunctionMetadata<SqlServerObjectName, SqlDbType>(new SqlServerObjectName(actualSchema, actualName), parameters, columns);
        }

        List<ParameterMetadata<SqlDbType>> GetParameters(int objectId)
        {
            const string ParameterSql =
                @"SELECT 
				p.name AS ParameterName,
				t.name as TypeName
				FROM sys.parameters p 
				LEFT JOIN sys.types t ON p.system_type_id = t.user_type_id
				WHERE p.object_id = @ObjectId
				ORDER BY parameter_id";

            var parameters = new List<ParameterMetadata<SqlDbType>>();

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
                            var typeName = reader.GetString(reader.GetOrdinal("TypeName"));
                            parameters.Add(new ParameterMetadata<SqlDbType>(name, typeName, TypeNameToSqlDbType(typeName)));
                        }
                    }
                }
            }
            return parameters;
        }

        private StoredProcedureMetadata<SqlServerObjectName, SqlDbType> GetStoredProcedureInternal(SqlServerObjectName procedureName)
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

            return new StoredProcedureMetadata<SqlServerObjectName, SqlDbType>(new SqlServerObjectName(actualSchema, actualName), parameters);
        }
        TableOrViewMetadata<SqlServerObjectName, SqlDbType> GetTableOrViewInternal(SqlServerObjectName tableName)
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

            return new TableOrViewMetadata<SqlServerObjectName, SqlDbType>(new SqlServerObjectName(actualSchema, actualName), isTable, columns);
        }

        /// <summary>
        /// Preloads metadata for all tables.
        /// </summary>
        /// <remarks>This is normally used only for testing. By default, metadata is loaded as needed.</remarks>
        public void PreloadTables()
        {
            const string tableList = "SELECT t.name AS Name, s.name AS SchemaName FROM sys.tables t INNER JOIN sys.schemas s ON t.schema_id=s.schema_id ORDER BY s.name, t.name";

            using (var con = new SqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(tableList, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(reader.GetOrdinal("SchemaName"));
                            var name = reader.GetString(reader.GetOrdinal("Name"));
                            GetTableOrView(new SqlServerObjectName(schema, name));
                        }
                    }
                }
            }

        }


        /// <summary>
        /// Preloads metadata for all views.
        /// </summary>
        /// <remarks>This is normally used only for testing. By default, metadata is loaded as needed.</remarks>
        public void PreloadViews()
        {
            const string tableList = "SELECT t.name AS Name, s.name AS SchemaName FROM sys.views t INNER JOIN sys.schemas s ON t.schema_id=s.schema_id ORDER BY s.name, t.name";

            using (var con = new SqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(tableList, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(reader.GetOrdinal("SchemaName"));
                            var name = reader.GetString(reader.GetOrdinal("Name"));
                            GetTableOrView(new SqlServerObjectName(schema, name));
                        }
                    }
                }
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal static SqlDbType? TypeNameToSqlDbType(string typeName)
        {
            switch (typeName)
            {
                case "bigint": return SqlDbType.BigInt;
                case "binary": return SqlDbType.Binary;
                case "bit": return SqlDbType.Bit;
                case "char": return SqlDbType.Char;
                case "date": return SqlDbType.Date;
                case "datetime": return SqlDbType.DateTime;
                case "datetime2": return SqlDbType.DateTime2;
                case "datetimeoffset": return SqlDbType.DateTimeOffset;
                case "decimal": return SqlDbType.Decimal;
                case "float": return SqlDbType.Float;
                //case "geography": m_SqlDbType = SqlDbType.; 
                //case "geometry": m_SqlDbType = SqlDbType; 
                //case "hierarchyid": m_SqlDbType = SqlDbType.; 
                case "image": return SqlDbType.Image;
                case "int": return SqlDbType.Int;
                case "money": return SqlDbType.Money;
                case "nchar": return SqlDbType.NChar;
                case "ntext": return SqlDbType.NText;
                case "numeric": return SqlDbType.Decimal;
                case "nvarchar": return SqlDbType.NVarChar;
                case "real": return SqlDbType.Real;
                case "smalldatetime": return SqlDbType.SmallDateTime;
                case "smallint": return SqlDbType.SmallInt;
                case "smallmoney": return SqlDbType.SmallMoney;
                //case "sql_variant": m_SqlDbType = SqlDbType; 
                //case "sysname": m_SqlDbType = SqlDbType; 
                case "text": return SqlDbType.Text;
                case "time": return SqlDbType.Time;
                case "timestamp": return SqlDbType.Timestamp;
                case "tinyint": return SqlDbType.TinyInt;
                case "uniqueidentifier": return SqlDbType.UniqueIdentifier;
                case "varbinary": return SqlDbType.VarBinary;
                case "varchar": return SqlDbType.VarChar;
                case "xml": return SqlDbType.Xml;
            }

            return null;
        }


    }

}


