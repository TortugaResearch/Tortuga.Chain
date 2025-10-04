using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Appenders;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain;

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
	public static ICacheLink<TResult> Cache<TResult>(this ILink<TResult> previousLink, string cacheKey, CachePolicy? policy = null)
	{
		return new CacheResultAppender<TResult>(previousLink, cacheKey, policy);
	}

	/// <summary>
	/// Executes the previous link and caches the result.
	/// </summary>
	/// <typeparam name="TResult">The type of the t result.</typeparam>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="cacheKeyFunction">Function to generate cache keys.</param>
	/// <param name="policy">Optional cache policy.</param>
	/// <returns>ICacheLink&lt;TResult&gt;.</returns>
	public static ICacheLink<TResult> Cache<TResult>(this ILink<TResult> previousLink, Func<TResult, string> cacheKeyFunction, CachePolicy? policy = null)
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
	public static ICacheLink<TCollection> CacheAllItems<TCollection, TItem>(this ILink<TCollection> previousLink, Func<TItem, string> cacheKeyFunction, CachePolicy? policy = null)
		where TCollection : IEnumerable<TItem>
	{
		return new CacheAllItemsAppender<TCollection, TItem>(previousLink, cacheKeyFunction, policy);
	}

	/// <summary>
	/// Invalidates the cache for the indicated key.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="cacheKey">The cache key.</param>
	public static ILink<TResult> InvalidateCache<TResult>(this ILink<TResult> previousLink, string cacheKey)
	{
		return new InvalidateCacheAppender<TResult>(previousLink, cacheKey);
	}

	/// <summary>
	/// Clears the cache.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	public static ILink<TResult> ClearCache<TResult>(this ILink<TResult> previousLink)
	{
		return new ClearCacheAppender<TResult>(previousLink);
	}

	/// <summary>
	/// Invalidates the cache for the indicated key.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <param name="cacheKey">The cache key.</param>
	/// <returns>ILink.</returns>
	public static ILink<int?> InvalidateCache(this IDbCommandBuilder commandBuilder, string cacheKey)
	{
		if (commandBuilder == null)
			throw new ArgumentNullException(nameof(commandBuilder), $"{nameof(commandBuilder)} is null.");
		return new InvalidateCacheAppender<int?>(commandBuilder.AsNonQuery(), cacheKey);
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
	public static ILink<List<T1>> Join<T1, T2, TKey>(this ILink<Tuple<List<T1>, List<T2>>> previousLink,
		Func<T1, TKey> primaryKeyExpression,
		Func<T2, TKey> foreignKeyExpression,
		Func<T1, ICollection<T2>> targetCollectionExpression,
		JoinOptions joinOptions = JoinOptions.None)
		where TKey : notnull
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
	public static ILink<List<T1>> Join<T1, T2, TKey>(this ILink<Tuple<List<T1>, List<T2>>> previousLink,
		Func<T1, TKey> primaryKeyExpression,
		Func<T2, TKey> foreignKeyExpression,
		string targetCollectionName,
		JoinOptions joinOptions = JoinOptions.None)
		where TKey : notnull
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
		var keyType = MetadataCache.GetMetadata<T1>().Properties[primaryKeyName].PropertyType;
		var methodType = typeof(CommonAppenders).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(m => m.Name == "Join_Helper");
		var genericMethod = methodType.MakeGenericMethod(typeof(T1), typeof(T2), keyType);

		return (ILink<List<T1>>)genericMethod.Invoke(null, new object[] { previousLink, primaryKeyName, foreignKeyName, targetCollectionName, joinOptions })!;
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
		var keyType = MetadataCache.GetMetadata<T1>().Properties[keyName].PropertyType;
		var methodType = typeof(CommonAppenders).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(m => m.Name == "Join_Helper");
		var genericMethod = methodType.MakeGenericMethod(typeof(T1), typeof(T2), keyType);

		return (ILink<List<T1>>)genericMethod.Invoke(null, new object[] { previousLink, keyName, keyName, targetCollectionName, joinOptions })!;
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
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	public static ILink<List<T1>> Join<T1, T2, TKey>(this ILink<Tuple<List<T1>, List<T2>>> previousLink, string primaryKeyName, string foreignKeyName, string targetCollectionName, JoinOptions joinOptions = JoinOptions.None)
		where TKey : notnull
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
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	public static ILink<List<T1>> Join<T1, T2, TKey>(this ILink<Tuple<List<T1>, List<T2>>> previousLink, string keyName, string targetCollectionName, JoinOptions joinOptions = JoinOptions.None)
		where TKey : notnull
	{
		if (string.IsNullOrEmpty(keyName))
			throw new ArgumentException("keyName is null or empty.", nameof(keyName));
		//other parameters are checked by the constructor.

		return new KeyJoinAppender<T1, T2, TKey>(previousLink, keyName, keyName, targetCollectionName, joinOptions);
	}

	/// <summary>
	/// Ensures that a null will never be returned.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	/// <remarks>If the previous link returns a null, an exception is thrown instead.</remarks>
	public static ILink<TResult> NeverNull<TResult>(this ILink<TResult?> previousLink) where TResult : class
	{
		return new NonNullLink<TResult>(previousLink);
	}

	/// <summary>
	/// Ensures that a null will never be returned.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	/// <remarks>If the previous link returns a null, an exception is thrown instead.</remarks>
	public static ILink<TResult> NeverNull<TResult>(this ILink<TResult?> previousLink) where TResult : struct
	{
		return new ValueNonNullLink<TResult>(previousLink);
	}

	/// <summary>
	/// Reads the cache. If the value isn't found, the execute the previous link and cache the result.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="policy">Optional cache policy.</param>
	public static ILink<TResult> ReadOrCache<TResult>(this ILink<TResult> previousLink, string cacheKey, CachePolicy? policy = null)
	{
		return new ReadOrCacheResultAppender<TResult>(previousLink, cacheKey, policy);
	}

	/// <summary>
	/// Sets the sequential access mode, overriding the value set in the DataSource.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="sequentialAccessMode">if set to <c>true</c> enable sequential access.</param>
	/// <returns></returns>
	public static ILink<TResult> SetSequentialAccessMode<TResult>(this ILink<TResult> previousLink, bool sequentialAccessMode)
	{
		return new SequentialAccessModeAppender<TResult>(previousLink, sequentialAccessMode);
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
	/// Prepends a comment to SQL statement. This is used mostly for logging.
	/// </summary>
	/// <typeparam name="TResult">The type of the t result.</typeparam>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="message">The message to prepend to the SQL.</param>
	/// <returns>ILink&lt;TResult&gt;.</returns>
	/// <remarks>The message should be a constant to avoid execution plan cache issues.</remarks>
	public static ILink<TResult> Tag<TResult>(this ILink<TResult> previousLink, string message)
	{
		return new TagAppender<TResult>(previousLink, message);
	}

	/// <summary>
	/// Prepends a comment to SQL statement with the file name, member name, and line number. This is used mostly for logging.
	/// </summary>
	/// <typeparam name="TResult">The type of the t result.</typeparam>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="callerFilePath">The caller file path.</param>
	/// <param name="callerMemberName">Name of the caller member.</param>
	/// <param name="callerLineNumber">The caller line number.</param>
	/// <returns>ILink&lt;TResult&gt;.</returns>
	/// <remarks>Tip: Use the same name for the file as your class.</remarks>>
	public static ILink<TResult> Tag<TResult>(this ILink<TResult> previousLink, [CallerFilePath] string callerFilePath = null!, [CallerMemberName] string callerMemberName = null!, [CallerLineNumber] int callerLineNumber = default)
	{
		var callerTypeName = Path.GetFileNameWithoutExtension(callerFilePath);
		var message = $"{callerTypeName}.{callerMemberName}:{callerLineNumber}";

		return new TagAppender<TResult>(previousLink, message);
	}

	/// <summary>
	/// Performs a transformation on a result.
	/// </summary>
	public static ILink<TResult> Transform<TSource, TResult>(this ILink<TSource> previousLink, Func<TSource, TResult> transformation)
	{
		return new TransformLink<TSource, TResult>(previousLink, transformation);
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
	/// Adds DB Command tracing. Information is send to the Debug stream.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	public static ILink<TResult> WithTracingToConsole<TResult>(this ILink<TResult> previousLink)
	{
		return new TraceAppender<TResult>(previousLink, Console.Out);
	}

	/// <summary>
	/// Adds DB Command tracing. Information is send to the Debug stream.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	public static ILink<TResult> WithTracingToDebug<TResult>(this ILink<TResult> previousLink)
	{
		return new TraceAppender<TResult>(previousLink);
	}

	internal static ILink<List<T1>> Join_Helper<T1, T2, TKey>(ILink<Tuple<List<T1>, List<T2>>> previousLink, string primaryKeyName, string foreignKeyName, string targetCollectionName, JoinOptions joinOptions)
		where TKey : notnull
	{
		return new KeyJoinAppender<T1, T2, TKey>(previousLink, primaryKeyName, foreignKeyName, targetCollectionName, joinOptions);
	}
}
