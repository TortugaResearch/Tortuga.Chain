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





		///// <summary>Truncates the specified table.</summary>
		///// <param name="tableName">Name of the table to truncate.</param>
		///// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
		//public partial ILink<int?> Truncate(AbstractObjectName tableName);

		///// <summary>Truncates the specified table.</summary>
		///// <typeparam name="TObject">This class used to determine which table to truncate</typeparam>
		///// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
		//public ILink<int?> Truncate<TObject>() where TObject : class
		//{
		//	return Truncate(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name);
		//}
	}
}
