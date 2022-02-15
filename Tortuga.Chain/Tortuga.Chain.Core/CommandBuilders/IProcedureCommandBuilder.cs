namespace Tortuga.Chain.CommandBuilders
{
	/// <summary>
	/// This command builder can capture output parameters.
	/// </summary>
	public interface IProcedureDbCommandBuilder : IMultipleTableDbCommandBuilder
	{
		/// <summary>
		/// Captures the output parameters as a dictionary.
		/// </summary>
		/// <returns>The output parameters as a dictionary.</returns>
		/// <remarks>This will throw an exception if there are no output parameters.</remarks>
		ILink<Dictionary<string, object?>> AsOutputs();

		/// <summary>
		/// Captures the output parameters and use them to populate a new object.
		/// </summary>
		/// <typeparam name="TObject">The type that will hold the output parameters.</typeparam>
		/// <returns>The output parameters as an object.</returns>
		/// <remarks>This will throw an exception if there are no output parameters.</remarks>
		ILink<TObject> AsOutputs<TObject>() where TObject : class, new();
	}
}
