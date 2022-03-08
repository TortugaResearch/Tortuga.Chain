using Tortuga.Chain.Core;

namespace Tortuga.Chain.Appenders
{
	/// <summary>
	/// Class SequentialAccessModeAppender.
	/// Implements the <see cref="Tortuga.Chain.Appenders.Appender{TResult}" />
	/// </summary>
	/// <typeparam name="TResult">The type of the t result.</typeparam>
	/// <seealso cref="Tortuga.Chain.Appenders.Appender{TResult}" />
	internal sealed class SequentialAccessModeAppender<TResult> : Appender<TResult>
	{
		readonly bool m_SequentialAccessMode;

		/// <summary>
		/// Initializes a new instance of the <see cref="StrictModeAppender{TResult}"/> class.
		/// </summary>
		/// <param name="previousLink">The previous link.</param>
		/// <param name="sequentialAccessMode">if set to <c>true</c> enable sequential access.</param>
		public SequentialAccessModeAppender(ILink<TResult> previousLink, bool sequentialAccessMode)
			: base(previousLink)
		{
			m_SequentialAccessMode = sequentialAccessMode;
		}

		protected override void OnExecutionTokenPreparing(ExecutionTokenPreparingEventArgs e)
		{
			if (e == null)
				throw new ArgumentNullException(nameof(e), $"{nameof(e)} is null.");
			e.CommandBuilder.SequentialAccessMode = m_SequentialAccessMode;
		}
	}
}
