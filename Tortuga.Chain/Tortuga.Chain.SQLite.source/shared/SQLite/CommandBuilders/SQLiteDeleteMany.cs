using System;
using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using System.Text;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#endif


namespace Tortuga.Chain.SQLite.CommandBuilders
{
    /// <summary>
    /// Class SQLiteDeleteWithFilter.
    /// </summary>
    internal sealed class SQLiteDeleteMany : MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter>
    {
        //readonly DeleteOptions m_Options;
        readonly IEnumerable<SQLiteParameter> m_Parameters;
        readonly TableOrViewMetadata<SQLiteObjectName, DbType> m_Table;
        readonly string m_WhereClause;
        readonly object m_ArgumentValue;
        readonly object m_FilterValue;
        readonly FilterOptions m_FilterOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDeleteWithFilter" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        public SQLiteDeleteMany(SQLiteDataSourceBase dataSource, SQLiteObjectName tableName, string whereClause, IEnumerable<SQLiteParameter> parameters, DeleteOptions options) : base(dataSource)
        {
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                throw new NotSupportedException("Cannot use Key attributes with this operation.");

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_WhereClause = whereClause;
            //m_Options = options;
            m_Parameters = parameters;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDeleteWithFilter"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        public SQLiteDeleteMany(SQLiteDataSourceBase dataSource, SQLiteObjectName tableName, string whereClause, object argumentValue) : base(dataSource)
        {
            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_WhereClause = whereClause;
            m_ArgumentValue = argumentValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDeleteWithFilter"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The options.</param>
        public SQLiteDeleteMany(SQLiteDataSourceBase dataSource, SQLiteObjectName tableName, object filterValue, FilterOptions filterOptions) : base(dataSource)
        {
            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_FilterValue = filterValue;
            m_FilterOptions = filterOptions;
        }


        public override CommandExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            List<SQLiteParameter> parameters;
            var sql = new StringBuilder();

            if (sqlBuilder.HasReadFields)
            {
                sqlBuilder.BuildSelectClause(sql, "SELECT ", null, " FROM " + m_Table.Name.ToQuotedString());
                if (m_FilterValue != null)
                    sql.Append(" WHERE " + sqlBuilder.ApplyFilterValue(m_FilterValue, m_FilterOptions));
                else if (!string.IsNullOrWhiteSpace(m_WhereClause))
                    sql.Append(" WHERE " + m_WhereClause);
                sql.AppendLine(";");
            }

            sql.Append("DELETE FROM " + m_Table.Name.ToQuotedString());
            if (m_FilterValue != null)
            {
                sql.Append(" WHERE " + sqlBuilder.ApplyFilterValue(m_FilterValue, m_FilterOptions));
                parameters = sqlBuilder.GetParameters();
            }
            else if (!string.IsNullOrWhiteSpace(m_WhereClause))
            {
                sql.Append(" WHERE " + m_WhereClause);
                parameters = SqlBuilder.GetParameters<SQLiteParameter>(m_ArgumentValue);
                parameters.AddRange(sqlBuilder.GetParameters());
            }
            else
            {
                parameters = sqlBuilder.GetParameters();
            }
            sql.Append(";");

            if (m_Parameters != null)
                parameters.AddRange(m_Parameters);

            return new SQLiteCommandExecutionToken(DataSource, "Delete from " + m_Table.Name, sql.ToString(), parameters, lockType: LockType.Write);
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

