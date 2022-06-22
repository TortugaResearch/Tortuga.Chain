namespace Tortuga.Shipwright;

/// <summary>
/// When exposing a property setter, this enumeration indicates what accessibility to use. 
/// </summary>
[Flags]
public enum Setter
{
	/// <summary>
	/// No additional modifier is applied to the setter. 
	/// </summary>
	None = 0,

	/// <summary>
	/// Apply the `protected` modifier to the setter.
	/// </summary>
	Protected = 1,

	/// <summary>
	/// Apply the `internal` modifier to the setter.
	/// </summary>
	Internal = 2,

	/// <summary>
	/// Apply the `protected internal` modifier to the setter.
	/// </summary>
	ProtectedOrInternal = 3,

	/// <summary>
	/// Apply the `private` modifier to the setter.
	/// </summary>
	Private = 4,

	/// <summary>
	/// The setter is exposed as `init`. This may be combined another accessibility modifier.
	/// </summary>
	Init = 8
}
