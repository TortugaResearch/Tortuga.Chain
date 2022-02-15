namespace Tortuga.Chain
{
	/// <summary>
	/// Enum JoinOptions
	/// </summary>
	[Flags]
	public enum JoinOptions
	{
		/// <summary>
		/// The default behavior is to assign the child object to the first matching parent object. If there are no matches, an error is thrown.
		/// </summary>
		None = 0,

		/// <summary>
		/// Continues searching when the first match is found, possibly assigning one the detail record to multiple parent records.
		/// </summary>
		MultipleParents = 1,

		/// <summary>
		/// If there are unmatchable child records, silently discard them instead of throwing an exception.
		/// </summary>
		IgnoreUnmatchedChildren = 2,

		/// <summary>
		/// Perform the join in parallel.
		/// </summary>
		/// <remarks>Lock overhead may make this slower than the normal mode. This relies on PLINQ, which may decide to not go parallel for small collections.</remarks>
		Parallel = 4,
	}
}
