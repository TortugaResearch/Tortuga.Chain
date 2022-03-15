namespace Tortuga.Shipwright;

/// <summary>
/// When exposing a method, event, or property, this enumeration indicates what accessibility to use. 
/// </summary>
[Flags]
public enum Accessibility
{
	/// <summary>
	/// Apply the `public` modifer to the exposed member.
	/// </summary>
	Public = 0,

	/// <summary>
	/// Apply the `protected` modifer to the exposed member.
	/// </summary>
	Protected = 1,

	/// <summary>
	/// Apply the `internal` modifer to the exposed member.
	/// </summary>
	Internal = 2,

	/// <summary>
	/// Apply the `protected internal` modifer to the exposed member.
	/// </summary>
	ProtectedOrInternal = 3,

	/// <summary>
	/// Apply the `private` modifer to the exposed member.
	/// </summary>
	Private = 4,
}
