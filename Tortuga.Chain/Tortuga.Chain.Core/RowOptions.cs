using System.ComponentModel;

namespace Tortuga.Chain
{
	/// <summary>
	/// Controls what happens when the wrong number of rows are returned.
	/// </summary>
	[Flags]
	public enum RowOptions
	{
		/// <summary>
		/// Use the default behavior for the materializer.
		/// </summary>
		/// <remarks></remarks>
		None = 0,

		/// <summary>
		/// An error will not be raised if no rows are returned.
		/// </summary>
		/// <remarks></remarks>
		[Obsolete("This option is no longer supported. Use the XxxOrNull version of the function instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		AllowEmptyResults = 1,

		/// <summary>
		/// An error will not be raised if extra rows are returned. The extras will be discarded.
		/// </summary>
		/// <remarks></remarks>
		DiscardExtraRows = 2,

		/// <summary>
		/// Infer which constructor to use. When this option is chosen, individual properties will not be set.
		/// </summary>
		InferConstructor = 8,

		/// <summary>
		/// An error will be raised if no rows are returned
		/// </summary>
		/// <remarks></remarks>
		PreventEmptyResults = 16,

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
