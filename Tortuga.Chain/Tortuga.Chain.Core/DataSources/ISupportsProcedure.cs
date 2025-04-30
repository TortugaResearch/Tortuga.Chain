using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources;

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
