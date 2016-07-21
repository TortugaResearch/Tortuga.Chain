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
    /// Class PostgreSqlDeleteMany.
    /// </summary>
    internal sealed class PostgreSqlDeleteMany : MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter>
    {
        //readonly DeleteOptions m_Options;
        readonly IEnumerable<NpgsqlParameter> m_Parameters;
        readonly TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> m_Table;
        readonly string m_WhereClause;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDeleteMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        public PostgreSqlDeleteMany(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, string whereClause, IEnumerable<NpgsqlParameter> parameters, DeleteOptions options) : base(dataSource)
        {
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                throw new NotSupportedException("Cannot use Key attributes with this operation.");

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_WhereClause = whereClause;
            //m_Options = options;
            m_Parameters = parameters;
        }

        public override CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            sql.Append("DELETE FROM " + m_Table.Name.ToQuotedString());
            sql.Append(" WHERE " + m_WhereClause);
            sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, ";");

            var parameters = sqlBuilder.GetParameters();
            parameters.AddRange(m_Parameters);

            return new PostgreSqlCommandExecutionToken(DataSource, "Delete from " + m_Table.Name, sql.ToString(), parameters);
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

