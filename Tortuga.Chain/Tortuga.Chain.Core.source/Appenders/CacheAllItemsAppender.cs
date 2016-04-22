#if !WINDOWS_UWP
using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Appenders
{
    /// <summary>
    /// Caches each individual item in the collection.
    /// </summary>
    /// <remarks>This operation will not read from the cache.</remarks>
    internal sealed class CacheAllItemsAppender<TCollection, TItem> : Appender<TCollection>
        where TCollection : IEnumerable<TItem>
    {
        private readonly Func<TItem, string> m_CacheKeyFunction;
        private readonly CacheItemPolicy m_Policy;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheAllItemsAppender{TCollection, TItem}" /> class.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKeyFunction">Function to generate cache keys.</param>
        /// <param name="policy">Optional cache policy.</param>
        /// <exception cref="ArgumentNullException">previousLink;previousLink is null.</exception>
        /// <exception cref="ArgumentException">cacheKey is null or empty.;cacheKey</exception>
        public CacheAllItemsAppender(ILink<TCollection> previousLink, Func<TItem, string> cacheKeyFunction, CacheItemPolicy policy = null) : base(previousLink)
        {
            if (cacheKeyFunction == null)
                throw new ArgumentNullException("cacheKeyFunction", "cacheKeyFunction is null.");
            if (previousLink == null)
                throw new ArgumentNullException("previousLink", "previousLink is null.");

            m_CacheKeyFunction = cacheKeyFunction;
            m_Policy = policy;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        public override TCollection Execute(object state = null)
        {
            var result = PreviousLink.Execute(state);

            CacheItems(result);

            return result;
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<TCollection> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var result = await PreviousLink.ExecuteAsync(cancellationToken, state).ConfigureAwait(false);

            CacheItems(result);

            return result;
        }

        private void CacheItems(IEnumerable<TItem> list)
        {
            foreach (var item in list)
                ((DataSource)DataSource).WriteToCache(new CacheItem(m_CacheKeyFunction(item), item, null), m_Policy);
        }
    }
}
#endif