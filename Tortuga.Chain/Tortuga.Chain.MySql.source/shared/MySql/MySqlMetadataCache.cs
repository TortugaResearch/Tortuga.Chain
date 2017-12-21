using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using Tortuga.Anchor;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql
{
    /// <summary>
    /// Class MySqlMetadataCache.
    /// </summary>
    public class MySqlMetadataCache : DatabaseMetadataCache<MySqlObjectName, MySqlDbType>
    {
        readonly MySqlConnectionStringBuilder m_ConnectionBuilder;
        readonly ConcurrentDictionary<MySqlObjectName, TableOrViewMetadata<MySqlObjectName, MySqlDbType>> m_Tables = new ConcurrentDictionary<MySqlObjectName, TableOrViewMetadata<MySqlObjectName, MySqlDbType>>();
        readonly ConcurrentDictionary<MySqlObjectName, StoredProcedureMetadata<MySqlObjectName, MySqlDbType>> m_StoredProcedures = new ConcurrentDictionary<MySqlObjectName, StoredProcedureMetadata<MySqlObjectName, MySqlDbType>>();
        readonly ConcurrentDictionary<Type, TableOrViewMetadata<MySqlObjectName, MySqlDbType>> m_TypeTableMap = new ConcurrentDictionary<Type, TableOrViewMetadata<MySqlObjectName, MySqlDbType>>();
        readonly ConcurrentDictionary<MySqlObjectName, TableFunctionMetadata<MySqlObjectName, MySqlDbType>> m_TableFunctions = new ConcurrentDictionary<MySqlObjectName, TableFunctionMetadata<MySqlObjectName, MySqlDbType>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlMetadataCache"/> class.
        /// </summary>
        /// <param name="connectionBuilder">The connection builder.</param>
        public MySqlMetadataCache(MySqlConnectionStringBuilder connectionBuilder)
        {
            m_ConnectionBuilder = connectionBuilder;
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
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override TableFunctionMetadata<MySqlObjectName, MySqlDbType> GetTableFunction(MySqlObjectName tableFunctionName)
        {
            return m_TableFunctions.GetOrAdd(tableFunctionName, GetTableFunctionInternal);
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
            PreloadTableFunctions();
            PreloadStoredProcedures();
        }

        /// <summary>
        /// Preloads the metadata for all tables.
        /// </summary>
        /// <remarks>This will also load all views.</remarks>
        public void PreloadTables()
        {
            const string tableList = "SHOW FULL TABLES";

            using (var con = new MySqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new MySqlCommand(tableList, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(0);
                            var tableType = reader.GetString(1);
                            GetTableOrView(new MySqlObjectName(name));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Preloads the metadata for all views.
        /// </summary>
        /// <remarks>In MySQL this has the same effect as calling PreloadTables.</remarks>
        public void PreloadViews()
        {
            PreloadTables();
        }

        private TableOrViewMetadata<MySqlObjectName, MySqlDbType> GetTableOrViewInternal(MySqlObjectName tableName)
        {
            const string TableSql =
                @"SHOW FULL TABLES LIKE @TableName";


            string actualName;
            string tableType;

            using (var con = new MySqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new MySqlCommand(TableSql, con))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName.Name);
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find table or view {tableName}");
                        actualName = reader.GetString(0);
                        tableType = reader.GetString(1);
                    }
                }
            }

            var isTable = tableType == "BASE TABLE";
            var columns = GetColumns(actualName);

            return new MySqlTableOrViewMetadata<MySqlDbType>(new MySqlObjectName(actualName), isTable, columns);
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        /// <remarks>WARNING: Only call this with verified table names. Otherwise a SQL injection attack can occur.</remarks>
        List<ColumnMetadata<MySqlDbType>> GetColumns(string tableName)
        {
            var columnSql = "SHOW FULL COLUMNS FROM " + tableName;

            var columns = new List<ColumnMetadata<MySqlDbType>>();
            using (var con = new MySqlConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new MySqlCommand(columnSql, con))
                {
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(reader.GetOrdinal("Field"));
                            var rawType = reader.GetString(reader.GetOrdinal("Type"));
                            var collation = reader.IsDBNull(reader.GetOrdinal("Collation")) ? null : reader.GetString(reader.GetOrdinal("Collation"));
                            var isNullable = reader.GetString("Null") == "YES";
                            var key = reader.GetString("Key");
                            var @default = reader.IsDBNull(reader.GetOrdinal("Default")) ? null : reader.GetString("Default");
                            var extra = reader.GetString("Extra");
                            var comment = reader.GetString("Comment");


                            var computed = extra.Contains("VIRTUAL");
                            var primary = key.Contains("PRI");
                            var isIdentity = extra.Contains("auto_increment");
                            MySqlDbType dbType;

                            string typeName;
                            int? maxLength;
                            int? precision;
                            int? scale;

                            AdjustTypeDetails(rawType, out typeName, out maxLength, out precision, out scale, out dbType);

                            columns.Add(new ColumnMetadata<MySqlDbType>(name, computed, primary, isIdentity, typeName, dbType, "`" + name + "`", isNullable, maxLength, precision, scale, rawType));
                        }
                    }
                }
            }
            return columns;
        }

        void AdjustTypeDetails(string rawType, out string typeName, out int? maxLength, out int? precision, out int? scale, out MySqlDbType dbType)
        {
            typeName = rawType;
            if (typeName.Contains("("))
                typeName = typeName.Substring(typeName.IndexOf("("));
            if (typeName.Contains(" "))
                typeName = typeName.Substring(typeName.IndexOf(" "));

            maxLength = null;
            precision = null;
            scale = null;
            dbType = MySqlDbType.Binary;

            throw new NotImplementedException("Finish me!");
        }

        private TableFunctionMetadata<MySqlObjectName, MySqlDbType> GetTableFunctionInternal(MySqlObjectName tableFunctionName)
        {
            throw new NotImplementedException();


        }



        private StoredProcedureMetadata<MySqlObjectName, MySqlDbType> GetStoredProcedureInteral(MySqlObjectName storedProcedureName)
        {
            throw new NotImplementedException();
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
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public override TableOrViewMetadata<MySqlObjectName, MySqlDbType> GetTableOrViewFromClass<TObject>()
        {

            throw new NotImplementedException();

        }

        /// <summary>
        /// Types the type of the name to NPG SQL database.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        internal static MySqlDbType? TypeNameToNpgSqlDbType(string typeName)
        {
            throw new NotImplementedException();
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
        }

        /// <summary>
        /// Preloads the table value functions.
        /// </summary>
        public void PreloadStoredProcedures()
        {
            throw new NotImplementedException();

        }

        /// <summary>
        /// Preloads the table value functions.
        /// </summary>
        public void PreloadTableFunctions()
        {
            throw new NotImplementedException();

        }
    }
}
