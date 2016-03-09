using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.SqlServer.Core;
namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// This is applied to command builders that support SqlDependency operations.
    /// </summary>
    public interface ISupportsChangeListener
    {
        /// <summary>
        /// Waits for change in the data that is returned by this operation.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns>Task that can be waited for.</returns>
        /// <remarks>This requires the use of SQL Dependency</remarks>
        Task WaitForChange(CancellationToken cancellationToken, object state = null);

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        SqlServerExecutionToken Prepare(Materializer<SqlCommand, SqlParameter> materializer);
    }
}
