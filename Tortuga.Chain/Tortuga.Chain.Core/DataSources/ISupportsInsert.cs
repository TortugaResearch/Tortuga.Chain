using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources;

/// <summary>
/// Used to mark data sources that support the Insert command.
/// </summary>
public interface ISupportsInsert
{
	/// <summary>
	/// Inserts an object into the specified table.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="options">The options for how the insert occurs.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
	IObjectDbCommandBuilder<TArgument> Insert<TArgument>(string tableName, TArgument argumentValue, InsertOptions options = InsertOptions.None) where TArgument : class;

	/// <summary>
	/// Inserts an object into the specified table.
	/// </summary>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="options">The options for how the insert occurs.</param>
	/// <returns></returns>
	IObjectDbCommandBuilder<TArgument> Insert<TArgument>(TArgument argumentValue, InsertOptions options = InsertOptions.None) where TArgument : class;
}
