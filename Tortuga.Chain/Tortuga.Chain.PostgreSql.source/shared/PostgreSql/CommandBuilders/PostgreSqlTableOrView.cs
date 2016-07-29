using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{

    /// <summary>
    /// Class PostgreSqlTableOrView
    /// </summary>
    public class PostgreSqlTableOrView : TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption>
    {
        readonly TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> m_Table;
        private object m_FilterValue;
        private string m_WhereClause;
        private object m_ArgumentValue;

        private IEnumerable<SortExpression> m_SortExpressions = Enumerable.Empty<SortExpression>();
        private PostgreSqlLimitOption m_LimitOptions;
        private int? m_Skip;
        private int? m_Take;
        private int? m_Seed;
        private string m_SelectClause;
        private FilterOptions m_FilterOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlTableOrView" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <exception cref="ArgumentException"></exception>
        public PostgreSqlTableOrView(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableOrViewName) :
            base(dataSource)
        {
            if (tableOrViewName == PostgreSqlObjectName.Empty)
                throw new ArgumentException($"{nameof(tableOrViewName)} is empty", nameof(tableOrViewName));

            m_Table = DataSource.DatabaseMetadata.GetTableOrView(tableOrViewName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlTableOrView" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public PostgreSqlTableOrView(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableOrViewName, object filterValue, FilterOptions filterOptions = FilterOptions.None) :
            base(dataSource)
        {
            if (tableOrViewName == PostgreSqlObjectName.Empty)
                throw new ArgumentException($"{nameof(tableOrViewName)} is empty", nameof(tableOrViewName));

            m_FilterValue = filterValue;
            m_FilterOptions = filterOptions;
            m_Table = DataSource.DatabaseMetadata.GetTableOrView(tableOrViewName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlTableOrView"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <exception cref="ArgumentException"></exception>
        public PostgreSqlTableOrView(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableOrViewName, string whereClause, object argumentValue)
            : base(dataSource)
        {
            if (tableOrViewName == PostgreSqlObjectName.Empty)
                throw new ArgumentException($"{nameof(tableOrViewName)} is empty", nameof(tableOrViewName));

            m_WhereClause = whereClause;
            m_ArgumentValue = argumentValue;
            m_Table = DataSource.DatabaseMetadata.GetTableOrView(tableOrViewName);
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>
        /// ExecutionToken&lt;TCommand&gt;.
        /// </returns>
        public override CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyRulesForSelect(DataSource);

            if (m_SelectClause == null)
                sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            //Support check
            if (!Enum.IsDefined(typeof(PostgreSqlLimitOption), m_LimitOptions))
                throw new NotSupportedException($"PostgreSQL does not support limit option {(LimitOptions)m_LimitOptions}");

            //Validation
            if (m_Skip < 0)
                throw new InvalidOperationException($"Cannot skip {m_Skip} rows");

            if (m_Skip > 0 && m_LimitOptions != PostgreSqlLimitOption.Rows)
                throw new InvalidOperationException($"Cannot perform a Skip operation with limit option {m_LimitOptions}");

            if (m_Take <= 0)
                throw new InvalidOperationException($"Cannot take {m_Take} rows");

            if ((m_LimitOptions == PostgreSqlLimitOption.TableSampleBernoulliPercentage || m_LimitOptions == PostgreSqlLimitOption.TableSampleSystemPercentage) && m_SortExpressions.Any())
                throw new InvalidOperationException($"Cannot perform random sampling when sorting.");


            //SQL Generation
            List<NpgsqlParameter> parameters;
            var sql = new StringBuilder();

            if (m_SelectClause != null)
                sql.Append($"SELECT {m_SelectClause} ");
            else
                sqlBuilder.BuildSelectClause(sql, "SELECT ", null, null);

            sql.Append(" FROM " + m_Table.Name);

            switch (m_LimitOptions)
            {
                case PostgreSqlLimitOption.TableSampleSystemPercentage:
                    sql.Append($" TABLESAMPLE SYSTEM ({m_Take}) ");
                    if (m_Seed.HasValue)
                        sql.Append($"REPEATABLE ({m_Seed}) ");
                    break;

                case PostgreSqlLimitOption.TableSampleBernoulliPercentage:
                    sql.Append($" TABLESAMPLE BERNOULLI ({m_Take}) ");
                    if (m_Seed.HasValue)
                        sql.Append($"REPEATABLE ({m_Seed}) ");
                    break;
            }

            if (m_FilterValue != null)
            {
                sql.Append(" WHERE " + sqlBuilder.ApplyFilterValue(m_FilterValue, m_FilterOptions));
                sqlBuilder.BuildSoftDeleteClause(sql, " AND ", DataSource, null);

                parameters = sqlBuilder.GetParameters();
            }
            else if (!string.IsNullOrWhiteSpace(m_WhereClause))
            {
                sql.Append(" WHERE " + m_WhereClause);
                sqlBuilder.BuildSoftDeleteClause(sql, " AND ", DataSource, null);

                parameters = SqlBuilder.GetParameters<NpgsqlParameter>(m_ArgumentValue);
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
                case PostgreSqlLimitOption.Rows:

                    sql.Append(" OFFSET @offset_row_count_expression");
                    parameters.Add(new NpgsqlParameter("@offset_row_count_expression", m_Skip ?? 0));

                    if (m_Take.HasValue)
                    {
                        sql.Append(" LIMIT @limit_row_count_expression");
                        parameters.Add(new NpgsqlParameter("@limit_row_count_expression", m_Take));
                    }

                    break;
            }

            sql.Append(";");

            return new PostgreSqlCommandExecutionToken(DataSource, "Query " + m_Table.Name, sql.ToString(), parameters);
        }


        /// <summary>
        /// Adds sorting to the command builder.
        /// </summary>
        /// <param name="sortExpressions">The sort expressions.</param>
        /// <returns></returns>
        public override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> WithSorting(IEnumerable<SortExpression> sortExpressions)
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
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        protected override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> OnWithLimits(int? skip, int? take, PostgreSqlLimitOption limitOptions, int? seed)
        {
            m_Seed = seed;
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
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        protected override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> OnWithLimits(int? skip, int? take, LimitOptions limitOptions, int? seed)
        {
            m_Seed = seed;
            m_Skip = skip;
            m_Take = take;
            m_LimitOptions = (PostgreSqlLimitOption)limitOptions;
            return this;
        }

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        public override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> WithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None)
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
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        public override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> WithFilter(string whereClause)
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
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        public override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> WithFilter(string whereClause, object argumentValue)
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
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public new PostgreSqlDataSourceBase DataSource
        {
            get { return (PostgreSqlDataSourceBase)base.DataSource; }
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

