using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Tortuga.Chain.Appenders;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain
{
    /// <summary>
    /// Class CommonAppenders.
    /// </summary>
    public static class CommonAppenders
    {
        /// <summary>
        /// Invalidates the cache.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="regionName">Optional name of the cache region. WARNING: The default cache does not support region names.</param>
        /// <param name="policy">Optional cache policy.</param>
        public static ILink<TResultType> InvalidateCache<TResultType>(this ILink<TResultType> previousLink, string cacheKey, string regionName = null, CacheItemPolicy policy = null)
        {
            return new InvalidateCacheAppender<TResultType>(previousLink, cacheKey, regionName, policy);
        }

        /// <summary>
        /// Invalidates the cache.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="regionName">Optional name of the cache region. WARNING: The default cache does not support region names.</param>
        /// <param name="policy">Optional cache policy.</param>
        public static ILink InvalidateCache(this ILink previousLink, string cacheKey, string regionName = null, CacheItemPolicy policy = null)
        {
            return new InvalidateCacheAppender(previousLink, cacheKey, regionName, policy);
        }

        /// <summary>
        /// Reads the cache. If the value isn't found, the execute the previous link and cache the result.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="regionName">Optional name of the cache region. WARNING: The default cache does not support region names.</param>
        /// <param name="policy">Optional cache policy.</param>
        public static ILink<TResultType> ReadOrCache<TResultType>(this ILink<TResultType> previousLink, string cacheKey, string regionName = null, CacheItemPolicy policy = null)
        {
            return new ReadOrCacheResultAppender<TResultType>(previousLink, cacheKey, regionName, policy);
        }

        /// <summary>
        /// Executes the previous link and caches the result.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="regionName">Optional name of the cache region. WARNING: The default cache does not support region names.</param>
        /// <param name="policy">Optional cache policy.</param>
        public static ILink<TResultType> Cache<TResultType>(this ILink<TResultType> previousLink, string cacheKey, string regionName = null, CacheItemPolicy policy = null)
        {
            return new CacheResultAppender<TResultType>(previousLink, cacheKey, regionName, policy);
        }

        /// <summary>
        /// Executes the previous link and caches the result.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKeyFunction">Function to generate cache keys.</param>
        /// <param name="regionNameFunction">Optional function to generate region names.</param>
        /// <param name="policy">Optional cache policy.</param>
        public static ILink<TResultType> Cache<TResultType>(this ILink<TResultType> previousLink, Func<TResultType, string> cacheKeyFunction, Func<TResultType, string> regionNameFunction = null, CacheItemPolicy policy = null)
        {
            return new CacheResultAppender<TResultType>(previousLink, cacheKeyFunction, regionNameFunction, policy);
        }

        /// <summary>
        /// Caches all items in the result set.
        /// </summary>
        /// <typeparam name="TCollection">The type of the t collection.</typeparam>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKeyFunction">Function to generate cache keys.</param>
        /// <param name="regionNameFunction">Optional function to generate region names.</param>
        /// <param name="policy">Optional cache policy.</param>
        /// <returns>ILink&lt;TCollection&gt;.</returns>
        public static ILink<TCollection> CacheAllItems<TCollection, TItem>(this ILink<TCollection> previousLink, Func<TItem, string> cacheKeyFunction, Func<TItem, string> regionNameFunction = null, CacheItemPolicy policy = null)
            where TCollection : IEnumerable<TItem>
        {
            return new CacheAllItemsAppender<TCollection, TItem>(previousLink, cacheKeyFunction, regionNameFunction, policy);
        }

        /// <summary>
        /// Invalidates the cache.
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="regionName">Optional name of the cache region. WARNING: The default cache does not support region names.</param>
        /// <param name="policy">Optional cache policy.</param>
        /// <returns>ILink.</returns>
        public static ILink InvalidateCache(this IDbCommandBuilder commandBuilder, string cacheKey, string regionName = null, CacheItemPolicy policy = null)
        {
            return new InvalidateCacheAppender(commandBuilder.AsNonQuery(), cacheKey, regionName, policy);
        }
    }
}
