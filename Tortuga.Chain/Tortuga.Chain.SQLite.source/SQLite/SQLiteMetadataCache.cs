using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.Metadata;
using System;
using Tortuga.Anchor.Metadata;
using Tortuga.Anchor;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteConnection = Microsoft.Data.Sqlite.SqliteConnection;
using SQLiteConnectionStringBuilder = Microsoft.Data.Sqlite.SqliteConnectionStringBuilder;
#endif

namespace Tortuga.Chain.SQLite
{
    /// <summary>
    /// Handles caching of metadata for various SQLite tables and views.
    /// </summary>
    public sealed class SQLiteMetadataCache : DatabaseMetadataCache<string, DbType>
    {
        readonly SQLiteConnectionStringBuilder m_ConnectionBuilder;
        readonly ConcurrentDictionary<string, TableOrViewMetadata<string, DbType>> m_Tables = new ConcurrentDictionary<string, TableOrViewMetadata<string, DbType>>(StringComparer.OrdinalIgnoreCase);

        readonly ConcurrentDictionary<Type, TableOrViewMetadata<string, DbType>> m_TypeTableMap = new ConcurrentDictionary<Type, TableOrViewMetadata<string, DbType>>();


        /// <summary>
        /// Creates a new instance of <see cref="SQLiteMetadataCache"/>
        /// </summary>
        /// <param name="connectionBuilder">The connection builder.</param>
        public SQLiteMetadataCache(SQLiteConnectionStringBuilder connectionBuilder)
        {
            m_ConnectionBuilder = connectionBuilder;
        }

        /// <summary>
        /// Gets the metadata for a table or view.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override TableOrViewMetadata<string, DbType> GetTableOrView(string tableName)
        {
            return m_Tables.GetOrAdd(tableName, GetTableOrViewInternal);
        }

        private TableOrViewMetadata<string, DbType> GetTableOrViewInternal(string tableName)
        {
            const string tableSql =
                @"SELECT 
                type AS ObjectType,
                tbl_name AS ObjectName
                FROM sqlite_master
                WHERE tbl_name = @Name AND
                      (type='table' OR type='view')";

            string actualName;
            bool isTable;

            using (var con = new SQLiteConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(tableSql, con))
                {
                    cmd.Parameters.AddWithValue("@Name", tableName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find table or view {tableName}");

                        actualName = reader.GetString(reader.GetOrdinal("ObjectName"));
                        var objectType = reader.GetString(reader.GetOrdinal("ObjectType"));
                        isTable = objectType.Equals("table");
                    }
                }
            }

            var columns = GetColumns(tableName, isTable);
            return new TableOrViewMetadata<string, DbType>(actualName, isTable, columns);
        }

        /// <summary>
        /// Preloads metadata for all database tables.
        /// </summary>
        /// <remarks>This is normally used only for testing. By default, metadata is loaded as needed.</remarks>
        public void PreloadTables()
        {
            const string tableSql =
                @"SELECT
                tbl_name as TableName
                FROM sqlite_master
                WHERE type = 'table'";

            using (var con = new SQLiteConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(tableSql, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tableName = reader.GetString(reader.GetOrdinal("TableName"));
                            GetTableOrView(tableName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Preloads metadata for all database views.
        /// </summary>
        /// <remarks>This is normally used only for testing. By default, metadata is loaded as needed.</remarks>
        public void PreloadViews()
        {
            const string viewSql =
                @"SELECT
                tbl_name as ViewName
                FROM sqlite_master
                WHERE type = 'view'";

            using (var con = new SQLiteConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(viewSql, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var viewName = reader.GetString(reader.GetOrdinal("ViewName"));
                            GetTableOrView(viewName);
                        }
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private List<ColumnMetadata<DbType>> GetColumns(string tableName, bool isTable)
        {
            /*  NOTE: Should be safe since GetTableOrViewInternal returns null after querying the table name with a 
            **  prepared statement. 
            */
            var hasPrimarykey = false;
            var columnSql = "PRAGMA table_info('" + tableName + "')";

            var columns = new List<ColumnMetadata<DbType>>();
            using (var con = new SQLiteConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(columnSql, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(reader.GetOrdinal("name"));
                            var typeName = reader.GetString(reader.GetOrdinal("type"));
                            var isPrimaryKey = reader.GetInt32(reader.GetOrdinal("pk")) != 0 ? true : false;
                            hasPrimarykey = hasPrimarykey || isPrimaryKey;

                            columns.Add(new ColumnMetadata<DbType>(name, false, isPrimaryKey, false, typeName, null, "[" + name + "]"));
                        }
                    }
                }
            }

            //Tables wihtout a primary key always have a ROWID.
            //We can't tell if other tables have one or not.
            if (isTable && !hasPrimarykey)
                columns.Add(new ColumnMetadata<DbType>("ROWID", true, false, true, "INTEGER", null, "[ROWID]"));

            return columns;
        }

        /// <summary>
        /// Parses the name of the database object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        protected override string ParseObjectName(string name)
        {
            return name;
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
        /// Gets the tables and views that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all tables and views were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<TableOrViewMetadata<string, DbType>> GetTablesAndViews()
        {
            return m_Tables.GetValues();
        }

        /// <summary>
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <returns>TableOrViewMetadata&lt;System.String, DbType&gt;.</returns>
        public override TableOrViewMetadata<string, DbType> GetTableOrViewFromClass<TObject>()
        {
            var type = typeof(TObject);
            TableOrViewMetadata<string, DbType> result;
            if (m_TypeTableMap.TryGetValue(type, out result))
                return result;

            var typeInfo = MetadataCache.GetMetadata(type);
            if (!string.IsNullOrEmpty(typeInfo.MappedTableName))
            {
                result = GetTableOrView(typeInfo.MappedTableName);
                m_TypeTableMap[type] = result;
                return result;
            }

            //infer table from class name
            result = GetTableOrView(type.Name);
            m_TypeTableMap[type] = result;
            return result;
        }

        /// <summary>
        /// Resets the metadata cache, clearing out all cached metadata.
        /// </summary>
        public override void Reset()
        {
            m_Tables.Clear();
            m_TypeTableMap.Clear();
        }
    }
}
