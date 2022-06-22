namespace Tortuga.Chain.Core;

/// <summary>
/// Generic caching adapter.
/// </summary>
public interface ICacheAdapter
{
	/// <summary>
	/// Clears the cache.
	/// </summary>
	void Clear();

	/// <summary>
	/// Clears the cache asynchronously.
	/// </summary>
	/// <returns>Task.</returns>
	Task ClearAsync();

	/// <summary>
	/// Invalidates the cache.
	/// </summary>
	/// <param name="cacheKey">The cache key.</param>
	void Invalidate(string cacheKey);

	/// <summary>
	/// Invalidates the cache asynchronously.
	/// </summary>
	/// <param name="cacheKey">The cache key.</param>
	/// <returns>Task.</returns>
	Task InvalidateAsync(string cacheKey);

	/// <summary>
	/// Tries the read from cache.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="result">The result.</param>
	/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
	bool TryRead<T>(string cacheKey, out T result);

	/// <summary>
	/// Tries the read from cache asynchronously.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="cacheKey">The cache key.</param>
	/// <returns>Task&lt;Tuple&lt;System.Boolean, System.Object&gt;&gt;.</returns>
	Task<CacheReadResult<T>> TryReadAsync<T>(string cacheKey);

	/// <summary>
	/// Writes to cache.
	/// </summary>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="value">The value.</param>
	/// <param name="policy">The policy.</param>
	void Write(string cacheKey, object? value, CachePolicy? policy);

	/// <summary>
	/// Writes to cache asynchronously.
	/// </summary>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="value">The value.</param>
	/// <param name="policy">The policy.</param>
	/// <returns>
	/// Task.
	/// </returns>
	Task WriteAsync(string cacheKey, object? value, CachePolicy? policy);
}
