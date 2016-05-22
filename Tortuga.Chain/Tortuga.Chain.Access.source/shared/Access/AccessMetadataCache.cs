using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using Tortuga.Anchor;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Access
{
    /// <summary>
    /// Handles caching of metadata for various Access tables and views.
    /// </summary>
    public sealed class AccessMetadataCache : DatabaseMetadataCache<AccessObjectName, OleDbType>
    {
        readonly OleDbConnectionStringBuilder m_ConnectionBuilder;
        readonly ConcurrentDictionary<AccessObjectName, TableOrViewMetadata<AccessObjectName, OleDbType>> m_Tables = new ConcurrentDictionary<AccessObjectName, TableOrViewMetadata<AccessObjectName, OleDbType>>();
        readonly ConcurrentDictionary<Type, TableOrViewMetadata<AccessObjectName, OleDbType>> m_TypeTableMap = new ConcurrentDictionary<Type, TableOrViewMetadata<AccessObjectName, OleDbType>>();

        bool m_SchemaLoaded;

        /// <summary>
        /// Creates a new instance of <see cref="AccessMetadataCache"/>
        /// </summary>
        /// <param name="connectionBuilder">The connection builder.</param>
        public AccessMetadataCache(OleDbConnectionStringBuilder connectionBuilder)
        {
            m_ConnectionBuilder = connectionBuilder;
        }

        /// <summary>
        /// Gets the metadata for a table or view.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override TableOrViewMetadata<AccessObjectName, OleDbType> GetTableOrView(AccessObjectName tableName)
        {
            if (!m_SchemaLoaded)
                Preload();

            TableOrViewMetadata<AccessObjectName, OleDbType> result;
            if (m_Tables.TryGetValue(tableName, out result))
                return result;

            throw new MissingObjectException($"Could not find table or view {tableName}");
        }

        /// <summary>
        /// Preloads metadata for all database tables.
        /// </summary>
        /// <remarks>This is normally used only for testing. By default, metadata is loaded as needed.</remarks>
        public void PreloadTables()
        {
            PreloadTables(GetColumnsDataTable(), GetPrimaryKeysDataTable());
        }

        void PreloadTables(DataTable columnsDataTable, DataTable primaryKeys)
        {
            using (var connection = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                connection.Open();
                var dtTables = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow row in dtTables.Rows)
                {
                    if (row["TABLE_TYPE"].ToString() != "TABLE")
                        continue;

                    var name = row["TABLE_NAME"].ToString();
                    var columns = GetColumns(name, columnsDataTable, primaryKeys);
                    m_Tables[name] = new TableOrViewMetadata<AccessObjectName, OleDbType>(name, true, columns);
                }
            }
        }

        /// <summary>
        /// Preloads metadata for all database views.
        /// </summary>
        /// <remarks>This is normally used only for testing. By default, metadata is loaded as needed.</remarks>
        public void PreloadViews()
        {
            PreloadViews(GetColumnsDataTable());
        }

        private void PreloadViews(DataTable columnsDataTable)
        {
            using (var connection = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                connection.Open();
                var dtViews = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Views, null);
                foreach (DataRow row in dtViews.Rows)
                {
                    var name = row["TABLE_NAME"].ToString();
                    var columns = GetColumns(name, columnsDataTable, null);
                    m_Tables[name] = new TableOrViewMetadata<AccessObjectName, OleDbType>(name, false, columns);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private List<ColumnMetadata<OleDbType>> GetColumns(string tableName, DataTable columns, DataTable primaryKeys)
        {
            var result = new List<ColumnMetadata<OleDbType>>();
            DataTable tableSchema;
            using (var con = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                using (var adapter = new OleDbDataAdapter($"SELECT * FROM [{tableName}] WHERE 1=0", con))
                    tableSchema = adapter.FillSchema(new DataTable() {Locale = CultureInfo.InvariantCulture }, SchemaType.Source);
            }


            foreach (DataColumn col in tableSchema.Columns)
            {
                var name = col.ColumnName;
                var isPrimaryKey = false;
                var isIdentity = col.AutoIncrement;
                OleDbType? type = null;

                if (primaryKeys != null)
                    foreach (DataRow row in primaryKeys.Rows)
                    {
                        if (row["TABLE_NAME"].ToString() == tableName && row["COLUMN_NAME"].ToString() == name)
                        {
                            isPrimaryKey = true;
                            break;
                        }
                    }

                foreach (DataRow row in columns.Rows)
                {
                    if (row["TABLE_NAME"].ToString() == tableName && row["COLUMN_NAME"].ToString() == name)
                    {
                        type = (OleDbType)row["DATA_TYPE"];
                        break;
                    }
                }

                bool isNullable = false;
                int? maxLength = null;
                int? precision = null;
                int? scale = null;
                string fullTypeName = null;

                result.Add(new ColumnMetadata<OleDbType>(name, false, isPrimaryKey, isIdentity, type.Value.ToString(), type.Value, $"[{name}]", isNullable, maxLength, precision, scale, fullTypeName));

            }

            return result;
        }

        /// <summary>
        /// Parses the name of the database object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        protected override AccessObjectName ParseObjectName(string name)
        {
            return name;
        }

        /// <summary>
        /// Preloads all of the metadata for this data source.
        /// </summary>
        public override void Preload()
        {
            var columns = GetColumnsDataTable();
            var primaryKeys = GetPrimaryKeysDataTable();

            PreloadTables(columns, primaryKeys);
            PreloadViews(columns);

            m_SchemaLoaded = true;
        }

        private DataTable GetColumnsDataTable()
        {
            using (var connection = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                connection.Open();
                return connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, null);
            }
        }

        private DataTable GetPrimaryKeysDataTable()
        {
            using (var connection = new OleDbConnection(m_ConnectionBuilder.ConnectionString))
            {
                connection.Open();
                return connection.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, null);
            }
        }
        /// <summary>
        /// Gets the tables and views that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all tables and views were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<TableOrViewMetadata<AccessObjectName, OleDbType>> GetTablesAndViews()
        {
            return m_Tables.GetValues();
        }

        /// <summary>
        /// Resets the metadata cache, clearing out all cached metadata.
        /// </summary>
        public override void Reset()
        {
            m_Tables.Clear();
            m_TypeTableMap.Clear();
            m_SchemaLoaded = false;
        }

        /// <summary>
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <returns>TableOrViewMetadata&lt;AccessObjectName, OleDbType&gt;.</returns>
        public override TableOrViewMetadata<AccessObjectName, OleDbType> GetTableOrViewFromClass<TObject>()
        {
            var type = typeof(TObject);
            TableOrViewMetadata<AccessObjectName, OleDbType> result;
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
    }
}
