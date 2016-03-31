using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql
{
    public class PostgreSqlMetadataCache : DatabaseMetadataCache<PostgreSqlObjectName, DbType>
    {
        private readonly NpgsqlConnectionStringBuilder m_ConnectionBuilder;
        private readonly ConcurrentDictionary<PostgreSqlObjectName, TableOrViewMetadata<PostgreSqlObjectName, DbType>> m_Tables = new ConcurrentDictionary<PostgreSqlObjectName, TableOrViewMetadata<PostgreSqlObjectName, DbType>>();

        public PostgreSqlMetadataCache(NpgsqlConnectionStringBuilder connectionBuilder)
        {
            m_ConnectionBuilder = connectionBuilder;
        }

        public override StoredProcedureMetadata<PostgreSqlObjectName, DbType> GetStoredProcedure(PostgreSqlObjectName procedureName)
        {
            throw new NotImplementedException();
        }

        public override TableFunctionMetadata<PostgreSqlObjectName, DbType> GetTableFunction(PostgreSqlObjectName tableFunctionName)
        {
            throw new NotImplementedException();
        }

        public override TableOrViewMetadata<PostgreSqlObjectName, DbType> GetTableOrView(PostgreSqlObjectName tableName)
        {
            return m_Tables.GetOrAdd(tableName, GetTableOrViewInternal);
        }

        private TableOrViewMetadata<PostgreSqlObjectName, DbType> GetTableOrViewInternal(PostgreSqlObjectName tableName)
        {
            const string tableSql =
                @"SELECT ";

            return null;
        }

        private List<ColumnMetadata<DbType>> GetColumns(PostgreSqlObjectName tableName)
        {
            return null;
        }

        protected override PostgreSqlObjectName ParseObjectName(string name)
        {
            return new PostgreSqlObjectName(name);
        }
    }
}
