using Tortuga.Chain.CommandBuilders;


#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain.SqlServer
{
	partial class SqlServerDataSourceBase : IClass2DataSource
#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
    partial class OleDbSqlServerDataSourceBase : IClass1DataSource

#elif SQLITE

namespace Tortuga.Chain.SQLite
{
	partial class SQLiteDataSourceBase : IClass2DataSource

#elif MYSQL

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase : IClass1DataSource

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
    partial class PostgreSqlDataSourceBase : IClass1DataSource

#endif
	{

		IObjectDbCommandBuilder<TArgument> IClass2DataSource.Upsert<TArgument>(string tableName, TArgument argumentValue, UpsertOptions options)
		{
			return Upsert(tableName, argumentValue, options);
		}

		IObjectDbCommandBuilder<TArgument> IClass2DataSource.Upsert<TArgument>(TArgument argumentValue, UpsertOptions options)
		{
			return Upsert(argumentValue, options);
		}

		ILink<int?> IClass2DataSource.Truncate(string tableName)
		{
			return Truncate(tableName);
		}

		ILink<int?> IClass2DataSource.Truncate<TObject>() where TObject : class
		{
			return Truncate<TObject>();
		}
	}
}
