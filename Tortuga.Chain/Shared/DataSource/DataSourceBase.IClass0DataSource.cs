/*
 * using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;

#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain.SqlServer
{
    partial class SqlServerDataSourceBase : ISupportsSqlQueries

#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
    partial class OleDbSqlServerDataSourceBase : ISupportsSqlQueries

#elif SQLITE

namespace Tortuga.Chain.SQLite
{
    partial class SQLiteDataSourceBase : ISupportsSqlQueries
#elif MYSQL

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase : ISupportsSqlQueries

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
    partial class PostgreSqlDataSourceBase : ISupportsSqlQueries

#elif ACCESS

namespace Tortuga.Chain.Access
{
	partial class AccessDataSourceBase : ISupportsSqlQueries

#endif
	{
		IMultipleTableDbCommandBuilder ISupportsSqlQueries.Sql(string sqlStatement, object argumentValue)
		{
			return OnSql(sqlStatement, argumentValue);
		}
	}
}
*/