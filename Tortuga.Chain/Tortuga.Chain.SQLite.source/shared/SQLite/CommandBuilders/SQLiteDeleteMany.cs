using System;
using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using System.Text;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#endif


namespace Tortuga.Chain.SQLite.CommandBuilders
{
    /// <summary>
    /// Class SQLiteDeleteMany.
    /// </summary>
    internal sealed class SQLiteDeleteMany : MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter>
    {
        //readonly DeleteOptions m_Options;
        readonly IEnumerable<SQLiteParameter> m_Parameters;
        readonly TableOrViewMetadata<SQLiteObjectName, DbType> m_Table;
        readonly string m_WhereClause;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDeleteMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        public SQLiteDeleteMany(SQLiteDataSourceBase dataSource, SQLiteObjectName tableName, string whereClause, IEnumerable<SQLiteParameter> parameters, DeleteOptions options) : base(dataSource)
        {
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                throw new NotSupportedException("Cannot use Key attributes with this operation.");

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_WhereClause = whereClause;
            //m_Options = options;
            m_Parameters = parameters;
        }

        public override CommandExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();

            if (sqlBuilder.HasReadFields)
            {
                sqlBuilder.BuildSelectClause(sql, "SELECT ", null, " FROM " + m_Table.Name.ToQuotedString());
                sql.AppendLine(" WHERE " + m_WhereClause + ";");
            }

            sql.Append("DELETE FROM " + m_Table.Name.ToQuotedString());
            sql.AppendLine(" WHERE " + m_WhereClause + ";");

            var parameters = sqlBuilder.GetParameters();
            if (m_Parameters != null)
                parameters.AddRange(m_Parameters);

            return new SQLiteCommandExecutionToken(DataSource, "Delete from " + m_Table.Name, sql.ToString(), parameters, lockType: LockType.Write);
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

