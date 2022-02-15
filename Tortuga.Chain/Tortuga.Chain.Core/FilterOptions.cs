namespace Tortuga.Chain
{
	/// <summary>
	/// Enum FilterOptions
	/// </summary>
	[Flags]
	public enum FilterOptions
	{
		/// <summary>
		/// The properties that are null will be used when constructing a WHERE clause. (e.g. "ColumnName IS NULL")
		/// </summary>
		None = 0,

		/// <summary>
		/// The ignore properties that are null when constructing a WHERE clause.
		/// </summary>
		IgnoreNullProperties = 1
	}
}
