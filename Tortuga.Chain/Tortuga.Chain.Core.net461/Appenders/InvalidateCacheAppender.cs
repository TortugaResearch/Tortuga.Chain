using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace Tortuga.Chain.Appenders
{
    /// <summary>
    /// Causes the cache to be invalidated when this operation is executed.
    /// </summary>
    public class InvalidateCacheAppender : Appender
    {
        private readonly string m_CacheKey;

        private readonly string m_RegionName;
        private readonly CacheItemPolicy m_Policy;



        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidateCacheAppender{TResultType}"/> class.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="regionName">Optional name of the cache region. WARNING: The default cache does not support region names.</param>
        /// <param name="policy">Optional cache policy.</param>
        /// <exception cref="ArgumentNullException">previousLink;previousLink is null.</exception>
        /// <exception cref="ArgumentException">cacheKey is null or empty.;cacheKey</exception>
        public InvalidateCacheAppender(ILink previousLink, string cacheKey, string regionName, CacheItemPolicy policy) : base(previousLink)
        {
            if (string.IsNullOrEmpty(cacheKey))
                throw new ArgumentException("cacheKey is null or empty.", "cacheKey");

            m_Policy = policy;
            m_RegionName = regionName;
            m_CacheKey = cacheKey;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        public override void Execute(object state = null)
        {
            DataSource.InvalidateCache(m_CacheKey, m_RegionName);

            PreviousLink.Execute(state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override Task ExecuteAsync(object state = null)
        {
            return ExecuteAsync(CancellationToken.None, state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            DataSource.InvalidateCache(m_CacheKey, m_RegionName);

            await PreviousLink.ExecuteAsync(state).ConfigureAwait(false);
        }
    }
}
