namespace Tortuga.Chain.Core;

/// <summary>
/// Class CacheWrittenEventArgs.
/// Implements the <see cref="EventArgs" />
/// </summary>
/// <seealso cref="EventArgs" />
public class CacheWrittenEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CacheWrittenEventArgs"/> class.
	/// </summary>
	/// <param name="cacheKey">The cache key.</param>
	public CacheWrittenEventArgs(string cacheKey)
	{
		CacheKey = cacheKey;
	}

	/// <summary>
	/// Gets the cache key.
	/// </summary>
	/// <value>The cache key.</value>
	public string CacheKey { get; }
}
