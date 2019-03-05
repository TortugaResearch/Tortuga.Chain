using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.MySql.CommandBuilders
{
    /// <summary>
    /// Class that represents a MySql Insert.
    /// </summary>
    internal sealed class MySqlInsertObject<TArgument> : MySqlObjectCommand<TArgument>
        where TArgument : class
    {
        readonly InsertOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlInsertObject{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public MySqlInsertObject(MySqlDataSourceBase dataSource, MySqlObjectName tableName, TArgument argumentValue, InsertOptions options)
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

            var identityInsert = m_Options.HasFlag(InsertOptions.IdentityInsert);

            var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {Table.Name.ToString()} (", null, ")", identityInsert);
            sqlBuilder.BuildValuesClause(sql, " VALUES (", ");", identityInsert);

            if (sqlBuilder.HasReadFields)
            {
                var identityColumn = Table.Columns.Where(c => c.IsIdentity).SingleOrDefault();
                if (!identityInsert && identityColumn != null)
                {
                    sqlBuilder.BuildSelectClause(sql, "SELECT ", null, null);
                    sql.Append(" FROM " + Table.Name.ToQuotedString());
                    sql.Append(" WHERE " + identityColumn.QuotedSqlName + " = LAST_INSERT_ID()");
                    sql.Append(";");
                }
                else
                {
                    var primaryKeys = Table.Columns.Where(c => c.IsPrimaryKey).ToList();
                    if (primaryKeys.Count == 0)
                        throw new MappingException($"Insert operation cannot return any values for { Table.Name} because it doesn't have a primary key.");

                    sqlBuilder.BuildSelectClause(sql, "SELECT ", null, null);
                    sql.Append(" FROM " + Table.Name.ToQuotedString());
                    sqlBuilder.BuildWhereClause(sql, " WHERE ", null);
                    sql.Append(";");
                }
            }

            return new MySqlCommandExecutionToken(DataSource, "Insert into " + Table.Name, sql.ToString(), sqlBuilder.GetParameters());
        }
    }
}