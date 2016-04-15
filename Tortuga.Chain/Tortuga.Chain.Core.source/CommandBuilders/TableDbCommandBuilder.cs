using System.Collections.Generic;
using System.Data.Common;
using Tortuga.Chain.DataSources;

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
    public abstract class TableDbCommandBuilder<TCommand, TParameter, TLimit> : MultipleRowDbCommandBuilder<TCommand, TParameter>, ITableDbCommandBuilder
        where TCommand : DbCommand
        where TParameter : DbParameter
        where TLimit : struct //really an enum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableDbCommandBuilder{TCommand, TParameter, TLimit}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected TableDbCommandBuilder(DataSource<TCommand, TParameter> dataSource) : base(dataSource)
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
            return OnWithLimits(skip, take, LimitOptions.Rows, null);
        }

        /// <summary>
        /// Adds limits to the command builder.
        /// </summary>
        /// <param name="take">Number of rows to take.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<TCommand, TParameter, TLimit> WithLimits(int? take)
        {
            return OnWithLimits(null, take, LimitOptions.Rows, null);
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
            return OnWithLimits(skip, take, LimitOptions.Rows, null);
        }
    }
}
