﻿using Npgsql;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
	/// <summary>
	/// Class PostgreSqlSqlCall
	/// </summary>
	public class PostgreSqlSqlCall : SqlCallCommandBuilder<NpgsqlCommand, NpgsqlParameter>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PostgreSqlSqlCall"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="sqlStatement">The SQL statement.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <exception cref="ArgumentException">sqlStatement is null or empty.;sqlStatement</exception>
		public PostgreSqlSqlCall(PostgreSqlDataSourceBase dataSource, string sqlStatement, object? argumentValue) : base(dataSource, sqlStatement, argumentValue)
		{
			if (string.IsNullOrEmpty(sqlStatement))
				throw new ArgumentException($"{nameof(sqlStatement)} is null or empty.", nameof(sqlStatement));
		}

		/// <summary>
		/// Prepares the command for execution by generating any necessary SQL.
		/// </summary>
		/// <param name="materializer">The materializer.</param>
		/// <returns>
		/// ExecutionToken&lt;TCommand&gt;.
		/// </returns>
		public override CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
		{
			return new PostgreSqlCommandExecutionToken(DataSource, "Raw SQL Call", SqlStatement, SqlBuilder.GetParameters<NpgsqlParameter>(ArgumentValue));
		}
	}
}
