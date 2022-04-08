using Tortuga.Chain.Core;

namespace Tortuga.Chain.Appenders;

/// <summary>
/// Class StrictModeAppender.
/// Implements the <see cref="Tortuga.Chain.Appenders.Appender{TResult}" />
/// </summary>
/// <typeparam name="TResult">The type of the t result.</typeparam>
/// <seealso cref="Tortuga.Chain.Appenders.Appender{TResult}" />
internal sealed class StrictModeAppender<TResult> : Appender<TResult>
{
	readonly bool m_StrictMode;

	/// <summary>
	/// Initializes a new instance of the <see cref="StrictModeAppender{TResult}"/> class.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="strictMode">if set to <c>true</c> [strict mode].</param>
	public StrictModeAppender(ILink<TResult> previousLink, bool strictMode)
		: base(previousLink)
	{
		m_StrictMode = strictMode;
	}

	protected override void OnExecutionTokenPreparing(ExecutionTokenPreparingEventArgs e)
	{
		if (e == null)
			throw new ArgumentNullException(nameof(e), $"{nameof(e)} is null.");
		e.CommandBuilder.StrictMode = m_StrictMode;
	}
}
