using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain;

/// <summary>
/// Class ObjectStream.
/// Implements the <see cref="IEnumerator{TObject}" />
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <typeparam name="TObject">The type of the t object.</typeparam>
/// <seealso cref="IEnumerator{TObject}" />
/// <seealso cref="IDisposable" />
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "<Pending>")]
public sealed class ObjectStream<TObject> : IEnumerable<TObject>, IDisposable, IAsyncDisposable, IAsyncEnumerable<TObject>
	where TObject : class
{
	readonly StreamingCommandCompletionToken m_CompletionToken;
	readonly StreamingObjectConstructor<TObject> m_ObjectStream;
	bool m_AlreadyRead;

	internal ObjectStream(StreamingObjectConstructor<TObject> objectStream, StreamingCommandCompletionToken completionToken)
	{
		m_ObjectStream = objectStream;
		m_CompletionToken = completionToken;
	}

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	/// </summary>
	public void Dispose()
	{
		m_ObjectStream.Dispose();
		m_CompletionToken.Dispose();
	}

	/// <summary>
	/// Disposes the asynchronous.
	/// </summary>
	/// <returns>ValueTask.</returns>
	public async ValueTask DisposeAsync()
	{
		await m_ObjectStream.DisposeAsync().ConfigureAwait(false);
		await m_CompletionToken.DisposeAsync().ConfigureAwait(false);
	}

	/// <summary>
	/// Gets the asynchronous enumerator.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	public async IAsyncEnumerator<TObject> GetAsyncEnumerator(CancellationToken cancellationToken = default)
	{
		if (m_AlreadyRead)
			throw new InvalidOperationException("This class cannot be enumerated multiple times.");
		m_AlreadyRead = true;

		while (await m_ObjectStream.ReadAsync(cancellationToken).ConfigureAwait(false))
			yield return m_ObjectStream.Current!;
	}

	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>An enumerator that can be used to iterate through the collection.</returns>
	public IEnumerator<TObject> GetEnumerator()
	{
		if (m_AlreadyRead)
			throw new InvalidOperationException("This class cannot be enumerated multiple times.");
		m_AlreadyRead = true;

		while (m_ObjectStream.Read(out var item))
			yield return item;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
