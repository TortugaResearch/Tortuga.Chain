using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    /// <summary>
    /// SQliteTableOrView supports queries against tables and views.
    /// </summary>
    public class SQLiteTableOrView : MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter>
    {
        private readonly object m_FilterValue;
        private readonly TableOrViewMetadata<string, DbType> m_MetaData;
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
            m_MetaData = ((SQLiteDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(tableOrViewName);
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
            m_MetaData = ((SQLiteDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(tableOrViewName);
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns></returns>
        public override Tortuga.Chain.Core.ExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            var parameters = new List<SQLiteParameter>();

            var select = SelectClause(materializer);
            var from = $"FROM {m_MetaData.Name}";

            string where = null;

            if (m_FilterValue != null)
                where = WhereClauseFromFilter(parameters);
            else if (!string.IsNullOrEmpty(m_WhereClause))
                where = WhereClauseFromString(parameters);

            var sql = $"{select} {from} {where}";

            return new SQLiteExecutionToken(DataSource, "Query " + m_MetaData.Name, sql, parameters, lockType: LockType.Read);
        }

        private string SelectClause(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            var desiredColumns = materializer.DesiredColumns().ToDictionary(c => c, StringComparer.InvariantCultureIgnoreCase);
            var availableColumns = m_MetaData.Columns;

            if (desiredColumns.Count == 0)
                return "SELECT " + string.Join(",", availableColumns.Select(c => c.QuotedSqlName));

            var actualColumns = availableColumns.Where(c => desiredColumns.ContainsKey(c.ClrName)).ToList();
            if (actualColumns.Count == 0)
                throw new MappingException($"None of the request columns were found in {m_MetaData.Name}");

            return "SELECT " + string.Join(",", actualColumns.Select(c => c.QuotedSqlName));
        }

        private string WhereClauseFromFilter(List<SQLiteParameter> parameters)
        {
            var availableColumns = m_MetaData.Columns.ToDictionary(c => c.ClrName, StringComparer.InvariantCultureIgnoreCase);
            var properties = Anchor.Metadata.MetadataCache.GetMetadata(m_FilterValue.GetType()).Properties;
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
                        parameters.Add(parameter);

                        if (value == DBNull.Value)
                            actualColumns.Add($"{column.QuotedSqlName} IS NULL");
                        else
                            actualColumns.Add($"{column.QuotedSqlName} = {column.SqlVariableName}");
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
                        parameters.Add(parameter);

                        if (value == DBNull.Value)
                            actualColumns.Add($"{column.QuotedSqlName} IS NULL");
                        else
                            actualColumns.Add($"{column.QuotedSqlName} = {column.SqlVariableName}");
                    }
                }
            }

            if (actualColumns.Count == 0)
                throw new MappingException($"Unable to find any properties on type {m_FilterValue.GetType().Name} that match the columns on {m_MetaData.Name}");

            return "WHERE " + string.Join(" AND ", actualColumns);
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
