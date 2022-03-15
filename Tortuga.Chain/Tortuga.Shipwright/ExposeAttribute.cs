namespace Tortuga.Shipwright;

/// <summary>
/// Expose the method, event, or property on the class using this trait.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Property, AllowMultiple = false)]
public class ExposeAttribute : Attribute
{
	/// <summary>
	/// Sets the accessibility for a given method, event, or property when exposed as a trait.
	/// </summary>
	/// <remarks>This defaults to Public</remarks>
	public Accessibility Accessibility { get; set; } = Accessibility.Public;

	/// <summary>
	/// Sets the inheritance rule for a given method, event, or property when exposed as a trait.
	/// </summary>
	public Inheritance Inheritance { get; set; } = Inheritance.None;

	/// <summary>
	/// Sets the accessibility the property's setter. May also be used to indicate a property is `init`.
	/// </summary>
	/// <remarks>This is ignored if the property is read-only or used on a non-property member.</remarks>
	public Setter Setter { get; set; } = Setter.None;

	/// <summary>
	/// Sets the accessibility the property's getter. 
	/// </summary>
	/// <remarks>This is ignored if the property is write-only or used on a non-property member.</remarks>
	public Getter Getter { get; set; } = Getter.None;
}

