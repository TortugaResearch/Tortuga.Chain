using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Access.CommandBuilders
{
    /// <summary>
    /// Class AccessUpdateSet.
    /// </summary>
    internal sealed class AccessUpdateMany : UpdateManyCommandBuilder<OleDbCommand, OleDbParameter>
    {
        readonly int? m_ExpectedRowCount;
        readonly IEnumerable<OleDbParameter> m_Parameters;
        readonly TableOrViewMetadata<AccessObjectName, OleDbType> m_Table;

        readonly object m_NewValues;
        readonly string m_UpdateExpression;
        readonly object m_UpdateArgumentValue;
        readonly UpdateOptions m_Options;

        string m_WhereClause;
        object m_WhereArgumentValue;
        object m_FilterValue;
        FilterOptions m_FilterOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessUpdateMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedRowCount">The expected row count.</param>
        /// <param name="options">The options.</param>
        public AccessUpdateMany(AccessDataSourceBase dataSource, AccessObjectName tableName, object newValues, string whereClause, IEnumerable<OleDbParameter> parameters, int? expectedRowCount, UpdateOptions options) : base(dataSource)
        {
            if (options.HasFlag(UpdateOptions.UseKeyAttribute))
                throw new NotSupportedException("Cannot use Key attributes with this operation.");

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_NewValues = newValues;
            m_WhereClause = whereClause;
            m_ExpectedRowCount = expectedRowCount;
            m_Options = options;
            m_Parameters = parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessUpdateMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="System.NotSupportedException">Cannot use Key attributes with this operation.</exception>
        public AccessUpdateMany(AccessDataSourceBase dataSource, AccessObjectName tableName, object newValues, UpdateOptions options) : base(dataSource)
        {
            if (options.HasFlag(UpdateOptions.UseKeyAttribute))
                throw new NotSupportedException("Cannot use Key attributes with this operation.");

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_NewValues = newValues;
            m_Options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessUpdateMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="updateArgumentValue">The update argument value.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="System.NotSupportedException">Cannot use Key attributes with this operation.</exception>
        public AccessUpdateMany(AccessDataSourceBase dataSource, AccessObjectName tableName, string updateExpression, object updateArgumentValue, UpdateOptions options) : base(dataSource)
        {
            if (options.HasFlag(UpdateOptions.UseKeyAttribute))
                throw new NotSupportedException("Cannot use Key attributes with this operation.");

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_UpdateExpression = updateExpression;
            m_Options = options;
            m_UpdateArgumentValue = updateArgumentValue;
        }



        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns><see cref="AccessCommandExecutionToken" /></returns>
        public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            SqlBuilder.CheckForOverlaps(m_NewValues, m_WhereArgumentValue, "The same parameter '{0}' appears in both the newValue object and the where clause argument. Rename the parameter in the where expression to resolve the conflict.");
            SqlBuilder.CheckForOverlaps(m_NewValues, m_FilterValue, "The same parameter '{0}' appears in both the newValue object and the filter object. Use an update expression or where expression to resolve the conflict.");
            SqlBuilder.CheckForOverlaps(m_UpdateArgumentValue, m_WhereArgumentValue, "The same parameter '{0}' appears in both the update expression argument and the where clause argument. Rename the parameter in the where expression to resolve the conflict.");
            SqlBuilder.CheckForOverlaps(m_UpdateArgumentValue, m_FilterValue, "The same parameter '{0}' appears in both the update expression argument and the filter object. Use an update expression or where expression to resolve the conflict.");

            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, m_NewValues, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var parameters = new List<OleDbParameter>();
            var sql = new StringBuilder("UPDATE " + m_Table.Name.ToQuotedString());
            if (m_UpdateExpression == null)
            {
                sqlBuilder.BuildSetClause(sql, " SET ", null, null);
            }
            else
            {
                sql.Append(" SET " + m_UpdateExpression);
                parameters.AddRange(SqlBuilder.GetParameters<OleDbParameter>(m_UpdateArgumentValue));
            }

            if (m_FilterValue != null)
            {
                sql.Append(" WHERE " + sqlBuilder.ApplyAnonymousFilterValue(m_FilterValue, m_FilterOptions, true));
                parameters = sqlBuilder.GetParameters();
            }
            else if (!string.IsNullOrWhiteSpace(m_WhereClause))
            {
                sql.Append(" WHERE " + m_WhereClause);
                parameters.AddRange(sqlBuilder.GetParameters());
                parameters.AddRange(SqlBuilder.GetParameters<OleDbParameter>(m_WhereArgumentValue));
            }
            else
            {
                parameters.AddRange(sqlBuilder.GetParameters());
            }
            sql.Append(";");

            if (m_Parameters != null)
                parameters.AddRange(m_Parameters);

            var updateCommand = new AccessCommandExecutionToken(DataSource, "Update " + m_Table.Name, sql.ToString(), parameters).CheckUpdateRowCount(m_Options, m_ExpectedRowCount);
            updateCommand.ExecutionMode = AccessCommandExecutionMode.NonQuery;

            var desiredColumns = materializer.DesiredColumns();
            if (desiredColumns == Materializer.NoColumns)
                return updateCommand;

            if (m_Options.HasFlag(UpdateOptions.ReturnOldValues))
            {
                var result = PrepareRead(desiredColumns);
                result.NextCommand = updateCommand;
                return result;
            }
            else
            {
                updateCommand.NextCommand = PrepareRead(desiredColumns);
                return updateCommand;
            }
        }


        AccessCommandExecutionToken PrepareRead(IReadOnlyList<string> desiredColumns)
        {
            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyDesiredColumns(desiredColumns);

            List<OleDbParameter> parameters;
            var sql = new StringBuilder();
            sqlBuilder.BuildSelectClause(sql, "SELECT ", null, null);
            sql.Append(" FROM " + m_Table.Name.ToQuotedString());
            if (m_FilterValue != null)
            {
                sql.Append(" WHERE " + sqlBuilder.ApplyAnonymousFilterValue(m_FilterValue, m_FilterOptions));
                parameters = sqlBuilder.GetParameters();
            }
            else if (!string.IsNullOrWhiteSpace(m_WhereClause))
            {
                sql.Append(" WHERE " + m_WhereClause);
                parameters = SqlBuilder.GetParameters<OleDbParameter>(m_WhereArgumentValue);
                parameters.AddRange(sqlBuilder.GetParameters());
            }
            else
            {
                parameters = sqlBuilder.GetParameters();
            }
            sql.Append(";");

            if (m_Parameters != null)
                parameters.AddRange(m_Parameters.Select(p => p.Clone()));


            return new AccessCommandExecutionToken(DataSource, "Query after updating " + m_Table.Name, sql.ToString(), parameters);
        }


        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <remarks>
        /// If the column name was not found, this will return null
        /// </remarks>
        public override ColumnMetadata TryGetColumn(string columnName) => m_Table.Columns.TryGetColumn(columnName);

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        public override UpdateManyCommandBuilder<OleDbCommand, OleDbParameter> WithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            m_WhereClause = null;
            m_WhereArgumentValue = null;
            m_FilterValue = filterValue;
            m_FilterOptions = filterOptions;
            return this;
        }

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <returns></returns>
        public override UpdateManyCommandBuilder<OleDbCommand, OleDbParameter> WithFilter(string whereClause)
        {
            m_WhereClause = whereClause;
            m_WhereArgumentValue = null;
            m_FilterValue = null;
            m_FilterOptions = FilterOptions.None;
            return this;
        }

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        public override UpdateManyCommandBuilder<OleDbCommand, OleDbParameter> WithFilter(string whereClause, object argumentValue)
        {
            m_WhereClause = whereClause;
            m_WhereArgumentValue = argumentValue;
            m_FilterValue = null;
            m_FilterOptions = FilterOptions.None;
            return this;
        }


        /// <summary>
        /// Applies this command to all rows.
        /// </summary>
        /// <returns></returns>
        public override UpdateManyCommandBuilder<OleDbCommand, OleDbParameter> All()
        {
            m_WhereClause = null;
            m_WhereArgumentValue = null;
            m_FilterValue = null;
            m_FilterOptions = FilterOptions.None;
            return this;
        }

    }
}
