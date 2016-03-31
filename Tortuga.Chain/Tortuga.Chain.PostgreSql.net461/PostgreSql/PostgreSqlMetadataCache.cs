using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql
{
    public class PostgreSqlMetadataCache : DatabaseMetadataCache<PostgreSqlObjectName, NpgsqlDbType>
    {
        private readonly NpgsqlConnectionStringBuilder m_ConnectionBuilder;
        private readonly ConcurrentDictionary<PostgreSqlObjectName, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_Tables = new ConcurrentDictionary<PostgreSqlObjectName, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>>();

        public PostgreSqlMetadataCache(NpgsqlConnectionStringBuilder connectionBuilder)
        {
            m_ConnectionBuilder = connectionBuilder;
        }

        public override StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType> GetStoredProcedure(PostgreSqlObjectName procedureName)
        {
            throw new NotImplementedException();
        }

        public override TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableFunction(PostgreSqlObjectName tableFunctionName)
        {
            throw new NotImplementedException();
        }

        public override TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableOrView(PostgreSqlObjectName tableName)
        {
            return m_Tables.GetOrAdd(tableName, GetTableOrViewInternal);
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

        private List<ColumnMetadata<NpgsqlDbType>> GetColumns(PostgreSqlObjectName tableName)
        {
            const string ColumnSql =
                @"SELECT 
                column_name,
                data_type
                FROM information_schema.columns
                WHERE table_schema=@Schema AND
                      table_name=@Name";

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
                        while(reader.Read())
                        {
                            var name = reader.GetString(reader.GetOrdinal("column_name"));
                            var type = reader.GetString(reader.GetOrdinal("data_type"));

                        }
                    }
                }
            }
            return columns;
        }

        protected override PostgreSqlObjectName ParseObjectName(string name)
        {
            return new PostgreSqlObjectName(name);
        }
    }
}
