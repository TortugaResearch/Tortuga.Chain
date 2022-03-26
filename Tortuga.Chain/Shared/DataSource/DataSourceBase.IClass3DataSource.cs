using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;

#if MYSQL

using System;

#endif

#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain.SqlServer
{
	partial class SqlServerDataSourceBase : ISupportsProcedure, ISupportsScalarFunction, ISupportsTableFunction

#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
    partial class OleDbSqlServerDataSourceBase : ISupportsProcedure, ISupportsScalarFunction, ISupportsTableFunction

#elif MYSQL

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase : ISupportsProcedure, ISupportsScalarFunction, ISupportsTableFunction

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
	partial class PostgreSqlDataSourceBase : ISupportsProcedure, ISupportsScalarFunction, ISupportsTableFunction

#endif
	{
		IProcedureDbCommandBuilder ISupportsProcedure.Procedure(string procedureName)
		{
			return Procedure(procedureName);
		}

		IProcedureDbCommandBuilder ISupportsProcedure.Procedure(string procedureName, object argumentValue)
		{
			return Procedure(procedureName, argumentValue);
		}

		IScalarDbCommandBuilder ISupportsScalarFunction.ScalarFunction(string scalarFunctionName)
		{
			return ScalarFunction(scalarFunctionName);
		}

		IScalarDbCommandBuilder ISupportsScalarFunction.ScalarFunction(string scalarFunctionName, object functionArgumentValue)
		{
			return ScalarFunction(scalarFunctionName, functionArgumentValue);
		}

		ITableDbCommandBuilder ISupportsTableFunction.TableFunction(string functionName)
		{
#if MYSQL
			throw new NotSupportedException("MySQL does not support table-valued functions.");
#else
			return TableFunction(functionName);
#endif
		}

		ITableDbCommandBuilder ISupportsTableFunction.TableFunction(string functionName, object functionArgumentValue)
		{
#if MYSQL
			throw new NotSupportedException("MySQL does not support table-valued functions.");
#else
			return TableFunction(functionName, functionArgumentValue);
#endif
		}
	}
}

