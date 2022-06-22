using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer.Materializers;

internal class WaitForChangeMaterializer<TCommandBuilder> : Materializer<SqlCommand, SqlParameter>
		where TCommandBuilder : DbCommandBuilder<SqlCommand, SqlParameter>, ISupportsChangeListener
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Materializer{TCommand, TParameter}"/> class.
	/// </summary>
	/// <param name="commandBuilder">The associated command builder.</param>
	public WaitForChangeMaterializer(TCommandBuilder commandBuilder)
		: base(commandBuilder)
	{ }

	internal Task GenerateTask(CancellationToken cancellationToken, object? state)
	{
		var tcs = new TaskCompletionSource<SqlNotificationEventArgs>();

		var token = ((ISupportsChangeListener)CommandBuilder).Prepare(this);

		token.AddChangeListener((s, e) => tcs.TrySetResult(e));

		if (cancellationToken.CanBeCanceled)
			cancellationToken.Register(() => tcs.TrySetCanceled());

		token.ExecuteAsync(async cmd => await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false), cancellationToken, state);

		return tcs.Task;
	}
}
