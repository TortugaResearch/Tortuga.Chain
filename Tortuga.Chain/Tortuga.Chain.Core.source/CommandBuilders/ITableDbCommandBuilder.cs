using System.Collections.Generic;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This is a specialization of IMultipleRowDbCommandBuilder that includes support for sorting and limiting
    /// </summary>
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
    }
}
