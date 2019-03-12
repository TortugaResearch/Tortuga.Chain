using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This is the base class for command builders that perform set-based update operations.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter.</typeparam>
    /// <seealso cref="MultipleRowDbCommandBuilder{TCommand, TParameter}" />
    /// <seealso cref="IUpdateManyDbCommandBuilder" />
    [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
    public abstract class UpdateManyDbCommandBuilder<TCommand, TParameter> : MultipleRowDbCommandBuilder<TCommand, TParameter>, IUpdateManyDbCommandBuilder<TCommand, TParameter>
        where TCommand : DbCommand
        where TParameter : DbParameter
    {
        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected UpdateManyDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource)
            : base(dataSource)
        {
        }

        /// <summary>
        /// Applies this command to all rows.
        /// </summary>
        /// <returns></returns>
        public abstract UpdateManyDbCommandBuilder<TCommand, TParameter> All();

        MultipleRowDbCommandBuilder<TCommand, TParameter> IUpdateManyDbCommandBuilder<TCommand, TParameter>.All() => All();

        IMultipleRowDbCommandBuilder IUpdateManyDbCommandBuilder.All() => All();

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        public abstract UpdateManyDbCommandBuilder<TCommand, TParameter> WithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None);

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <returns></returns>
        public abstract UpdateManyDbCommandBuilder<TCommand, TParameter> WithFilter(string whereClause);

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        public abstract UpdateManyDbCommandBuilder<TCommand, TParameter> WithFilter(string whereClause, object argumentValue);

        IMultipleRowDbCommandBuilder IUpdateManyDbCommandBuilder.WithFilter(object filterValue, FilterOptions filterOptions) => WithFilter(filterValue, filterOptions);

        IMultipleRowDbCommandBuilder IUpdateManyDbCommandBuilder.WithFilter(string whereClause) => WithFilter(whereClause);

        IMultipleRowDbCommandBuilder IUpdateManyDbCommandBuilder.WithFilter(string whereClause, object argumentValue) => WithFilter(whereClause, argumentValue);

        MultipleRowDbCommandBuilder<TCommand, TParameter> IUpdateManyDbCommandBuilder<TCommand, TParameter>.WithFilter(object filterValue, FilterOptions filterOptions) => WithFilter(filterValue, filterOptions);

        MultipleRowDbCommandBuilder<TCommand, TParameter> IUpdateManyDbCommandBuilder<TCommand, TParameter>.WithFilter(string whereClause) => WithFilter(whereClause);

        MultipleRowDbCommandBuilder<TCommand, TParameter> IUpdateManyDbCommandBuilder<TCommand, TParameter>.WithFilter(string whereClause, object argumentValue) => WithFilter(whereClause, argumentValue);
    }
}
