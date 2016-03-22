using System;
using System.IO;
using Tortuga.Chain.Appenders;

#if !WINDOWS_UWP
using System.Collections.Generic;
using System.Runtime.Caching;
using Tortuga.Chain.CommandBuilders;
#endif

namespace Tortuga.Chain
{
    /// <summary>
    /// Class CommonAppenders.
    /// </summary>
    public static class CommonAppenders
    {
#if !WINDOWS_UWP
        /// <summary>
        /// Executes the previous link and caches the result.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="regionName">Optional name of the cache region. WARNING: The default cache does not support region names.</param>
        /// <param name="policy">Optional cache policy.</param>
        public static ILink<TResult> Cache<TResult>(this ILink<TResult> previousLink, string cacheKey, string regionName = null, CacheItemPolicy policy = null)
        {
            return new CacheResultAppender<TResult>(previousLink, cacheKey, regionName, policy);
        }

        /// <summary>
        /// Executes the previous link and caches the result.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKeyFunction">Function to generate cache keys.</param>
        /// <param name="regionNameFunction">Optional function to generate region names.</param>
        /// <param name="policy">Optional cache policy.</param>
        public static ILink<TResult> Cache<TResult>(this ILink<TResult> previousLink, Func<TResult, string> cacheKeyFunction, Func<TResult, string> regionNameFunction = null, CacheItemPolicy policy = null)
        {
            return new CacheResultAppender<TResult>(previousLink, cacheKeyFunction, regionNameFunction, policy);
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
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="regionName">Optional name of the cache region. WARNING: The default cache does not support region names.</param>
        public static ILink<TResult> InvalidateCache<TResult>(this ILink<TResult> previousLink, string cacheKey, string regionName = null)
        {
            return new InvalidateCacheAppender<TResult>(previousLink, cacheKey, regionName);
        }

        /// <summary>
        /// Invalidates the cache.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="regionName">Optional name of the cache region. WARNING: The default cache does not support region names.</param>
        public static ILink InvalidateCache(this ILink previousLink, string cacheKey, string regionName = null)
        {
            return new InvalidateCacheAppender(previousLink, cacheKey, regionName);
        }

        /// <summary>
        /// Invalidates the cache.
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="regionName">Optional name of the cache region. WARNING: The default cache does not support region names.</param>
        /// <returns>ILink.</returns>
        public static ILink InvalidateCache(this IDbCommandBuilder commandBuilder, string cacheKey, string regionName = null)
        {
            if (commandBuilder == null)
                throw new ArgumentNullException("commandBuilder", "commandBuilder is null.");
            return new InvalidateCacheAppender(commandBuilder.AsNonQuery(), cacheKey, regionName);
        }

        /// <summary>
        /// Reads the cache. If the value isn't found, the execute the previous link and cache the result.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="regionName">Optional name of the cache region. WARNING: The default cache does not support region names.</param>
        /// <param name="policy">Optional cache policy.</param>
        public static ILink<TResult> ReadOrCache<TResult>(this ILink<TResult> previousLink, string cacheKey, string regionName = null, CacheItemPolicy policy = null)
        {
            return new ReadOrCacheResultAppender<TResult>(previousLink, cacheKey, regionName, policy);
        }
#endif

        /// <summary>
        /// Adds DB Command tracing. Information is send to the Debug stream.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        public static ILink WithTracingToDebug(this ILink previousLink)
        {
            return new TraceAppender(previousLink);
        }

        /// <summary>
        /// Adds DB Command tracing. Information is send to the Debug stream.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        public static ILink<TResult> WithTracingToDebug<TResult>(this ILink<TResult> previousLink)
        {
            return new TraceAppender<TResult>(previousLink);
        }

#if !WINDOWS_UWP
        /// <summary>
        /// Adds DB Command tracing. Information is send to the Debug stream.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        public static ILink WithTracingToConsole(this ILink previousLink)
        {
            return new TraceAppender(previousLink, Console.Out);
        }

        /// <summary>
        /// Adds DB Command tracing. Information is send to the Debug stream.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        public static ILink<TResult> WithTracingToConsole<TResult>(this ILink<TResult> previousLink)
        {
            return new TraceAppender<TResult>(previousLink, Console.Out);
        }
#endif

        /// <summary>
        /// Adds DB Command tracing. Information is send to the Debug stream.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>ILink.</returns>
        public static ILink WithTracing(this ILink previousLink, TextWriter stream)
        {
            return new TraceAppender(previousLink, stream);
        }


        /// <summary>
        /// Adds DB Command tracing. Information is send to the Debug stream.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>ILink&lt;TResult&gt;.</returns>
        public static ILink<TResult> WithTracing<TResult>(this ILink<TResult> previousLink, TextWriter stream)
        {
            return new TraceAppender<TResult>(previousLink, stream);
        }

        /// <summary>
        /// Sets the command timeout, overriding the value set in the DataSource.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>ILink&lt;TResult&gt;.</returns>
        public static ILink<TResult> SetTimeout<TResult>(this ILink<TResult> previousLink, TimeSpan timeout)
        {
            return new TimeoutAppender<TResult>(previousLink, timeout);
        }

        /// <summary>
        /// Sets the command timeout, overriding the value set in the DataSource.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>ILink.</returns>
        public static ILink SetTimeout(this ILink previousLink, TimeSpan timeout)
        {
            return new TimeoutAppender(previousLink, timeout);
        }
    }
}
