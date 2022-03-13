using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;


#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain.SqlServer
{
	partial class SqlServerDataSourceBase : IAdvancedCrudDataSource
#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
    partial class OleDbSqlServerDataSourceBase : IAdvancedCrudDataSource

#elif SQLITE

namespace Tortuga.Chain.SQLite
{
	partial class SQLiteDataSourceBase : IAdvancedCrudDataSource

#elif MYSQL

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase : IAdvancedCrudDataSource

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
    partial class PostgreSqlDataSourceBase : IAdvancedCrudDataSource

#endif
	{

		IObjectDbCommandBuilder<TArgument> ISupportsUpsert.Upsert<TArgument>(string tableName, TArgument argumentValue, UpsertOptions options)
		{
			return Upsert(tableName, argumentValue, options);
		}

		IObjectDbCommandBuilder<TArgument> ISupportsUpsert.Upsert<TArgument>(TArgument argumentValue, UpsertOptions options)
		{
			return Upsert(argumentValue, options);
		}

		//ILink<int?> IClass2DataSource.Truncate(string tableName)
		//{
		//	return Truncate(tableName);
		//}

		//ILink<int?> IClass2DataSource.Truncate<TObject>() where TObject : class
		//{
		//	return Truncate<TObject>();
		//}
	}
}
