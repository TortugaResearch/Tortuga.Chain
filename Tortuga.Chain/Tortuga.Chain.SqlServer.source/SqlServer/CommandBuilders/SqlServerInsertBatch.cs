using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    internal class SqlServerInsertBatch : MultipleRowDbCommandBuilder<SqlCommand, SqlParameter>
    {
        private readonly object m_Source;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "For future use")]
        private readonly InsertOptions m_Options;
        private readonly TableOrViewMetadata<SqlServerObjectName, SqlDbType> m_Table;
        private readonly UserDefinedTypeMetadata<SqlServerObjectName, SqlDbType> m_TableType;

        public SqlServerInsertBatch(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, SqlServerObjectName tableTypeName, DataTable dataTable, InsertOptions options) : base(dataSource)
        {
            m_Source = dataTable;
            m_Options = options;
            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_TableType = dataSource.DatabaseMetadata.GetUserDefinedType(tableTypeName);
            if (!m_TableType.IsTableType)
                throw new MappingException($"{m_TableType.Name} is not a user defined table type");
        }

        public SqlServerInsertBatch(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, SqlServerObjectName tableTypeName, DbDataReader dataReader, InsertOptions options) : base(dataSource)
        {
            m_Source = dataReader;
            m_Options = options;
            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_TableType = dataSource.DatabaseMetadata.GetUserDefinedType(tableTypeName);
            if (!m_TableType.IsTableType)
                throw new MappingException($"{m_TableType.Name} is not a user defined table type");
        }

        public override ExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyTableType(DataSource, OperationTypes.Insert, m_TableType.Columns);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {m_Table.Name.ToQuotedString()} (", null, ")");
            sqlBuilder.BuildSelectClause(sql, " OUTPUT ", "Inserted.", null);
            sqlBuilder.BuildSelectTvpClause(sql, " SELECT ", null, " FROM @ValuesParameter ");
            sql.Append(";");

            var parameters = sqlBuilder.GetParameters();
            parameters.Add(new SqlParameter()
            {
                ParameterName = "@ValuesParameter",
                Value = m_Source,
                SqlDbType = SqlDbType.Structured,
                TypeName = m_TableType.Name.ToQuotedString()
            });
            return new SqlServerExecutionToken(DataSource, "Insert batch into " + m_Table.Name, sql.ToString(), parameters);
        }
    }
}
