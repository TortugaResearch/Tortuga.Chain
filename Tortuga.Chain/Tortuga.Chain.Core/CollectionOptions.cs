using System;

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
		None = 0,

		/// <summary>
		/// Infer which constructor to use. When this option is chosen, individual properties will not be set.
		/// </summary>
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
