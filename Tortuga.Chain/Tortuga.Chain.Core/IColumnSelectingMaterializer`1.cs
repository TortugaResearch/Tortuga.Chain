namespace Tortuga.Chain
{
	/// <summary>
	/// This interface denotes an materializer that allows overriding which columns are selected.
	/// </summary>
	/// <typeparam name="TResult">The type of the t result.</typeparam>
	/// <seealso cref="ILink{TResult}"/>
	public interface IColumnSelectingMaterializer<TResult> : ILink<TResult>
	{
		/// <summary>
		/// Excludes the properties from the list of what will be populated in the object.
		/// </summary>
		/// <param name="propertiesToOmit">The properties to omit.</param>
		/// <returns>ILink&lt;TResult&gt;.</returns>
		/// <remarks>This feature is not available for all command builders. For example, those that bind to a stored procedure may ignore this option.</remarks>
		ILink<TResult> ExceptProperties(params string[] propertiesToOmit);

		/// <summary>
		/// Limits the list of properties to populate to just the indicated list.
		/// </summary>
		/// <param name="propertiesToPopulate">The properties of the object to populate.</param>
		/// <returns>ILink&lt;TResult&gt;.</returns>
		/// <remarks>This feature is not available for all command builders. For example, those that bind to a stored procedure may ignore this option.</remarks>
		ILink<TResult> WithProperties(params string[] propertiesToPopulate);
	}
}
