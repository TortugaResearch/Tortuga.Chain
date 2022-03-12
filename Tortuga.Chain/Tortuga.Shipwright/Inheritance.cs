namespace Tortuga.Shipwright
{
	/// <summary>
	/// When exposing a method, event, or property, this enumeration indicates which inheritance modifies to use. 
	/// </summary>
	[Flags]
	public enum Inheritance
	{
		None = 0,
		Virtual = 1,
		Override = 2,
		Abstract = 4,
		AbstractOverride = Abstract | Override,
		Sealed = 8,
		SealedOverride = Sealed + Override
	}
}