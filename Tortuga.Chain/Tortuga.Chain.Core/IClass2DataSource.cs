using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain
{
	/// <summary>
	/// A class 2 data source supports enhanced CRUD operations.
	/// </summary>
	/// <remarks>Warning: This interface is meant to simulate multiple inheritance and work-around some issues with exposing generic types. Do not implement it in client code, as new methods will be added over time.</remarks>
	public interface IClass2DataSource : IClass1DataSource
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


		/// <summary>Truncates the specified table.</summary>
		/// <param name="tableName">Name of the table to truncate.</param>
		/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
		ILink<int?> Truncate(string tableName);

		/// <summary>Truncates the specified table.</summary>
		/// <typeparam name="TObject">This class used to determine which table to truncate</typeparam>
		/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
		ILink<int?> Truncate<TObject>() where TObject : class;

	}
}
