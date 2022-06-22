namespace Tortuga.Chain
{
	/// <summary>
	/// Options for populating dictionaries.
	/// </summary>
	[Flags]
	public enum DictionaryOptions
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
		/// If two rows have the same key, no error will be raised. This option is not compatible with immutable dictionaries.
		/// </summary>
		/// <remarks>This option uses IDictionary.Item[] instead of IDictionary.Add().</remarks>
		DiscardDuplicates = 16,

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
