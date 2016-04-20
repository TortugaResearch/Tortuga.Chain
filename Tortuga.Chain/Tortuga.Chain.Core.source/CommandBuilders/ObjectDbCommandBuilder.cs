using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This representes command builders that operate on single object parameters: Insert, Update, Upsert, Delete
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TArgument">The type of the argument.</typeparam>
    public abstract class ObjectDbCommandBuilder<TCommand, TParameter, TArgument> : SingleRowDbCommandBuilder<TCommand, TParameter>, IObjectDbCommandBuilder<TArgument>
        where TCommand : DbCommand
        where TParameter : DbParameter
        where TArgument : class
    {
        public TArgument ArgumentValue { get; }

        protected ObjectDbCommandBuilder(DataSource<TCommand, TParameter> dataSource, TArgument argumentValue) : base(dataSource)
        {
            ArgumentValue = argumentValue;
        }

        /// <summary>
        /// Materializes the result as a new instance of the same type as the argumentValue
        /// </summary>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        /// <remarks>To update the argumentValue itselt, use WithRefresh() instead.</remarks>
        public ILink<TArgument> ToObject(RowOptions rowOptions = RowOptions.None)
        {
            return new ObjectMaterializer<TCommand, TParameter, TArgument>(this, rowOptions);
        }

        /// <summary>
        /// After executing the operation, refreshes the properties on the argumentValue by reading the updated values from the database.
        /// </summary>
        /// <returns></returns>
        public ILink<TArgument> WithRefresh()
        {
            return new RefreshMaterializer<TCommand, TParameter, TArgument>(this);
        }
    }
}
