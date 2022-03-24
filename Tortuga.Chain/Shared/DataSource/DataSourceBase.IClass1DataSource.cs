using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;

#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain.SqlServer
{
	partial class SqlServerDataSourceBase : ICrudDataSource
#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
	partial class OleDbSqlServerDataSourceBase : ICrudDataSource

#elif SQLITE

namespace Tortuga.Chain.SQLite
{
	partial class SQLiteDataSourceBase : ICrudDataSource

#elif MYSQL

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase : ICrudDataSource

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
	partial class PostgreSqlDataSourceBase : ICrudDataSource

#elif ACCESS

namespace Tortuga.Chain.Access
{
	partial class AccessDataSourceBase : ICrudDataSource

#endif
	{




		ISingleRowDbCommandBuilder ISupportsGetByKey.GetByKey<TKey>(string tableName, TKey key)
		{
			return GetByKey(tableName, key);
		}

		ISingleRowDbCommandBuilder ISupportsGetByKey.GetByKey(string tableName, string key)
		{
			return GetByKey(tableName, key);
		}

		IMultipleRowDbCommandBuilder ISupportsGetByKeyList.GetByKeyList<T>(string tableName, string keyColumn, IEnumerable<T> keys)
		{
			return GetByKeyList(tableName, keyColumn, keys);
		}

		IMultipleRowDbCommandBuilder ISupportsGetByKeyList.GetByKeyList<T>(string tableName, IEnumerable<T> keys)
		{
			return GetByKeyList(tableName, keys);
		}


	}
}
