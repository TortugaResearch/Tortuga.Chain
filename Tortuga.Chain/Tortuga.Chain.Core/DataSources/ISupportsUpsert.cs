using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources;

/// <summary>
/// Used to mark data sources that support the Upsert command.
/// </summary>
public interface ISupportsUpsert
{
	/// <summary>
	/// Perform an insert or update operation as appropriate.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="options">The options for how the insert/update occurs.</param>
	/// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
	IObjectDbCommandBuilder<TArgument> Upsert<TArgument>(string tableName, TArgument argumentValue, UpsertOptions options = UpsertOptions.None) where TArgument : class;

	/// <summary>
	/// Perform an insert or update operation as appropriate.
	/// </summary>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="options">The options for how the insert/update occurs.</param>
	/// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
	IObjectDbCommandBuilder<TArgument> Upsert<TArgument>(TArgument argumentValue, UpsertOptions options = UpsertOptions.None) where TArgument : class;
}
