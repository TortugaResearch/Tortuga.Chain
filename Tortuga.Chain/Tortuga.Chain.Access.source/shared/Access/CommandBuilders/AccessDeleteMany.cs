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
    /// Class AccessDeleteMany.
    /// </summary>
    internal sealed class AccessDeleteMany : MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter>
    {
        //readonly DeleteOptions m_Options;
        readonly IEnumerable<OleDbParameter> m_Parameters;
        readonly TableOrViewMetadata<AccessObjectName, OleDbType> m_Table;
        readonly string m_WhereClause;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDeleteMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        public AccessDeleteMany(AccessDataSourceBase dataSource, AccessObjectName tableName, string whereClause, IEnumerable<OleDbParameter> parameters, DeleteOptions options) : base(dataSource)
        {
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                throw new NotSupportedException("Cannot use Key attributes with this operation.");

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_WhereClause = whereClause;
            //m_Options = options;
            m_Parameters = parameters;
        }

        public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();



            sql.Append("DELETE FROM " + m_Table.Name.ToQuotedString());
            sql.AppendLine(" WHERE " + m_WhereClause + ";");

            var parameters = sqlBuilder.GetParameters();
            if (m_Parameters != null)
                parameters.AddRange(m_Parameters);

            var deleteCommand = new AccessCommandExecutionToken(DataSource, "Delete from " + m_Table.Name, sql.ToString(), parameters);

            var desiredColumns = materializer.DesiredColumns();
            if (desiredColumns == Materializer.NoColumns)
                return deleteCommand;

            var result = PrepareRead(desiredColumns);
            result.NextCommand = deleteCommand;
            return result;

            //var sqlBuilder2 

            //    sqlBuilder.BuildSelectClause(sql, "SELECT ", null, " FROM " + m_Table.Name.ToQuotedString());
            //    sql.AppendLine(" WHERE " + m_WhereClause + ";");

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


            return new AccessCommandExecutionToken(DataSource, "Query after deleting " + m_Table.Name, sql.ToString(), parameters);
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

