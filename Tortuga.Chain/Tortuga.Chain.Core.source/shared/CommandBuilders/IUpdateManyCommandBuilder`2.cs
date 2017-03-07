using System.Data.Common;

namespace Tortuga.Chain.CommandBuilders
{
    public interface IUpdateManyCommandBuilder<TCommand, TParameter> : IUpdateManyCommandBuilder
                    where TCommand : DbCommand
            where TParameter : DbParameter
    {
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
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        new MultipleRowDbCommandBuilder<TCommand, TParameter> WithFilter(string whereClause, object argumentValue);

        /// <summary>
        /// Applies this command to all rows.
        /// </summary>
        /// <returns></returns>
        new MultipleRowDbCommandBuilder<TCommand, TParameter> All();
    }
}
