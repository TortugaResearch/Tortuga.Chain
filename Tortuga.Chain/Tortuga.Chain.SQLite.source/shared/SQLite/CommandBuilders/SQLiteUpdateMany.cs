using System;
using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using System.Text;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Core;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#endif

namespace Tortuga.Chain.SQLite.CommandBuilders
{
    /// <summary>
    /// Class SQLiteUpdateMany.
    /// </summary>
    internal sealed class SQLiteUpdateMany : MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter>
    {
        readonly int? m_ExpectedRowCount;
        readonly object m_NewValues;
        readonly UpdateOptions m_Options;
        readonly IEnumerable<SQLiteParameter> m_Parameters;
        readonly TableOrViewMetadata<SQLiteObjectName, DbType> m_Table;
        readonly string m_WhereClause;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteUpdateMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedRowCount">The expected row count.</param>
        /// <param name="options">The options.</param>
        public SQLiteUpdateMany(SQLiteDataSourceBase dataSource, SQLiteObjectName tableName, object newValues, string whereClause, IEnumerable<SQLiteParameter> parameters, int? expectedRowCount, UpdateOptions options) : base(dataSource)
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
        /// <returns><see cref="SQLiteCommandExecutionToken" /></returns>
        public override CommandExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, m_NewValues, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            if (sqlBuilder.HasReadFields && m_Options.HasFlag(UpdateOptions.ReturnOldValues))
            {
                sqlBuilder.BuildSelectClause(sql, "SELECT ", null, " FROM " + m_Table.Name.ToQuotedString());
                sql.AppendLine(" WHERE " + m_WhereClause + ";");
            }

            sql.Append($"UPDATE {m_Table.Name.ToQuotedString()}");
            sqlBuilder.BuildSetClause(sql, " SET ", null, null);
            sql.Append(" WHERE " + m_WhereClause);
            sql.AppendLine(";");


            if (sqlBuilder.HasReadFields && !m_Options.HasFlag(UpdateOptions.ReturnOldValues))
            {
                sqlBuilder.BuildSelectClause(sql, "SELECT ", null, " FROM " + m_Table.Name.ToQuotedString());
                sql.AppendLine(" WHERE " + m_WhereClause + ";");
            }

            var parameters = sqlBuilder.GetParameters();
            if (m_Parameters != null)
                parameters.AddRange(m_Parameters);

            return new SQLiteCommandExecutionToken(DataSource, "Update " + m_Table.Name, sql.ToString(), parameters, lockType: LockType.Write).CheckUpdateRowCount(m_Options, m_ExpectedRowCount);
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


