#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB || MYSQL || POSTGRESQL

using Tortuga.Chain.CommandBuilders;

#if MYSQL

using System;

#endif

#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain.SqlServer
{
    partial class SqlServerDataSourceBase : IClass2DataSource

#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
    partial class OleDbSqlServerDataSourceBase : IClass2DataSource

#elif MYSQL

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase : IClass2DataSource

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
    partial class PostgreSqlDataSourceBase : IClass2DataSource

#endif
	{
		IProcedureDbCommandBuilder IClass2DataSource.Procedure(string procedureName)
		{
			return Procedure(procedureName);
		}

		IProcedureDbCommandBuilder IClass2DataSource.Procedure(string procedureName, object argumentValue)
		{
			return Procedure(procedureName, argumentValue);
		}

		IScalarDbCommandBuilder IClass2DataSource.ScalarFunction(string scalarFunctionName)
		{
			return ScalarFunction(scalarFunctionName);
		}

		IScalarDbCommandBuilder IClass2DataSource.ScalarFunction(string scalarFunctionName, object functionArgumentValue)
		{
			return ScalarFunction(scalarFunctionName, functionArgumentValue);
		}

		ITableDbCommandBuilder IClass2DataSource.TableFunction(string functionName)
		{
#if MYSQL
			throw new NotSupportedException("MySQL does not support table-valued functions.");
#else
            return TableFunction(functionName);
#endif
		}

		ITableDbCommandBuilder IClass2DataSource.TableFunction(string functionName, object functionArgumentValue)
		{
#if MYSQL
			throw new NotSupportedException("MySQL does not support table-valued functions.");
#else
            return TableFunction(functionName, functionArgumentValue);
#endif
		}
	}
}

#endif
