using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql
{
    public class PostgreSqlMetadataCache : DatabaseMetadataCache<PostgreSqlObjectName, NpgsqlDbType>
    {
        readonly NpgsqlConnectionStringBuilder m_ConnectionBuilder;
        readonly ConcurrentDictionary<PostgreSqlObjectName, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_Tables = new ConcurrentDictionary<PostgreSqlObjectName, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>>();
        readonly ConcurrentDictionary<PostgreSqlObjectName, StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_StoredProcedures = new ConcurrentDictionary<PostgreSqlObjectName, StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType>>();
        readonly ConcurrentDictionary<Type, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_TypeTableMap = new ConcurrentDictionary<Type, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>>();

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
            return m_StoredProcedures.GetOrAdd(procedureName, GetStoredProcedureInteral);
        }

        /// <summary>
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableFunction(PostgreSqlObjectName tableFunctionName)
        {
            throw new NotImplementedException();
        }

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
        public override ICollection<TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>> GetTablesAndViews()
        {
            return m_Tables.Values;
        }

        /// <summary>
        /// Preloads all of the metadata for this data source.
        /// </summary>
        public override void Preload()
        {
            PreloadTables();
            PreloadViews();
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
                WHERE table_schema=@Schema AND
                      table_name=@Name AND
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

        private StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType> GetStoredProcedureInteral(PostgreSqlObjectName storedProcedureName)
        {
            const string StoredProcSql =
                @"
                SELECT 
                sp.proname AS procname,
                ns.nspname AS schema
                FROM pg_proc AS sp
                INNER JOIN pg_namespace AS ns ON ns.oid=sp.pronamespace
                WHERE sp.proname=@ProcName AND
                      ns.nspname=@Schema;";

            string actualSchema;
            string actualName;

            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(StoredProcSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", storedProcedureName.Schema);
                    cmd.Parameters.AddWithValue("@Name", storedProcedureName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find pgsql function {storedProcedureName}");
                        actualSchema = reader.GetString(reader.GetOrdinal("procname"));
                        actualName = reader.GetString(reader.GetOrdinal("schema"));
                    }
                }
            }

            var parameters = GetParameters(actualSchema, actualName);
            return new StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType>(new PostgreSqlObjectName(actualSchema, actualName), parameters);
        }


        private List<ColumnMetadata<NpgsqlDbType>> GetColumns(PostgreSqlObjectName tableName)
        {
            const string ColumnSql =
                @"
SELECT att.attname as column_name,
       t.typname as data_type,
       pk.contype as is_primary_key,
       seq.relname as is_identity
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
WHERE c.relname=@Name AND
      ns.nspname=@Schema;";

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
                            var typename = reader.GetString(reader.GetOrdinal("data_type"));
                            bool isPrimary = reader.IsDBNull(reader.GetOrdinal("is_primary_key")) ? false : true;
                            bool isIdentity = reader.IsDBNull(reader.GetOrdinal("is_identity")) ? false : true;
                            columns.Add(new ColumnMetadata<NpgsqlDbType>(name, false, isPrimary, isIdentity, typename, null, "\"" + name + "\""));
                        }
                    }
                }
            }
            return columns;
        }

        private List<ParameterMetadata<NpgsqlDbType>> GetParameters(string schema, string procedureName)
        {
            const string parameterSql =
                @"
                SELECT
                data_type,
                parameter_name
                FROM information_schema.parameters
                WHERE specific_schema=@Schema AND
                      specific_name~@Name;";

            var parameters = new List<ParameterMetadata<NpgsqlDbType>>();
            using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(parameterSql, con))
                {
                    cmd.Parameters.AddWithValue("@Schema", schema);
                    cmd.Parameters.AddWithValue("@Name", procedureName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var parameterName = reader.GetString(reader.GetOrdinal("parameter_name"));
                            var dataType = reader.GetString(reader.GetOrdinal("data_type"));
                            parameters.Add(new ParameterMetadata<NpgsqlDbType>(parameterName, dataType, null));
                        }
                    }
                }
            }
            return parameters;
        }

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
                schema = schema.Substring(schema.LastIndexOf(".") + 1);
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
        internal static NpgsqlDbType? TypeNameToNpgSqlDbType(string typeName)
        {
            switch(typeName)
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
    }
}
