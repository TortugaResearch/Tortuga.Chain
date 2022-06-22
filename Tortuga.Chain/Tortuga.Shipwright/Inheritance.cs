namespace Tortuga.Shipwright;

/// <summary>
/// When exposing a method, event, or property, this enumeration indicates which inheritance modifies to use. 
/// </summary>
[Flags]
public enum Inheritance
{
	/// <summary>
	/// No additional modifier is applied.
	/// </summary>
	None = 0,

	/// <summary>
	/// Apply the `virtual` modifier to the exposed member.
	/// </summary>
	Virtual = 1,

	/// <summary>
	/// Apply the `override` modifier to the exposed member.
	/// </summary>
	Override = 2,

	/// <summary>
	/// Apply the `abstract` modifier to the exposed member.
	/// </summary>
	Abstract = 4,

	/// <summary>
	/// Apply the `abstract override` modifier to the exposed member.
	/// </summary>
	AbstractOverride = Abstract | Override,

	/// <summary>
	/// Apply the `sealed` modifier to the exposed member.
	/// </summary>
	Sealed = 8,

	/// <summary>
	/// Apply the `sealed override` modifier to the exposed member.
	/// </summary>
	SealedOverride = Sealed + Override
}
