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
    /// Class SqlServerDeleteSet.
    /// </summary>
    internal sealed class OleDbSqlServerDeleteSet : MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter>
    {
        readonly IEnumerable<OleDbParameter> m_Parameters;
        readonly TableOrViewMetadata<SqlServerObjectName, OleDbType> m_Table;
        readonly string m_WhereClause;
        readonly object m_ArgumentValue;
        readonly object m_FilterValue;
        readonly FilterOptions m_FilterOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDeleteSet" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The parameters.</param>
        public OleDbSqlServerDeleteSet(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName tableName, string whereClause, IEnumerable<OleDbParameter> parameters) : base(dataSource)
        {

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_WhereClause = whereClause;
            m_Parameters = parameters;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbSqlServerDeleteSet"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        public OleDbSqlServerDeleteSet(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName tableName, string whereClause, object argumentValue) : base(dataSource)
        {
            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_WhereClause = whereClause;
            m_ArgumentValue = argumentValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbSqlServerDeleteSet"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The options.</param>
        public OleDbSqlServerDeleteSet(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName tableName, object filterValue, FilterOptions filterOptions) : base(dataSource)
        {
            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_FilterValue = filterValue;
            m_FilterOptions = filterOptions;
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
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            List<OleDbParameter> parameters;
            var sql = new StringBuilder();
            sql.Append("DELETE FROM " + m_Table.Name.ToQuotedString());
            sqlBuilder.BuildSelectClause(sql, " OUTPUT ", "Deleted.", null);

            if (m_FilterValue != null)
            {
                sql.Append(" WHERE " + sqlBuilder.ApplyAnonymousFilterValue(m_FilterValue, m_FilterOptions));

                parameters = sqlBuilder.GetParameters();
            }
            else if (!string.IsNullOrWhiteSpace(m_WhereClause))
            {
                sql.Append(" WHERE " + m_WhereClause);

                parameters = SqlBuilder.GetParameters<OleDbParameter>(m_ArgumentValue);
                parameters.AddRange(sqlBuilder.GetParameters());
            }
            else
            {
                parameters = sqlBuilder.GetParameters();
            }
            sql.Append(";");

            if (m_Parameters != null)
                parameters.AddRange(m_Parameters);

            return new OleDbCommandExecutionToken(DataSource, "Delete from " + m_Table.Name, sql.ToString(), parameters);
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