using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tortuga.Anchor.Metadata;
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
        /// Executes the previous link and caches the result.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="policy">Optional cache policy.</param>
        public static ILink<TResult> Cache<TResult>(this ILink<TResult> previousLink, string cacheKey, CachePolicy policy = null)
        {
            return new CacheResultAppender<TResult>(previousLink, cacheKey, policy);
        }

        /// <summary>
        /// Executes the previous link and caches the result.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKeyFunction">Function to generate cache keys.</param>
        /// <param name="policy">Optional cache policy.</param>
        public static ILink<TResult> Cache<TResult>(this ILink<TResult> previousLink, Func<TResult, string> cacheKeyFunction, CachePolicy policy = null)
        {
            return new CacheResultAppender<TResult>(previousLink, cacheKeyFunction, policy);
        }

        /// <summary>
        /// Caches all items in the result set.
        /// </summary>
        /// <typeparam name="TCollection">The type of the t collection.</typeparam>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKeyFunction">Function to generate cache keys.</param>
        /// <param name="policy">Optional cache policy.</param>
        /// <returns>ILink&lt;TCollection&gt;.</returns>
        public static ILink<TCollection> CacheAllItems<TCollection, TItem>(this ILink<TCollection> previousLink, Func<TItem, string> cacheKeyFunction, CachePolicy policy = null)
            where TCollection : IEnumerable<TItem>
        {
            return new CacheAllItemsAppender<TCollection, TItem>(previousLink, cacheKeyFunction, policy);
        }

        /// <summary>
        /// Invalidates the cache.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        public static ILink<TResult> InvalidateCache<TResult>(this ILink<TResult> previousLink, string cacheKey)
        {
            return new InvalidateCacheAppender<TResult>(previousLink, cacheKey);
        }

        /// <summary>
        /// Invalidates the cache.
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns>ILink.</returns>
        public static ILink<int?> InvalidateCache(this IDbCommandBuilder commandBuilder, string cacheKey)
        {
            if (commandBuilder == null)
                throw new ArgumentNullException("commandBuilder", "commandBuilder is null.");
            return new InvalidateCacheAppender<int?>(commandBuilder.AsNonQuery(), cacheKey);
        }

        /// <summary>
        /// Reads the cache. If the value isn't found, the execute the previous link and cache the result.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="policy">Optional cache policy.</param>
        public static ILink<TResult> ReadOrCache<TResult>(this ILink<TResult> previousLink, string cacheKey, CachePolicy policy = null)
        {
            return new ReadOrCacheResultAppender<TResult>(previousLink, cacheKey, policy);
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
        /// Sets the strict mode, overriding the value set in the DataSource.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="strictMode">if set to <c>true</c> [strict mode].</param>
        /// <returns></returns>
        public static ILink<TResult> SetStrictMode<TResult>(this ILink<TResult> previousLink, bool strictMode)
        {
            return new StrictModeAppender<TResult>(previousLink, strictMode);
        }

        /// <summary>
        /// Joins a set of child objects to their parent objects.
        /// </summary>
        /// <typeparam name="T1">The type of the parent object.</typeparam>
        /// <typeparam name="T2">The type of the child object.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="joinExpression">The expression used to test of a parent and child match.</param>
        /// <param name="targetCollectionExpression">The expression to get the collection on the parent to add the child to.</param>
        /// <param name="joinOptions">The join options.</param>
        public static ILink<List<T1>> Join<T1, T2>(this ILink<Tuple<List<T1>, List<T2>>> previousLink, Func<T1, T2, bool> joinExpression, Func<T1, ICollection<T2>> targetCollectionExpression, JoinOptions joinOptions = JoinOptions.None)
        {
            return new ExpressionJoinAppender<T1, T2>(previousLink, joinExpression, targetCollectionExpression, joinOptions);
        }

        /// <summary>
        /// Joins a set of child objects to their parent objects.
        /// </summary>
        /// <typeparam name="T1">The type of the parent object.</typeparam>
        /// <typeparam name="T2">The type of the child object.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="joinExpression">The expression used to test of a parent and child match.</param>
        /// <param name="targetCollectionName">The name of the collection property on the parent to add the child to.</param>
        /// <param name="joinOptions">The join options.</param>
        public static ILink<List<T1>> Join<T1, T2>(this ILink<Tuple<List<T1>, List<T2>>> previousLink, Func<T1, T2, bool> joinExpression, string targetCollectionName, JoinOptions joinOptions = JoinOptions.None)
        {
            return new ExpressionJoinAppender<T1, T2>(previousLink, joinExpression, targetCollectionName, joinOptions);
        }


        /// <summary>
        /// Joins a set of child objects to their parent objects.
        /// </summary>
        /// <typeparam name="T1">The type of the parent object.</typeparam>
        /// <typeparam name="T2">The type of the child object.</typeparam>
        /// <typeparam name="TKey">The type of the primary/foreign key.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="primaryKeyExpression">The expression to get the primary key from the parent object.</param>
        /// <param name="foreignKeyExpression">The expression to get the foreign key from the child object.</param>
        /// <param name="targetCollectionExpression">The expression to get the collection on the parent to add the child to.</param>
        /// <param name="joinOptions">The join options.</param>
        public static ILink<List<T1>> Join<T1, T2, TKey>(this ILink<Tuple<List<T1>, List<T2>>> previousLink, Func<T1, TKey> primaryKeyExpression, Func<T2, TKey> foreignKeyExpression, Func<T1, ICollection<T2>> targetCollectionExpression, JoinOptions joinOptions = JoinOptions.None)
        {
            return new KeyJoinAppender<T1, T2, TKey>(previousLink, primaryKeyExpression, foreignKeyExpression, targetCollectionExpression, joinOptions);
        }

        /// <summary>
        /// Joins a set of child objects to their parent objects.
        /// </summary>
        /// <typeparam name="T1">The type of the parent object.</typeparam>
        /// <typeparam name="T2">The type of the child object.</typeparam>
        /// <typeparam name="TKey">The type of the primary/foreign key.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="primaryKeyExpression">The expression to get the primary key from the parent object.</param>
        /// <param name="foreignKeyExpression">The expression to get the foreign key from the child object.</param>
        /// <param name="targetCollectionName">The name of the collection property on the parent to add the child to.</param>
        /// <param name="joinOptions">The join options.</param>
        public static ILink<List<T1>> Join<T1, T2, TKey>(this ILink<Tuple<List<T1>, List<T2>>> previousLink, Func<T1, TKey> primaryKeyExpression, Func<T2, TKey> foreignKeyExpression, string targetCollectionName, JoinOptions joinOptions = JoinOptions.None)
        {
            return new KeyJoinAppender<T1, T2, TKey>(previousLink, primaryKeyExpression, foreignKeyExpression, targetCollectionName, joinOptions);
        }

        /// <summary>
        /// Joins a set of child objects to their parent objects.
        /// </summary>
        /// <typeparam name="T1">The type of the parent object.</typeparam>
        /// <typeparam name="T2">The type of the child object.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="primaryKeyName">The name of the property used to get the primary key from the parent object.</param>
        /// <param name="foreignKeyName">The name of the property used to get the foreign key from the child object.</param>
        /// <param name="targetCollectionName">The name of the collection property on the parent to add the child to.</param>
        /// <param name="joinOptions">The join options.</param>
        public static ILink<List<T1>> Join<T1, T2>(this ILink<Tuple<List<T1>, List<T2>>> previousLink, string primaryKeyName, string foreignKeyName, string targetCollectionName, JoinOptions joinOptions = JoinOptions.None)
        {
            var keyType = MetadataCache.GetMetadata(typeof(T1)).Properties[primaryKeyName].PropertyType;
            var methodType = typeof(CommonAppenders).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Where(m => m.Name == "Join_Helper").Single();
            var genericMethod = methodType.MakeGenericMethod(typeof(T1), typeof(T2), keyType);

            return (ILink<List<T1>>)genericMethod.Invoke(null, new object[] { previousLink, primaryKeyName, foreignKeyName, targetCollectionName, joinOptions });
        }

        /// <summary>
        /// Joins a set of child objects to their parent objects.
        /// </summary>
        /// <typeparam name="T1">The type of the parent object.</typeparam>
        /// <typeparam name="T2">The type of the child object.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="keyName">The name of the property used to get the primary key from the parent object and the foreign key from the child object.</param>
        /// <param name="targetCollectionName">The name of the collection property on the parent to add the child to.</param>
        /// <param name="joinOptions">The join options.</param>
        public static ILink<List<T1>> Join<T1, T2>(this ILink<Tuple<List<T1>, List<T2>>> previousLink, string keyName, string targetCollectionName, JoinOptions joinOptions = JoinOptions.None)
        {
            var keyType = MetadataCache.GetMetadata(typeof(T1)).Properties[keyName].PropertyType;
            var methodType = typeof(CommonAppenders).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Where(m => m.Name == "Join_Helper").Single();
            var genericMethod = methodType.MakeGenericMethod(typeof(T1), typeof(T2), keyType);

            return (ILink<List<T1>>)genericMethod.Invoke(null, new object[] { previousLink, keyName, keyName, targetCollectionName, joinOptions });
        }

        internal static ILink<List<T1>> Join_Helper<T1, T2, TKey>(ILink<Tuple<List<T1>, List<T2>>> previousLink, string primaryKeyName, string foreignKeyName, string targetCollectionName, JoinOptions joinOptions)
        {
            return new KeyJoinAppender<T1, T2, TKey>(previousLink, primaryKeyName, foreignKeyName, targetCollectionName, joinOptions);
        }

        /// <summary>
        /// Joins a set of child objects to their parent objects.
        /// </summary>
        /// <typeparam name="T1">The type of the parent object.</typeparam>
        /// <typeparam name="T2">The type of the child object.</typeparam>
        /// <typeparam name="TKey">The type of the primary/foreign key.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="primaryKeyName">The name of the property used to get the primary key from the parent object.</param>
        /// <param name="foreignKeyName">The name of the property used to get the foreign key from the child object.</param>
        /// <param name="targetCollectionName">The name of the collection property on the parent to add the child to.</param>
        /// <param name="joinOptions">The join options.</param>
        public static ILink<List<T1>> Join<T1, T2, TKey>(this ILink<Tuple<List<T1>, List<T2>>> previousLink, string primaryKeyName, string foreignKeyName, string targetCollectionName, JoinOptions joinOptions = JoinOptions.None)
        {
            return new KeyJoinAppender<T1, T2, TKey>(previousLink, primaryKeyName, foreignKeyName, targetCollectionName, joinOptions);
        }

        /// <summary>
        /// Joins a set of child objects to their parent objects.
        /// </summary>
        /// <typeparam name="T1">The type of the parent object.</typeparam>
        /// <typeparam name="T2">The type of the child object.</typeparam>
        /// <typeparam name="TKey">The type of the primary/foreign key.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="keyName">The name of the property used to get the primary key from the parent object and the foreign key from the child object.</param>
        /// <param name="targetCollectionName">The name of the collection property on the parent to add the child to.</param>
        /// <param name="joinOptions">The join options.</param>
        public static ILink<List<T1>> Join<T1, T2, TKey>(this ILink<Tuple<List<T1>, List<T2>>> previousLink, string keyName, string targetCollectionName, JoinOptions joinOptions = JoinOptions.None)
        {
            if (string.IsNullOrEmpty(keyName))
                throw new ArgumentException("keyName is null or empty.", "keyName");
            //other parameters are checked by the constructor.

            return new KeyJoinAppender<T1, T2, TKey>(previousLink, keyName, keyName, targetCollectionName, joinOptions);
        }
    }
}



