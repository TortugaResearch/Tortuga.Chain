using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql
{
    public class PostgreSqlMetadataCache : DatabaseMetadataCache<PostgreSqlObjectName, NpgsqlDbType>
    {
        readonly NpgsqlConnectionStringBuilder m_ConnectionBuilder;
        readonly ConcurrentDictionary<PostgreSqlObjectName, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_Tables = new ConcurrentDictionary<PostgreSqlObjectName, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>>();

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
        /// <exception cref="System.NotImplementedException"></exception>
        public override StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType> GetStoredProcedure(PostgreSqlObjectName procedureName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
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

        }

        /// <summary>
        /// Preloads the metadata for all views.
        /// </summary>
        public void PreloadViews()
        {

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
                    c.column_name as column_name, 
                    c.data_type as data_type, 
                    tc.constraint_type as is_primary_key
                  FROM information_schema.columns AS c
                  JOIN information_schema.constraint_column_usage AS ccu ON ccu.table_schema=c.table_schema AND 
                                                                            ccu.table_name=c.table_name
                  LEFT JOIN information_schema.table_constraints AS tc ON tc.table_schema=c.table_schema AND
                                                                          tc.table_name=c.table_name AND
                                                                          ccu.column_name=c.column_name AND 
                                                                          tc.constraint_type='PRIMARY KEY'
                  WHERE c.table_schema='chainschema' AND
                        c.table_name='currency';";

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
                            var primary = reader.GetString(reader.GetOrdinal("is_primary_key")).Equals("PRIMARY KEY");
                            columns.Add(new ColumnMetadata<NpgsqlDbType>(name, false, primary, false, typename, null, name));
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
