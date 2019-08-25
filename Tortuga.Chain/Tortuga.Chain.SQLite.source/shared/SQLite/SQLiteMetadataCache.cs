using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tortuga.Anchor;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SQLite
{
    /// <summary>
    /// Handles caching of metadata for various SQLite tables and views.
    /// </summary>
    public sealed class SQLiteMetadataCache : DbDatabaseMetadataCache<SQLiteObjectName>
    {
        readonly SQLiteConnectionStringBuilder m_ConnectionBuilder;
        readonly ConcurrentDictionary<SQLiteObjectName, TableOrViewMetadata<SQLiteObjectName, DbType>> m_Tables = new ConcurrentDictionary<SQLiteObjectName, TableOrViewMetadata<SQLiteObjectName, DbType>>();

        readonly ConcurrentDictionary<Type, TableOrViewMetadata<SQLiteObjectName, DbType>> m_TypeTableMap = new ConcurrentDictionary<Type, TableOrViewMetadata<SQLiteObjectName, DbType>>();

        /// <summary>
        /// Creates a new instance of <see cref="SQLiteMetadataCache"/>
        /// </summary>
        /// <param name="connectionBuilder">The connection builder.</param>
        public SQLiteMetadataCache(SQLiteConnectionStringBuilder connectionBuilder)
        {
            m_ConnectionBuilder = connectionBuilder;
        }

        /// <summary>
        /// Gets the indexes for a table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Indexes are not supported by this data source</exception>
        /// <remarks>
        /// This should be cached on a TableOrViewMetadata object.
        /// </remarks>
        public override IndexMetadataCollection<SQLiteObjectName, DbType> GetIndexesForTable(SQLiteObjectName tableName)
        {
            var table = GetTableOrView(tableName);

            var indexSql = $"PRAGMA index_list('{tableName.Name}')";
            var results = new List<IndexMetadata<SQLiteObjectName, DbType>>();
            using (var con = new SQLiteConnection(m_ConnectionBuilder.ConnectionString))
            using (var con2 = new SQLiteConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                con2.Open();
                using (var cmd = new SQLiteCommand(indexSql, con))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString("name");
                        var isUnique = reader.GetInt64("unique") != 0;
                        var origin = reader.GetString("origin");
                        var isPrimaryKey = string.Equals(origin, "pk", StringComparison.Ordinal);
                        var isUniqueConstraint = string.Equals(origin, "u", StringComparison.Ordinal);

                        var columns = new List<IndexColumnMetadata<DbType>>();

                        using (var cmd2 = new SQLiteCommand($"PRAGMA index_xinfo('{name}')", con2))
                        using (var reader2 = cmd2.ExecuteReader())
                        {
                            while (reader2.Read())
                            {
                                var colName = reader2.GetStringOrNull("name");
                                var isIncluded = reader2.GetInt64("key") == 0;
                                var isDescending = isIncluded ? (bool?)null : reader2.GetInt64("desc") != 0;

                                ColumnMetadata<DbType> column;
                                if (colName != null)
                                    column = table.Columns.SingleOrDefault(c => string.Equals(c.SqlName, colName, StringComparison.Ordinal));
                                else //a null column name is really the ROWID
                                {
                                    column = table.Columns.SingleOrDefault(c => string.Equals(c.SqlName, "ROWID", StringComparison.Ordinal));

                                    //The ROWID may be aliased as the primary key
                                    column = table.PrimaryKeyColumns.Single();
                                }

                                columns.Add(new IndexColumnMetadata<DbType>(column, isDescending, isIncluded));
                            }
                        }

                        results.Add(new IndexMetadata<SQLiteObjectName, DbType>(tableName, name, isPrimaryKey, isUnique, isUniqueConstraint, new IndexColumnMetadataCollection<DbType>(columns), null, null));
                    }
                }
            }

            var pkColumns = table.PrimaryKeyColumns;

            if (pkColumns.Count == 1 && !results.Any(i => i.IsPrimaryKey)) //need to infer a PK
            {
                results.Add(new IndexMetadata<SQLiteObjectName, DbType>(tableName, "(primary key)", true, false, false,
                    new IndexColumnMetadataCollection<DbType>(new[] { new IndexColumnMetadata<DbType>(pkColumns.Single(), false, false) }), null, null));
            }

            return new IndexMetadataCollection<SQLiteObjectName, DbType>(results);
        }

        /// <summary>
        /// Gets the metadata for a table or view.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override TableOrViewMetadata<SQLiteObjectName, DbType> GetTableOrView(SQLiteObjectName tableName)
        {
            return m_Tables.GetOrAdd(tableName, GetTableOrViewInternal);
        }

        /// <summary>
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <returns>TableOrViewMetadata&lt;System.String, DbType&gt;.</returns>
        public override TableOrViewMetadata<SQLiteObjectName, DbType> GetTableOrViewFromClass<TObject>()
        {
            var type = typeof(TObject);
            TableOrViewMetadata<SQLiteObjectName, DbType> result;
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
        /// Gets the tables and views that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all tables and views were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<TableOrViewMetadata<SQLiteObjectName, DbType>> GetTablesAndViews()
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
                            var tableName = reader.GetString("TableName");
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
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var viewName = reader.GetString("ViewName");
                        GetTableOrView(viewName);
                    }
                }
            }
        }

        /// <summary>
        /// Resets the metadata cache, clearing out all cached metadata.
        /// </summary>
        public override void Reset()
        {
            m_Tables.Clear();
            m_TypeTableMap.Clear();
        }

        /// <summary>
        /// Parses the name of the database object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        protected override SQLiteObjectName ParseObjectName(string name)
        {
            return new SQLiteObjectName(name);
        }

        /// <summary>
        /// Determines the database column type from the column type name.
        /// </summary>
        /// <param name="typeName">Name of the database column type.</param>
        /// <param name="isUnsigned">NOT USED</param>
        /// <returns></returns>
        /// <remarks>This does not honor registered types. This is only used for the database's hard-coded list of native types.</remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected override DbType? SqlTypeNameToDbType(string typeName, bool? isUnsigned = null)
        {
            var cleanTypeName = typeName.ToUpperInvariant();
            if (cleanTypeName.IndexOf("(", StringComparison.OrdinalIgnoreCase) >= 0)
                cleanTypeName = cleanTypeName.Substring(0, cleanTypeName.IndexOf("(", StringComparison.OrdinalIgnoreCase));

            switch (cleanTypeName)
            {
                case "BIGINT": return DbType.Int64;
                case "BIT": return DbType.Boolean;
                case "BLOB": return DbType.Binary;
                case "BOOLEAN": return DbType.Boolean;
                case "CHAR": return DbType.AnsiString;
                case "CHARACTER": return DbType.AnsiString;
                case "CLOB": return DbType.String;
                case "DATE": return DbType.Date;
                case "DATETIME": return DbType.DateTime;
                case "DATETIME2": return DbType.DateTime2;
                case "DATETIMEOFFSET": return DbType.DateTimeOffset;
                case "DECIMAL": return DbType.Decimal;
                case "DOUBLE PRECISION": return DbType.Double;
                case "DOUBLE": return DbType.Double;
                case "FLOAT": return DbType.Single;
                case "INT": return DbType.Int64;
                case "INT2": return DbType.Int16;
                case "INT8": return DbType.Int64;
                case "INTEGER": return DbType.Int64;
                case "MEDIUMINT": return DbType.Int32;
                case "NATIVE CHARACTER": return DbType.String;
                case "NCHAR": return DbType.String;
                case "NUMERIC": return DbType.Decimal;
                case "NVARCHAR": return DbType.String;
                case "REAL": return DbType.Single;
                case "SMALLINT": return DbType.Int16;
                case "TEXT": return DbType.AnsiString;
                case "TINYINT": return DbType.SByte;
                case "UNSIGNED BIG INT": return DbType.UInt64;
                case "VARCHAR": return DbType.AnsiString;
                case "VARYING CHARACTER": return DbType.AnsiString;
                default: return null;
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        List<ColumnMetadata<DbType>> GetColumns(SQLiteObjectName tableName, bool isTable)
        {
            /*  NOTE: Should be safe since GetTableOrViewInternal returns null after querying the table name with a
            **  prepared statement, thus proving that the table name exists.
            */
            var hasPrimarykey = false;
            var columnSql = $"PRAGMA table_info('{tableName.Name}')";

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
                            var name = reader.GetString("name");
                            var typeName = reader.GetString("type");
                            var isPrimaryKey = reader.GetInt32("pk") != 0 ? true : false;
                            var isnNullable = !reader.GetBoolean("notnull");
                            hasPrimarykey = hasPrimarykey || isPrimaryKey;

                            columns.Add(new ColumnMetadata<DbType>(name, false, isPrimaryKey, false, typeName, SqlTypeNameToDbType(typeName), "[" + name + "]", isnNullable, null, null, null, null, ToClrType(typeName, isnNullable, null)));
                        }
                    }
                }
            }

            //Tables wihtout a primary key always have a ROWID.
            //We can't tell if other tables have one or not.
            if (isTable && !hasPrimarykey)
                columns.Add(new ColumnMetadata<DbType>("ROWID", true, false, true, "INTEGER", DbType.Int64, "[ROWID]", false, null, null, null, null, typeof(long)));

            return columns;
        }

        private TableOrViewMetadata<SQLiteObjectName, DbType> GetTableOrViewInternal(SQLiteObjectName tableName)
        {
            const string tableSql =
                @"SELECT
                type AS ObjectType,
                tbl_name AS ObjectName
                FROM sqlite_master
                WHERE UPPER(tbl_name) = UPPER(@Name) AND
                      (type='table' OR type='view')";

            string actualName;
            bool isTable;

            using (var con = new SQLiteConnection(m_ConnectionBuilder.ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(tableSql, con))
                {
                    cmd.Parameters.AddWithValue("@Name", tableName.Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new MissingObjectException($"Could not find table or view {tableName}");

                        actualName = reader.GetString("ObjectName");
                        var objectType = reader.GetString("ObjectType");
                        isTable = objectType.Equals("table", StringComparison.Ordinal);
                    }
                }
            }

            var columns = GetColumns(tableName, isTable);
            return new TableOrViewMetadata<SQLiteObjectName, DbType>(this, actualName, isTable, columns);
        }
    }
}
