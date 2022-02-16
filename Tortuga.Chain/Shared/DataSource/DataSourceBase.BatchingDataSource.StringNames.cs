
#if SQL_SERVER_SDS

using AbstractCommand = System.Data.SqlClient.SqlCommand;
using AbstractParameter = System.Data.SqlClient.SqlParameter;
using AbstractObjectName = Tortuga.Chain.SqlServer.SqlServerObjectName;
using AbstractLimitOption = Tortuga.Chain.SqlServerLimitOption;
using InsertBatchResult = Tortuga.Chain.CommandBuilders.MultipleRowDbCommandBuilder<System.Data.SqlClient.SqlCommand, System.Data.SqlClient.SqlParameter>;

#elif SQL_SERVER_MDS

using AbstractCommand = Microsoft.Data.SqlClient.SqlCommand;
using AbstractParameter = Microsoft.Data.SqlClient.SqlParameter;
using AbstractObjectName = Tortuga.Chain.SqlServer.SqlServerObjectName;
using AbstractLimitOption = Tortuga.Chain.SqlServerLimitOption;
using InsertBatchResult = Tortuga.Chain.CommandBuilders.MultipleRowDbCommandBuilder<Microsoft.Data.SqlClient.SqlCommand, Microsoft.Data.SqlClient.SqlParameter>;

#elif SQL_SERVER_OLEDB

using AbstractCommand = System.Data.OleDb.OleDbCommand;
using AbstractLimitOption = Tortuga.Chain.SqlServerLimitOption;
using AbstractObjectName = Tortuga.Chain.SqlServer.SqlServerObjectName;
using AbstractParameter = System.Data.OleDb.OleDbParameter;

#elif SQLITE

using AbstractCommand = System.Data.SQLite.SQLiteCommand;
using AbstractParameter = System.Data.SQLite.SQLiteParameter;
using AbstractObjectName = Tortuga.Chain.SQLite.SQLiteObjectName;
using AbstractLimitOption = Tortuga.Chain.SQLiteLimitOption;
using InsertBatchResult = Tortuga.Chain.CommandBuilders.DbCommandBuilder<System.Data.SQLite.SQLiteCommand, System.Data.SQLite.SQLiteParameter>;

#elif MYSQL

using AbstractCommand = MySqlConnector.MySqlCommand;
using AbstractLimitOption = Tortuga.Chain.MySqlLimitOption;
using AbstractObjectName = Tortuga.Chain.MySql.MySqlObjectName;
using AbstractParameter = MySqlConnector.MySqlParameter;

#elif POSTGRESQL

using AbstractCommand = Npgsql.NpgsqlCommand;
using AbstractParameter = Npgsql.NpgsqlParameter;
using AbstractObjectName = Tortuga.Chain.PostgreSql.PostgreSqlObjectName;
using AbstractLimitOption = Tortuga.Chain.PostgreSqlLimitOption;
using InsertBatchResult = Tortuga.Chain.CommandBuilders.MultipleRowDbCommandBuilder<Npgsql.NpgsqlCommand, Npgsql.NpgsqlParameter>;

#elif ACCESS

using AbstractCommand = System.Data.OleDb.OleDbCommand;
using AbstractLimitOption = Tortuga.Chain.AccessLimitOption;
using AbstractObjectName = Tortuga.Chain.Access.AccessObjectName;
using AbstractParameter = System.Data.OleDb.OleDbParameter;

#endif

#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain.SqlServer
{
    partial class SqlServerDataSourceBase
    {

#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
    partial class OleDbSqlServerDataSourceBase
    {

#elif SQLITE

namespace Tortuga.Chain.SQLite
{
    partial class SQLiteDataSourceBase
    {

#elif MYSQL

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase
	{

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
    partial class PostgreSqlDataSourceBase
    {

#elif ACCESS

namespace Tortuga.Chain.Access
{
    partial class AccessDataSourceBase
    {

#endif

#if !SQL_SERVER_OLEDB && !ACCESS && !MYSQL

        /// <summary>
        /// Inserts the batch of records as one operation..
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="objects">The objects.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public InsertBatchResult InsertBatch<TObject>(string tableName, IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None)
        where TObject : class
        {
            return InsertBatch<TObject>(new AbstractObjectName(tableName), objects, options);
        }

#endif
	}
}
