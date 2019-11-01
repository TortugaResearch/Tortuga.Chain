using MySql.Data.MySqlClient;
using System;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.MySql.CommandBuilders
{
    /// <summary>
    /// Command object that represents a delete operation.
    /// </summary>
    internal sealed class MySqlDeleteObject<TArgument> : MySqlObjectCommand<TArgument>
        where TArgument : class
    {
        readonly DeleteOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDeleteObject{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="table">The table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public MySqlDeleteObject(MySqlDataSourceBase dataSource, MySqlObjectName table, TArgument argumentValue, DeleteOptions options)
            : base(dataSource, table, argumentValue)
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

            if (KeyColumns.Count > 0)
                sqlBuilder.OverrideKeys(KeyColumns);

            var sql = new StringBuilder();
            sqlBuilder.BuildSelectByKeyStatement(sql, Table.Name.ToQuotedString(), ";");
            sql.AppendLine();
            sqlBuilder.BuildDeleteStatement(sql, Table.Name.ToQuotedString(), ";");

            return new MySqlCommandExecutionToken(DataSource, "Delete from " + Table.Name, sql.ToString(), sqlBuilder.GetParameters());
        }
    }
}
