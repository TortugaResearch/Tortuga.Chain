using Npgsql;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

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
		/// Initializes a new instance of the <see cref="PostgreSqlInsertOrUpdateObject{TArgument}"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The options.</param>
		public PostgreSqlInsertOrUpdateObject(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, TArgument argumentValue, UpsertOptions options)
			: base(dataSource, tableName, argumentValue)
		{
			m_Options = options;
		}

		/// <summary>
		/// Prepares the command for execution by generating any necessary SQL.
		/// </summary>
		/// <param name="materializer"></param>
		/// <returns><see cref="PostgreSqlCommandExecutionToken" /></returns>
		public override CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
		{
			if (materializer == null)
				throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

			var identityInsert = m_Options.HasFlag(UpsertOptions.IdentityInsert);
			//var primaryKeyNames = Table.PrimaryKeyColumns.Select(x => x.QuotedSqlName);

			var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
			sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
			sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

			if (KeyColumns.Count > 0)
			{
				sqlBuilder.OverrideKeys(KeyColumns);
			}

			var sql = new StringBuilder();
			List<NpgsqlParameter> keyParameters;
			var isPrimaryKeyIdentity = sqlBuilder.PrimaryKeyIsIdentity(out keyParameters);
			if (isPrimaryKeyIdentity && KeyColumns.Count == 0)
			{
				var areKeysNull = keyParameters.Any(c => c.Value == DBNull.Value || c.Value == null) ? true : false;
				if (areKeysNull)
				{
					sqlBuilder.BuildInsertStatement(sql, Table.Name.ToString(), null);
					sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, ";");
				}
				else if (identityInsert)
				{
					string conflictNames = string.Join(", ", sqlBuilder.GetKeyColumns().Select(x => x.QuotedSqlName));

					sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {Table.Name.ToString()} (", null, ")", identityInsert);
					sql.Append(" OVERRIDING SYSTEM VALUE"); 
					sqlBuilder.BuildValuesClause(sql, " VALUES (", ")", identityInsert);
					sqlBuilder.BuildSetClause(sql, $" ON CONFLICT ({conflictNames}) DO UPDATE SET ", null, null);
					sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, ";");

					//var identityColumn = Table.Columns.Single(c => c.IsIdentity);

					//sql.AppendLine("DO RETURNS RECORD $$");
					//sql.AppendLine("DECLARE resultset RECORD;");
					//sql.AppendLine("BEGIN");


					//sql.AppendLine($"IF NOT EXISTS (SELECT * FROM {Table.Name.ToQuotedString()} WHERE {identityColumn.QuotedSqlName} = {identityColumn.SqlVariableName}) THEN");

					//sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {Table.Name.ToString()} (", null, ")", identityInsert);
					//sql.Append(" OVERRIDING SYSTEM VALUE");
					//sqlBuilder.BuildValuesClause(sql, " VALUES (", ")", identityInsert);
					//sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, " INTO resultset");
					//sql.AppendLine(";");
					//sql.AppendLine("RETURN resultset;");

					//sql.AppendLine("ELSE");

					//sqlBuilder.BuildUpdateByKeyStatement(sql, Table.Name.ToString(), null);
					//sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, " INTO resultset");
					//sql.AppendLine(";");
					//sql.AppendLine("RETURN resultset;");

					//sql.AppendLine("END IF;");

					//sql.AppendLine("END $$;");

					//sql = sql.Replace("@", "");
				}
				else
				{
					sqlBuilder.BuildUpdateByKeyStatement(sql, Table.Name.ToString(), null);
					sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, ";");
				}
			}
			else
			{
				string conflictNames = string.Join(", ", sqlBuilder.GetKeyColumns().Select(x => x.QuotedSqlName));

				sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {Table.Name.ToString()} (", null, ")");
				sqlBuilder.BuildValuesClause(sql, " VALUES (", ")");
				sqlBuilder.BuildSetClause(sql, $" ON CONFLICT ({conflictNames}) DO UPDATE SET ", null, null);
				sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, ";");
			}

			var temp = sql.ToString();

			return new PostgreSqlCommandExecutionToken(DataSource, "Insert or update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters());
		}
	}
}
