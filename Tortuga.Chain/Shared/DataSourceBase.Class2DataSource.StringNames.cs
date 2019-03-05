using Tortuga.Chain.CommandBuilders;

#if SQL_SERVER

using AbstractCommand = System.Data.SqlClient.SqlCommand;
using AbstractParameter = System.Data.SqlClient.SqlParameter;
using AbstractObjectName = Tortuga.Chain.SqlServer.SqlServerObjectName;
using AbstractLimitOption = Tortuga.Chain.SqlServerLimitOption;

#elif SQL_SERVER_OLEDB

using AbstractCommand = System.Data.OleDb.OleDbCommand;
using AbstractLimitOption = Tortuga.Chain.SqlServerLimitOption;
using AbstractObjectName = Tortuga.Chain.SqlServer.SqlServerObjectName;
using AbstractParameter = System.Data.OleDb.OleDbParameter;

#elif MYSQL

using AbstractCommand = MySql.Data.MySqlClient.MySqlCommand;
using AbstractParameter = MySql.Data.MySqlClient.MySqlParameter;
using AbstractObjectName = Tortuga.Chain.MySql.MySqlObjectName;
using AbstractLimitOption = Tortuga.Chain.MySqlLimitOption;

#elif POSTGRESQL

using AbstractCommand = Npgsql.NpgsqlCommand;
using AbstractParameter = Npgsql.NpgsqlParameter;
using AbstractObjectName = Tortuga.Chain.PostgreSql.PostgreSqlObjectName;
using AbstractLimitOption = Tortuga.Chain.PostgreSqlLimitOption;

#endif

#if SQL_SERVER

namespace Tortuga.Chain.SqlServer
{
    partial class SqlServerDataSourceBase
#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
    partial class OleDbSqlServerDataSourceBase

#elif MYSQL

namespace Tortuga.Chain.MySql
{
    partial class MySqlDataSourceBase

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
    partial class PostgreSqlDataSourceBase

#endif
    {
        /// <summary>
        /// Loads a procedure definition
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        public MultipleTableDbCommandBuilder<AbstractCommand, AbstractParameter> Procedure(string procedureName)
        {
            return Procedure(new AbstractObjectName(procedureName));
        }

        /// <summary>
        /// Loads a procedure definition and populates it using the parameter object.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        /// <remarks>
        /// The procedure's definition is loaded from the database and used to determine which properties on the parameter object to use.
        /// </remarks>
        public MultipleTableDbCommandBuilder<AbstractCommand, AbstractParameter> Procedure(string procedureName, object argumentValue)
        {
            return Procedure(new AbstractObjectName(procedureName), argumentValue);
        }

        /// <summary>
        /// This is used to query a scalar function.
        /// </summary>
        /// <param name="scalarFunctionName">Name of the scalar function.</param>
        /// <returns></returns>
        public ScalarDbCommandBuilder<AbstractCommand, AbstractParameter> ScalarFunction(string scalarFunctionName)
        {
            return ScalarFunction(new AbstractObjectName(scalarFunctionName));
        }

        /// <summary>
        /// This is used to query a scalar function.
        /// </summary>
        /// <param name="scalarFunctionName">Name of the scalar function.</param>
        /// <param name="functionArgumentValue">The function argument.</param>
        /// <returns></returns>
        public ScalarDbCommandBuilder<AbstractCommand, AbstractParameter> ScalarFunction(string scalarFunctionName, object functionArgumentValue)
        {
            return ScalarFunction(new AbstractObjectName(scalarFunctionName), functionArgumentValue);
        }

#if !MYSQL

        /// <summary>
        /// This is used to query a table valued function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> TableFunction(string tableFunctionName)
        {
            return TableFunction(new AbstractObjectName(tableFunctionName));
        }

        /// <summary>
        /// This is used to query a table valued function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <param name="functionArgumentValue">The function argument.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> TableFunction(string tableFunctionName, object functionArgumentValue)
        {
            return TableFunction(new AbstractObjectName(tableFunctionName), functionArgumentValue);
        }

#endif
    }
}