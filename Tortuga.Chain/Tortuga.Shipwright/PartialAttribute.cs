namespace Tortuga.Shipwright;


/// <summary>
/// Generate a partial method on the class using this trait and attach it to this delegate property.
/// </summary>
/// <remarks>This may only be used on properties of type Action or Func</remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class PartialAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="PartialAttribute"/> class.
	/// </summary>
	/// <param name="parameterNames">A comma separated list of parameter names to use in the generated partial method.</param>
	public PartialAttribute(string parameterNames = "")
	{
		ParameterNames = parameterNames;
	}

	/// <summary>
	/// Gets the comma separated list of parameter names to use in the generated partial method.
	/// </summary>
	public string ParameterNames { get; }
}
