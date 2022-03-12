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
		/// Creates a operation used to perform an "upsert" operation.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="argumentValue"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Upsert<TArgument>(AbstractObjectName tableName, TArgument argumentValue, UpsertOptions options = UpsertOptions.None)
		where TArgument : class
		{
			return OnInsertOrUpdateObject<TArgument>(tableName, argumentValue, options);
		}

		/// <summary>
		/// Perform an insert or update operation as appropriate.
		/// </summary>
		/// <typeparam name="TArgument"></typeparam>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The options for how the insert/update occurs.</param>
		/// <returns></returns>
		public ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> Upsert<TArgument>(TArgument argumentValue, UpsertOptions options = UpsertOptions.None) where TArgument : class
		{
			return OnInsertOrUpdateObject<TArgument>(DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
		}


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
