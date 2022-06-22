namespace Tortuga.Chain.Appenders;

/// <summary>
/// Executes the previous link and caches the result.
/// </summary>
/// <typeparam name="TResult">The type of the t result type.</typeparam>
internal sealed class CacheResultAppender<TResult> : Appender<TResult>, ICacheLink<TResult>
{
	readonly string? m_CacheKey;
	readonly Func<TResult, string>? m_CacheKeyFunction;
	readonly CachePolicy? m_Policy;
	string? m_ActualCacheKey;

	/// <summary>
	/// Initializes a new instance of the <see cref="CacheResultAppender{TResult}" /> class.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="cacheKeyFunction">Function to generate cache keys.</param>
	/// <param name="policy">Optional cache policy.</param>
	/// <exception cref="ArgumentNullException">cacheKeyFunction</exception>
	public CacheResultAppender(ILink<TResult> previousLink, Func<TResult, string> cacheKeyFunction, CachePolicy? policy = null) : base(previousLink)
	{
		m_CacheKeyFunction = cacheKeyFunction ?? throw new ArgumentNullException(nameof(cacheKeyFunction), $"{nameof(cacheKeyFunction)} is null.");
		m_Policy = policy;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CacheResultAppender{TResult}" /> class.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="policy">Optional cache policy.</param>
	public CacheResultAppender(ILink<TResult> previousLink, string cacheKey, CachePolicy? policy) : base(previousLink)
	{
		if (previousLink == null)
			throw new ArgumentNullException(nameof(previousLink), $"{nameof(previousLink)} is null.");
		if (string.IsNullOrEmpty(cacheKey))
			throw new ArgumentException($"{nameof(cacheKey)} is null or empty.", nameof(cacheKey));

		m_Policy = policy;
		m_CacheKey = cacheKey;
	}

	/// <summary>
	/// Execute the operation synchronously.
	/// </summary>
	/// <param name="state">User defined state, usually used for logging.</param>
	public override TResult Execute(object? state = null)
	{
		var result = PreviousLink.Execute(state);

		m_ActualCacheKey = m_CacheKey ?? m_CacheKeyFunction!(result);
		DataSource.Cache.Write(m_ActualCacheKey, result, m_Policy);

		return result;
	}

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns></returns>
	public override async Task<TResult> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		var result = await PreviousLink.ExecuteAsync(state).ConfigureAwait(false);

		m_ActualCacheKey = m_CacheKey ?? m_CacheKeyFunction!(result);
		await DataSource.Cache.WriteAsync(m_ActualCacheKey, result, m_Policy).ConfigureAwait(false);

		return result;
	}

	void ICacheLink<TResult>.Invalidate()
	{
		if (m_ActualCacheKey != null)
			DataSource.Cache.Invalidate(m_ActualCacheKey);
	}
}
