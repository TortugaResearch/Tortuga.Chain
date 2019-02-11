using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;


namespace Tortuga.Chain.SQLite.CommandBuilders
{
    /// <summary>
    /// SQliteTableOrView supports queries against tables and views.
    /// </summary>
    internal sealed class SQLiteTableOrView : TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption>
    {
        readonly TableOrViewMetadata<SQLiteObjectName, DbType> m_Table;
        object m_FilterValue;
        string m_WhereClause;
        object m_ArgumentValue;
        IEnumerable<SortExpression> m_SortExpressions = Enumerable.Empty<SortExpression>();
        SQLiteLimitOption m_LimitOptions;
        int? m_Skip;
        int? m_Take;
        string m_SelectClause;
        FilterOptions m_FilterOptions;

        //public object MetadataCache { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteTableOrView" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        public SQLiteTableOrView(SQLiteDataSourceBase dataSource, SQLiteObjectName tableOrViewName, object filterValue, FilterOptions filterOptions = FilterOptions.None) :
            base(dataSource)
        {
            m_FilterValue = filterValue;
            m_FilterOptions = filterOptions;
            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableOrViewName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteTableOrView" /> class.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="tableOrViewName"></param>
        /// <param name="whereClause"></param>
        /// <param name="argumentValue"></param>
        public SQLiteTableOrView(SQLiteDataSourceBase dataSource, SQLiteObjectName tableOrViewName, string whereClause, object argumentValue) :
            base(dataSource)
        {
            m_ArgumentValue = argumentValue;
            m_WhereClause = whereClause;
            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableOrViewName);
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

            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyRulesForSelect(DataSource);

            if (m_SelectClause == null)
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

            sql.Append(" FROM " + m_Table.Name);

            if (m_FilterValue != null)
            {
                sql.Append(" WHERE (" + sqlBuilder.ApplyFilterValue(m_FilterValue, m_FilterOptions) + ")");
                sqlBuilder.BuildSoftDeleteClause(sql, " AND (", DataSource, ") ");

                parameters = sqlBuilder.GetParameters();
            }
            else if (!string.IsNullOrWhiteSpace(m_WhereClause))
            {
                sql.Append(" WHERE (" + m_WhereClause + ")");
                sqlBuilder.BuildSoftDeleteClause(sql, " AND (", DataSource, ") ");

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

            return new SQLiteCommandExecutionToken(DataSource, "Query " + m_Table.Name, sql.ToString(), parameters, lockType: LockType.Read);
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
        /// <param name="filterOptions">The filter options.</param>
        /// <returns>TableDbCommandBuilder&lt;SQLiteCommand, SQLiteParameter, SQLiteLimitOption&gt;.</returns>
        public override TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> WithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            m_FilterValue = filterValue;
            m_WhereClause = null;
            m_ArgumentValue = null;
            m_FilterOptions = filterOptions;
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
            var column = m_Table.Columns[columnName];
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
        public override ColumnMetadata TryGetColumn(string columnName)
        {
            return m_Table.Columns.TryGetColumn(columnName);
        }

        /// <summary>
        /// Returns a list of columns known to be non-nullable.
        /// </summary>
        /// <returns>
        /// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
        /// </returns>
        /// <remarks>
        /// This is used by materializers to skip IsNull checks.
        /// </remarks>
        public override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => m_Table.NonNullableColumns;
    }
}


