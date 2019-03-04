using System.Data.Common;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This represents command builder that operate on single object parameters and performs an insert or update operation.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TArgument">The type of the argument.</typeparam>
    public abstract class UpsertDbCommandBuilder<TCommand, TParameter, TArgument> : ObjectDbCommandBuilder<TCommand, TParameter, TArgument>, IUpsertCommandBuilder<TArgument>
                where TCommand : DbCommand
        where TParameter : DbParameter
        where TArgument : class
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="argumentValue">The argument value.</param>
        protected UpsertDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource, TArgument argumentValue) : base(dataSource, argumentValue)
        {
        }

        /// <summary>
        /// Matches the on an alternate column(s). Normally matches need to be on the primary key.
        /// </summary>
        /// <param name="columnNames">The column names that form a unique key.</param>
        /// <returns></returns>
        public abstract UpsertDbCommandBuilder<TCommand, TParameter, TArgument> MatchOn(params string[] columnNames);

        IUpsertCommandBuilder<TArgument> IUpsertCommandBuilder<TArgument>.MatchOn(params string[] columnNames)
        {
            return MatchOn(columnNames);
        }
    }
}
