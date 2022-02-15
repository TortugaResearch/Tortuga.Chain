namespace Tortuga.Chain.Metadata
{
	/// <summary>
	/// This class represents user defined table types.
	/// </summary>
	/// <typeparam name="TName">The type of the t name.</typeparam>
	/// <typeparam name="TDbType">The type of the t database type.</typeparam>
	public sealed class UserDefinedTableTypeMetadata<TName, TDbType> : UserDefinedTableTypeMetadata
		where TName : struct
		where TDbType : struct
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UserDefinedTableTypeMetadata{TName, TDbType}" /> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="columns">The columns.</param>
		public UserDefinedTableTypeMetadata(TName name, ColumnMetadataCollection<TDbType> columns)
			: base(name.ToString()!, columns?.GenericCollection!)
		{
			Name = name;
			Columns = columns ?? throw new ArgumentNullException(nameof(columns), $"{nameof(columns)} is null.");
		}

		/// <summary>
		/// Gets the columns.
		/// </summary>
		/// <value>
		/// The columns.
		/// </value>
		public new ColumnMetadataCollection<TDbType> Columns { get; }

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public new TName Name { get; }
	}
}
