using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain
{
    /// <summary>
    /// This indicates that the data source provides minimal functionality.
    /// </summary>
    /// <remarks>Warning: This interface is meant to simulate multiple inheritance and work-around some issues with exposing generic types. Do not implement it in client code, as new methods will be added over time.</remarks>
    public interface IClass0DataSource : IDataSource
    {

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        IMultipleTableDbCommandBuilder Sql(string sqlStatement, object argumentValue);

    }
}
