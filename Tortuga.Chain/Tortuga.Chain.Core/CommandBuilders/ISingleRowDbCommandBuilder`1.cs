namespace Tortuga.Chain.CommandBuilders
{
	/// <summary>
	/// This allows the use of scalar and single row materializers against a command builder.
	/// </summary>
	/// <remarks>Warning: This interface is meant to simulate multiple inheritance and work-around some issues with exposing generic types. Do not implement it in client code, as new methods will be added over time.</remarks>
	public interface ISingleRowDbCommandBuilder<TObject> : ISingleRowDbCommandBuilder
			where TObject : class
	{
		/// <summary>
		/// Materializes the result as an instance of the indicated type
		/// </summary>
		/// <param name="rowOptions">The row options.</param>
		/// <returns></returns>
		IConstructibleMaterializer<TObject> ToObject(RowOptions rowOptions = RowOptions.None);

		/// <summary>
		/// Materializes the result as an instance of the indicated type
		/// </summary>
		/// <param name="rowOptions">The row options.</param>
		/// <returns></returns>
		IConstructibleMaterializer<TObject?> ToObjectOrNull(RowOptions rowOptions = RowOptions.None);
	}
}
