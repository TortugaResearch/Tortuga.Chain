using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SQLite
{
    /// <summary>
    /// Handles caching of metadata for various SQLite tables and views.
    /// </summary>
    public class SQLiteMetadataCache : DatabaseMetadataCache<string, DbType>
    {
        private readonly SQLiteConnectionStringBuilder m_ConnectionBuilder;
        private readonly ConcurrentDictionary<string, TableOrViewMetadata<string, DbType>> m_Tables = new ConcurrentDictionary<string, TableOrViewMetadata<string, DbType>>();
        //private readonly ImmutableHashSet<string> m_Curr;

        /// <summary>
        /// Creates a new instance of <see cref="SQLiteMetadataCache"/>
        /// </summary>
        /// <param name="connectionBuilder">The connection builder.</param>
        public SQLiteMetadataCache(SQLiteConnectionStringBuilder connectionBuilder)
        {
            m_ConnectionBuilder = connectionBuilder;
        }

        /// <summary>
        /// Gets the metadata for a stored procedure.
        /// NOTE:Currently throws a <see cref"NotSupportedException" /> since SQLite doesn't support stored procedures.
        /// </summary>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        public override StoredProcedureMetadata<string, DbType> GetStoredProcedure(string procedureName)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the metadata for a table function.
        /// NOTE:Currently throws a <see cref"NotSupportedException" /> since SQLite doesn't support stored procedures. 
        /// </summary>
        /// <param name="tableFunctionName"></param>
        /// <returns></returns>
        public override TableFunctionMetadata<string, DbType> GetTableFunction(string tableFunctionName)
        {
            throw new NotSupportedException();
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
                            return null;
                        actualName = reader.GetString(reader.GetOrdinal("ObjectName"));
                        var objectType = reader.GetString(reader.GetOrdinal("ObjectType"));
                        isTable = objectType.Equals("table");
                    }
                }
            }

            var columns = GetColumns(tableName);
            return new TableOrViewMetadata<string, DbType>(tableName, isTable, columns);
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

        private List<ColumnMetadata<DbType>> GetColumns(string tableName)
        {
            /*  NOTE: Should be safe since GetTableOrViewInternal returns null after querying the table name with a 
            **  prepared statement. 
            */
            string columnSql = "PRAGMA table_info('" + tableName + "')";

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

                            columns.Add(new ColumnMetadata<DbType>(name, false, isPrimaryKey, false, typeName, TypeNameToDbType(typeName)));
                        }
                    }
                }
            }

            return columns;
        }

        internal static DbType? TypeNameToDbType(string typeName)
        {
            //var typeParts = typeName.Split('(');
            //var baseTypeName = typeParts[0].ToUpper();

            /*  NOTE: It looks like SQLite has a very loose typing system. This follows the Column Affinity
            **  information, but might need to be refactored for more granularity.
            */


            //if (string.IsNullOrEmpty(typeName))
            //    return DbType.Binary;
            //else if (typeName.Contains("INT"))
            //    return DbType.Byte;
            //else if (typeName.Contains("BLOB"))
            //    return DbType.Binary;
            //else if (typeName.Contains("CHAR") ||
            //         typeName.Contains("CLOB") ||
            //         typeName.Contains("TEXT") ||
            //         typeName.Contains("NVARCHAR")) 
            //    return DbType.String;
            //else if (typeName.Contains("REAL") ||
            //         typeName.Contains("FLOA") ||
            //         typeName.Contains("DOUB"))
            //    return DbType.Double;
            //else
            //    return DbType.Decimal;

            //SQLite's type system is so screwy that we should just skip type detection for now.
            return null;
        }

        protected override string ParseObjectName(string name)
        {
            return name;
        }
    }
}
