namespace Tortuga.Chain.Metadata
{
	/// <summary>
	/// Class TableFunctionMetadata.
	/// </summary>
	public abstract class ScalarFunctionMetadata : DatabaseObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ScalarFunctionMetadata"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="typeName">Name of the type.</param>
		/// <param name="dbType">Type of the database.</param>
		/// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
		/// <param name="maxLength">The maximum length.</param>
		/// <param name="precision">The precision.</param>
		/// <param name="scale">The scale.</param>
		/// <param name="fullTypeName">Full name of the type.</param>
		protected ScalarFunctionMetadata(string name, ParameterMetadataCollection parameters, string typeName, object? dbType, bool isNullable, int? maxLength, int? precision, int? scale, string fullTypeName) : base(name)
		{
			Parameters = parameters;
			TypeName = typeName;
			DbType = dbType;
			IsNullable = isNullable;
			MaxLength = maxLength;
			Precision = precision;
			Scale = scale;
			FullTypeName = fullTypeName;
		}

		/// <summary>
		/// Gets the type used by the database.
		/// </summary>
		public object? DbType { get; }

		/// <summary>
		/// Gets or sets the full name of the type including max length, precision, and/or scale.
		/// </summary>
		/// <value>
		/// The full name of the type.
		/// </value>
		public string FullTypeName { get; }

		/// <summary>
		/// Gets or sets a value indicating whether this column is nullable.
		/// </summary>
		/// <value>
		/// <c>true</c> if this column is nullable; otherwise, <c>false</c>.
		/// </value>
		public bool IsNullable { get; }

		/// <summary>
		/// Gets or sets the maximum length.
		/// </summary>
		/// <value>
		/// The maximum length.
		/// </value>
		public int? MaxLength { get; }

		/// <summary>
		/// Gets the parameters.
		/// </summary>
		/// <value>
		/// The parameters.
		/// </value>
		public ParameterMetadataCollection Parameters { get; }

		/// <summary>
		/// Gets or sets the precision.
		/// </summary>
		/// <value>
		/// The precision.
		/// </value>
		public int? Precision { get; }

		/// <summary>
		/// Gets or sets the scale.
		/// </summary>
		/// <value>
		/// The scale.
		/// </value>
		public int? Scale { get; }

		/// <summary>
		/// Gets the name of the type.
		/// </summary>
		/// <value>The name of the type.</value>
		public string TypeName { get; }
	}
}
