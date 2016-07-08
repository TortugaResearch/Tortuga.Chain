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
    /// Class AccessUpdateMany.
    /// </summary>
    internal sealed class AccessUpdateMany : MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter>
    {
        readonly int? m_ExpectedRowCount;
        readonly object m_NewValues;
        readonly UpdateOptions m_Options;
        readonly IEnumerable<OleDbParameter> m_Parameters;
        readonly TableOrViewMetadata<AccessObjectName, OleDbType> m_Table;
        readonly string m_WhereClause;

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
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns><see cref="AccessCommandExecutionToken" /></returns>
        public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, m_NewValues, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder($"UPDATE {m_Table.Name.ToQuotedString()}");
            sqlBuilder.BuildSetClause(sql, " SET ", null, null);
            sql.Append(" WHERE " + m_WhereClause);
            sql.Append(";");

            var parameters = sqlBuilder.GetParameters();
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


        private AccessCommandExecutionToken PrepareRead(IReadOnlyList<string> desiredColumns)
        {
            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyDesiredColumns(desiredColumns);

            var sql = new StringBuilder();
            sqlBuilder.BuildSelectClause(sql, "SELECT ", null, null);
            sql.Append(" FROM " + m_Table.Name.ToQuotedString());
            sql.AppendLine(" WHERE " + m_WhereClause + ";");

            var parameters = sqlBuilder.GetParameters();
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
        public override ColumnMetadata TryGetColumn(string columnName)
        {
            return m_Table.Columns.TryGetColumn(columnName);
        }

    }
}
