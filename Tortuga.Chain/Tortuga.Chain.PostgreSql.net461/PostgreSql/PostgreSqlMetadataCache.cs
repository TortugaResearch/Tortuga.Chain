using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Data;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql
{
    public class PostgreSqlMetadataCache : DatabaseMetadataCache<string, DbType>
    {
        private readonly NpgsqlConnectionStringBuilder m_ConnectionBuilder;
        private readonly ConcurrentDictionary<string, TableOrViewMetadata<string, DbType>> m_Tables = new ConcurrentDictionary<string, TableOrViewMetadata<string, DbType>>();

        public PostgreSqlMetadataCache(NpgsqlConnectionStringBuilder connectionBuilder)
        {
            m_ConnectionBuilder = connectionBuilder;
        }

        public override StoredProcedureMetadata<string, DbType> GetStoredProcedure(string procedureName)
        {
            throw new NotImplementedException();
        }

        public override TableFunctionMetadata<string, DbType> GetTableFunction(string tableFunctionName)
        {
            throw new NotImplementedException();
        }

        public override TableOrViewMetadata<string, DbType> GetTableOrView(string tableName)
        {
            return m_Tables.GetOrAdd(tableName, GetTableOrViewInternal);
        }

        private TableOrViewMetadata<string, DbType> GetTableOrViewInternal(string tableName)
        {
            const string tableSql =
                @"SELECT";
        }

        private List<ColumnMetadata<DbType>> GetColumns(string tableName)
        {

        }

        protected override string ParseObjectName(string name)
        {
            return name;
        }
    }
}
