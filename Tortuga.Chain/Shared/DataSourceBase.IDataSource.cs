using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

#if SQL_SERVER

namespace Tortuga.Chain.SqlServer
{
    partial class SqlServerDataSourceBase : IDataSource
#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
    partial class OleDbSqlServerDataSourceBase : IDataSource
#elif SQL_SERVER_OLEDB

#elif SQLITE

namespace Tortuga.Chain.SQLite
{
    partial class SQLiteDataSourceBase : IDataSource
#elif MYSQL

namespace Tortuga.Chain.MySql
{
    partial class MySqlDataSourceBase : IDataSource

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
    partial class PostgreSqlDataSourceBase : IDataSource

#elif ACCESS

namespace Tortuga.Chain.Access
{
    partial class AccessDataSourceBase : IDataSource

#endif
    {
        IDatabaseMetadataCache IDataSource.DatabaseMetadata
        {
            get { return DatabaseMetadata; }
        }
    }
}