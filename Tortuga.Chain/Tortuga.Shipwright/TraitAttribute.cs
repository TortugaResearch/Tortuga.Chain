namespace Tortuga.Shipwright;


/// <summary>
/// This attribute allows a struct to be used as a trait.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class TraitAttribute : Attribute
{

}
