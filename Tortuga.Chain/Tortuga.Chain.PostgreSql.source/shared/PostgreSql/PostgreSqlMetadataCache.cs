using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tortuga.Anchor;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql
{
    /// <summary>
    /// Class PostgreSqlMetadataCache.
    /// </summary>
    public class PostgreSqlMetadataCache : DatabaseMetadataCache<PostgreSqlObjectName, NpgsqlDbType>
    {
        readonly NpgsqlConnectionStringBuilder m_ConnectionBuilder;
        readonly ConcurrentDictionary<PostgreSqlObjectName, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_Tables = new ConcurrentDictionary<PostgreSqlObjectName, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>>();
        readonly ConcurrentDictionary<PostgreSqlObjectName, StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_StoredProcedures = new ConcurrentDictionary<PostgreSqlObjectName, StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType>>();
        readonly ConcurrentDictionary<Type, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_TypeTableMap = new ConcurrentDictionary<Type, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>>();
        readonly ConcurrentDictionary<PostgreSqlObjectName, TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_TableFunctions = new ConcurrentDictionary<PostgreSqlObjectName, TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>>();
        readonly ConcurrentDictionary<PostgreSqlObjectName, ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_ScalarFunctions = new ConcurrentDictionary<PostgreSqlObjectName, ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlMetadataCache"/> class.
        /// </summary>
        /// <param name="connectionBuilder">The connection builder.</param>
        public PostgreSqlMetadataCache(NpgsqlConnectionStringBuilder connectionBuilder)
        {
            m_ConnectionBuilder = connectionBuilder;
        }

        /// <summary>
        /// Gets the stored procedure's metadata.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType> GetStoredProcedure(PostgreSqlObjectName procedureName)
        {
            return m_StoredProcedures.GetOrAdd(procedureName, GetStoredProcedureInternal);
        }

        /// <summary>
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableFunction(PostgreSqlObjectName tableFunctionName)
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
        public override IReadOnlyCollection<TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>> GetTableFunctions()
        {
            return m_TableFunctions.GetValues();
        }

        /// <summary>
        /// Gets the stored procedures that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all stored procedures were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType>> GetStoredProcedures()
        {
            return m_StoredProcedures.GetValues();
        }

        /// <summary>
        /// Gets the scalar functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all scalar functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>> GetScalarFunctions()
        {
            return m_ScalarFunctions.GetValues();
        }

        /// <summary>
        /// Gets the metadata for a scalar function.
        /// </summary>
        /// <param name="scalarFunctionName">Name of the scalar function.</param>
        /// <returns>Null if the object could not be found.</returns>
        public override ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType> GetScalarFunction(PostgreSqlObjectName scalarFunctionName)
        {
            return m_ScalarFunctions.GetOrAdd(scalarFunctionName, GetScalarFunctionInternal);
        }

        /// <summary>
        /// Gets the metadata for a table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>TableOrViewMetadata&lt;PostgreSqlObjectName, NpgsqlDbType&gt;.</returns>
        public override TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableOrView(PostgreSqlObjectName tableName)
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
        public override IReadOnlyCollection<TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>> GetTablesAndViews()
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
            PreloadScalarFunctions();
        }

        /// <summary>
        /// Preloads the metadata for all tables.
        /// </summary>
        public void PreloadTables()
        {
            const string TableSql =
                @"SELECT
                table_schema as schemaname,
                table_name as tablename,
                table_type as type
                FROM information_schema.tables
                WHERE table_type='BASE TABLE' AND
                      table_schema<>'pg_catalog' AND
                      table_schema<>'information_schema';";

            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(TableSql, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(reader.GetOrdinal("schemaname"));
                            var name = reader.GetString(reader.GetOrdinal("tablename"));
                            GetTableOrView(new PostgreSqlObjectName(schema, name));
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
            const string ViewSql =
                @"SELECT
                table_schema as schemaname,
                table_name as tablename,
                table_type as type
                FROM information_schema.tables
                WHERE table_type='VIEW' AND
                      table_schema<>'pg_catalog' AND
                      table_schema<>'information_schema';";

            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(ViewSql, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(reader.GetOrdinal("schemaname"));
                            var name = reader.GetString(reader.GetOrdinal("tablename"));
                            GetTableOrView(new PostgreSqlObjectName(schema, name));
                        }
                    }
                }
            }
        }

        private TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableOrViewInternal(PostgreSqlObjectName tableName)
        {
            const string TableSql =
                @"SELECT
                table_schema as schemaname,
                table_name as tablename,
                table_type as type
                FROM information_schema.tables
                WHERE table_schema ILIKE @Schema AND
                      table_name ILIKE @Name AND
                      (table_type='BASE TABLE' OR
                       table_type='VIEW');";

            string actualSchema;
            string actualName;
            bool isTable;

            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(TableSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", tableName.Schema);
                    cmd.Parameters.AddWithValue("@Name", tableName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find table or view {tableName}");
                        actualSchema = reader.GetString(reader.GetOrdinal("schemaname"));
                        actualName = reader.GetString(reader.GetOrdinal("tablename"));
                        var type = reader.GetString(reader.GetOrdinal("type"));
                        isTable = type.Equals("BASE TABLE");
                    }
                }
            }

            var columns = GetColumns(tableName);
            return new TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>(new PostgreSqlObjectName(actualSchema, actualName), isTable, columns);
        }

        private TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableFunctionInternal(PostgreSqlObjectName tableFunctionName)
        {
            const string functionSql = @"SELECT routine_schema, routine_name, specific_name FROM information_schema.routines WHERE routine_type = 'FUNCTION' AND data_type='record' AND routine_schema ILIKE @Schema AND routine_name ILIKE @Name;";

            string actualSchema;
            string actualName;
            string specificName;

            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(functionSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", tableFunctionName.Schema);
                    cmd.Parameters.AddWithValue("@Name", tableFunctionName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find function {tableFunctionName}");
                        actualSchema = reader.GetString(reader.GetOrdinal("routine_schema"));
                        actualName = reader.GetString(reader.GetOrdinal("routine_name"));
                        specificName = reader.GetString(reader.GetOrdinal("specific_name"));
                    }
                }
            }

            var objectName = new PostgreSqlObjectName(actualSchema, actualName);

            var pAndC = GetParametersAndColumns(specificName);

            return new TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>(objectName, pAndC.Item1, pAndC.Item2);


        }

        private ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType> GetScalarFunctionInternal(PostgreSqlObjectName tableFunctionName)
        {
            const string functionSql = @"SELECT routine_schema, routine_name, specific_name, data_type FROM information_schema.routines WHERE routine_type = 'FUNCTION' AND data_type<>'record' AND routine_schema ILIKE @Schema AND routine_name ILIKE @Name;";

            string actualSchema;
            string actualName;
            string specificName;
            string typeName;

            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(functionSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", tableFunctionName.Schema);
                    cmd.Parameters.AddWithValue("@Name", tableFunctionName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find scalar function {tableFunctionName}");
                        actualSchema = reader.GetString(reader.GetOrdinal("routine_schema"));
                        actualName = reader.GetString(reader.GetOrdinal("routine_name"));
                        specificName = reader.GetString(reader.GetOrdinal("specific_name"));
                        typeName = reader.GetString(reader.GetOrdinal("data_type"));
                    }
                }
            }

            var objectName = new PostgreSqlObjectName(actualSchema, actualName);

            var pAndC = GetParametersAndColumns(specificName);

            //Task-120: Add support for length, precision, and scale for return type
            return new ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>(objectName, pAndC.Item1, typeName, TypeNameToNpgSqlDbType(typeName), true, null, null, null, null);


        }

        private Tuple<List<ParameterMetadata<NpgsqlDbType>>, List<ColumnMetadata<NpgsqlDbType>>> GetParametersAndColumns(string specificName)
        {
            //private List<ParameterMetadata<NpgsqlDbType>> GetParameters(string schema, string procedureName)
            const string parameterSql = @"SELECT * FROM information_schema.parameters WHERE specific_name = @SpecificName ORDER BY ordinal_position";

            var parameters = new List<ParameterMetadata<NpgsqlDbType>>();
            var columns = new List<ColumnMetadata<NpgsqlDbType>>();
            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(parameterSql, con))
                {
                    cmd.Parameters.AddWithValue("@SpecificName", specificName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.GetString(reader.GetOrdinal("parameter_mode")) == "IN")
                            {
                                var parameterNameOrd = reader.GetOrdinal("parameter_name");
                                var parameterName = !reader.IsDBNull(parameterNameOrd) ? reader.GetString(parameterNameOrd) : "Parameter" + reader.GetInt32(reader.GetOrdinal("ordinal_position"));

                                var typeName = reader.GetString(reader.GetOrdinal("udt_name"));
                                parameters.Add(new ParameterMetadata<NpgsqlDbType>(parameterName, "@" + parameterName, typeName, TypeNameToNpgSqlDbType(typeName)));
                            }
                            else
                            {
                                var name = reader.GetString(reader.GetOrdinal("parameter_name"));
                                var typeName = reader.GetString(reader.GetOrdinal("udt_name"));
                                bool isPrimary = false;
                                bool isIdentity = false;
                                bool isNullable = true;
                                int? maxLength = null;
                                int? precision = null;
                                int? scale = null;
                                string fullTypeName = null;

                                //Task-120: Add support for length, precision, and scale

                                columns.Add(new ColumnMetadata<NpgsqlDbType>(name, false, isPrimary, isIdentity, typeName, TypeNameToNpgSqlDbType(typeName), "\"" + name + "\"", isNullable, maxLength, precision, scale, fullTypeName));

                            }
                        }
                    }
                }
            }
            return Tuple.Create(parameters, columns);
        }

        private StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType> GetStoredProcedureInternal(PostgreSqlObjectName storedProcedureName)
        {
            const string functionSql = @"SELECT routine_schema, routine_name, specific_name FROM information_schema.routines WHERE routine_type = 'FUNCTION' AND data_type='refcursor' AND routine_schema ILIKE @Schema AND routine_name ILIKE @Name;";

            string actualSchema;
            string actualName;
            string specificName;

            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(functionSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", storedProcedureName.Schema);
                    cmd.Parameters.AddWithValue("@Name", storedProcedureName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find function {storedProcedureName}");
                        actualSchema = reader.GetString(reader.GetOrdinal("routine_schema"));
                        actualName = reader.GetString(reader.GetOrdinal("routine_name"));
                        specificName = reader.GetString(reader.GetOrdinal("specific_name"));
                    }
                }
            }

            var objectName = new PostgreSqlObjectName(actualSchema, actualName);

            var pAndC = GetParametersAndColumns(specificName);

            return new StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType>(objectName, pAndC.Item1);
        }


        private List<ColumnMetadata<NpgsqlDbType>> GetColumns(PostgreSqlObjectName tableName)
        {
            const string ColumnSql =
                @"
SELECT att.attname as column_name,
       t.typname as data_type,
       pk.contype as is_primary_key,
       seq.relname as is_identity,
       att.attnotnull as not_null
FROM pg_class as c
JOIN pg_namespace as ns on ns.oid=c.relnamespace
JOIN pg_attribute as att on c.oid=att.attrelid AND
                            att.attnum>0
JOIN pg_type as t on t.oid=att.atttypid
LEFT JOIN (SELECT cnst.conrelid,
                  cnst.conkey,
                  cnst.contype
           FROM pg_constraint as cnst
           WHERE cnst.contype='p') pk ON att.attnum=ANY(pk.conkey) AND
                                         pk.conrelid=c.oid
LEFT JOIN (SELECT c.relname
           FROM pg_class as c
           WHERE c.relkind='S') seq ON seq.relname~att.attname AND
                                       seq.relname~c.relname
WHERE c.relname ILIKE @Name AND
      ns.nspname ILIKE @Schema;";

            var columns = new List<ColumnMetadata<NpgsqlDbType>>();
            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(ColumnSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", tableName.Schema);
                    cmd.Parameters.AddWithValue("@Name", tableName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(reader.GetOrdinal("column_name"));
                            var typeName = reader.GetString(reader.GetOrdinal("data_type"));
                            bool isPrimary = reader.IsDBNull(reader.GetOrdinal("is_primary_key")) ? false : true;
                            bool isIdentity = reader.IsDBNull(reader.GetOrdinal("is_identity")) ? false : true;
                            bool isNullable = !reader.GetBoolean(reader.GetOrdinal("not_null"));

                            //Task-120: Add support for length, precision, and scale
                            int? maxLength = null;
                            int? precision = null;
                            int? scale = null;
                            string fullTypeName = null;

                            columns.Add(new ColumnMetadata<NpgsqlDbType>(name, false, isPrimary, isIdentity, typeName, TypeNameToNpgSqlDbType(typeName), "\"" + name + "\"", isNullable, maxLength, precision, scale, fullTypeName));
                        }
                    }
                }
            }
            return columns;
        }

        /// <summary>
        /// Parse a string and return the database specific representation of the object name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>PostgreSqlObjectName.</returns>
        protected override PostgreSqlObjectName ParseObjectName(string name)
        {
            return new PostgreSqlObjectName(name);
        }

        /// <summary>
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public override TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableOrViewFromClass<TObject>()
        {

            var type = typeof(TObject);
            TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> result;
            if (m_TypeTableMap.TryGetValue(type, out result))
                return result;

            var typeInfo = MetadataCache.GetMetadata(type);
            if (!string.IsNullOrEmpty(typeInfo.MappedTableName))
            {
                if (string.IsNullOrEmpty(typeInfo.MappedSchemaName))
                    result = GetTableOrView(new PostgreSqlObjectName(typeInfo.MappedTableName));
                else
                    result = GetTableOrView(new PostgreSqlObjectName(typeInfo.MappedSchemaName, typeInfo.MappedTableName));
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
                result = GetTableOrView(new PostgreSqlObjectName(schema, name));
                m_TypeTableMap[type] = result;
                return result;
            }
            catch (MissingObjectException) { }


            //that didn't work, so try the default schema
            result = GetTableOrView(new PostgreSqlObjectName(null, name));
            m_TypeTableMap[type] = result;
            return result;

        }

        /// <summary>
        /// Types the type of the name to NPG SQL database.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal static NpgsqlDbType? TypeNameToNpgSqlDbType(string typeName)
        {
            switch (typeName)
            {
                case "bool": return NpgsqlDbType.Boolean;
                case "int2": return NpgsqlDbType.Smallint;
                case "int4": return NpgsqlDbType.Integer;
                case "int8": return NpgsqlDbType.Bigint;
                case "float4": return NpgsqlDbType.Real;
                case "float8": return NpgsqlDbType.Double;
                case "numeric": return NpgsqlDbType.Numeric;
                case "money": return NpgsqlDbType.Money;
                case "text": return NpgsqlDbType.Text;
                case "varchar": return NpgsqlDbType.Varchar;
                case "character":
                case "char": return NpgsqlDbType.Char;
                case "citext": return NpgsqlDbType.Citext;
                case "json": return NpgsqlDbType.Json;

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
            m_ScalarFunctions.Clear();
        }

        /// <summary>
        /// Preloads the table value functions.
        /// </summary>
        public void PreloadStoredProcedures()
        {
            const string procSql = @"SELECT routine_schema, routine_name FROM information_schema.routines where routine_type = 'FUNCTION' AND data_type='refcursor';";


            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(procSql, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(reader.GetOrdinal("routine_schema"));
                            var name = reader.GetString(reader.GetOrdinal("routine_name"));
                            GetStoredProcedure(new PostgreSqlObjectName(schema, name));
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Preloads the table value functions.
        /// </summary>
        public void PreloadTableFunctions()
        {
            const string TvfSql = @"SELECT routine_schema, routine_name FROM information_schema.routines where routine_type = 'FUNCTION' AND data_type='record';";


            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(TvfSql, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(reader.GetOrdinal("routine_schema"));
                            var name = reader.GetString(reader.GetOrdinal("routine_name"));
                            GetTableFunction(new PostgreSqlObjectName(schema, name));
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Preloads the scalar functions.
        /// </summary>
        public void PreloadScalarFunctions()
        {
            const string TvfSql = @"SELECT routine_schema, routine_name FROM information_schema.routines where routine_type = 'FUNCTION' AND data_type<>'record';";


            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(TvfSql, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(reader.GetOrdinal("routine_schema"));
                            var name = reader.GetString(reader.GetOrdinal("routine_name"));
                            GetScalarFunction(new PostgreSqlObjectName(schema, name));
                        }
                    }
                }
            }

        }
    }
}
