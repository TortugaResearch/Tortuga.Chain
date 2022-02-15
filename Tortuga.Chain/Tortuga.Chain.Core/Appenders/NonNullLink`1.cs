namespace Tortuga.Chain.Appenders
{
	/// <summary>
	/// Converts an null returning ILink into a non-null returning ILink.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks>If the previous link returns a null, this will throw an exception.</remarks>
	internal class NonNullLink<T> : Appender<T?, T> where T : class
	{
		/// <summary>
		///Initializes a new instance of the <see cref="NonNullLink{TResult}" /> class.
		/// </summary>
		/// <param name="previousLink">The previous link.</param>
		public NonNullLink(ILink<T?> previousLink) : base(previousLink)
		{
		}

		/// <summary>
		/// Execute the operation synchronously.
		/// </summary>
		/// <param name="state">User defined state, usually used for logging.</param>
		/// <remarks>If you don't override this method, it will call execute on the previous link.</remarks>
		public override T Execute(object? state = null)
		{
			var result = PreviousLink.Execute(state);
			if (result == null)
				throw new MissingDataException("An unexpected null was returned.");
			return result;
		}

		/// <summary>
		/// Execute the operation asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">User defined state, usually used for logging.</param>
		/// <returns></returns>
		/// <remarks>If you don't override this method, it will call execute on the previous link.</remarks>
		public override async Task<T> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
		{
			var result = await PreviousLink.ExecuteAsync(cancellationToken, state).ConfigureAwait(false);
			if (result == null)
				throw new MissingDataException("An unexpected null was returned.");
			return result;
		}
	}
}
