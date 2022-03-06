

using Tortuga.Chain.CommandBuilders;

#if SQL_SERVER_SDS || SQL_SERVER_MDS

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
	public ProcedureDbCommandBuilder<AbstractCommand, AbstractParameter> Procedure(AbstractObjectName procedureName)
	{
		return new AbstractProcedureCall(this, procedureName, null);
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
	public ProcedureDbCommandBuilder<AbstractCommand, AbstractParameter> Procedure(AbstractObjectName procedureName, object argumentValue)
	{
		return new AbstractProcedureCall(this, procedureName, argumentValue);
	}

	/// <summary>
	/// This is used to query a scalar function.
	/// </summary>
	/// <param name="scalarFunctionName">Name of the scalar function.</param>
	/// <returns></returns>
	public ScalarDbCommandBuilder<AbstractCommand, AbstractParameter> ScalarFunction(AbstractObjectName scalarFunctionName)
	{
		return new AbstractScalarFunction(this, scalarFunctionName, null);
	}

	/// <summary>
	/// This is used to query a scalar function.
	/// </summary>
	/// <param name="scalarFunctionName">Name of the scalar function.</param>
	/// <param name="functionArgumentValue">The function argument.</param>
	/// <returns></returns>
	public ScalarDbCommandBuilder<AbstractCommand, AbstractParameter> ScalarFunction(AbstractObjectName scalarFunctionName, object functionArgumentValue)
	{
		return new AbstractScalarFunction(this, scalarFunctionName, functionArgumentValue);
	}

#if !MYSQL

	/// <summary>
	/// This is used to query a table valued function.
	/// </summary>
	/// <param name="tableFunctionName">Name of the table function.</param>
	/// <returns></returns>
	public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> TableFunction(AbstractObjectName tableFunctionName)
	{
		return new AbstractTableFunction(this, tableFunctionName, null);
	}

	/// <summary>
	/// This is used to query a table valued function.
	/// </summary>
	/// <param name="tableFunctionName">Name of the table function.</param>
	/// <param name="functionArgumentValue">The function argument.</param>
	/// <returns></returns>
	public TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> TableFunction(AbstractObjectName tableFunctionName, object functionArgumentValue)
	{
		return new AbstractTableFunction(this, tableFunctionName, functionArgumentValue);
	}

#endif
}
}

