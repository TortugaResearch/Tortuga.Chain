#if !WINDOWS_UWP
using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.DataSources;
namespace Tortuga.Chain.Appenders
{

    /// <summary>
    /// Reads the cache. If the value isn't found, the execute the previous link and cache the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the t result type.</typeparam>
    /// <remarks>If this successfully reads from the cache, it will prevent prior links from executing.</remarks>
    internal sealed class ReadOrCacheResultAppender<TResult> : Appender<TResult>
    {
        private readonly string m_CacheKey;
        private readonly CacheItemPolicy m_Policy;
        private readonly string m_RegionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOrCacheResultAppender{TResult}" /> class.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="regionName">Optional name of the cache region. WARNING: The default cache does not support region names.</param>
        /// <param name="policy">Optional cache policy.</param>
        public ReadOrCacheResultAppender(ILink<TResult> previousLink, string cacheKey, string regionName, CacheItemPolicy policy) : base(previousLink)
        {
            if (previousLink == null)
                throw new ArgumentNullException("previousLink", "previousLink is null.");
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
        public override TResult Execute(object state = null)
        {
            TResult result;
            if (PreviousLink.DataSource.TryReadFromCache(m_CacheKey, m_RegionName, out result))
                return result;

            result = PreviousLink.Execute(state);

            DataSource.WriteToCache(new CacheItem(m_CacheKey, result, m_RegionName), m_Policy);

            return result;
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<TResult> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            TResult result;
            if (DataSource.TryReadFromCache(m_CacheKey, m_RegionName, out result))
                return result;

            result = await PreviousLink.ExecuteAsync(state).ConfigureAwait(false);

            DataSource.WriteToCache(new CacheItem(m_CacheKey, result, m_RegionName), m_Policy);

            return result;
        }

    }

}
#endif