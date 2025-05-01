using System.Collections.Immutable;

namespace Tortuga.Chain.Appenders;

/// <summary>
/// Caches each individual item in the collection.
/// Implements the <see cref="Tortuga.Chain.Appenders.Appender{TCollection}" />
/// Implements the <see cref="Tortuga.Chain.ICacheLink{TCollection}" />
/// </summary>
/// <typeparam name="TCollection">The type of the t collection.</typeparam>
/// <typeparam name="TItem">The type of the t item.</typeparam>
/// <seealso cref="Tortuga.Chain.Appenders.Appender{TCollection}" />
/// <seealso cref="Tortuga.Chain.ICacheLink{TCollection}" />
/// <remarks>This operation will not read from the cache.</remarks>
internal sealed class CacheAllItemsAppender<TCollection, TItem> : Appender<TCollection>, ICacheLink<TCollection>
	where TCollection : IEnumerable<TItem>
{
	readonly Func<TItem, string> m_CacheKeyFunction;
	readonly CachePolicy? m_Policy;

	ImmutableArray<string> m_ActualCacheKeys;

	/// <summary>
	/// Initializes a new instance of the <see cref="CacheAllItemsAppender{TCollection, TItem}" /> class.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="cacheKeyFunction">Function to generate cache keys.</param>
	/// <param name="policy">Optional cache policy.</param>
	/// <exception cref="ArgumentNullException">previousLink;previousLink is null.</exception>
	/// <exception cref="ArgumentException">cacheKey is null or empty.;cacheKey</exception>
	public CacheAllItemsAppender(ILink<TCollection> previousLink, Func<TItem, string> cacheKeyFunction, CachePolicy? policy = null) : base(previousLink)
	{
		m_CacheKeyFunction = cacheKeyFunction ?? throw new ArgumentNullException(nameof(cacheKeyFunction), $"{nameof(cacheKeyFunction)} is null.");
		m_Policy = policy;
	}

	/// <summary>
	/// Execute the operation synchronously.
	/// </summary>
	/// <param name="state">User defined state, usually used for logging.</param>
	public override TCollection Execute(object? state = null)
	{
		var result = PreviousLink.Execute(state);

		var cacheKeys = new List<string>();

		foreach (var item in result)
		{
			var cacheKey = m_CacheKeyFunction(item);
			DataSource.Cache.Write(cacheKey, item, m_Policy);
			cacheKeys.Add(cacheKey);
		}

		m_ActualCacheKeys = [.. cacheKeys];
		return result;
	}

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns></returns>
	public override async Task<TCollection> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		var result = await PreviousLink.ExecuteAsync(cancellationToken, state).ConfigureAwait(false);

		var cacheKeys = new List<string>();
		foreach (var item in result)
		{
			var cacheKey = m_CacheKeyFunction(item);
			await DataSource.Cache.WriteAsync(cacheKey, item, m_Policy).ConfigureAwait(false);
			cacheKeys.Add(cacheKey);
		}
		m_ActualCacheKeys = [.. cacheKeys];
		return result;
	}

	void ICacheLink<TCollection>.Invalidate()
	{
		foreach (var cacheKey in m_ActualCacheKeys)
			DataSource.Cache.Invalidate(cacheKey);
	}
}
