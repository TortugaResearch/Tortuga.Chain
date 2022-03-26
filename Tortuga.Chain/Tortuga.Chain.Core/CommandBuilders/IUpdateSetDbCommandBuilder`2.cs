using System.Data.Common;

namespace Tortuga.Chain.CommandBuilders
{
	/// <summary>
	/// Interface IUpdateSetCommandBuilder
	/// </summary>
	/// <typeparam name="TCommand">The type of the t command.</typeparam>
	/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
	/// <seealso cref="Tortuga.Chain.CommandBuilders.IUpdateSetDbCommandBuilder" />
	public interface IUpdateSetDbCommandBuilder<TCommand, TParameter> : IUpdateSetDbCommandBuilder
					where TCommand : DbCommand
			where TParameter : DbParameter
	{
		/// <summary>
		/// Applies this command to all rows.
		/// </summary>
		/// <returns></returns>
		new MultipleRowDbCommandBuilder<TCommand, TParameter> All();

		/// <summary>
		/// Adds (or replaces) the filter on this command builder.
		/// </summary>
		/// <param name="filterValue">The filter value.</param>
		/// <param name="filterOptions">The filter options.</param>
		/// <returns></returns>
		new MultipleRowDbCommandBuilder<TCommand, TParameter> WithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None);

		/// <summary>
		/// Adds (or replaces) the filter on this command builder.
		/// </summary>
		/// <param name="whereClause">The where clause.</param>
		/// <returns></returns>
		new MultipleRowDbCommandBuilder<TCommand, TParameter> WithFilter(string whereClause);

		/// <summary>
		/// Adds (or replaces) the filter on this command builder.
		/// </summary>
		/// <param name="whereClause">The where clause.</param>
		/// <param name="whereArgumentValue">The where clause argument value.</param>
		/// <returns></returns>
		new MultipleRowDbCommandBuilder<TCommand, TParameter> WithFilter(string whereClause, object? whereArgumentValue);
	}
}
