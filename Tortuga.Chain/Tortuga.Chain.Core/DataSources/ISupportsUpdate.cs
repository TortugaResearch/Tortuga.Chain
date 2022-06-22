using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources;

/// <summary>
/// Used to mark data sources that support the Update command.
/// </summary>
public interface ISupportsUpdate
{
	/// <summary>
	/// Update an object in the specified table.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="options">The update options.</param>
	/// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
	IObjectDbCommandBuilder<TArgument> Update<TArgument>(string tableName, TArgument argumentValue, UpdateOptions options = UpdateOptions.None) where TArgument : class;

	/// <summary>
	/// Update an object in the specified table.
	/// </summary>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="options">The update options.</param>
	/// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
	IObjectDbCommandBuilder<TArgument> Update<TArgument>(TArgument argumentValue, UpdateOptions options = UpdateOptions.None) where TArgument : class;
}
