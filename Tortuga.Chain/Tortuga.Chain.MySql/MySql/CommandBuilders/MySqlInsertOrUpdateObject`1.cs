using MySqlConnector;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.MySql.CommandBuilders
{
	/// <summary>
	/// Class MySqlInsertOrUpdateObject
	/// </summary>
	internal sealed class MySqlInsertOrUpdateObject<TArgument> : MySqlObjectCommand<TArgument>
		where TArgument : class
	{
		private readonly UpsertOptions m_Options;

		/// <summary>
		/// Initializes a new instance of the <see cref="MySqlInsertOrUpdateObject{TArgument}"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The options.</param>
		public MySqlInsertOrUpdateObject(MySqlDataSourceBase dataSource, MySqlObjectName tableName, TArgument argumentValue, UpsertOptions options)
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

			if (KeyColumns.Count > 0)
				sqlBuilder.OverrideKeys(KeyColumns);

			var identityInsert = m_Options.HasFlag(UpsertOptions.IdentityInsert);

			var sql = new StringBuilder();
			List<MySqlParameter> keyParameters;
			var isPrimaryKeyIdentity = sqlBuilder.PrimaryKeyIsIdentity(out keyParameters);
			if (isPrimaryKeyIdentity && !identityInsert)
			{
				var areKeysNull = keyParameters.Any(c => c.Value == DBNull.Value || c.Value == null) ? true : false;
				if (areKeysNull)
					sqlBuilder.BuildInsertStatement(sql, Table.Name.ToString(), null);
				else
					sqlBuilder.BuildUpdateByKeyStatement(sql, Table.Name.ToString(), null);
				sql.Append(";");
			}
			else
			{
				sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {Table.Name.ToString()} (", null, ")", identityInsert);
				sqlBuilder.BuildValuesClause(sql, " VALUES (", ")", identityInsert);
				sqlBuilder.BuildSetClause(sql, $" ON DUPLICATE KEY UPDATE ", null, null);
				sql.Append(";");
			}

			if (sqlBuilder.HasReadFields)
			{
				var keys = sqlBuilder.GetKeyColumns().ToList();
				if (keys.Count != 1)
					throw new NotSupportedException("Cannot return data from a MySQL Upsert unless there is a single primary key.");
				var key = keys[0];

				if (KeyColumns.Count == 0)
					sqlBuilder.BuildSelectClause(sql, "SELECT ", null, $" FROM {Table.Name.ToQuotedString()} WHERE {key.QuotedSqlName} = CASE WHEN {key.SqlVariableName} IS NULL OR {key.SqlVariableName} = 0 THEN LAST_INSERT_ID() ELSE {key.SqlVariableName} END;");
				else
					sqlBuilder.BuildSelectByKeyStatement(sql, Table.Name.ToQuotedString(), ";");
			}

			return new MySqlCommandExecutionToken(DataSource, "Insert or update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters());
		}
	}
}
