using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources;

///// <summary>
///// A class 3 data source that includes stored procedures and functions.
///// </summary>
///// <seealso cref="ICrudDataSource" />
///// <remarks>Warning: This interface is meant to simulate multiple inheritance and work-around some issues with exposing generic types. Do not implement it in client code, as new methods will be added over time.</remarks>
//public interface IClass3DataSource : IAdvancedCrudDataSource, ISupportsProcedure, ISupportsScalarFunction, ISupportsTableFunction
//{
//}

/// <summary>
/// Used to mark data sources that supports stored procedures
/// </summary>
public interface ISupportsProcedure
{
	/// <summary>
	/// Executes the indicated procedure.
	/// </summary>
	/// <param name="procedureName">Name of the procedure.</param>
	/// <returns></returns>
	IProcedureDbCommandBuilder Procedure(string procedureName);

	/// <summary>
	/// Executes the indicated procedure.
	/// </summary>
	/// <param name="procedureName">Name of the procedure.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <returns></returns>
	IProcedureDbCommandBuilder Procedure(string procedureName, object argumentValue);
}

/// <summary>
/// Used to mark data sources that supports scalar functions
/// </summary>
public interface ISupportsScalarFunction
{
	/// <summary>
	/// This is used to query a scalar function.
	/// </summary>
	/// <param name="scalarFunctionName">Name of the scalar function.</param>
	/// <returns></returns>
	IScalarDbCommandBuilder ScalarFunction(string scalarFunctionName);

	/// <summary>
	/// This is used to query a scalar function.
	/// </summary>
	/// <param name="scalarFunctionName">Name of the scalar function.</param>
	/// <param name="functionArgumentValue">The function arguments.</param>
	/// <returns></returns>
	IScalarDbCommandBuilder ScalarFunction(string scalarFunctionName, object functionArgumentValue);
}

/// <summary>
/// Used to mark data sources that supports table-valued functions
/// </summary>
public interface ISupportsTableFunction
{
	/// <summary>
	/// Selects from the indicated table-value function.
	/// </summary>
	/// <param name="functionName">Name of the function.</param>
	/// <returns></returns>
	ITableDbCommandBuilder TableFunction(string functionName);

	/// <summary>
	/// Selects from the indicated table-value function.
	/// </summary>
	/// <param name="functionName">Name of the function.</param>
	/// <param name="functionArgumentValue">The function argument value.</param>
	/// <returns></returns>
	ITableDbCommandBuilder TableFunction(string functionName, object functionArgumentValue);
}
