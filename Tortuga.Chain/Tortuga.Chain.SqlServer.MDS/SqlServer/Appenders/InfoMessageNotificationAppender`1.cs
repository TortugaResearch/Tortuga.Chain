using Tortuga.Chain.Appenders;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.SqlServer.Appenders;

sealed internal class InfoMessageNotificationAppender<TResult> : Appender<TResult>
{
	readonly SqlInfoMessageEventHandler m_EventHandler;

	/// <summary>
	/// Initializes a new instance of the <see cref="Appender{TResult}" /> class.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="eventHandler">The event handler to fire when then associated SQL Dependency is fired..</param>
	public InfoMessageNotificationAppender(ILink<TResult> previousLink, SqlInfoMessageEventHandler eventHandler)
		: base(previousLink)
	{
		if (previousLink == null)
			throw new ArgumentNullException(nameof(previousLink), $"{nameof(previousLink)} is null.");
		if (eventHandler == null)
			throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");

		m_EventHandler = eventHandler;
	}

	[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "InfoMessageNotificationAppender")]
	protected override void OnExecutionTokenPrepared(ExecutionTokenPreparedEventArgs e)
	{
		if (e == null)
			throw new ArgumentNullException(nameof(e), $"{nameof(e)} is null.");

		var token = e.ExecutionToken as SqlServerCommandExecutionToken;
		if (token == null)
			throw new NotSupportedException($"This type of command builder does not support SQL Server Info Messages, which is required for the {nameof(InfoMessageNotificationAppender<TResult>)}.");
		token.AddInfoMessageEventListener(m_EventHandler);
	}
}
