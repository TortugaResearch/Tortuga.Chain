using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
    /// <summary>
    /// Class PostgreSqlUpdateMany.
    /// </summary>
    internal sealed class PostgreSqlUpdateMany : MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter>
    {
        readonly int? m_ExpectedRowCount;
        readonly object m_NewValues;
        readonly UpdateOptions m_Options;
        readonly IEnumerable<NpgsqlParameter> m_Parameters;
        readonly TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> m_Table;
        readonly string m_WhereClause;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlUpdateMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedRowCount">The expected row count.</param>
        /// <param name="options">The options.</param>
        public PostgreSqlUpdateMany(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, object newValues, string whereClause, IEnumerable<NpgsqlParameter> parameters, int? expectedRowCount, UpdateOptions options) : base(dataSource)
        {
            if (options.HasFlag(UpdateOptions.UseKeyAttribute))
                throw new NotSupportedException("Cannot use Key attributes with this operation.");

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_NewValues = newValues;
            m_WhereClause = whereClause;
            m_ExpectedRowCount = expectedRowCount;
            m_Options = options;
            m_Parameters = parameters;
        }

        public override CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, m_NewValues, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            if (sqlBuilder.HasReadFields && m_Options.HasFlag(UpdateOptions.ReturnOldValues))
            {
                sqlBuilder.BuildSelectClause(sql, "SELECT ", null, " FROM " + m_Table.Name.ToQuotedString());
                sql.AppendLine(" WHERE " + m_WhereClause + ";");
            }

            sqlBuilder.BuildSetClause(sql, "UPDATE " + m_Table.Name.ToQuotedString() + " SET ", null, null);
            sql.Append(" WHERE " + m_WhereClause);

            if (!m_Options.HasFlag(UpdateOptions.ReturnOldValues))
            {
                sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, null);
            }
            sql.Append(";");

            var parameters = sqlBuilder.GetParameters();
            if (m_Parameters != null)
                parameters.AddRange(m_Parameters);

            return new PostgreSqlExecutionToken(DataSource, "Update " + m_Table.Name, sql.ToString(), parameters).CheckUpdateRowCount(m_Options);
        }



        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <remarks>
        /// If the column name was not found, this will return null
        /// </remarks>
        public override ColumnMetadata TryGetColumn(string columnName)
        {
            return m_Table.Columns.TryGetColumn(columnName);
        }

    }
}


