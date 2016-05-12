using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using NpgsqlTypes;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
    /// <summary>
    /// Class PostgreSqlInsertOrUpdateObject
    /// </summary>
    internal sealed class PostgreSqlInsertOrUpdateObject<TArgument> : PostgreSqlObjectCommand<TArgument>
        where TArgument : class
    {
        readonly UpsertOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlInsertOrUpdateObject"/> class.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        public PostgreSqlInsertOrUpdateObject(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, TArgument argumentValue, UpsertOptions options)
            : base(dataSource, tableName, argumentValue)
        {
            m_Options = options;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns><see cref="PostgreSqlExecutionToken" /></returns>
        public override CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            foreach (ColumnMetadata<NpgsqlDbType> item in Table.Columns)
            {

            }
            var primaryKeyNames = Table.Columns.Where(x => x.IsPrimaryKey).Select(x => x.QuotedSqlName);
            string conflictNames = string.Join(", ", primaryKeyNames);

            var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            List<NpgsqlParameter> keyParameters;
            var isPrimaryKeyIdentity = sqlBuilder.PrimaryKeyisIdentity(out keyParameters);
            if(isPrimaryKeyIdentity)
            {
                var areKeysNull = keyParameters.Any(c => c.Value == DBNull.Value || c.Value == null) ? true : false;
                if (areKeysNull)
                    sqlBuilder.BuildInsertStatement(sql, Table.Name.ToString(), null);
                else
                    sqlBuilder.BuildUpdateByKeyStatement(sql, Table.Name.ToString(), null);
                sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, ";");
            }
            else
            {
                sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {Table.Name.ToString()} (", null, ")");
                sqlBuilder.BuildValuesClause(sql, " VALUES (", ")");
                sqlBuilder.BuildSetClause(sql, $" ON CONFLICT ({conflictNames}) DO UPDATE SET ", null, null);
                sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, ";");
            }

            //Looks like ON CONFLICT is useful here http://www.postgresql.org/docs/current/static/sql-insert.html
            //Use RETURNING in place of SQL Servers OUTPUT clause http://www.postgresql.org/docs/current/static/sql-insert.html

            return new PostgreSqlExecutionToken(DataSource, "Insert or update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters());
        }
    }
}
