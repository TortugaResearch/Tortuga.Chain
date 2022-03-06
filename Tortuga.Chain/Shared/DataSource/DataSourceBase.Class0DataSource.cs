using Tortuga.Chain.CommandBuilders;


#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain.SqlServer
{
	partial class SqlServerDataSourceBase

#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
	partial class OleDbSqlServerDataSourceBase

#elif SQLITE

namespace Tortuga.Chain.SQLite
{
	partial class SQLiteDataSourceBase

#elif MYSQL

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
	partial class PostgreSqlDataSourceBase

#elif ACCESS

namespace Tortuga.Chain.Access
{
	partial class AccessDataSourceBase

#endif
	{
		/// <summary>
		/// Creates a operation based on a raw SQL statement.
		/// </summary>
		/// <param name="sqlStatement">The SQL statement.</param>
		/// <returns></returns>
		public MultipleTableDbCommandBuilder<AbstractCommand, AbstractParameter> Sql(string sqlStatement)
		{
			return OnSql(sqlStatement, null);
		}

		/// <summary>
		/// Creates a operation based on a raw SQL statement.
		/// </summary>
		/// <param name="sqlStatement">The SQL statement.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <returns>SqlServerSqlCall.</returns>
		public MultipleTableDbCommandBuilder<AbstractCommand, AbstractParameter> Sql(string sqlStatement, object argumentValue)
		{
			return OnSql(sqlStatement, argumentValue);
		}
	}
}
