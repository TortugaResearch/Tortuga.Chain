using System.Threading;
using System.Threading.Tasks;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// This indicates the associated operation should be executed without returning a result set.
    /// </summary>
    public interface IMaterializer
    {
        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        void Execute(object state = null);

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        Task ExecuteAsync(object state = null);

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        Task ExecuteAsync(CancellationToken cancellationToken, object state = null);
    }
}