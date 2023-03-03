using System.Data.SQLite;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SQLite.CommandBuilders
{
	/// <summary>
	/// Class that represents an operation based on a raw SQL statement.
	/// </summary>
	internal sealed class SQLiteSqlCall : SqlCallCommandBuilder<SQLiteCommand, SQLiteParameter>
	{
		readonly LockType m_LockType;

		/// <summary>
		/// Creates a new instance of <see cref="SQLiteSqlCall" />
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="sqlStatement">The SQL statement.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="lockType">Type of the lock.</param>
		/// <exception cref="ArgumentException">SQL statement is null or empty.;sqlStatement</exception>
		public SQLiteSqlCall(SQLiteDataSourceBase dataSource, string sqlStatement, object? argumentValue, LockType lockType) :
			base(dataSource, sqlStatement, argumentValue)
		{
			m_LockType = lockType;
			if (string.IsNullOrEmpty(sqlStatement))
				throw new ArgumentException("SQL statement is null or empty.", nameof(sqlStatement));
		}

		/// <summary>
		/// Prepares the command for execution by generating any necessary SQL.
		/// </summary>
		/// <param name="materializer"></param>
		/// <returns></returns>
		public override CommandExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
		{
			return new SQLiteCommandExecutionToken(DataSource, "Raw SQL call", SqlStatement, SqlBuilder.GetParameters<SQLiteParameter>(ArgumentValue), lockType: m_LockType);
		}
	}
}
