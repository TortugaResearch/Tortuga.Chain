using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// SqlServerTableOrView supports queries against tables and views.
    /// </summary>
    public class SqlServerTableOrView : MultipleRowDbCommandBuilder<SqlCommand, SqlParameter>
    {
        private readonly object m_FilterValue;
        private readonly TableOrViewMetadata<SqlServerObjectName, SqlDbType> m_Metadata;
        private readonly string m_WhereClause;
        private readonly object m_ArgumentValue;


        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerTableOrView"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value.</param>
        public SqlServerTableOrView(SqlServerDataSourceBase dataSource, SqlServerObjectName tableOrViewName, object filterValue) : base(dataSource)
        {
            if (tableOrViewName == SqlServerObjectName.Empty)
                throw new ArgumentException($"{nameof(tableOrViewName)} is empty", nameof(tableOrViewName));

            m_FilterValue = filterValue;
            m_Metadata = ((SqlServerDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(tableOrViewName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerTableOrView"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        public SqlServerTableOrView(SqlServerDataSourceBase dataSource, SqlServerObjectName tableOrViewName, string whereClause, object argumentValue) : base(dataSource)
        {
            if (tableOrViewName == SqlServerObjectName.Empty)
                throw new ArgumentException($"{nameof(tableOrViewName)} is empty", nameof(tableOrViewName));

            m_ArgumentValue = argumentValue;
            m_WhereClause = whereClause;
            m_Metadata = ((SqlServerDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(tableOrViewName);
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommandType&gt;.</returns>
        public override ExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
        {
            var parameters = new List<SqlParameter>();

            var select = SelectClause(materializer);
            var from = $"FROM {m_Metadata.Name.ToQuotedString()}";

            string where = null;

            if (m_FilterValue != null)
                where = WhereClauseA(parameters);
            else if (!string.IsNullOrWhiteSpace(m_WhereClause))
                where = WhereClauseB(parameters);

            var sql = $"{select} {from} {where};";

            return new ExecutionToken<SqlCommand, SqlParameter>(DataSource, "Query " + m_Metadata.Name, sql, parameters);
        }

        private string SelectClause(Materializer<SqlCommand, SqlParameter> materializer)
        {
            var desiredColumns = materializer.DesiredColumns().ToDictionary(c => c, StringComparer.OrdinalIgnoreCase);
            var availableColumns = m_Metadata.Columns;

            if (desiredColumns.Count == 0)
                return "SELECT " + string.Join(",", availableColumns.Select(c => c.QuotedSqlName));

            var actualColumns = availableColumns.Where(c => desiredColumns.ContainsKey(c.ClrName)).ToList();
            if (actualColumns.Count == 0)
                throw new MappingException($"None of the requested columns were found in {m_Metadata.Name}.");

            return "SELECT " + string.Join(",", actualColumns.Select(c => c.QuotedSqlName));

        }

        private string WhereClauseA(List<SqlParameter> parameters)
        {
            var availableColumns = m_Metadata.Columns.ToDictionary(c => c.ClrName, StringComparer.OrdinalIgnoreCase);
            var properties = MetadataCache.GetMetadata(m_FilterValue.GetType()).Properties;
            var actualColumns = new List<string>();

            if (m_FilterValue is IReadOnlyDictionary<string, object>)
            {
                foreach (var item in (IReadOnlyDictionary<string, object>)m_FilterValue)
                {
                    ColumnMetadata<SqlDbType> column;
                    if (availableColumns.TryGetValue(item.Key, out column))
                    {
                        object value = item.Value ?? DBNull.Value;
                        var parameter = new SqlParameter(column.SqlVariableName, value);
                        if (column.SqlDbType.HasValue)
                            parameter.SqlDbType = column.SqlDbType.Value;
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
                    ColumnMetadata<SqlDbType> column;
                    if (availableColumns.TryGetValue(item.MappedColumnName, out column))
                    {
                        object value = item.InvokeGet(m_FilterValue) ?? DBNull.Value;
                        var parameter = new SqlParameter(column.SqlVariableName, value);
                        if (column.SqlDbType.HasValue)
                            parameter.SqlDbType = column.SqlDbType.Value;
                        parameters.Add(parameter);

                        if (value == DBNull.Value)
                            actualColumns.Add($"{column.QuotedSqlName} IS NULL");
                        else
                            actualColumns.Add($"{column.QuotedSqlName} = {column.SqlVariableName}");
                    }
                }
            }

            if (actualColumns.Count == 0)
                throw new MappingException($"Unable to find any properties on type {m_FilterValue.GetType().Name} that match the columns on {m_Metadata.Name}");

            return "WHERE " + string.Join(" AND ", actualColumns);
        }

        private string WhereClauseB(List<SqlParameter> parameters)
        {
            if (m_ArgumentValue is IEnumerable<SqlParameter>)
                foreach (var param in (IEnumerable<SqlParameter>)m_ArgumentValue)
                    parameters.Add(param);
            else if (m_ArgumentValue is IReadOnlyDictionary<string, object>)
                foreach (var item in (IReadOnlyDictionary<string, object>)m_ArgumentValue)
                    parameters.Add(new SqlParameter("@" + item.Key, item.Value ?? DBNull.Value));
            else if (m_ArgumentValue != null)
                foreach (var property in MetadataCache.GetMetadata(m_ArgumentValue.GetType()).Properties)
                    parameters.Add(new SqlParameter("@" + property.MappedColumnName, property.InvokeGet(m_ArgumentValue) ?? DBNull.Value));

            return "WHERE " + m_WhereClause;
        }

    }
}


