using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;
namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This allows executing command builders without returning anything.
    /// </summary>
    public interface IDbCommandBuilder
    {
        /// <summary>
        /// Indicates this operation has no result set.
        /// </summary>
        /// <returns></returns>
        ILink AsNonQuery();
        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        void Execute(object state = null);
        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns>Task.</returns>
        Task ExecuteAsync(object state = null);
        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns>Task.</returns>
        Task ExecuteAsync(CancellationToken cancellationToken, object state = null);
    }
}
