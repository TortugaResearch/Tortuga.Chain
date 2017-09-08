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
    /// <seealso cref="IUpdateManyCommandBuilder" />
    [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
    public abstract class UpdateManyCommandBuilder<TCommand, TParameter> : MultipleRowDbCommandBuilder<TCommand, TParameter>, IUpdateManyCommandBuilder<TCommand, TParameter>
        where TCommand : DbCommand
        where TParameter : DbParameter
    {
        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected UpdateManyCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource)
            : base(dataSource)
        {

        }

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        public abstract UpdateManyCommandBuilder<TCommand, TParameter> WithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None);

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <returns></returns>
        public abstract UpdateManyCommandBuilder<TCommand, TParameter> WithFilter(string whereClause);

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        public abstract UpdateManyCommandBuilder<TCommand, TParameter> WithFilter(string whereClause, object argumentValue);

        /// <summary>
        /// Applies this command to all rows.
        /// </summary>
        /// <returns></returns>
        public abstract UpdateManyCommandBuilder<TCommand, TParameter> All();



        IMultipleRowDbCommandBuilder IUpdateManyCommandBuilder.WithFilter(object filterValue, FilterOptions filterOptions)
        {
            return WithFilter(filterValue, filterOptions);
        }

        IMultipleRowDbCommandBuilder IUpdateManyCommandBuilder.WithFilter(string whereClause)
        {
            return WithFilter(whereClause);
        }

        IMultipleRowDbCommandBuilder IUpdateManyCommandBuilder.WithFilter(string whereClause, object argumentValue)
        {
            return WithFilter(whereClause, argumentValue);
        }

        MultipleRowDbCommandBuilder<TCommand, TParameter> IUpdateManyCommandBuilder<TCommand, TParameter>.WithFilter(object filterValue, FilterOptions filterOptions)
        {
            return WithFilter(filterValue, filterOptions);
        }

        MultipleRowDbCommandBuilder<TCommand, TParameter> IUpdateManyCommandBuilder<TCommand, TParameter>.WithFilter(string whereClause)
        {
            return WithFilter(whereClause);
        }

        MultipleRowDbCommandBuilder<TCommand, TParameter> IUpdateManyCommandBuilder<TCommand, TParameter>.WithFilter(string whereClause, object argumentValue)
        {
            return WithFilter(whereClause, argumentValue);
        }

        MultipleRowDbCommandBuilder<TCommand, TParameter> IUpdateManyCommandBuilder<TCommand, TParameter>.All()
        {
            return All();
        }

        IMultipleRowDbCommandBuilder IUpdateManyCommandBuilder.All()
        {
            return All();
        }
    }
}
