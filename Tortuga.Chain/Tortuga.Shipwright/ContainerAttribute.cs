namespace Tortuga.Shipwright
{

	/// <summary>
	/// When placed on a property, that property is automatically set to the container of the trait object. 
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ContainerAttribute : Attribute
	{

		/// <summary>
		/// If set to true and the property's type is an interface, then that interface will be added to the owning class. 
		/// </summary>
		public bool RegisterInterface { get; set; }
	}
}