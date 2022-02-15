namespace Tortuga.Chain.Appenders
{
	/// <summary>
	/// Performs a transformation on a result.
	/// </summary>
	internal class TransformLink<TSource, TResult> : Appender<TSource, TResult>
	{
		Func<TSource, TResult> m_Transformation;

		public TransformLink(ILink<TSource> previousLink, Func<TSource, TResult> transformation) : base(previousLink)
		{
			m_Transformation = transformation;
		}

		public override TResult Execute(object? state = null)
		{
			var result = PreviousLink.Execute(state);
			return m_Transformation(result);
		}

		public override async Task<TResult> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
		{
			var result = await PreviousLink.ExecuteAsync(cancellationToken, state).ConfigureAwait(false);
			return m_Transformation(result);
		}
	}
}
