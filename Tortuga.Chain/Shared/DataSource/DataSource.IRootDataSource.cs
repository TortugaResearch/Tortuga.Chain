using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.DataSources;

#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain
{
	partial class SqlServerDataSource : IRootDataSource
	{
#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain
{
	partial class OleDbSqlServerDataSource : IRootDataSource
	{

#elif SQLITE

namespace Tortuga.Chain
{
	partial class SQLiteDataSource : IRootDataSource
	{
#elif MYSQL

namespace Tortuga.Chain
{
	partial class MySqlDataSource : IRootDataSource
	{

#elif POSTGRESQL

namespace Tortuga.Chain
{
	partial class PostgreSqlDataSource : IRootDataSource
	{

#elif ACCESS

namespace Tortuga.Chain
{
	partial class AccessDataSource : IRootDataSource
	{

#endif
		IOpenDataSource IRootDataSource.CreateOpenDataSource() => CreateOpenDataSource();

		IOpenDataSource IRootDataSource.CreateOpenDataSource(DbConnection connection, DbTransaction? transaction)
		{
			return CreateOpenDataSource((AbstractConnection)connection, (AbstractTransaction?)transaction);
		}

		IOpenDataSource IRootDataSource.CreateOpenDataSource(IDbConnection connection, IDbTransaction? transaction)
		{
			return CreateOpenDataSource((AbstractConnection)connection, (AbstractTransaction?)transaction);
		}

		ITransactionalDataSource IRootDataSource.BeginTransaction()
		{
			return BeginTransaction();
		}

		async Task<ITransactionalDataSource> IRootDataSource.BeginTransactionAsync()
		{
			return await BeginTransactionAsync().ConfigureAwait(false);
		}

		[SuppressMessage("Design", "CA1033")]
		DbConnection IRootDataSource.CreateConnection() => CreateConnection();

		[SuppressMessage("Design", "CA1033")]
		async Task<DbConnection> IRootDataSource.CreateConnectionAsync() => await CreateConnectionAsync().ConfigureAwait(false);
	}
}
