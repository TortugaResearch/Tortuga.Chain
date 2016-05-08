using System;
using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;
using System.Text;
using System.Linq;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#endif


namespace Tortuga.Chain.SQLite.CommandBuilders
{
    /// <summary>
    /// SQliteTableOrView supports queries against tables and views.
    /// </summary>
    internal sealed class SQLiteTableOrView : TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption>
    {
        readonly TableOrViewMetadata<string, DbType> m_Metadata;
        private object m_FilterValue;
        private string m_WhereClause;
        private object m_ArgumentValue;
        private IEnumerable<SortExpression> m_SortExpressions = Enumerable.Empty<SortExpression>();
        private SQLiteLimitOption m_LimitOptions;
        private int? m_Skip;
        private int? m_Take;
        private string m_SelectClause;

        //public object MetadataCache { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteTableOrView" /> class.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="tableOrViewName"></param>
        /// <param name="filterValue"></param>
        public SQLiteTableOrView(SQLiteDataSourceBase dataSource, string tableOrViewName, object filterValue) :
            base(dataSource)
        {
            if (string.IsNullOrEmpty(tableOrViewName))
                throw new ArgumentException("table/view name string is empty");

            m_FilterValue = filterValue;
            m_Metadata = ((SQLiteDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(tableOrViewName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteTableOrView" /> class.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="tableOrViewName"></param>
        /// <param name="whereClause"></param>
        /// <param name="argumentValue"></param>
        public SQLiteTableOrView(SQLiteDataSourceBase dataSource, string tableOrViewName, string whereClause, object argumentValue) :
            base(dataSource)
        {
            if (string.IsNullOrEmpty(tableOrViewName))
                throw new ArgumentException("table/view name string is empty");

            m_ArgumentValue = argumentValue;
            m_WhereClause = whereClause;
            m_Metadata = ((SQLiteDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(tableOrViewName);
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns></returns>
        public override CommandExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = m_Metadata.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            //Support check
            if (!Enum.IsDefined(typeof(SQLiteLimitOption), m_LimitOptions))
                throw new NotSupportedException($"SQL Server does not support limit option {(LimitOptions)m_LimitOptions}");

            //Validation
            if (m_Skip < 0)
                throw new InvalidOperationException($"Cannot skip {m_Skip} rows");
            if (m_Skip > 0 && m_LimitOptions != SQLiteLimitOption.Rows)
                throw new InvalidOperationException($"Cannot perform a Skip operation with limit option {m_LimitOptions}");
            if (m_Take <= 0)
                throw new InvalidOperationException($"Cannot take {m_Take} rows");
            if (m_LimitOptions == SQLiteLimitOption.RandomSampleRows && m_SortExpressions.Any())
                throw new InvalidOperationException($"Cannot perform a random sampling when sorting.");

            //SQL Generation
            List<SQLiteParameter> parameters;

            var sql = new StringBuilder();

            if (m_SelectClause != null)
                sql.Append($"SELECT {m_SelectClause} ");
            else
                sqlBuilder.BuildSelectClause(sql, "SELECT ", null, null);

            sql.Append(" FROM " + m_Metadata.Name);

            if (m_FilterValue != null)
            {
                sql.Append(" WHERE " + sqlBuilder.ApplyFilterValue(m_FilterValue));
                sqlBuilder.BuildSoftDeleteClause(sql, " AND ", DataSource, null);

                parameters = sqlBuilder.GetParameters();
            }
            else if (!string.IsNullOrWhiteSpace(m_WhereClause))
            {
                sql.Append(" WHERE " + m_WhereClause);
                sqlBuilder.BuildSoftDeleteClause(sql, " AND ", DataSource, null);

                parameters = SqlBuilder.GetParameters<SQLiteParameter>(m_ArgumentValue);
                parameters.AddRange(sqlBuilder.GetParameters());
            }
            else
            {
                sqlBuilder.BuildSoftDeleteClause(sql, " WHERE ", DataSource, null);
                parameters = sqlBuilder.GetParameters();
            }
            sqlBuilder.BuildOrderByClause(sql, " ORDER BY ", m_SortExpressions, null);

            switch (m_LimitOptions)
            {
                case SQLiteLimitOption.Rows:
                    sql.Append(" LIMIT @fetch_row_count_expression OFFSET @offset_row_count_expression ");
                    parameters.Add(new SQLiteParameter("@fetch_row_count_expression", m_Take));
                    parameters.Add(new SQLiteParameter("@offset_row_count_expression", m_Skip ?? 0));

                    break;
                case SQLiteLimitOption.RandomSampleRows:
                    sql.Append(" ORDER BY RANDOM() LIMIT @fetch_row_count_expression ");
                    parameters.Add(new SQLiteParameter("@fetch_row_count_expression", m_Take));
                    break;
            }

            sql.Append(";");

            return new SQLiteCommandExecutionToken(DataSource, "Query " + m_Metadata.Name, sql.ToString(), parameters, lockType: LockType.Read);
        }

        /// <summary>
        /// Adds sorting to the command builder.
        /// </summary>
        /// <param name="sortExpressions">The sort expressions.</param>
        /// <returns></returns>
        public override TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> WithSorting(IEnumerable<SortExpression> sortExpressions)
        {
            if (sortExpressions == null)
                throw new ArgumentNullException(nameof(sortExpressions), $"{nameof(sortExpressions)} is null.");

            m_SortExpressions = sortExpressions;
            return this;
        }

        /// <summary>
        /// Adds limits to the command builder.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="take">Number of rows to take.</param>
        /// <param name="limitOptions">The limit options.</param>
        /// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
        /// <returns></returns>
        protected override TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> OnWithLimits(int? skip, int? take, SQLiteLimitOption limitOptions, int? seed)
        {
            //m_Seed = seed;
            m_Skip = skip;
            m_Take = take;
            m_LimitOptions = limitOptions;
            return this;
        }

        /// <summary>
        /// Adds limits to the command builder.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="take">Number of rows to take.</param>
        /// <param name="limitOptions">The limit options.</param>
        /// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
        /// <returns></returns>
        protected override TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> OnWithLimits(int? skip, int? take, LimitOptions limitOptions, int? seed)
        {
            //m_Seed = seed;
            m_Skip = skip;
            m_Take = take;
            m_LimitOptions = (SQLiteLimitOption)limitOptions;
            return this;
        }

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="filterValue">The filter value.</param>
        /// <returns></returns>
        public override TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> WithFilter(object filterValue)
        {
            m_FilterValue = filterValue;
            m_WhereClause = null;
            m_ArgumentValue = null;
            return this;
        }

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <returns></returns>
        public override TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> WithFilter(string whereClause)
        {
            m_FilterValue = null;
            m_WhereClause = whereClause;
            m_ArgumentValue = null;
            return this;
        }

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        public override TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> WithFilter(string whereClause, object argumentValue)
        {
            m_FilterValue = null;
            m_WhereClause = whereClause;
            m_ArgumentValue = argumentValue;
            return this;
        }

        /// <summary>
        /// Returns the row count using a <c>SELECT COUNT_BIG(*)</c> style query.
        /// </summary>
        /// <returns></returns>
        public override ILink<long> AsCount()
        {
            m_SelectClause = "COUNT(*)";
            return ToInt64();
        }

        /// <summary>
        /// Returns the row count for a given column. <c>SELECT COUNT_BIG(columnName)</c>
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="distinct">if set to <c>true</c> use <c>SELECT COUNT_BIG(DISTINCT columnName)</c>.</param>
        /// <returns></returns>
        public override ILink<long> AsCount(string columnName, bool distinct = false)
        {
            var column = m_Metadata.Columns[columnName];
            if (distinct)
                m_SelectClause = $"COUNT(DISTINCT {column.QuotedSqlName})";
            else
                m_SelectClause = $"COUNT({column.QuotedSqlName})";

            return ToInt64();
        }

        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <remarks>
        /// If the column name was not found, this will return null
        /// </remarks>
        public override IColumnMetadata TryGetColumn(string columnName)
        {
            return m_Metadata.Columns.TryGetColumn(columnName);
        }
    }
}


