using MySql.Data.MySqlClient;
using System;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.MySql.CommandBuilders
{
    /// <summary>
    /// Command object that represents an update operation.
    /// </summary>
    internal sealed class MySqlUpdateObject<TArgument> : MySqlObjectCommand<TArgument>
        where TArgument : class
    {
        readonly UpdateOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlUpdateObject{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public MySqlUpdateObject(MySqlDataSourceBase dataSource, MySqlObjectName tableName, TArgument argumentValue, UpdateOptions options)
            : base(dataSource, tableName, argumentValue)
        {
            m_Options = options;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns><see cref="MySqlCommandExecutionToken" /></returns>
        public override CommandExecutionToken<MySqlCommand, MySqlParameter> Prepare(Materializer<MySqlCommand, MySqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            if (m_Options.HasFlag(UpdateOptions.ReturnOldValues))
            {
                sqlBuilder.BuildSelectByKeyStatement(sql, Table.Name.ToQuotedString(), ";");
                sql.AppendLine();
            }
            sqlBuilder.BuildUpdateByKeyStatement(sql, Table.Name.ToQuotedString(), ";");
            if (!m_Options.HasFlag(UpdateOptions.ReturnOldValues))
            {
                sql.AppendLine();
                sqlBuilder.BuildSelectByKeyStatement(sql, Table.Name.ToQuotedString(), ";");
            }

            return new MySqlCommandExecutionToken(DataSource, "Update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters()).CheckUpdateRowCount(m_Options);
        }
    }
}
