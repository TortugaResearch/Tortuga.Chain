using Tortuga.Chain.Core;

namespace Tortuga.Chain.Appenders;


/// <summary>
/// Class TagAppender.
/// </summary>
internal sealed class TagAppender<TResult> : Appender<TResult>
{
	readonly string m_Message;

	/// <summary>
	/// Initializes a new instance of the <see cref="Appender{TResult}" /> class.
	/// </summary>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="message">The message to prepend to the SQL.</param>
	public TagAppender(ILink<TResult> previousLink, string message)
		: base(previousLink)
	{
		m_Message = message;
	}

	/// <summary>
	/// Override this if you want to examine or modify the DBCommand before it is executed.
	/// </summary>
	/// <param name="e">The <see cref="CommandBuiltEventArgs" /> instance containing the event data.</param>
	/// <exception cref="ArgumentNullException">nam - f(e), "e is</exception>
	protected override void OnCommandBuilt(CommandBuiltEventArgs e)
	{
		if (e == null)
			throw new ArgumentNullException(nameof(e), $"{nameof(e)} is null.");

		var commentString = DataSource.DatabaseMetadata.CommentString;

		if (commentString != null)
			e.Command.CommandText = $"{commentString} {m_Message}\r\n{e.Command.CommandText}";
	}
}
