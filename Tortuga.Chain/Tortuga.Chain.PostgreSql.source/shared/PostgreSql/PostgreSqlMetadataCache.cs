using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
        readonly ConcurrentDictionary<PostgreSqlObjectName, ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_ScalarFunctions = new ConcurrentDictionary<PostgreSqlObjectName, ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>>();
        readonly ConcurrentDictionary<PostgreSqlObjectName, StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_StoredProcedures = new ConcurrentDictionary<PostgreSqlObjectName, StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType>>();
        readonly ConcurrentDictionary<PostgreSqlObjectName, TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_TableFunctions = new ConcurrentDictionary<PostgreSqlObjectName, TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>>();
        readonly ConcurrentDictionary<PostgreSqlObjectName, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_Tables = new ConcurrentDictionary<PostgreSqlObjectName, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>>();
        readonly ConcurrentDictionary<Type, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_TypeTableMap = new ConcurrentDictionary<Type, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>>();
        string m_DatabaseName;
        ImmutableArray<string> m_DefaultSchemaList;
        ImmutableDictionary<PostgreSqlObjectName, ImmutableHashSet<string>> m_SequenceColumns;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlMetadataCache"/> class.
        /// </summary>
        /// <param name="connectionBuilder">The connection builder.</param>
        public PostgreSqlMetadataCache(NpgsqlConnectionStringBuilder connectionBuilder)
        {
            m_ConnectionBuilder = connectionBuilder;
        }

        /// <summary>
        /// Returns the current database.
        /// </summary>
        /// <returns></returns>
        public string DatabaseName
        {
            get
            {
                if (m_DatabaseName == null)
                {
                    using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
                    {
                        con.Open();
                        using (var cmd = new NpgsqlCommand("select current_database()", con))
                        {
                            m_DatabaseName = (string)cmd.ExecuteScalar();
                        }
                    }
                }
                return m_DatabaseName;
            }
        }

        /// <summary>
        /// Returns the user's default schema.
        /// </summary>
        /// <returns></returns>
        public ImmutableArray<string> DefaultSchemaList
        {
            get
            {
                if (m_DefaultSchemaList == default)
                {
                    using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
                    {
                        con.Open();

                        string currentUser;
                        string defaultSchema;

                        using (var cmd = new NpgsqlCommand("select current_user;", con))
                        {
                            currentUser = (string)cmd.ExecuteScalar();
                        }

                        using (var cmd = new NpgsqlCommand("SHOW search_path;", con))
                        {
                            defaultSchema = (string)cmd.ExecuteScalar();
                        }
                        defaultSchema = defaultSchema.Replace("\"$user\"", currentUser);
                        m_DefaultSchemaList = defaultSchema.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToImmutableArray();
                    }
                }
                return m_DefaultSchemaList;
            }
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
        /// Gets the scalar functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all scalar functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>> GetScalarFunctions() => m_ScalarFunctions.GetValues();

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
        /// Gets the stored procedures that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all stored procedures were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType>> GetStoredProcedures() => m_StoredProcedures.GetValues();

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
        public override IReadOnlyCollection<TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>> GetTableFunctions() => m_TableFunctions.GetValues();

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

        /// <summary>
        /// Resets the metadata cache, clearing out all cached metadata.
        /// </summary>
        public override void Reset()
        {
            m_DatabaseName = null;
            m_DefaultSchemaList = default;
            m_SequenceColumns = null;
            m_StoredProcedures.Clear();
            m_TableFunctions.Clear();
            m_Tables.Clear();
            m_TypeTableMap.Clear();
            m_ScalarFunctions.Clear();
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
        /// Parse a string and return the database specific representation of the object name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>PostgreSqlObjectName.</returns>
        protected override PostgreSqlObjectName ParseObjectName(string name) => new PostgreSqlObjectName(name);

        static Tuple<List<ParameterMetadata<NpgsqlDbType>>, List<ColumnMetadata<NpgsqlDbType>>> GetParametersAndColumns(string specificName, NpgsqlConnection connection)
        {
            const string parameterSql = @"SELECT * FROM information_schema.parameters WHERE specific_name = @SpecificName ORDER BY ordinal_position";

            var parameters = new List<ParameterMetadata<NpgsqlDbType>>();
            var columns = new List<ColumnMetadata<NpgsqlDbType>>();
            using (var cmd = new NpgsqlCommand(parameterSql, connection))
            {
                cmd.Parameters.AddWithValue("@SpecificName", specificName);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (string.Equals(reader.GetString(reader.GetOrdinal("parameter_mode")), "IN", StringComparison.Ordinal))
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
            return Tuple.Create(parameters, columns);
        }

        List<ColumnMetadata<NpgsqlDbType>> GetColumns(PostgreSqlObjectName tableName, NpgsqlConnection connection)
        {
            const string ColumnSql =
                @"
SELECT att.attname as column_name,
       t.typname as data_type,
       pk.contype as is_primary_key,
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
WHERE c.relname ILIKE @Name AND
      ns.nspname ILIKE @Schema;";

            var columns = new List<ColumnMetadata<NpgsqlDbType>>();
            var identityColumns = GetSequenceColumns(tableName);

            using (var cmd = new NpgsqlCommand(ColumnSql, connection))
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
                        bool isIdentity = identityColumns.Contains(name);
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

            return columns;
        }

        ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType> GetScalarFunctionInternal(PostgreSqlObjectName tableFunctionName)
        {
            const string functionSql = @"SELECT routine_schema, routine_name, specific_name, data_type FROM information_schema.routines WHERE routine_type = 'FUNCTION' AND data_type<>'record' AND routine_schema ILIKE @Schema AND routine_name ILIKE @Name;";

            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();

                foreach (var schema in GetSchemasToCheck(tableFunctionName))
                {
                    string actualSchema;
                    string actualName;
                    string specificName;
                    string typeName;

                    using (var cmd = new NpgsqlCommand(functionSql, con))
                    {
                        cmd.Parameters.AddWithValue("@Schema", schema);
                        cmd.Parameters.AddWithValue("@Name", tableFunctionName.Name);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                continue;

                            actualSchema = reader.GetString(reader.GetOrdinal("routine_schema"));
                            actualName = reader.GetString(reader.GetOrdinal("routine_name"));
                            specificName = reader.GetString(reader.GetOrdinal("specific_name"));
                            typeName = reader.GetString(reader.GetOrdinal("data_type"));
                        }
                    }

                    var pAndC = GetParametersAndColumns(specificName, con);

                    //Task-120: Add support for length, precision, and scale for return type
                    return new ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>(new PostgreSqlObjectName(actualSchema, actualName), pAndC.Item1, typeName, TypeNameToNpgSqlDbType(typeName), true, null, null, null, null);
                }
            }

            throw new MissingObjectException($"Could not find scalar function {tableFunctionName}");
        }

        IEnumerable<string> GetSchemasToCheck(PostgreSqlObjectName objectName) => objectName.Schema == null ? DefaultSchemaList : ImmutableArray.Create(objectName.Schema);

        ImmutableHashSet<string> GetSequenceColumns(PostgreSqlObjectName tableName)
        {
            const string sql = @"select s.relname as SequenceName, n.nspname as SchemaName, t.relname as TableName, a.attname as ColumnName
from pg_class s
  join pg_depend d on d.objid=s.oid and d.classid='pg_class'::regclass and d.refclassid='pg_class'::regclass
  join pg_class t on t.oid=d.refobjid
  join pg_namespace n on n.oid=t.relnamespace
  join pg_attribute a on a.attrelid=t.oid and a.attnum=d.refobjsubid
where s.relkind='S' and d.deptype='a'";

            if (m_SequenceColumns == null)
            {
                using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
                {
                    con.Open();

                    using (var cmd = new NpgsqlCommand(sql, con))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            var result = new Dictionary<PostgreSqlObjectName, HashSet<string>>();
                            while (reader.Read())
                            {
                                var schemaTableName = new PostgreSqlObjectName(reader.GetString(reader.GetOrdinal("SchemaName")), reader.GetString(reader.GetOrdinal("TableName")));
                                var columnName = reader.GetString(reader.GetOrdinal("ColumnName"));

                                if (result.TryGetValue(schemaTableName, out var identityColumns))
                                {
                                    identityColumns.Add(columnName);
                                }
                                else
                                {
                                    identityColumns = new HashSet<string>() { columnName };
                                    result.Add(schemaTableName, identityColumns);
                                }
                            }
                            m_SequenceColumns = result.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableHashSet(StringComparer.OrdinalIgnoreCase));
                        }
                    }
                }
            }
            return m_SequenceColumns.GetValueOrDefault(tableName, ImmutableHashSet<string>.Empty);
        }

        StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType> GetStoredProcedureInternal(PostgreSqlObjectName storedProcedureName)
        {
            const string functionSql = @"SELECT routine_schema, routine_name, specific_name FROM information_schema.routines WHERE routine_type = 'FUNCTION' AND data_type='refcursor' AND routine_schema ILIKE @Schema AND routine_name ILIKE @Name;";

            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();

                foreach (var schema in GetSchemasToCheck(storedProcedureName))
                {
                    string actualSchema;
                    string actualName;
                    string specificName;
                    using (var cmd = new NpgsqlCommand(functionSql, con))
                    {
                        cmd.Parameters.AddWithValue("@Schema", schema);
                        cmd.Parameters.AddWithValue("@Name", storedProcedureName.Name);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                continue;
                            actualSchema = reader.GetString(reader.GetOrdinal("routine_schema"));
                            actualName = reader.GetString(reader.GetOrdinal("routine_name"));
                            specificName = reader.GetString(reader.GetOrdinal("specific_name"));
                        }
                    }

                    var pAndC = GetParametersAndColumns(specificName, con);

                    return new StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType>(new PostgreSqlObjectName(actualSchema, actualName), pAndC.Item1);
                }
            }

            throw new MissingObjectException($"Could not find function {storedProcedureName}");
        }

        TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableFunctionInternal(PostgreSqlObjectName tableFunctionName)
        {
            const string functionSql = @"SELECT routine_schema, routine_name, specific_name FROM information_schema.routines WHERE routine_type = 'FUNCTION' AND data_type='record' AND routine_schema ILIKE @Schema AND routine_name ILIKE @Name;";

            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();

                foreach (var schema in GetSchemasToCheck(tableFunctionName))
                {
                    string actualSchema;
                    string actualName;
                    string specificName;

                    using (var cmd = new NpgsqlCommand(functionSql, con))
                    {
                        cmd.Parameters.AddWithValue("@Schema", schema);
                        cmd.Parameters.AddWithValue("@Name", tableFunctionName.Name);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                continue;

                            actualSchema = reader.GetString(reader.GetOrdinal("routine_schema"));
                            actualName = reader.GetString(reader.GetOrdinal("routine_name"));
                            specificName = reader.GetString(reader.GetOrdinal("specific_name"));
                        }
                    }

                    var pAndC = GetParametersAndColumns(specificName, con);

                    return new TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>(new PostgreSqlObjectName(actualSchema, actualName), pAndC.Item1, pAndC.Item2);
                }
            }

            throw new MissingObjectException($"Could not find function {tableFunctionName}");
        }

        TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableOrViewInternal(PostgreSqlObjectName tableName)
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

            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();

                foreach (var schmea in GetSchemasToCheck(tableName))
                {
                    string actualSchema;
                    string actualTableName;
                    bool isTable;

                    using (var cmd = new NpgsqlCommand(TableSql, con))
                    {
                        cmd.Parameters.AddWithValue("@Schema", schmea);
                        cmd.Parameters.AddWithValue("@Name", tableName.Name);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                continue; //try the next schema in the search path
                            actualSchema = reader.GetString(reader.GetOrdinal("schemaname"));
                            actualTableName = reader.GetString(reader.GetOrdinal("tablename"));
                            var type = reader.GetString(reader.GetOrdinal("type"));
                            isTable = type.Equals("BASE TABLE", StringComparison.Ordinal);
                        }
                    }

                    var actualName = new PostgreSqlObjectName(actualSchema, actualTableName);
                    var columns = GetColumns(actualName, con);
                    return new TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>(this, actualName, isTable, columns);
                }
            }

            throw new MissingObjectException($"Could not find table or view {tableName}");
        }
    }
}
