using System.Runtime.Caching;

namespace Tortuga.Chain.Core;

/// <summary>
/// Class ObjectCacheAdapter.
/// </summary>
/// <seealso cref="ICacheAdapter" />
public class ObjectCacheAdapter : ICacheAdapter
{
	readonly ObjectCache m_ObjectCache;

	/// <summary>
	/// Initializes a new instance of the <see cref="ObjectCacheAdapter"/> class.
	/// </summary>
	/// <param name="objectCache">The object cache.</param>
	public ObjectCacheAdapter(ObjectCache objectCache)
	{
		m_ObjectCache = objectCache;
	}

	/// <summary>
	/// Occurs when cache is cleared.
	/// </summary>
	public event EventHandler<CacheClearedEventArgs>? CacheCleared;

	/// <summary>
	/// Occurs when cache invalidated for a specific key.
	/// </summary>
	public event EventHandler<CacheInvalidatedEventArgs>? CacheInvalidated;

	/// <summary>
	/// Occurs when the cache is read.
	/// </summary>
	public event EventHandler<CacheReadEventArgs>? CacheRead;

	/// <summary>
	/// Occurs when the cache is written to.
	/// </summary>
	public event EventHandler<CacheWrittenEventArgs>? CacheWritten;

	/// <summary>
	/// Clears the cache.
	/// </summary>
	public void Clear()
	{
		var memoryCache = m_ObjectCache as MemoryCache;
		memoryCache?.Trim(100); //this wont' actually trim 100%, but it helps
		foreach (var item in m_ObjectCache)
			m_ObjectCache.Remove(item.Key);

		CacheCleared?.Invoke(this, new());
	}

	/// <summary>
	/// Clears the cache asynchronously.
	/// </summary>
	/// <returns>Task.</returns>
	public Task ClearAsync()
	{
		Clear();
		CacheCleared?.Invoke(this, new());
		return Task.CompletedTask;
	}

	/// <summary>
	/// Invalidates the cache.
	/// </summary>
	/// <param name="cacheKey">The cache key.</param>
	/// <exception cref="ArgumentException"></exception>
	public void Invalidate(string cacheKey)
	{
		if (string.IsNullOrEmpty(cacheKey))
			throw new ArgumentException($"{nameof(cacheKey)} is null or empty.", nameof(cacheKey));

		m_ObjectCache.Remove(cacheKey);
		CacheInvalidated?.Invoke(this, new(cacheKey));
	}

	/// <summary>
	/// Invalidates the cache asynchronously.
	/// </summary>
	/// <param name="cacheKey">The cache key.</param>
	/// <returns>Task.</returns>
	public Task InvalidateAsync(string cacheKey)
	{
		if (string.IsNullOrEmpty(cacheKey))
			throw new ArgumentException($"{nameof(cacheKey)} is null or empty.", nameof(cacheKey));

		m_ObjectCache.Remove(cacheKey);
		CacheInvalidated?.Invoke(this, new(cacheKey));
		return Task.CompletedTask;
	}

#nullable disable

	/// <summary>
	/// Tries the read from cache.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="result">The result.</param>
	/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public bool TryRead<T>(string cacheKey, out T result)
	{
		if (string.IsNullOrEmpty(cacheKey))
			throw new ArgumentException($"{nameof(cacheKey)} is null or empty.", nameof(cacheKey));

		var cacheItem = m_ObjectCache.GetCacheItem(cacheKey, null);
		if (cacheItem == null)
		{
			result = default;
			CacheRead?.Invoke(this, new(cacheKey, false));
			return false;
		}

		//Nulls can't be stored in a cache, so we simulate it using NullObject.Default.
		if (cacheItem.Value == NullObject.Default)
		{
			result = default;
			CacheRead?.Invoke(this, new(cacheKey, true));
			return true;
		}

		if (cacheItem.Value is not T)
			throw new InvalidOperationException($"Cache is corrupted. Cache Key \"{cacheKey}\" is a {cacheItem.Value.GetType().Name} not a {typeof(T).Name}");

		result = (T)cacheItem.Value;

		CacheRead?.Invoke(this, new(cacheKey, true));
		return true;
	}

#nullable restore

	/// <summary>
	/// try read from cache as an asynchronous operation.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <returns>Task&lt;Tuple&lt;System.Boolean, System.Object&gt;&gt;.</returns>
	public Task<CacheReadResult<T>> TryReadAsync<T>(string cacheKey)
	{
		var result2 = TryRead(cacheKey, out T result);
		return Task.FromResult(new CacheReadResult<T>(result2, result));
	}

	/// <summary>
	/// Writes to cache.
	/// </summary>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="value">The value.</param>
	/// <param name="policy">The policy.</param>
	/// <exception cref="ArgumentException"></exception>
	public void Write(string cacheKey, object? value, CachePolicy? policy)
	{
		if (string.IsNullOrEmpty(cacheKey))
			throw new ArgumentException($"{nameof(cacheKey)} is null or empty.", nameof(cacheKey));

		//Nulls can't be stored in a cache, so we simulate it using NullObject.Default.
		value ??= NullObject.Default;

		var mappedPolicy = new CacheItemPolicy();
		if (policy != null)
		{
			if (policy.AbsoluteExpiration.HasValue)
				mappedPolicy.AbsoluteExpiration = policy.AbsoluteExpiration.Value;
			if (policy.SlidingExpiration.HasValue)
				mappedPolicy.SlidingExpiration = policy.SlidingExpiration.Value;
		}

		m_ObjectCache.Set(new CacheItem(cacheKey, value), mappedPolicy);
		CacheWritten?.Invoke(this, new(cacheKey));
	}

	/// <summary>
	/// Writes to cache asynchronously.
	/// </summary>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="value">The value.</param>
	/// <param name="policy">The policy.</param>
	/// <returns>
	/// Task.
	/// </returns>
	public Task WriteAsync(string cacheKey, object? value, CachePolicy? policy)
	{
		Write(cacheKey, value, policy);
		return Task.CompletedTask;
	}

	/// <summary>
	/// Class NullObject is used to store nulls in a cache that doesn't natively support it.
	/// </summary>
	class NullObject
	{
		public static readonly NullObject Default = new();

		NullObject()
		{
		}
	}
}
