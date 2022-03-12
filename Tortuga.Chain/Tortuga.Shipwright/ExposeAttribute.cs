namespace Tortuga.Shipwright;

/// <summary>
/// Expose the method, event, or property on the class using this trait.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Property, AllowMultiple = false)]
public class ExposeAttribute : Attribute
{
	/// <summary>
	/// Gets or sets the accessibility for a given method, event, or property when exposed as a trait.
	/// </summary>
	/// <remarks>This defaults to Public</remarks>
	public Accessibility Accessibility { get; set; } = Accessibility.Public;
	public Inheritance Inheritance { get; set; } = Inheritance.None;
}
