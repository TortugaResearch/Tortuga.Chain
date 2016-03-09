using System;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;
namespace Tortuga.Chain.Appenders
{
    /// <summary>
    /// Causes the cache to be invalidated when this operation is executed.
    /// </summary>
    /// <typeparam name="TResultType"></typeparam>
    public class InvalidateCacheAppender<TResultType> : Appender<TResultType>
    {
        private readonly string m_CacheKey;
        private readonly string m_RegionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidateCacheAppender{TResultType}"/> class.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="regionName">Optional name of the cache region. WARNING: The default cache does not support region names.</param>
        /// <exception cref="ArgumentNullException">previousLink;previousLink is null.</exception>
        /// <exception cref="ArgumentException">cacheKey is null or empty.;cacheKey</exception>
        public InvalidateCacheAppender(ILink<TResultType> previousLink, string cacheKey, string regionName) : base(previousLink)
        {
            if (string.IsNullOrEmpty(cacheKey))
                throw new ArgumentException("cacheKey is null or empty.", "cacheKey");

            m_RegionName = regionName;
            m_CacheKey = cacheKey;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        public override TResultType Execute(object state = null)
        {
            PreviousLink.DataSource.InvalidateCache(m_CacheKey, m_RegionName);

            return PreviousLink.Execute(state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<TResultType> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            PreviousLink.DataSource.InvalidateCache(m_CacheKey, m_RegionName);

            return await PreviousLink.ExecuteAsync(state).ConfigureAwait(false);
        }
    }
}
