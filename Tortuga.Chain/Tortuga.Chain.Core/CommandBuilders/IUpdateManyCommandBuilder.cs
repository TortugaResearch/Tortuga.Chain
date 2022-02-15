namespace Tortuga.Chain.CommandBuilders
{
	/// <summary>
	/// This is a specialization of IMultipleRowDbCommandBuilder that includes support for sorting. It is only used for set-based update operations.
	/// </summary>
	/// <seealso cref="IMultipleRowDbCommandBuilder" />
	public interface IUpdateManyDbCommandBuilder
	{
		/// <summary>
		/// Applies this command to all rows.
		/// </summary>
		/// <returns></returns>
		IMultipleRowDbCommandBuilder All();

		/// <summary>
		/// Adds (or replaces) the filter on this command builder.
		/// </summary>
		/// <param name="filterValue">The filter value.</param>
		/// <param name="filterOptions">The filter options.</param>
		/// <returns></returns>
		IMultipleRowDbCommandBuilder WithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None);

		/// <summary>
		/// Adds (or replaces) the filter on this command builder.
		/// </summary>
		/// <param name="whereClause">The where clause.</param>
		/// <returns></returns>
		IMultipleRowDbCommandBuilder WithFilter(string whereClause);

		/// <summary>
		/// Adds (or replaces) the filter on this command builder.
		/// </summary>
		/// <param name="whereClause">The where clause.</param>
		/// <param name="whereArgumentValue">The argument value.</param>
		/// <returns></returns>
		IMultipleRowDbCommandBuilder WithFilter(string whereClause, object whereArgumentValue);
	}
}
