namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This represents command builder that operate on single object parameters and performs an insert or update operation.
    /// </summary>
    /// <typeparam name="TArgument">The type of the argument.</typeparam>
    public interface IUpsertCommandBuilder<TArgument> : IObjectDbCommandBuilder<TArgument>
    {
        /// <summary>
        /// Matches the on an alternate column(s). Normally matches need to be on the primary key.
        /// </summary>
        /// <param name="columnNames">The column names that form a unique key.</param>
        /// <returns></returns>
        IUpsertCommandBuilder<TArgument> MatchOn(params string[] columnNames);
    }
}
