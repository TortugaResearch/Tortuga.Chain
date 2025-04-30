using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources;

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
