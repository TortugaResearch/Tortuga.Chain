using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer;

namespace Tortuga.Chain.MySql
{
    /// <summary>
    /// Class MySqlMetadataCache.
    /// </summary>
    public class MySqlMetadataCache : DatabaseMetadataCache<MySqlObjectName, MySqlDbType>
    {
        private readonly MySqlConnectionStringBuilder m_ConnectionBuilder;
        private readonly ConcurrentDictionary<MySqlObjectName, ScalarFunctionMetadata<MySqlObjectName, MySqlDbType>> m_ScalarFunctions = new ConcurrentDictionary<MySqlObjectName, ScalarFunctionMetadata<MySqlObjectName, MySqlDbType>>();
        private readonly ConcurrentDictionary<MySqlObjectName, StoredProcedureMetadata<MySqlObjectName, MySqlDbType>> m_StoredProcedures = new ConcurrentDictionary<MySqlObjectName, StoredProcedureMetadata<MySqlObjectName, MySqlDbType>>();
        private readonly ConcurrentDictionary<MySqlObjectName, TableOrViewMetadata<MySqlObjectName, MySqlDbType>> m_Tables = new ConcurrentDictionary<MySqlObjectName, TableOrViewMetadata<MySqlObjectName, MySqlDbType>>();
        private readonly ConcurrentDictionary<Type, TableOrViewMetadata<MySqlObjectName, MySqlDbType>> m_TypeTableMap = new ConcurrentDictionary<Type, TableOrViewMetadata<MySqlObjectName, MySqlDbType>>();
        private string m_DefaultSchema;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlMetadataCache"/> class.
        /// </summary>
        /// <param name="connectionBuilder">The connection builder.</param>
        public MySqlMetadataCache(MySqlConnectionStringBuilder connectionBuilder)
        {
            m_ConnectionBuilder = connectionBuilder;
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
                    using (var con = new MySqlConnection(m_ConnectionBuilder.ConnectionString))
                    {
                        con.Open();
                        using (var cmd = new MySqlCommand("SELECT Database()", con))
                        {
                            m_DefaultSchema = (string)cmd.ExecuteScalar();
                        }
                    }
                }
                return m_DefaultSchema;
            }
        }

        /// <summary>
        /// Gets the metadata for a scalar function.
        /// </summary>
        /// <param name="scalarFunctionName">Name of the scalar function.</param>
        /// <returns>Null if the object could not be found.</returns>
        public override ScalarFunctionMetadata<MySqlObjectName, MySqlDbType> GetScalarFunction(MySqlObjectName scalarFunctionName)
        {
            return m_ScalarFunctions.GetOrAdd(scalarFunctionName, GetScalarFunctionInternal);
        }

        /// <summary>
        /// Gets the scalar functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all scalar functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<ScalarFunctionMetadata<MySqlObjectName, MySqlDbType>> GetScalarFunctions()
        {
            return m_ScalarFunctions.GetValues();
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
        /// Gets the stored procedures that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all stored procedures were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<StoredProcedureMetadata<MySqlObjectName, MySqlDbType>> GetStoredProcedures()
        {
            return m_StoredProcedures.GetValues();
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
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public override TableOrViewMetadata<MySqlObjectName, MySqlDbType> GetTableOrViewFromClass<TObject>()
        {
            var type = typeof(TObject);
            TableOrViewMetadata<MySqlObjectName, MySqlDbType> result;
            if (m_TypeTableMap.TryGetValue(type, out result))
                return result;

            var typeInfo = MetadataCache.GetMetadata(type);
            if (!string.IsNullOrEmpty(typeInfo.MappedTableName))
            {
                if (string.IsNullOrEmpty(typeInfo.MappedSchemaName))
                    result = GetTableOrView(new MySqlObjectName(typeInfo.MappedTableName));
                else
                    result = GetTableOrView(new MySqlObjectName(typeInfo.MappedSchemaName, typeInfo.MappedTableName));
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
                result = GetTableOrView(new MySqlObjectName(schema, name));
                m_TypeTableMap[type] = result;
                return result;
            }
            catch (MissingObjectException) { }

            //that didn't work, so try the default schema
            result = GetTableOrView(new MySqlObjectName(null, name));
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
            PreloadScalarFunctions();
            PreloadStoredProcedures();
        }

        /// <summary>
        /// Preloads the scalar functions.
        /// </summary>
        public void PreloadScalarFunctions()
        {
            const string sql = "SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'FUNCTION'";

            using (var con = new MySqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new MySqlCommand(sql, con))
                {
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(0);
                            var name = reader.GetString(1);
                            GetScalarFunction(new MySqlObjectName(schema, name));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Preloads the table value functions.
        /// </summary>
        public void PreloadStoredProcedures()
        {
            const string sql = "SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE'";

            using (var con = new MySqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new MySqlCommand(sql, con))
                {
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(0);
                            var name = reader.GetString(1);
                            GetStoredProcedure(new MySqlObjectName(schema, name));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Preloads the metadata for all tables.
        /// </summary>
        public void PreloadTables()
        {
            const string tableList = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.Tables WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA <> 'mysql' AND TABLE_SCHEMA <> 'mysql' AND TABLE_SCHEMA <> 'performance_schema' AND TABLE_SCHEMA <> 'sys'";

            using (var con = new MySqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new MySqlCommand(tableList, con))
                {
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(0);
                            var name = reader.GetString(1);
                            GetTableOrView(new MySqlObjectName(schema, name));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Preloads the metadata for all views.
        /// </summary>
        public void PreloadViews()
        {
            const string tableList = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.Tables WHERE TABLE_TYPE = 'VIEW' AND TABLE_SCHEMA <> 'sys';";

            using (var con = new MySqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new MySqlCommand(tableList, con))
                {
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(0);
                            var name = reader.GetString(1);
                            GetTableOrView(new MySqlObjectName(schema, name));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets the metadata cache, clearing out all cached metadata.
        /// </summary>
        public override void Reset()
        {
            m_StoredProcedures.Clear();
            m_ScalarFunctions.Clear();
            m_Tables.Clear();
            m_TypeTableMap.Clear();
            m_DefaultSchema = null;
        }

        /// <summary>
        /// Converts a value to a string suitable for use in a SQL statement.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dbType">Optional database column type.</param>
        /// <returns></returns>
        public override string ValueToSqlValue(object value, MySqlDbType? dbType)
        {
            switch (value)
            {
                case string s:
                    {
                        var result = new System.Text.StringBuilder((int)(s.Length * 1.1));

                        foreach (var c in s)
                        {
                            switch (c)
                            {
                                case '\'':
                                    result.Append(@"\'");
                                    break;

                                case '\"':
                                    result.Append(@"\""");
                                    break;

                                case '\b':
                                    result.Append(@"\b");
                                    break;

                                case '\n':
                                    result.Append(@"\n");
                                    break;

                                case '\r':
                                    result.Append(@"\r");
                                    break;

                                case '\t':
                                    result.Append(@"\t");
                                    break;

                                case '\\':
                                    result.Append(@"\\");
                                    break;

                                default:
                                    result.Append(c);
                                    break;
                            }
                        }

                        return "'" + result + "'";
                    }

                default:
                    return base.ValueToSqlValue(value, dbType);
            }
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
        /// Returns the CLR type that matches the indicated database column type.
        /// </summary>
        /// <param name="dbType">Type of the database column.</param>
        /// <param name="isNullable">If nullable, Nullable versions of primitive types are returned.</param>
        /// <param name="maxLength">Optional length. Used to distinguish between a char and string. Defaults to string.</param>
        /// <returns>
        /// A CLR type or NULL if the type is unknown.
        /// </returns>
        /// <remarks>This does not take into consideration registered types.</remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected override Type ToClrType(MySqlDbType dbType, bool isNullable, int? maxLength)
        {
            switch (dbType)
            {
                case MySqlDbType.Bit:
                case MySqlDbType.Bool:
                    return isNullable ? typeof(bool?) : typeof(bool);

                case MySqlDbType.Decimal:
                case MySqlDbType.NewDecimal:
                    return isNullable ? typeof(decimal?) : typeof(decimal);

                case MySqlDbType.Byte:
                    return isNullable ? typeof(sbyte?) : typeof(sbyte);

                case MySqlDbType.Int16:
                    return isNullable ? typeof(short?) : typeof(short);

                case MySqlDbType.Int32:
                    return isNullable ? typeof(int?) : typeof(int);

                case MySqlDbType.Float:
                    return isNullable ? typeof(float?) : typeof(float);

                case MySqlDbType.Double:
                    return isNullable ? typeof(double?) : typeof(double);

                case MySqlDbType.Timestamp:
                case MySqlDbType.Date:
                case MySqlDbType.DateTime:
                case MySqlDbType.Newdate:
                    return isNullable ? typeof(DateTime?) : typeof(DateTime);

                case MySqlDbType.Int64:
                    return isNullable ? typeof(long?) : typeof(long);

                case MySqlDbType.Int24:
                    return isNullable ? typeof(int?) : typeof(int);

                case MySqlDbType.Time:
                    return isNullable ? typeof(TimeSpan?) : typeof(TimeSpan);

                case MySqlDbType.Year:
                    return isNullable ? typeof(sbyte?) : typeof(sbyte);

                case MySqlDbType.VarString:
                case MySqlDbType.JSON:
                case MySqlDbType.VarChar:
                case MySqlDbType.TinyText:
                case MySqlDbType.MediumText:
                case MySqlDbType.LongText:
                case MySqlDbType.Text:
                case MySqlDbType.String:
                    return (maxLength == 1) ? (isNullable ? typeof(char?) : typeof(char)) : typeof(string);

                case MySqlDbType.UByte:
                    return isNullable ? typeof(byte?) : typeof(byte);

                case MySqlDbType.UInt16:
                    return isNullable ? typeof(ushort?) : typeof(ushort);

                case MySqlDbType.UInt32:
                    return isNullable ? typeof(uint?) : typeof(uint);

                case MySqlDbType.UInt64:
                    return isNullable ? typeof(ulong?) : typeof(ulong);

                case MySqlDbType.UInt24:
                    return isNullable ? typeof(uint?) : typeof(uint);

                case MySqlDbType.Binary:
                case MySqlDbType.VarBinary:
                    return typeof(byte[]);

                case MySqlDbType.Guid:
                    return isNullable ? typeof(Guid?) : typeof(Guid);

                case MySqlDbType.Enum:
                case MySqlDbType.Set:
                case MySqlDbType.TinyBlob:
                case MySqlDbType.MediumBlob:
                case MySqlDbType.LongBlob:
                case MySqlDbType.Blob:
                case MySqlDbType.Geometry:
                case MySqlDbType.Null:
                    return null;
            }

            return null;
        }

        /// <summary>
        /// Determines the database column type from the column type name.
        /// </summary>
        /// <param name="typeName">Name of the database column type.</param>
        /// <param name="isUnsigned">Indicates whether or not the column is unsigned. Only applicable to some databases.</param>
        /// <returns></returns>
        /// <remarks>This does not honor registered types. This is only used for the database's hard-coded list of native types.</remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected override MySqlDbType? TypeNameToDbType(string typeName, bool? isUnsigned)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;

            switch (typeName.ToUpperInvariant())
            {
                case "INT1":
                case "BOOL":
                case "BOOLEAN":
                case "TINYINT": if (isUnsigned == true) return MySqlDbType.UByte; else return MySqlDbType.Byte;
                case "INT2":
                case "SMALLINT": if (isUnsigned == true) return MySqlDbType.UInt16; else return MySqlDbType.Int16;
                case "INT3":
                case "MIDDLEINT":
                case "MEDIUMINT": if (isUnsigned == true) return MySqlDbType.UInt24; else return MySqlDbType.Int24;
                case "INT4":
                case "INTEGER":
                case "INT": if (isUnsigned == true) return MySqlDbType.UInt32; else return MySqlDbType.Int32;
                case "INT8":
                case "BIGINT": if (isUnsigned == true) return MySqlDbType.UInt64; else return MySqlDbType.Int64;
                case "FIXED":
                case "NUMERIC":
                case "DECIMAL": return MySqlDbType.Decimal;
                case "FLOAT4":
                case "FLOAT": return MySqlDbType.Float;
                case "DOUBLE PRECISION":
                case "REAL":
                case "FLOAT8":
                case "DOUBLE": return MySqlDbType.Double;
                case "BIT": return MySqlDbType.Bit;
                case "DATE": return MySqlDbType.Date;
                case "TIME": return MySqlDbType.Time;
                case "DATETIME": return MySqlDbType.DateTime;
                case "TIMESTAMP": return MySqlDbType.Timestamp;
                case "YEAR": return MySqlDbType.Year;
                case "CHAR": return MySqlDbType.VarChar;
                case "CHARACTER VARYING":
                case "VARCHAR": return MySqlDbType.VarChar;
                case "BINARY": return MySqlDbType.Binary;
                case "VARBINARY": return MySqlDbType.VarBinary;
                case "BLOB": return MySqlDbType.Blob;
                case "TEXT": return MySqlDbType.Text;
                case "ENUM": return MySqlDbType.Enum;
                case "SET": return MySqlDbType.Set;
                case "GEOMETRY": return MySqlDbType.Geometry;
                case "POINT": return MySqlDbType.Geometry;
                case "LINESTRING": return MySqlDbType.Geometry;
                case "POLYGON": return MySqlDbType.Geometry;
                case "MULTIPOINT": return MySqlDbType.Geometry;
                case "MULTILINESTRING": return MySqlDbType.Geometry;
                case "MULTIPOLYGON": return MySqlDbType.Geometry;
                case "GEOMETRYCOLLECTION": return MySqlDbType.Geometry;
                case "JSON": return MySqlDbType.JSON;
                case "TINYBLOB": return MySqlDbType.TinyBlob;
                case "LONG VARBINARY":
                case "MEDIUMBLOB": return MySqlDbType.MediumBlob;
                case "LONGBLOB": return MySqlDbType.LongBlob;
                case "TINYTEXT": return MySqlDbType.TinyText;
                case "LONG VARCHAR":
                case "MEDIUMTEXT": return MySqlDbType.MediumText;
                case "LONGTEXT": return MySqlDbType.LongText;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <remarks>WARNING: Only call this with verified table names. Otherwise a SQL injection attack can occur.</remarks>
        private List<ColumnMetadata<MySqlDbType>> GetColumns(string schema, string tableName)
        {
            const string ColumnSql = @"SELECT COLUMN_NAME, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE, DATETIME_PRECISION, COLUMN_TYPE, COLUMN_KEY, EXTRA, COLUMN_COMMENT, COLLATION_NAME FROM INFORMATION_SCHEMA.Columns WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @Name";

            var columns = new List<ColumnMetadata<MySqlDbType>>();
            using (var con = new MySqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new MySqlCommand(ColumnSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", schema);
                    cmd.Parameters.AddWithValue("@Name", tableName);

                    using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(reader.GetOrdinal("COLUMN_NAME"));
                            //var @default = reader.IsDBNull(reader.GetOrdinal("COLUMN_DEFAULT")) ? null : reader.GetString("COLUMN_DEFAULT"); #226
                            var isNullable = string.Equals(reader.GetString("IS_NULLABLE"), "YES", StringComparison.Ordinal);
                            var typeName = reader.GetString("DATA_TYPE");
                            var maxLength = reader.IsDBNull(reader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH")) ? (UInt64?)null : reader.GetUInt64("CHARACTER_MAXIMUM_LENGTH");
                            var precisionA = reader.IsDBNull(reader.GetOrdinal("NUMERIC_PRECISION")) ? (int?)null : reader.GetInt32("NUMERIC_PRECISION");
                            var scale = reader.IsDBNull(reader.GetOrdinal("NUMERIC_SCALE")) ? (int?)null : reader.GetInt32("NUMERIC_SCALE");
                            var precisionB = reader.IsDBNull(reader.GetOrdinal("DATETIME_PRECISION")) ? (int?)null : reader.GetInt32("DATETIME_PRECISION");
                            var precision = precisionA ?? precisionB;
                            var fullTypeName = reader.GetString("COLUMN_TYPE");
                            var key = reader.GetString("COLUMN_KEY");
                            var extra = reader.GetString("EXTRA");
                            //var comment = reader.GetString("COLUMN_COMMENT"); #224
                            //var collation = reader.IsDBNull(reader.GetOrdinal("COLLATION_NAME")) ? null : reader.GetString(reader.GetOrdinal("COLLATION_NAME")); #225

                            var computed = extra.Contains("VIRTUAL");
                            var primary = key.Contains("PRI");
                            var isIdentity = extra.Contains("auto_increment");
                            var isUnsigned = fullTypeName.Contains("unsigned");

                            var dbType = TypeNameToDbType(typeName, isUnsigned);

                            columns.Add(new ColumnMetadata<MySqlDbType>(name, computed, primary, isIdentity, typeName, dbType, "`" + name + "`", isNullable, (int?)maxLength, precision, scale, fullTypeName, ToClrType(typeName, isNullable, (int?)maxLength, isUnsigned)));
                        }
                    }
                }
            }
            return columns;
        }

        private IList<ParameterMetadata<MySqlDbType>> GetParameters(string schemaName, string specificName)
        {
            try
            {
                const string ParameterSql = "SELECT PARAMETER_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE, DATETIME_PRECISION, DTD_IDENTIFIER FROM INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_SCHEMA = @Schema AND SPECIFIC_NAME = @SpecificName AND PARAMETER_NAME IS NOT NULL";

                //we exclude parameter_id 0 because it is the return type of scalar functions.

                var parameters = new List<ParameterMetadata<MySqlDbType>>();

                using (var con = new MySqlConnection(m_ConnectionBuilder.ConnectionString))
                {
                    con.Open();

                    using (var cmd = new MySqlCommand(ParameterSql, con))
                    {
                        cmd.Parameters.AddWithValue("@Schema", schemaName);
                        cmd.Parameters.AddWithValue("@SpecificName", specificName);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var name = reader.GetString(reader.GetOrdinal("PARAMETER_NAME"));
                                var typeName = reader.GetString(reader.GetOrdinal("DATA_TYPE"));
                                bool isNullable = true;
                                var maxLength = reader.IsDBNull(reader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH")) ? (UInt64?)null : reader.GetUInt64("CHARACTER_MAXIMUM_LENGTH");
                                var precisionA = reader.IsDBNull(reader.GetOrdinal("NUMERIC_PRECISION")) ? (int?)null : reader.GetInt32("NUMERIC_PRECISION");
                                var scale = reader.IsDBNull(reader.GetOrdinal("NUMERIC_SCALE")) ? (int?)null : reader.GetInt32("NUMERIC_SCALE");
                                var precisionB = reader.IsDBNull(reader.GetOrdinal("DATETIME_PRECISION")) ? (int?)null : reader.GetInt32("DATETIME_PRECISION");
                                var precision = precisionA ?? precisionB;
                                var fullTypeName = reader.GetString("DTD_IDENTIFIER");

                                var isUnsigned = fullTypeName.Contains("unsigned");
                                var dbType = TypeNameToDbType(typeName, isUnsigned);

                                parameters.Add(new ParameterMetadata<MySqlDbType>(name, name, typeName, dbType, isNullable, (int?)maxLength, precision, scale, fullTypeName));
                            }
                        }
                    }
                }
                return parameters;
            }
            catch (Exception ex)
            {
                throw new MetadataException($"Error getting parameters for {schemaName}.{specificName}", ex);
            }
        }

        private ScalarFunctionMetadata<MySqlObjectName, MySqlDbType> GetScalarFunctionInternal(MySqlObjectName tableFunctionName)
        {
            const string sql = "SELECT ROUTINE_SCHEMA, ROUTINE_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE, DATETIME_PRECISION, DTD_IDENTIFIER, SPECIFIC_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'FUNCTION' AND ROUTINE_SCHEMA = @Schema AND ROUTINE_NAME = @Name;";

            string actualSchema;
            string actualName;

            string fullTypeName;
            string typeName;
            UInt64? maxLength;
            int? precision;
            int? scale;
            MySqlDbType? dbType;
            string specificName;

            using (var con = new MySqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", tableFunctionName.Schema ?? DefaultSchema);
                    cmd.Parameters.AddWithValue("@Name", tableFunctionName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find scalar function {tableFunctionName}");
                        actualSchema = reader.GetString(reader.GetOrdinal("ROUTINE_SCHEMA"));
                        actualName = reader.GetString(reader.GetOrdinal("ROUTINE_NAME"));

                        typeName = reader.GetString("DATA_TYPE");
                        maxLength = reader.IsDBNull(reader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH")) ? (UInt64?)null : reader.GetUInt64("CHARACTER_MAXIMUM_LENGTH");
                        var precisionA = reader.IsDBNull(reader.GetOrdinal("NUMERIC_PRECISION")) ? (int?)null : reader.GetInt32("NUMERIC_PRECISION");
                        scale = reader.IsDBNull(reader.GetOrdinal("NUMERIC_SCALE")) ? (int?)null : reader.GetInt32("NUMERIC_SCALE");
                        var precisionB = reader.IsDBNull(reader.GetOrdinal("DATETIME_PRECISION")) ? (int?)null : reader.GetInt32("DATETIME_PRECISION");
                        precision = precisionA ?? precisionB;
                        fullTypeName = reader.GetString("DTD_IDENTIFIER");
                        specificName = reader.GetString("SPECIFIC_NAME");

                        var isUnsigned = fullTypeName.Contains("unsigned");
                        dbType = TypeNameToDbType(typeName, isUnsigned);
                    }
                }
            }

            var objectName = new MySqlObjectName(actualSchema, actualName);

            var parameters = GetParameters(actualSchema, specificName);

            return new ScalarFunctionMetadata<MySqlObjectName, MySqlDbType>(objectName, parameters, typeName, dbType, true, (int?)maxLength, precision, scale, fullTypeName);
        }

        private StoredProcedureMetadata<MySqlObjectName, MySqlDbType> GetStoredProcedureInteral(MySqlObjectName storedProcedureName)
        {
            const string sql = "SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE' AND ROUTINE_SCHEMA = @Schema AND ROUTINE_NAME = @Name;";

            string actualSchema;
            string actualName;

            using (var con = new MySqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", storedProcedureName.Schema ?? DefaultSchema);
                    cmd.Parameters.AddWithValue("@Name", storedProcedureName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find stored procedure {storedProcedureName}");
                        actualSchema = reader.GetString(reader.GetOrdinal("ROUTINE_SCHEMA"));
                        actualName = reader.GetString(reader.GetOrdinal("ROUTINE_NAME"));
                    }
                }
            }

            var objectName = new MySqlObjectName(actualSchema, actualName);

            var parameters = GetParameters(actualSchema, actualName);

            return new StoredProcedureMetadata<MySqlObjectName, MySqlDbType>(objectName, parameters);
        }

        private TableOrViewMetadata<MySqlObjectName, MySqlDbType> GetTableOrViewInternal(MySqlObjectName tableName)
        {
            const string TableSql = @"SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE, ENGINE FROM INFORMATION_SCHEMA.Tables WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @Name";

            string actualSchemaName;
            string actualTableName;
            string engine;
            bool isTable;

            using (var con = new MySqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new MySqlCommand(TableSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", tableName.Schema ?? DefaultSchema);
                    cmd.Parameters.AddWithValue("@Name", tableName.Name);
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find table or view {tableName}");
                        actualSchemaName = reader.GetString("TABLE_SCHEMA");
                        actualTableName = reader.GetString("TABLE_NAME");
                        isTable = string.Equals(reader.GetString("TABLE_TYPE"), "BASE TABLE", StringComparison.Ordinal);
                        engine = reader.IsDBNull(reader.GetOrdinal("ENGINE")) ? null : reader.GetString("ENGINE");
                    }
                }
            }

            var columns = GetColumns(actualSchemaName, actualTableName);

            return new MySqlTableOrViewMetadata<MySqlDbType>(new MySqlObjectName(actualSchemaName, actualTableName), isTable, columns, engine);
        }
    }
}
