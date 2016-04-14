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
    /// <seealso cref="CommandBuilders.MultipleRowDbCommandBuilder{TCommand, TParameter}" />
    public abstract class TableDbCommandBuilder<TCommand, TParameter> : MultipleRowDbCommandBuilder<TCommand, TParameter>, ITableDbCommandBuilder
        where TCommand : DbCommand
        where TParameter : DbParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableDbCommandBuilder{TCommand, TParameter}"/> class.
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
        public abstract TableDbCommandBuilder<TCommand, TParameter> WithSorting(IEnumerable<SortExpression> sortExpressions);

        /// <summary>
        /// Adds sorting to the command builder
        /// </summary>
        /// <param name="sortExpressions">The sort expressions.</param>
        /// <returns></returns>
        public TableDbCommandBuilder<TCommand, TParameter> WithSorting(params SortExpression[] sortExpressions)
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

    }
}
