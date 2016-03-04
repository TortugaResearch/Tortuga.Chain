using System.Threading;
using System.Threading.Tasks;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// This is used for materializers that return a value.
    /// </summary>

    public interface IMaterializer<T>
    {
        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        T Execute(object state = null);

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        Task<T> ExecuteAsync(object state = null);

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        Task<T> ExecuteAsync(CancellationToken cancellationToken, object state = null);

    }
}