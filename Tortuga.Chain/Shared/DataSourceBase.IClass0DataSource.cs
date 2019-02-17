using Tortuga.Chain.CommandBuilders;

#if SQL_SERVER

namespace Tortuga.Chain.SqlServer
{
    partial class SqlServerDataSourceBase : IClass0DataSource
#elif SQLITE

namespace Tortuga.Chain.SQLite
{
    partial class SQLiteDataSourceBase : IClass0DataSource
#elif MYSQL

namespace Tortuga.Chain.MySql
{
    partial class MySqlDataSourceBase : IClass0DataSource

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
    partial class PostgreSqlDataSourceBase : IClass0DataSource

#elif ACCESS

namespace Tortuga.Chain.Access
{
    partial class AccessDataSourceBase : IClass0DataSource

#endif
    {
        IMultipleTableDbCommandBuilder IClass0DataSource.Sql(string sqlStatement, object argumentValue)
        {
            return OnSql(sqlStatement, argumentValue);
        }
    }
}