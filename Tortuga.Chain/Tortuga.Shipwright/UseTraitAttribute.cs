namespace Tortuga.Shipwright;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class UseTraitAttribute : Attribute
{
	public UseTraitAttribute(Type traitType) { TraitType = traitType; }

	public Type TraitType { get; }
}
