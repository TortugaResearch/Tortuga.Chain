using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;
using Tortuga.Anchor.Metadata;
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

        private string SelectClause(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            var desiredColumns = materializer.DesiredColumns();
            if (desiredColumns == Materializer.NoColumns)
                return "SELECT 1";

            var availableColumns = m_Metadata.Columns;

            if (desiredColumns == Materializer.AllColumns)
                return "SELECT " + string.Join(",", availableColumns.Select(c => c.QuotedSqlName));

            var lookup = desiredColumns.ToDictionary(c => c, StringComparer.OrdinalIgnoreCase);
            var actualColumns = availableColumns.Where(c => lookup.ContainsKey(c.ClrName)).ToList();
            if (actualColumns.Count == 0)
                throw new MappingException($"None of the request columns were found in {m_Metadata.Name}");

            return "SELECT " + string.Join(",", actualColumns.Select(c => c.QuotedSqlName));
        }

        private string WhereClauseFromFilter(List<SQLiteParameter> parameters)
        {
            var availableColumns = m_Metadata.Columns.ToDictionary(c => c.ClrName, StringComparer.OrdinalIgnoreCase);
            var properties = MetadataCache.GetMetadata(m_FilterValue.GetType()).Properties;
            var actualColumns = new List<string>();

            if (m_FilterValue is IReadOnlyDictionary<string, object>)
            {
                foreach (var item in (IReadOnlyDictionary<string, object>)m_FilterValue)
                {
                    ColumnMetadata<DbType> column;
                    if (availableColumns.TryGetValue(item.Key, out column))
                    {
                        object value = item.Value ?? DBNull.Value;
                        var parameter = new SQLiteParameter(column.SqlVariableName, value);
                        if (column.DbType.HasValue)
                            parameter.DbType = column.DbType.Value;

                        if (value == DBNull.Value)
                        {
                            actualColumns.Add($"{column.QuotedSqlName} IS NULL");
                        }
                        else
                        {
                            actualColumns.Add($"{column.QuotedSqlName} = {column.SqlVariableName}");
                            parameters.Add(parameter);
                        }
                    }
                }
            }
            else
            {
                foreach (var item in properties)
                {
                    ColumnMetadata<DbType> column;
                    if (availableColumns.TryGetValue(item.MappedColumnName, out column))
                    {
                        object value = item.InvokeGet(m_FilterValue) ?? DBNull.Value;
                        var parameter = new SQLiteParameter(column.SqlVariableName, value);
                        if (column.DbType.HasValue)
                            parameter.DbType = column.DbType.Value;

                        if (value == DBNull.Value)
                        {
                            actualColumns.Add($"{column.QuotedSqlName} IS NULL");
                        }
                        else
                        {
                            actualColumns.Add($"{column.QuotedSqlName} = {column.SqlVariableName}");
                            parameters.Add(parameter);
                        }
                    }
                }
            }

            if (actualColumns.Count == 0)
                throw new MappingException($"Unable to find any properties on type {m_FilterValue.GetType().Name} that match the columns on {m_Metadata.Name}");

            return " WHERE " + string.Join(" AND ", actualColumns);
        }

        private string WhereClauseFromString(List<SQLiteParameter> parameters)
        {
            if (m_ArgumentValue is IEnumerable<SQLiteParameter>)
                foreach (var param in (IEnumerable<SQLiteParameter>)m_ArgumentValue)
                    parameters.Add(param);
            else if (m_ArgumentValue is IReadOnlyDictionary<string, object>)
                foreach (var item in (IReadOnlyDictionary<string, object>)m_ArgumentValue)
                    parameters.Add(new SQLiteParameter("@" + item.Key, item.Value ?? DBNull.Value));
            else if (m_ArgumentValue != null)
                foreach (var property in Anchor.Metadata.MetadataCache.GetMetadata(m_ArgumentValue.GetType()).Properties)
                    parameters.Add(new SQLiteParameter("@" + property.MappedColumnName, property.InvokeGet(m_ArgumentValue) ?? DBNull.Value));

            return "WHERE " + m_WhereClause;
        }
    }
}


