using System;
using System.Data.SqlClient;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class SqlServerUpdateObject.
    /// </summary>
    internal sealed class SqlServerUpdateObject<TArgument> : SqlServerObjectCommand<TArgument>
        where TArgument : class
    {
        readonly UpdateOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerUpdateObject{TArgument}" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public SqlServerUpdateObject(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, TArgument argumentValue, UpdateOptions options) : base(dataSource, tableName, argumentValue)
        {
            m_Options = options;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>

        public override CommandExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var prefix = m_Options.HasFlag(UpdateOptions.ReturnOldValues) ? "Deleted." : "Inserted.";

            var sql = new StringBuilder();
            string header;
            string intoClause;
            string footer;

            sqlBuilder.UseTableVariable(Table, out header, out intoClause, out footer);

            sql.Append(header);
            sql.Append($"UPDATE {Table.Name.ToQuotedString()}");
            sqlBuilder.BuildSetClause(sql, " SET ", null, null);
            sqlBuilder.BuildSelectClause(sql, " OUTPUT ", prefix, intoClause);
            sqlBuilder.BuildWhereClause(sql, " WHERE ", null);
            sql.Append(";");
            sql.Append(footer);

            return new SqlServerCommandExecutionToken(DataSource, "Update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters()).CheckUpdateRowCount(m_Options);
        }


    }
}

