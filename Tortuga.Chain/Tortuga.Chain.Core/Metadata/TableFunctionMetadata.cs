namespace Tortuga.Chain.Metadata
{
	/// <summary>
	/// Class TableFunctionMetadata.
	/// </summary>
	public abstract class TableFunctionMetadata : DatabaseObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TableFunctionMetadata"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="columns">The columns.</param>
		/// <exception cref="ArgumentException">name</exception>
		/// <exception cref="ArgumentNullException">
		/// parameters
		/// or
		/// columns
		/// </exception>
		protected TableFunctionMetadata(string name, ParameterMetadataCollection parameters, ColumnMetadataCollection columns) : base(name)
		{
			Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters), $"{nameof(parameters)} is null.");
			Columns = columns ?? throw new ArgumentNullException(nameof(columns), $"{nameof(columns)} is null.");
			NullableColumns = new ColumnMetadataCollection(name, columns.Where(c => c.IsNullable == true).ToList());
		}

		/// <summary>
		/// Gets the columns.
		/// </summary>
		/// <value>
		/// The columns.
		/// </value>
		public ColumnMetadataCollection Columns { get; }

		/// <summary>
		/// Gets the columns known to be nullable.
		/// </summary>
		/// <value>
		/// The nullable columns.
		/// </value>
		/// <remarks>This is used to improve the performance of materializers by avoiding is null checks.</remarks>
		public ColumnMetadataCollection NullableColumns { get; }

		/// <summary>
		/// Gets the parameters.
		/// </summary>
		/// <value>
		/// The parameters.
		/// </value>
		public ParameterMetadataCollection Parameters { get; }
	}
}
