using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// This is the base class for command builders that perform set-based update operations.
/// </summary>
/// <typeparam name="TCommand">The type of the t command.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
/// <seealso cref="MultipleRowDbCommandBuilder{TCommand, TParameter}" />
/// <seealso cref="IUpdateSetDbCommandBuilder" />
[SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
public abstract class UpdateSetDbCommandBuilder<TCommand, TParameter> : MultipleRowDbCommandBuilder<TCommand, TParameter>, IUpdateSetDbCommandBuilder<TCommand, TParameter>
	where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.CommandBuilders.UpdateSetDbCommandBuilder{TCommand, TParameter}"/> class.</summary>
	/// <param name="dataSource">The data source.</param>
	protected UpdateSetDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource)
		: base(dataSource)
	{
	}

	/// <summary>
	/// Applies this command to all rows.
	/// </summary>
	/// <returns></returns>
	public abstract UpdateSetDbCommandBuilder<TCommand, TParameter> All();

	MultipleRowDbCommandBuilder<TCommand, TParameter> IUpdateSetDbCommandBuilder<TCommand, TParameter>.All() => All();

	IMultipleRowDbCommandBuilder IUpdateSetDbCommandBuilder.All() => All();

	/// <summary>
	/// Adds (or replaces) the filter on this command builder.
	/// </summary>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The filter options.</param>
	public abstract UpdateSetDbCommandBuilder<TCommand, TParameter> WithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None);

	/// <summary>
	/// Adds (or replaces) the filter on this command builder.
	/// </summary>
	/// <param name="whereClause">The where clause.</param>
	/// <returns></returns>
	public abstract UpdateSetDbCommandBuilder<TCommand, TParameter> WithFilter(string whereClause);

	/// <summary>
	/// Adds (or replaces) the filter on this command builder.
	/// </summary>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <returns></returns>
	public abstract UpdateSetDbCommandBuilder<TCommand, TParameter> WithFilter(string whereClause, object? argumentValue);

	IMultipleRowDbCommandBuilder IUpdateSetDbCommandBuilder.WithFilter(object filterValue, FilterOptions filterOptions) => WithFilter(filterValue, filterOptions);

	IMultipleRowDbCommandBuilder IUpdateSetDbCommandBuilder.WithFilter(string whereClause) => WithFilter(whereClause);

	IMultipleRowDbCommandBuilder IUpdateSetDbCommandBuilder.WithFilter(string whereClause, object argumentValue) => WithFilter(whereClause, argumentValue);

	MultipleRowDbCommandBuilder<TCommand, TParameter> IUpdateSetDbCommandBuilder<TCommand, TParameter>.WithFilter(object filterValue, FilterOptions filterOptions) => WithFilter(filterValue, filterOptions);

	MultipleRowDbCommandBuilder<TCommand, TParameter> IUpdateSetDbCommandBuilder<TCommand, TParameter>.WithFilter(string whereClause) => WithFilter(whereClause);

	MultipleRowDbCommandBuilder<TCommand, TParameter> IUpdateSetDbCommandBuilder<TCommand, TParameter>.WithFilter(string whereClause, object? argumentValue) => WithFilter(whereClause, argumentValue);
}
