using System.Collections.Generic;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This is a specialization of IMultipleRowDbCommandBuilder that includes support for sorting and limiting
    /// </summary>
    /// <seealso cref="IMultipleRowDbCommandBuilder" />
    /// <remarks>
    /// Warning: This interface is meant to simulate multiple inheritance and work-around some issues with exposing generic types. Do not implement it in client code, as new method will be added over time.
    /// </remarks>
    /// <seealso cref="IMultipleRowDbCommandBuilder" />
    public interface ITableDbCommandBuilder : IMultipleRowDbCommandBuilder
    {
        /// <summary>
        /// Adds sorting to the command builder.
        /// </summary>
        /// <param name="sortExpressions">The sort expressions.</param>
        /// <returns></returns>
        ITableDbCommandBuilder WithSorting(params SortExpression[] sortExpressions);

        /// <summary>
        /// Adds sorting to the command builder
        /// </summary>
        /// <param name="sortExpressions">The sort expressions.</param>
        /// <returns></returns>
        ITableDbCommandBuilder WithSorting(IEnumerable<SortExpression> sortExpressions);


        /// <summary>
        /// Adds limits to the command builder.
        /// </summary>
        /// <param name="take">Number of rows to take.</param>
        /// <param name="limitOptions">The limit options.</param>
        /// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
        /// <returns></returns>
        ITableDbCommandBuilder WithLimits(int take, LimitOptions limitOptions = LimitOptions.Rows, int? seed = null);


        /// <summary>
        /// Adds limits to the command builder.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="take">The number of rows to take.</param>
        /// <returns></returns>
        /// <remarks>Warning: row skipping using this method can be significantly slower than using a WHERE clause that uses an indexed column.</remarks>
        ITableDbCommandBuilder WithLimits(int skip, int take);


        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <returns></returns>
        ITableDbCommandBuilder WithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None);

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <returns></returns>
        ITableDbCommandBuilder WithFilter(string whereClause);

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        ITableDbCommandBuilder WithFilter(string whereClause, object argumentValue);

        /// <summary>
        /// Returns the row count using a <c>SELECT Count(*)</c> style query.
        /// </summary>
        /// <returns></returns>
        ILink<long> AsCount();

        /// <summary>
        /// Returns the row count for a given column. <c>SELECT Count(columnName)</c>
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="distinct">if set to <c>true</c> use <c>SELECT COUNT(DISTINCT columnName)</c>.</param>
        /// <returns></returns>
        ILink<long> AsCount(string columnName, bool distinct = false);

    }


}
