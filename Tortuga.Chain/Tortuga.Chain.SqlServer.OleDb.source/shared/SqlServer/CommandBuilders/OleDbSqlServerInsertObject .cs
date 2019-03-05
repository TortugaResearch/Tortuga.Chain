using System;
using System.Data.OleDb;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class OleDbSqlServerInsertObject.
    /// </summary>
    internal sealed class OleDbSqlServerInsertObject<TArgument> : OleDbSqlServerObjectCommand<TArgument>
        where TArgument : class
    {
        readonly InsertOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbSqlServerInsertObject{TArgument}" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public OleDbSqlServerInsertObject(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName tableName, TArgument argumentValue, InsertOptions options)
            : base(dataSource, tableName, argumentValue)
        {
            m_Options = options;
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

            var sql = new StringBuilder();
            string header;
            string intoClause;
            string footer;

            sqlBuilder.UseTableVariable(Table, out header, out intoClause, out footer);
            sql.Append(header);

            bool identityInsert = m_Options.HasFlag(InsertOptions.IdentityInsert);
            if (identityInsert)
                sql.AppendLine($"SET IDENTITY_INSERT {Table.Name.ToQuotedString()} ON;");

            sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {Table.Name.ToQuotedString()} (", null, ")", identityInsert);
            sqlBuilder.BuildSelectClause(sql, " OUTPUT ", "Inserted.", intoClause);
            sqlBuilder.BuildAnonymousValuesClause(sql, " VALUES (", ")", identityInsert);
            sql.Append(";");

            sql.Append(footer);

            if (identityInsert)
                sql.AppendLine($"SET IDENTITY_INSERT {Table.Name.ToQuotedString()} OFF;");

            return new OleDbCommandExecutionToken(DataSource, "Insert into " + Table.Name, sql.ToString(), sqlBuilder.GetParameters());
        }
    }
}