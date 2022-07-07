using System.Data.Common;
using System.Globalization;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// Materializes the result set as an char.
/// </summary>
/// <typeparam name="TCommand">The type of the t command type.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
internal sealed class CharMaterializer<TCommand, TParameter> : ScalarMaterializer<TCommand, TParameter, char> where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <param name="columnName">Name of the desired column.</param>
	public CharMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName = null)
		: base(commandBuilder, columnName)
	{ }

	/// <summary>
	/// Execute the operation synchronously.
	/// </summary>
	/// <returns></returns>
	public override char Execute(object? state = null)
	{
		object? temp = null;
		ExecuteCore(cmd => temp = cmd.ExecuteScalar(), state);

		return temp switch
		{
			null => throw new MissingDataException("Unexpected null result"),
			DBNull => throw new MissingDataException("Unexpected null result"),
			char c => c,
			string s => s.Length >= 1 ? s[0] : default,
			_ => Convert.ToChar(temp, CultureInfo.InvariantCulture),
		};
	}

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns></returns>
	public override async Task<char> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		object? temp = null;
		await ExecuteCoreAsync(async cmd => temp = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false), cancellationToken, state).ConfigureAwait(false);

		return temp switch
		{
			null => throw new MissingDataException("Unexpected null result"),
			DBNull => throw new MissingDataException("Unexpected null result"),
			char c => c,
			string s => s.Length >= 1 ? s[0] : default,
			_ => Convert.ToChar(temp, CultureInfo.InvariantCulture),
		};
	}
}
