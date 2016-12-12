#if !OleDb_Missing
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class OleDbSqlServerMetadataCache.
    /// </summary>
    /// <seealso cref="DatabaseMetadataCache{SqlServerObjectName, OleDbType}" />
    public sealed class OleDbSqlServerMetadataCache : AbstractSqlServerMetadataCache<OleDbType>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbSqlServerMetadataCache"/> class.
        /// </summary>
        /// <param name="connectionBuilder">The connection builder.</param>
        public OleDbSqlServerMetadataCache(OleDbConnectionStringBuilder connectionBuilder) : base(connectionBuilder)
        {
        }

        /// <summary>
        /// Returns the user's default schema.
        /// </summary>
        /// <returns></returns>
        public string DefaultSchema
        {
            get
            {
                if (m_DefaultSchema == null)
                {
                    using (var con = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
                    {
                        con.Open();
                        using (var cmd = new OleDbCommand("SELECT SCHEMA_NAME () AS DefaultSchema", con))
                        {
                            m_DefaultSchema = (string)cmd.ExecuteScalar();
                        }
                    }
                }
                return m_DefaultSchema;
            }
        }

        /// <summary>
        /// Preloads the stored procedures.
        /// </summary>
        public override void PreloadStoredProcedures()
        {
            const string StoredProcedureSql =
            @"SELECT 
				s.name AS SchemaName,
				sp.name AS Name
				FROM SYS.procedures sp
				INNER JOIN sys.schemas s ON sp.schema_id = s.schema_id;";


            using (var con = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new OleDbCommand(StoredProcedureSql, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(reader.GetOrdinal("SchemaName"));
                            var name = reader.GetString(reader.GetOrdinal("Name"));
                            GetStoredProcedure(new SqlServerObjectName(schema, name));
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Preloads the table value functions.
        /// </summary>
        public override void PreloadTableFunctions()
        {
            const string TvfSql =
                @"SELECT 
				s.name AS SchemaName,
				o.name AS Name,
				o.object_id AS ObjectId
				FROM sys.objects o
				INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
				WHERE o.type in ('TF', 'IF', 'FT')";


            using (var con = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new OleDbCommand(TvfSql, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(reader.GetOrdinal("SchemaName"));
                            var name = reader.GetString(reader.GetOrdinal("Name"));
                            GetTableFunction(new SqlServerObjectName(schema, name));
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Preloads metadata for all tables.
        /// </summary>
        /// <remarks>This is normally used only for testing. By default, metadata is loaded as needed.</remarks>
        public override void PreloadTables()
        {
            const string tableList = "SELECT t.name AS Name, s.name AS SchemaName FROM sys.tables t INNER JOIN sys.schemas s ON t.schema_id=s.schema_id ORDER BY s.name, t.name";

            using (var con = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new OleDbCommand(tableList, con))
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
        /// Preloads the user defined types.
        /// </summary>
        /// <remarks>This is normally used only for testing. By default, metadata is loaded as needed.</remarks>
        public override void PreloadUserDefinedTypes()
        {
            const string tableList = @"SELECT s.name AS SchemaName, t.name AS Name FROM sys.types t INNER JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE	t.is_user_defined = 1;";

            using (var con = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new OleDbCommand(tableList, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(reader.GetOrdinal("SchemaName"));
                            var name = reader.GetString(reader.GetOrdinal("Name"));
                            GetUserDefinedType(new SqlServerObjectName(schema, name));
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Preloads metadata for all views.
        /// </summary>
        /// <remarks>This is normally used only for testing. By default, metadata is loaded as needed.</remarks>
        public override void PreloadViews()
        {
            const string tableList = "SELECT t.name AS Name, s.name AS SchemaName FROM sys.views t INNER JOIN sys.schemas s ON t.schema_id=s.schema_id ORDER BY s.name, t.name";

            using (var con = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new OleDbCommand(tableList, con))
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

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal static OleDbType? TypeNameToSqlDbType(string typeName)
        {
            switch (typeName)
            {
                case "bigint": return OleDbType.BigInt;
                case "binary": return OleDbType.Binary;
                case "bit": return OleDbType.Boolean;
                case "char": return OleDbType.Char;
                case "date": return OleDbType.DBDate;
                case "datetime": return OleDbType.DBTimeStamp;
                case "datetime2": return OleDbType.DBTimeStamp;
                //case "datetimeoffset": return OleDbType;
                case "decimal": return OleDbType.Decimal;
                case "float": return OleDbType.Single;
                //case "geography": m_SqlDbType = OleDbType.; 
                //case "geometry": m_SqlDbType = OleDbType; 
                //case "hierarchyid": m_SqlDbType = OleDbType.; 
                //case "image": return OleDbType.Image;
                case "int": return OleDbType.Integer;
                case "money": return OleDbType.Currency;
                case "nchar": return OleDbType.WChar;
                case "ntext": return OleDbType.LongVarWChar;
                case "numeric": return OleDbType.Numeric;
                case "nvarchar": return OleDbType.VarWChar;
                case "real": return OleDbType.Single;
                case "smalldatetime": return OleDbType.DBTimeStamp;
                case "smallint": return OleDbType.SmallInt;
                case "smallmoney": return OleDbType.Currency;
                //case "sql_variant": m_SqlDbType = OleDbType; 
                //case "sysname": m_SqlDbType = OleDbType; 
                case "text": return OleDbType.LongVarWChar;
                case "time": return OleDbType.DBTime;
                case "timestamp": return OleDbType.DBTimeStamp;
                case "tinyint": return OleDbType.TinyInt;
                case "uniqueidentifier": return OleDbType.Guid;
                case "varbinary": return OleDbType.VarBinary;
                case "varchar": return OleDbType.VarChar;
                    //case "xml": return OleDbType;
            }

            return null;
        }

        internal override StoredProcedureMetadata<SqlServerObjectName, OleDbType> GetStoredProcedureInternal(SqlServerObjectName procedureName)
        {
            const string StoredProcedureSql =
                @"SELECT 
				s.name AS SchemaName,
				sp.name AS Name,
				sp.object_id AS ObjectId
				FROM SYS.procedures sp
				INNER JOIN sys.schemas s ON sp.schema_id = s.schema_id
				WHERE s.name = ? AND sp.Name = ?";


            string actualSchema;
            string actualName;
            int objectId;

            using (var con = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new OleDbCommand(StoredProcedureSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", procedureName.Schema ?? DefaultSchema);
                    cmd.Parameters.AddWithValue("@Name", procedureName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find stored procedure {procedureName}");

                        actualSchema = reader.GetString(reader.GetOrdinal("SchemaName"));
                        actualName = reader.GetString(reader.GetOrdinal("Name"));
                        objectId = reader.GetInt32(reader.GetOrdinal("ObjectId"));
                    }
                }
            }
            var objectName = new SqlServerObjectName(actualSchema, actualName);
            var parameters = GetParameters(objectName.ToString(), objectId);

            return new StoredProcedureMetadata<SqlServerObjectName, OleDbType>(objectName, parameters);
        }

        internal override TableFunctionMetadata<SqlServerObjectName, OleDbType> GetTableFunctionInternal(SqlServerObjectName tableFunctionName)
        {
            const string TvfSql =
                @"SELECT 
				s.name AS SchemaName,
				o.name AS Name,
				o.object_id AS ObjectId
				FROM sys.objects o
				INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
				WHERE o.type in ('TF', 'IF', 'FT') AND s.name = ? AND o.Name = ?";

            /*
             * TF = SQL table-valued-function
             * IF = SQL inline table-valued function
             * FT = Assembly (CLR) table-valued function
             */


            string actualSchema;
            string actualName;
            int objectId;

            using (var con = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new OleDbCommand(TvfSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", tableFunctionName.Schema ?? DefaultSchema);
                    cmd.Parameters.AddWithValue("@Name", tableFunctionName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find table valued function {tableFunctionName}");
                        actualSchema = reader.GetString(reader.GetOrdinal("SchemaName"));
                        actualName = reader.GetString(reader.GetOrdinal("Name"));
                        objectId = reader.GetInt32(reader.GetOrdinal("ObjectId"));
                    }
                }
            }
            var objectName = new SqlServerObjectName(actualSchema, actualName);

            var columns = GetColumns(objectId);
            var parameters = GetParameters(objectName.ToString(), objectId);

            return new TableFunctionMetadata<SqlServerObjectName, OleDbType>(objectName, parameters, columns);
        }

        internal override SqlServerTableOrViewMetadata<OleDbType> GetTableOrViewInternal(SqlServerObjectName tableName)
        {
            const string TableSql =
                @"SELECT 
				s.name AS SchemaName,
				t.name AS Name,
				t.object_id AS ObjectId,
				CONVERT(BIT, 1) AS IsTable,
		        (SELECT	COUNT(*) FROM sys.triggers t2 WHERE	t2.parent_id = t.object_id) AS Triggers  
				FROM SYS.tables t
				INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
				WHERE s.name = ? AND t.Name = ?

				UNION ALL

				SELECT 
				s.name AS SchemaName,
				t.name AS Name,
				t.object_id AS ObjectId,
				CONVERT(BIT, 0) AS IsTable,
		        (SELECT	COUNT(*) FROM sys.triggers t2 WHERE	t2.parent_id = t.object_id) AS Triggers  
				FROM SYS.views t
				INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
				WHERE s.name = ? AND t.Name = ?";


            string actualSchema;
            string actualName;
            int objectId;
            bool isTable;
            bool hasTriggers;

            using (var con = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new OleDbCommand(TableSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema1", tableName.Schema ?? DefaultSchema);
                    cmd.Parameters.AddWithValue("@Name1", tableName.Name);
                    cmd.Parameters.AddWithValue("@Schema2", tableName.Schema ?? DefaultSchema);
                    cmd.Parameters.AddWithValue("@Name2", tableName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find table or view {tableName}");
                        actualSchema = reader.GetString(reader.GetOrdinal("SchemaName"));
                        actualName = reader.GetString(reader.GetOrdinal("Name"));
                        objectId = reader.GetInt32(reader.GetOrdinal("ObjectId"));
                        isTable = reader.GetBoolean(reader.GetOrdinal("IsTable"));
                        hasTriggers = reader.GetInt32(reader.GetOrdinal("Triggers")) > 0;
                    }
                }
            }


            var columns = GetColumns(objectId);

            return new SqlServerTableOrViewMetadata<OleDbType>(new SqlServerObjectName(actualSchema, actualName), isTable, columns, hasTriggers);
        }

        internal override UserDefinedTypeMetadata<SqlServerObjectName, OleDbType> GetUserDefinedTypeInternal(SqlServerObjectName typeName)
        {
            const string sql =
                @"SELECT	s.name AS SchemaName,
		t.name AS Name,
		tt.type_table_object_id AS ObjectId,
		t.is_table_type AS IsTableType,
		t2.name AS BaseTypeName,
		t.is_nullable,
		CONVERT(INT, t.max_length) AS max_length, 
		CONVERT(INT, t.precision) AS precision,
		CONVERT(INT, t.scale) AS scale
FROM	sys.types t
		INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
		LEFT JOIN sys.table_types tt ON tt.user_type_id = t.user_type_id
		LEFT JOIN sys.types t2 ON t.system_type_id = t2.user_type_id
WHERE	s.name = ? AND t.name = ?;";

            string actualSchema;
            string actualName;
            string baseTypeName = null;
            int? objectId = null;
            bool isTableType;
            bool isNullable;
            int? maxLength;
            int? precision;
            int? scale;
            string fullTypeName;

            using (var con = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new OleDbCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", typeName.Schema ?? DefaultSchema);
                    cmd.Parameters.AddWithValue("@Name", typeName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find user defined type {typeName}");

                        actualSchema = reader.GetString(reader.GetOrdinal("SchemaName"));
                        actualName = reader.GetString(reader.GetOrdinal("Name"));
                        if (!reader.IsDBNull(reader.GetOrdinal("ObjectId")))
                            objectId = reader.GetInt32(reader.GetOrdinal("ObjectId"));
                        isTableType = reader.GetBoolean(reader.GetOrdinal("IsTableType"));
                        if (!reader.IsDBNull(reader.GetOrdinal("BaseTypeName")))
                            baseTypeName = reader.GetString(reader.GetOrdinal("BaseTypeName"));

                        isNullable = reader.GetBoolean(reader.GetOrdinal("is_nullable"));
                        maxLength = reader.GetInt32(reader.GetOrdinal("max_length"));
                        precision = reader.GetInt32(reader.GetOrdinal("precision"));
                        scale = reader.GetInt32(reader.GetOrdinal("scale"));

                        AdjustTypeDetails(baseTypeName, ref maxLength, ref precision, ref scale, out fullTypeName);

                    }
                }
            }

            List<ColumnMetadata<OleDbType>> columns;

            if (isTableType)
                columns = GetColumns(objectId.Value);
            else
            {
                columns = new List<ColumnMetadata<OleDbType>>();
                columns.Add(new ColumnMetadata<OleDbType>(null, false, false, false, baseTypeName, TypeNameToSqlDbType(baseTypeName), null, isNullable, maxLength, precision, scale, fullTypeName));
            }

            return new UserDefinedTypeMetadata<SqlServerObjectName, OleDbType>(new SqlServerObjectName(actualSchema, actualName), isTableType, columns);
        }

        List<ColumnMetadata<OleDbType>> GetColumns(int objectId)
        {
            const string ColumnSql =
                @"WITH    PKS
						  AS ( SELECT   c.name ,
										1 AS is_primary_key
							   FROM     sys.indexes i
										INNER JOIN sys.index_columns ic ON i.index_id = ic.index_id
																		   AND ic.object_id = ?
										INNER JOIN sys.columns c ON ic.column_id = c.column_id
																	AND c.object_id = ?
							   WHERE    i.is_primary_key = 1
										AND ic.is_included_column = 0
										AND i.object_id = ?
							 )
					SELECT  c.name AS ColumnName ,
							c.is_computed ,
							c.is_identity ,
							c.column_id ,
							Convert(bit, ISNULL(PKS.is_primary_key, 0)) AS is_primary_key,
							COALESCE(t.name, t2.name) AS TypeName,
							c.is_nullable,
		                    CONVERT(INT, COALESCE(t.max_length, t2.max_length)) AS max_length, 
		                    CONVERT(INT, COALESCE(t.precision, t2.precision)) AS precision,
		                    CONVERT(INT, COALESCE(t.scale, t2.scale)) AS scale
					FROM    sys.columns c
							LEFT JOIN PKS ON c.name = PKS.name
							LEFT JOIN sys.types t on c.system_type_id = t.user_type_id
							LEFT JOIN sys.types t2 ON c.user_type_id = t2.user_type_id
                            WHERE   object_id = ?;";

            var columns = new List<ColumnMetadata<OleDbType>>();
            using (var con = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new OleDbCommand(ColumnSql, con))
                {
                    cmd.Parameters.AddWithValue("@ObjectId1", objectId);
                    cmd.Parameters.AddWithValue("@ObjectId2", objectId);
                    cmd.Parameters.AddWithValue("@ObjectId3", objectId);
                    cmd.Parameters.AddWithValue("@ObjectId4", objectId);
                    using (var reader = cmd.ExecuteReader(/*CommandBehavior.SequentialAccess*/))
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(reader.GetOrdinal("ColumnName"));
                            var computed = reader.GetBoolean(reader.GetOrdinal("is_computed"));
                            var primary = reader.GetBoolean(reader.GetOrdinal("is_primary_key"));
                            var isIdentity = reader.GetBoolean(reader.GetOrdinal("is_identity"));
                            var typeName = reader.IsDBNull(reader.GetOrdinal("TypeName")) ? null : reader.GetString(reader.GetOrdinal("TypeName"));
                            var isNullable = reader.GetBoolean(reader.GetOrdinal("is_nullable"));
                            int? maxLength = reader.IsDBNull(reader.GetOrdinal("max_length")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("max_length"));
                            int? precision = reader.IsDBNull(reader.GetOrdinal("precision")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("precision"));
                            int? scale = reader.IsDBNull(reader.GetOrdinal("scale")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("scale"));
                            string fullTypeName;
                            AdjustTypeDetails(typeName, ref maxLength, ref precision, ref scale, out fullTypeName);

                            columns.Add(new ColumnMetadata<OleDbType>(name, computed, primary, isIdentity, typeName, TypeNameToSqlDbType(typeName), "[" + name + "]", isNullable, maxLength, precision, scale, fullTypeName));
                        }
                    }
                }
            }
            return columns;
        }

        List<ParameterMetadata<OleDbType>> GetParameters(string procedureName, int objectId)
        {
            try
            {
                const string ParameterSql =
                    @"SELECT  p.name AS ParameterName ,
            COALESCE(t.name, t2.name) AS TypeName,
			COALESCE(t.is_nullable, t2.is_nullable)  as is_nullable,
		    CONVERT(INT, t.max_length) AS max_length, 
		    CONVERT(INT, t.precision) AS precision,
		    CONVERT(INT, t.scale) AS scale
            FROM    sys.parameters p
                    LEFT JOIN sys.types t ON p.system_type_id = t.user_type_id
                    LEFT JOIN sys.types t2 ON p.user_type_id = t2.user_type_id
            WHERE   p.object_id = ?
            ORDER BY p.parameter_id;";

                var parameters = new List<ParameterMetadata<OleDbType>>();

                using (var con = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
                {
                    con.Open();

                    using (var cmd = new OleDbCommand(ParameterSql, con))
                    {
                        cmd.Parameters.AddWithValue("@ObjectId", objectId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var name = reader.GetString(reader.GetOrdinal("ParameterName"));
                                var typeName = reader.GetString(reader.GetOrdinal("TypeName"));
                                parameters.Add(new ParameterMetadata<OleDbType>(name, name, typeName, TypeNameToSqlDbType(typeName)));
                            }
                        }
                    }
                }
                return parameters;
            }
            catch (Exception ex)
            {
                throw new MetadataException($"Error getting parameters for {procedureName}", ex);
            }
        }

        /// <summary>
        /// Gets the detailed metadata for a table or view.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>SqlServerTableOrViewMetadata&lt;TDbType&gt;.</returns>
        public new SqlServerTableOrViewMetadata<OleDbType> GetTableOrView(SqlServerObjectName tableName)
        {
            return m_Tables.GetOrAdd(tableName, GetTableOrViewInternal);
        }
    }
}
#endif