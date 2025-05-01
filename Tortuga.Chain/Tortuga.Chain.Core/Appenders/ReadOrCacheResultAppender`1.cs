﻿namespace Tortuga.Chain.Appenders;

/// <summary>
/// Reads the cache. If the value isn't found, the execute the previous link and cache the result.
/// </summary>
/// <typeparam name="TResult">The type of the t result type.</typeparam>
/// <remarks>If this successfully reads from the cache, it will prevent prior links from executing.</remarks>
internal sealed class ReadOrCacheResultAppender<TResult> : Appender<TResult>, ICacheLink<TResult>
{
	readonly string m_CacheKey;
	readonly CachePolicy? m_Policy;

	/// <summary>
	/// Initializes a new instance of the <see cref="ReadOrCacheResultAppender{TResult}" /> class.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="policy">Optional cache policy.</param>
	/// <exception cref="ArgumentException">cacheKey</exception>
	public ReadOrCacheResultAppender(ILink<TResult> previousLink, string cacheKey, CachePolicy? policy) : base(previousLink)
	{
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
		if (DataSource.Cache.TryRead(m_CacheKey, out TResult result))
			return result;

		result = PreviousLink.Execute(state);

		DataSource.Cache.Write(m_CacheKey, result, m_Policy);

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
		var temp = await DataSource.Cache.TryReadAsync<TResult>(m_CacheKey).ConfigureAwait(false);
		if (temp.KeyFound)
			return temp.Value;

		var result = await PreviousLink.ExecuteAsync(cancellationToken, state).ConfigureAwait(false);

		await DataSource.Cache.WriteAsync(m_CacheKey, result, m_Policy).ConfigureAwait(false);

		return result;
	}

	void ICacheLink<TResult>.Invalidate() => DataSource.Cache.Invalidate(m_CacheKey);
}
