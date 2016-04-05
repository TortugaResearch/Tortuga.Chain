using System;
using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;
using System.Text;

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
    internal sealed class SQLiteTableOrView : MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter>
    {
        private readonly object m_FilterValue;
        private readonly TableOrViewMetadata<string, DbType> m_Metadata;
        private readonly string m_WhereClause;
        private readonly object m_ArgumentValue;

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

            var sqlBuilder = m_Metadata.CreateSqlBuilder();
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns(), DataSource.StrictMode);

            List<SQLiteParameter> parameters;

            var sql = new StringBuilder();
            sqlBuilder.BuildSelectClause(sql, "SELECT ", null, " FROM " + m_Metadata.Name);

            if (m_FilterValue != null)
            {
                sql.Append(" WHERE " + sqlBuilder.ApplyFilterValue(m_FilterValue, DataSource.StrictMode));
                parameters = sqlBuilder.GetParameters();
            }
            else if (!string.IsNullOrWhiteSpace(m_WhereClause))
            {
                parameters = SqlBuilder.GetParameters<SQLiteParameter>(m_ArgumentValue);
                sql.Append(" WHERE " + m_WhereClause);
            }
            else
            {
                parameters = new List<SQLiteParameter>();
            }
            sql.Append(";");

            return new SQLiteExecutionToken(DataSource, "Query " + m_Metadata.Name, sql.ToString(), parameters, lockType: LockType.Read);
        }

    }
}


