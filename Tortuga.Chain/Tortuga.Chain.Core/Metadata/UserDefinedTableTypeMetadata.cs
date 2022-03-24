namespace Tortuga.Chain.Metadata
{
	/// <summary>
	/// This represents user defined table types in the database.
	/// </summary>
	public abstract class UserDefinedTableTypeMetadata
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UserDefinedTableTypeMetadata{TObjectName, TDbType}" /> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="columns">The columns.</param>
		/// <exception cref="ArgumentException">name</exception>
		/// <exception cref="ArgumentNullException">columns</exception>
		protected UserDefinedTableTypeMetadata(string name, ColumnMetadataCollection columns)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException($"{nameof(name)} is null or empty.", nameof(name));

			Name = name;
			Columns = columns ?? throw new ArgumentNullException(nameof(columns), $"{nameof(columns)} is null.");
		}

		/// <summary>
		/// Gets the columns.
		/// </summary>
		/// <value>
		/// The columns.
		/// </value>
		public ColumnMetadataCollection Columns { get; }

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; }
	}
}
