using System.Data.Common;
using System.Globalization;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// Materializes the result set as an integer.
/// </summary>
/// <typeparam name="TCommand">The type of the tt command type.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
internal sealed class UInt64Materializer<TCommand, TParameter> : ScalarMaterializer<TCommand, TParameter, ulong> where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UInt64Materializer{TCommand, TParameter}"/> class.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <param name="columnName">Name of the desired column.</param>
	public UInt64Materializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName = null)
		: base(commandBuilder, columnName)
	{ }

	/// <summary>
	/// Execute the operation synchronously.
	/// </summary>
	/// <returns></returns>
	public override ulong Execute(object? state = null)
	{
		object? temp = null;
		ExecuteCore(cmd => temp = cmd.ExecuteScalar(), state);
		if (temp == DBNull.Value)
			throw new MissingDataException("Unexpected null result");

		return Convert.ToUInt64(temp, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns></returns>
	public override async Task<ulong> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		object? temp = null;
		await ExecuteCoreAsync(async cmd => temp = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false), cancellationToken, state).ConfigureAwait(false);
		if (temp == DBNull.Value)
			throw new MissingDataException("Unexpected null result");

		return Convert.ToUInt64(temp, CultureInfo.InvariantCulture);
	}
}
