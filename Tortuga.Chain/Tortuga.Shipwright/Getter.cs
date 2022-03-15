namespace Tortuga.Shipwright
{
	/// <summary>
	/// When exposing a property getter, this enumeration indicates what accessibility to use. 
	/// </summary>

	[Flags]
	public enum Getter
	{
		/// <summary>
		/// No additional modifier is applied to the getter. 
		/// </summary>
		None = 0,

		/// <summary>
		/// Apply the `protected` modifer to the getter.
		/// </summary>
		Protected = 1,

		/// <summary>
		/// Apply the `internal` modifer to the getter.
		/// </summary>
		Internal = 2,

		/// <summary>
		/// Apply the `protected internal` modifer to the getter.
		/// </summary>
		ProtectedOrInternal = 3,

		/// <summary>
		/// Apply the `private` modifer to the getter.
		/// </summary>
		Private = 4,
	}
}
