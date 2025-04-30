using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources;

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
