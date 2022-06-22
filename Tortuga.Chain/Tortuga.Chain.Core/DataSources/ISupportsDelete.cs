using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources
{

	/// <summary>
	/// Used to mark data sources that support the Delete command.
	/// </summary>
	public interface ISupportsDelete
	{
		/// <summary>
		/// Delete an object model from the specified table.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The delete options.</param>
		/// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
		IObjectDbCommandBuilder<TArgument> Delete<TArgument>(string tableName, TArgument argumentValue, DeleteOptions options = DeleteOptions.None) where TArgument : class;

		/// <summary>
		/// Delete an object model from the table indicated by the class's Table attribute.
		/// </summary>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The delete options.</param>
		IObjectDbCommandBuilder<TArgument> Delete<TArgument>(TArgument argumentValue, DeleteOptions options = DeleteOptions.None) where TArgument : class;
	}
}
