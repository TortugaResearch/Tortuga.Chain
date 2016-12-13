#if !OleDb_Missing
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class OleDbSqlServerUpdateMany.
    /// </summary>
    internal sealed class OleDbSqlServerUpdateMany : MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter>
    {
        readonly int? m_ExpectedRowCount;
        readonly object m_NewValues;
        readonly UpdateOptions m_Options;
        readonly IEnumerable<OleDbParameter> m_Parameters;
        readonly SqlServerTableOrViewMetadata<OleDbType> m_Table;
        readonly string m_WhereClause;

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbSqlServerUpdateMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedRowCount">The expected row count.</param>
        /// <param name="options">The options.</param>
        public OleDbSqlServerUpdateMany(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName tableName, object newValues, string whereClause, IEnumerable<OleDbParameter> parameters, int? expectedRowCount, UpdateOptions options) : base(dataSource)
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

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>

        public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, m_NewValues, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var prefix = m_Options.HasFlag(UpdateOptions.ReturnOldValues) ? "Deleted." : "Inserted.";

            var sql = new StringBuilder();
            string header;
            string intoClause;
            string footer;

            sqlBuilder.UseTableVariable(m_Table, out header, out intoClause, out footer);

            sql.Append(header);
            sql.Append($"UPDATE {m_Table.Name.ToQuotedString()}");
            sqlBuilder.BuildAnonymousSetClause(sql, " SET ", null, null);
            sqlBuilder.BuildSelectClause(sql, " OUTPUT ", prefix, intoClause);
            sql.Append(" WHERE " + m_WhereClause);
            sql.Append(";");
            sql.Append(footer);

            var parameters = sqlBuilder.GetParameters();
            parameters.AddRange(m_Parameters);

            return new OleDbCommandExecutionToken(DataSource, "Update " + m_Table.Name, sql.ToString(), parameters).CheckUpdateRowCount(m_Options, m_ExpectedRowCount);
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


#endif