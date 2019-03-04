using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class OleDbSqlServerInsertOrUpdateObject.
    /// </summary>
    internal sealed class OleDbSqlServerInsertOrUpdateObject<TArgument> : UpsertDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument>
        where TArgument : class
    {
        readonly UpsertOptions m_Options;
        ImmutableHashSet<string> m_KeyColumns = ImmutableHashSet<string>.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbSqlServerInsertOrUpdateObject{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public OleDbSqlServerInsertOrUpdateObject(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName tableName, TArgument argumentValue, UpsertOptions options) : base(dataSource, argumentValue)
        {
            Table = DataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_Options = options;
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public new OleDbSqlServerDataSourceBase DataSource
        {
            get { return (OleDbSqlServerDataSourceBase)base.DataSource; }
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public SqlServerTableOrViewMetadata<OleDbType> Table { get; }

        /// <summary>
        /// Matches the on an alternate column(s). Normally matches need to be on the primary key.
        /// </summary>
        /// <param name="columnNames">The column names that form a unique key.</param>
        /// <returns></returns>
        public override UpsertDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument> MatchOn(params string[] columnNames)
        {
            //normalize the column names.
            m_KeyColumns = columnNames.Select(c => Table.Columns[c].SqlName).ToImmutableHashSet();
            return this;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
        public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            if (m_KeyColumns.Count > 0)
                sqlBuilder.OverrideKeys(m_KeyColumns);

            var availableColumns = sqlBuilder.GetParameterizedColumns().ToList();

            var sql = new StringBuilder();
            string header;
            string intoClause;
            string footer;

            sqlBuilder.UseTableVariable(Table, out header, out intoClause, out footer);

            sql.Append(header);
            sql.Append($"MERGE INTO {Table.Name.ToQuotedString()} target USING ");
            sql.Append("(VALUES (" + string.Join(", ", availableColumns.Select(c => "?")) + ")) AS source (" + string.Join(", ", availableColumns.Select(c => c.QuotedSqlName)) + ")");
            sql.Append(" ON ");
            sql.Append(string.Join(" AND ", sqlBuilder.GetKeyColumns().ToList().Select(c => $"target.{c.QuotedSqlName} = source.{c.QuotedSqlName}")));

            sql.Append(" WHEN MATCHED THEN UPDATE SET ");
            sql.Append(string.Join(", ", sqlBuilder.GetUpdateColumns().Select(x => $"{x.QuotedSqlName} = source.{x.QuotedSqlName}")));

            var insertColumns = sqlBuilder.GetInsertColumns();
            sql.Append(" WHEN NOT MATCHED THEN INSERT (");
            sql.Append(string.Join(", ", insertColumns.Select(x => x.QuotedSqlName)));
            sql.Append(") VALUES (");
            sql.Append(string.Join(", ", insertColumns.Select(x => "source." + x.QuotedSqlName)));
            sql.Append(" )");
            sqlBuilder.BuildSelectClause(sql, " OUTPUT ", "Inserted.", intoClause);
            sql.Append(";");
            sql.Append(footer);

            return new OleDbCommandExecutionToken(DataSource, "Insert or update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters());
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
            return Table.Columns.TryGetColumn(columnName);
        }

        /// <summary>
        /// Returns a list of columns known to be non-nullable.
        /// </summary>
        /// <returns>
        /// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
        /// </returns>
        /// <remarks>
        /// This is used by materializers to skip IsNull checks.
        /// </remarks>
        public override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => Table.NonNullableColumns;
    }
}
