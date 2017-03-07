using System.Collections.Generic;
using System.Data.Common;
using Tortuga.Chain.DataSources;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This is the base class for table style command builders such as FromTableOrView and FromTableValueFunction.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TLimit">The type of the limit option.</typeparam>
    /// <seealso cref="CommandBuilders.MultipleRowDbCommandBuilder{TCommand, TParameter}" />
    /// <seealso cref="ITableDbCommandBuilder" />
    [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
    public abstract class TableDbCommandBuilder<TCommand, TParameter, TLimit> : MultipleRowDbCommandBuilder<TCommand, TParameter>, ITableDbCommandBuilder
        where TCommand : DbCommand
        where TParameter : DbParameter
        where TLimit : struct //really an enum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableDbCommandBuilder{TCommand, TParameter, TLimit}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected TableDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource) : base(dataSource)
        {

        }

        /// <summary>
        /// Adds sorting to the command builder.
        /// </summary>
        /// <param name="sortExpressions">The sort expressions.</param>
        /// <returns></returns>
        public abstract TableDbCommandBuilder<TCommand, TParameter, TLimit> WithSorting(IEnumerable<SortExpression> sortExpressions);

        /// <summary>
        /// Adds sorting to the command builder
        /// </summary>
        /// <param name="sortExpressions">The sort expressions.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithSorting(params SortExpression[] sortExpressions)
        {
            return WithSorting((IEnumerable<SortExpression>)sortExpressions);
        }

        ITableDbCommandBuilder ITableDbCommandBuilder.WithSorting(IEnumerable<SortExpression> sortExpressions)
        {
            return WithSorting(sortExpressions);
        }

        ITableDbCommandBuilder ITableDbCommandBuilder.WithSorting(params SortExpression[] sortExpressions)
        {
            return WithSorting((IEnumerable<SortExpression>)sortExpressions);
        }

        /// <summary>
        /// Adds limits to the command builder.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="take">Number of rows to take.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithLimits(int? skip, int? take)
        {
            return OnWithLimits(skip, take, DefaultLimitOption, null);
        }

        /// <summary>
        /// Adds limits to the command builder.
        /// </summary>
        /// <param name="take">Number of rows to take.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithLimits(int? take)
        {
            return OnWithLimits(null, take, DefaultLimitOption, null);
        }

        /// <summary>
        /// Adds limits to the command builder.
        /// </summary>
        /// <param name="take">Number of rows to take.</param>
        /// <param name="limitOptions">The limit options.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithLimits(int? take, TLimit limitOptions)
        {
            return OnWithLimits(null, take, limitOptions, null);
        }

        /// <summary>
        /// Adds limits to the command builder.
        /// </summary>
        /// <param name="take">Number of rows to take.</param>
        /// <param name="limitOptions">The limit options.</param>
        /// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
        /// <returns></returns>
        public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithLimits(int? take, TLimit limitOptions, int seed)
        {
            return OnWithLimits(null, take, limitOptions, seed);
        }

        /// <summary>
        /// Adds limits to the command builder.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="take">Number of rows to take.</param>
        /// <param name="limitOptions">The limit options.</param>
        /// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
        /// <returns></returns>
        protected abstract TableDbCommandBuilder<TCommand, TParameter, TLimit> OnWithLimits(int? skip, int? take, TLimit limitOptions, int? seed);

        /// <summary>
        /// Adds limits to the command builder.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="take">Number of rows to take.</param>
        /// <param name="limitOptions">The limit options.</param>
        /// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
        /// <returns></returns>
        protected abstract TableDbCommandBuilder<TCommand, TParameter, TLimit> OnWithLimits(int? skip, int? take, LimitOptions limitOptions, int? seed);


        ITableDbCommandBuilder ITableDbCommandBuilder.WithLimits(int take, LimitOptions limitOptions, int? seed)
        {
            return OnWithLimits(null, take, limitOptions, seed);
        }


        ITableDbCommandBuilder ITableDbCommandBuilder.WithLimits(int skip, int take)
        {
            return OnWithLimits(skip, take, DefaultLimitOption, null);
        }


        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        public abstract TableDbCommandBuilder<TCommand, TParameter, TLimit> WithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None);

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <returns></returns>
        public abstract TableDbCommandBuilder<TCommand, TParameter, TLimit> WithFilter(string whereClause);

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        public abstract TableDbCommandBuilder<TCommand, TParameter, TLimit> WithFilter(string whereClause, object argumentValue);

        ITableDbCommandBuilder ITableDbCommandBuilder.WithFilter(object filterValue, FilterOptions filterOptions )
        {
            return WithFilter(filterValue, filterOptions);
        }

        ITableDbCommandBuilder ITableDbCommandBuilder.WithFilter(string whereClause)
        {
            return WithFilter(whereClause);
        }

        ITableDbCommandBuilder ITableDbCommandBuilder.WithFilter(string whereClause, object argumentValue)
        {
            return WithFilter(whereClause, argumentValue);
        }

        /// <summary>
        /// Returns the row count using a <c>SELECT Count(*)</c> style query.
        /// </summary>
        /// <returns></returns>
        public abstract ILink<long> AsCount();

        /// <summary>
        /// Returns the row count for a given column. <c>SELECT Count(columnName)</c>
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="distinct">if set to <c>true</c> use <c>SELECT COUNT(DISTINCT columnName)</c>.</param>
        /// <returns></returns>
        public abstract ILink<long> AsCount(string columnName, bool distinct = false);

        /// <summary>
        /// Gets the default limit option.
        /// </summary>
        /// <value>
        /// The default limit options.
        /// </value>
        /// <remarks>For most data sources, this will be LimitOptions.Rows. </remarks>
        protected virtual LimitOptions DefaultLimitOption
        {
            get { return LimitOptions.Rows; }
        }
    }
}
