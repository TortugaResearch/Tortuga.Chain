namespace Tortuga.Chain;

/// <summary>
/// Class CachePolicy.
/// </summary>
public class CachePolicy
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CachePolicy"/> class.
	/// </summary>
	/// <param name="absoluteExpiration">The absolute expiration.</param>
	public CachePolicy(DateTimeOffset absoluteExpiration)
	{
		AbsoluteExpiration = absoluteExpiration;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CachePolicy"/> class.
	/// </summary>
	/// <param name="slidingExpiration">The sliding expiration.</param>
	public CachePolicy(TimeSpan slidingExpiration)
	{
		SlidingExpiration = slidingExpiration;
	}

	/// <summary>
	/// Gets or sets the absolute expiration.
	/// </summary>
	/// <value>The absolute expiration.</value>
	public DateTimeOffset? AbsoluteExpiration { get; private set; }

	/// <summary>
	/// Gets or sets the sliding expiration.
	/// </summary>
	/// <value>The sliding expiration.</value>
	public TimeSpan? SlidingExpiration { get; private set; }
}
