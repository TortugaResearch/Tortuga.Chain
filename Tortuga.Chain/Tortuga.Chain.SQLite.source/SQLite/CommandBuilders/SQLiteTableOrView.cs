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


namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    /// <summary>
    /// SQliteTableOrView supports queries against tables and views.
    /// </summary>
    internal sealed class SQLiteTableOrView : TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption>
    {
        private readonly object m_FilterValue;
        private readonly TableOrViewMetadata<string, DbType> m_Metadata;
        private readonly string m_WhereClause;
        private readonly object m_ArgumentValue;
        private IEnumerable<SortExpression> m_SortExpressions = Enumerable.Empty<SortExpression>();
        private SQLiteLimitOption m_LimitOptions;
        private int? m_Skip;
        private int? m_Take;

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
        public override ExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
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
            sqlBuilder.BuildSelectClause(sql, "SELECT ", null, " FROM " + m_Metadata.Name);

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

            return new SQLiteExecutionToken(DataSource, "Query " + m_Metadata.Name, sql.ToString(), parameters, lockType: LockType.Read);
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

        protected override TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> OnWithLimits(int? skip, int? take, SQLiteLimitOption limitOptions, int? seed)
        {
            //m_Seed = seed;
            m_Skip = skip;
            m_Take = take;
            m_LimitOptions = limitOptions;
            return this;
        }

        protected override TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption> OnWithLimits(int? skip, int? take, LimitOptions limitOptions, int? seed)
        {
            //m_Seed = seed;
            m_Skip = skip;
            m_Take = take;
            m_LimitOptions = (SQLiteLimitOption)limitOptions;
            return this;
        }
    }
}


