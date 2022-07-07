namespace Tortuga.Chain.Core;

/// <summary>
/// Class CacheReadEventArgs.
/// Implements the <see cref="EventArgs" />
/// </summary>
/// <seealso cref="EventArgs" />
public class CacheReadEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CacheReadEventArgs"/> class.
	/// </summary>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="found">If the value was found in the cache to true.</param>
	public CacheReadEventArgs(string cacheKey, bool found)
	{
		CacheKey = cacheKey;
		Found = found;
	}

	/// <summary>
	/// Gets the cache key.
	/// </summary>
	/// <value>The cache key.</value>
	public string CacheKey { get; }

	/// <summary>
	/// Gets a value indicating whether the value was found in the cache.
	/// </summary>
	public bool Found { get; }
}
