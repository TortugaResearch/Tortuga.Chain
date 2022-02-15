namespace Tortuga.Chain.Metadata
{
	/// <summary>
	/// Class StoredProcedureMetadata.
	/// </summary>
	public abstract class StoredProcedureMetadata : DatabaseObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Tortuga.Chain.Metadata.StoredProcedureMetadata" /> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="parameters">The parameters.</param>
		/// <exception cref="ArgumentException">name</exception>
		/// <exception cref="ArgumentNullException">parameters</exception>
		protected StoredProcedureMetadata(string name, ParameterMetadataCollection parameters) : base(name)
		{
			Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters), $"{nameof(parameters)} is null.");
		}

		/// <summary>
		/// Gets the parameters.
		/// </summary>
		/// <value>
		/// The parameters.
		/// </value>
		public ParameterMetadataCollection Parameters { get; }
	}
}
