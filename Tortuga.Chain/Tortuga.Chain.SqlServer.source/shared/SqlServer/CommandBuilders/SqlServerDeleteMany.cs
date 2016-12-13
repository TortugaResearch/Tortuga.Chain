using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class SqlServerDeleteMany.
    /// </summary>
    internal sealed class SqlServerDeleteMany : MultipleRowDbCommandBuilder<SqlCommand, SqlParameter>
    {
        //readonly DeleteOptions m_Options;
        readonly IEnumerable<SqlParameter> m_Parameters;
        readonly SqlServerTableOrViewMetadata<SqlDbType> m_Table;
        readonly string m_WhereClause;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDeleteMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        public SqlServerDeleteMany(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, string whereClause, IEnumerable<SqlParameter> parameters,  DeleteOptions options) : base(dataSource)
        {
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                throw new NotSupportedException("Cannot use Key attributes with this operation.");

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_WhereClause = whereClause;
            //m_Options = options;
            m_Parameters = parameters;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>

        public override CommandExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            string header;
            string intoClause;
            string footer;

            sqlBuilder.UseTableVariable(m_Table, out header, out intoClause, out footer);

            sql.Append(header);
            sql.Append("DELETE FROM " + m_Table.Name.ToQuotedString());
            sqlBuilder.BuildSelectClause(sql, " OUTPUT ", "Deleted.", intoClause);
            sql.Append(" WHERE " + m_WhereClause);
            sql.Append(";");
            sql.Append(footer);

            var parameters = sqlBuilder.GetParameters();
            parameters.AddRange(m_Parameters);

            return new SqlServerCommandExecutionToken(DataSource, "Delete from " + m_Table.Name, sql.ToString(), parameters);
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

