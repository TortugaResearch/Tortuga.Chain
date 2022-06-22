namespace Tortuga.Chain
{
	/// <summary>
	/// Indicates how the collection will be generated from a result set.
	/// </summary>
	[Flags]
	public enum CollectionOptions
	{
		/// <summary>
		/// Use the default behavior.
		/// </summary>
		/// <remarks>If a class has more than one constructor, the default constructor will be used.</remarks>
		None = 0,

		/// <summary>
		/// Infer which non-default constructor to use. When this option is chosen, individual properties will not be set.
		/// </summary>
		/// <remarks>This will throw an error unless there is exactly one public, non-default constructor.</remarks>
		InferConstructor = 8,

		/// <summary>
		/// Populate individual properties even when using a non-default constructor.
		/// </summary>
		WithProperties = 32,

		/// <summary>
		/// Infer which constructor to use and also populate individual properties.
		/// </summary>
		InferConstructorWithProperties = InferConstructor | WithProperties,

	}
}
