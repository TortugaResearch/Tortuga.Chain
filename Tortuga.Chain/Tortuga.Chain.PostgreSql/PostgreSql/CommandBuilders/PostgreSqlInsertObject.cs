using Npgsql;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
	/// <summary>
	/// Class that represents a PostgreSql Insert.
	/// </summary>
	internal sealed class PostgreSqlInsertObject<TArgument> : PostgreSqlObjectCommand<TArgument>
		where TArgument : class
	{
		readonly InsertOptions m_Options;

		/// <summary>
		/// Initializes a new instance of the <see cref="PostgreSqlInsertObject{TArgument}"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The options.</param>
		public PostgreSqlInsertObject(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, TArgument argumentValue, InsertOptions options)
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

			var identityInsert = m_Options.HasFlag(InsertOptions.IdentityInsert);
			if (identityInsert)
				throw new NotImplementedException("See issue 256. https://github.com/TortugaResearch/Tortuga.Chain/issues/256");

			var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
			sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
			sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

			if (KeyColumns.Count > 0)
				sqlBuilder.OverrideKeys(KeyColumns);

			var sql = new StringBuilder();
			sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {Table.Name.ToString()} (", null, ")", identityInsert);
			sqlBuilder.BuildValuesClause(sql, " VALUES (", ")", identityInsert);
			sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, ";");

			return new PostgreSqlCommandExecutionToken(DataSource, "Insert into " + Table.Name, sql.ToString(), sqlBuilder.GetParameters());
		}
	}
}
