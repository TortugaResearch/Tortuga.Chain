namespace Tortuga.Chain.Appenders;

/// <summary>
/// Causes the cache to be invalidated when this operation is executed.
/// </summary>
/// <typeparam name="TResult"></typeparam>
internal sealed class InvalidateCacheAppender<TResult> : Appender<TResult>
{
	readonly string m_CacheKey;

	/// <summary>
	/// Initializes a new instance of the <see cref="InvalidateCacheAppender{TResult}"/> class.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="cacheKey">The cache key.</param>
	/// <exception cref="ArgumentNullException">previousLink;previousLink is null.</exception>
	/// <exception cref="ArgumentException">cacheKey is null or empty.;cacheKey</exception>
	public InvalidateCacheAppender(ILink<TResult> previousLink, string cacheKey) : base(previousLink)
	{
		if (string.IsNullOrEmpty(cacheKey))
			throw new ArgumentException($"{nameof(cacheKey)} is null or empty.", nameof(cacheKey));

		m_CacheKey = cacheKey;
	}

	/// <summary>
	/// Execute the operation synchronously.
	/// </summary>
	/// <param name="state">User defined state, usually used for logging.</param>
	public override TResult Execute(object? state = null)
	{
		DataSource.Cache.Invalidate(m_CacheKey);

		return PreviousLink.Execute(state);
	}

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns></returns>
	public override async Task<TResult> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		await DataSource.Cache.InvalidateAsync(m_CacheKey).ConfigureAwait(false);

		return await PreviousLink.ExecuteAsync(cancellationToken, state).ConfigureAwait(false);
	}
}
