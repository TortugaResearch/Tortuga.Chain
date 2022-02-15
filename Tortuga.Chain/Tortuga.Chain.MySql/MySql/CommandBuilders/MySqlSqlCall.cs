using MySqlConnector;
using System.Collections.Immutable;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql.CommandBuilders
{
	/// <summary>
	/// Class MySqlSqlCall
	/// </summary>
	public class MySqlSqlCall : MultipleTableDbCommandBuilder<MySqlCommand, MySqlParameter>
	{
		readonly object? m_ArgumentValue;
		readonly string m_SqlStatement;

		/// <summary>
		/// Initializes a new instance of the <see cref="MySqlSqlCall"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="sqlStatement">The SQL statement.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <exception cref="ArgumentException">sqlStatement is null or empty.;sqlStatement</exception>
		public MySqlSqlCall(MySqlDataSourceBase dataSource, string sqlStatement, object? argumentValue) : base(dataSource)
		{
			if (string.IsNullOrEmpty(sqlStatement))
				throw new ArgumentException($"{nameof(sqlStatement)} is null or empty.", nameof(sqlStatement));

			m_SqlStatement = sqlStatement;
			m_ArgumentValue = argumentValue;
		}

		/// <summary>
		/// Prepares the command for execution by generating any necessary SQL.
		/// </summary>
		/// <param name="materializer">The materializer.</param>
		/// <returns>
		/// ExecutionToken&lt;TCommand&gt;.
		/// </returns>
		public override CommandExecutionToken<MySqlCommand, MySqlParameter> Prepare(Materializer<MySqlCommand, MySqlParameter> materializer)
		{
			return new MySqlCommandExecutionToken(DataSource, "Raw SQL call", m_SqlStatement, SqlBuilder.GetParameters<MySqlParameter>(m_ArgumentValue));
		}

		/// <summary>
		/// Returns the column associated with the column name.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <returns></returns>
		/// <remarks>
		/// If the column name was not found, this will return null
		/// </remarks>
		public override ColumnMetadata? TryGetColumn(string columnName)
		{
			return null;
		}

		/// <summary>
		/// Returns a list of columns known to be non-nullable.
		/// </summary>
		/// <returns>
		/// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
		/// </returns>
		/// <remarks>
		/// This is used by materializers to skip IsNull checks.
		/// </remarks>
		public override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => ImmutableList<ColumnMetadata>.Empty;
	}
}
