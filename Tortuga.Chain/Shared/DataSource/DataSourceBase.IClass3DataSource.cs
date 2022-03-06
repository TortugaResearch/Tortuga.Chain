
using Tortuga.Chain.CommandBuilders;

#if MYSQL

using System;

#endif

#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain.SqlServer
{
    partial class SqlServerDataSourceBase : IClass3DataSource

#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
    partial class OleDbSqlServerDataSourceBase : IClass3DataSource

#elif MYSQL

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase : IClass3DataSource

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
    partial class PostgreSqlDataSourceBase : IClass3DataSource

#endif
	{
		IProcedureDbCommandBuilder IClass3DataSource.Procedure(string procedureName)
		{
			return Procedure(procedureName);
		}

		IProcedureDbCommandBuilder IClass3DataSource.Procedure(string procedureName, object argumentValue)
		{
			return Procedure(procedureName, argumentValue);
		}

		IScalarDbCommandBuilder IClass3DataSource.ScalarFunction(string scalarFunctionName)
		{
			return ScalarFunction(scalarFunctionName);
		}

		IScalarDbCommandBuilder IClass3DataSource.ScalarFunction(string scalarFunctionName, object functionArgumentValue)
		{
			return ScalarFunction(scalarFunctionName, functionArgumentValue);
		}

		ITableDbCommandBuilder IClass3DataSource.TableFunction(string functionName)
		{
#if MYSQL
			throw new NotSupportedException("MySQL does not support table-valued functions.");
#else
            return TableFunction(functionName);
#endif
		}

		ITableDbCommandBuilder IClass3DataSource.TableFunction(string functionName, object functionArgumentValue)
		{
#if MYSQL
			throw new NotSupportedException("MySQL does not support table-valued functions.");
#else
            return TableFunction(functionName, functionArgumentValue);
#endif
		}
	}
}

