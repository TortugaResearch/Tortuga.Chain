using System.Data.SQLite;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SQLite.CommandBuilders
{
	/// <summary>
	/// Class SQLiteInsertOrUpdateObject
	/// </summary>
	internal sealed class SQLiteInsertOrUpdateObject<TArgument> : SQLiteObjectCommand<TArgument>
		where TArgument : class
	{
		readonly UpsertOptions m_Options;

		/// <summary>
		/// Initializes a new instance of the <see cref="SQLiteInsertOrUpdateObject{TArgument}"/> class.
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="tableName"></param>
		/// <param name="argumentValue"></param>
		/// <param name="options"></param>
		public SQLiteInsertOrUpdateObject(SQLiteDataSourceBase dataSource, SQLiteObjectName tableName, TArgument argumentValue, UpsertOptions options)
			: base(dataSource, tableName, argumentValue)
		{
			m_Options = options;
		}

		/// <summary>
		/// Prepares the command for execution by generating any necessary SQL.
		/// </summary>
		/// <param name="materializer"></param>
		/// <returns><see cref="SQLiteCommandExecutionToken" /></returns>
		public override CommandExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
		{
			if (materializer == null)
				throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

			var identityInsert = m_Options.HasFlag(UpsertOptions.IdentityInsert);

			var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
			sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
			sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

			if (KeyColumns.Count > 0)
				sqlBuilder.OverrideKeys(KeyColumns);

			var sql = new StringBuilder();
			sqlBuilder.BuildUpdateByKeyStatement(sql, Table.Name.ToQuotedString(), ";");
			sql.AppendLine();

			sqlBuilder.BuildInsertClause(sql, $"INSERT OR IGNORE INTO {Table.Name.ToQuotedString()} (", null, ")", identityInsert);
			sqlBuilder.BuildValuesClause(sql, " VALUES (", ");", identityInsert);
			sql.AppendLine();

			if (sqlBuilder.HasReadFields)
			{
				var keys = sqlBuilder.GetKeyColumns().ToList();
				if (keys.Count != 1)
					throw new NotSupportedException("Cannot return data from a SQLite Upsert unless there is a single primary key.");
				var key = keys[0];

				sqlBuilder.BuildSelectClause(sql, "SELECT ", null, $" FROM {Table.Name.ToQuotedString()} WHERE {key.QuotedSqlName} = CASE WHEN {key.SqlVariableName} IS NULL OR {key.SqlVariableName} = 0 THEN last_insert_rowid() ELSE {key.SqlVariableName} END;");
			}

			return new SQLiteCommandExecutionToken(DataSource, "Insert or update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters(), lockType: LockType.Write);
		}
	}
}
