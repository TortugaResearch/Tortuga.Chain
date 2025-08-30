using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// Materializes the result set as a TimeOnly.
/// </summary>
/// <typeparam name="TCommand">The type of the tt command type.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
internal sealed class TimeOnlyMaterializer<TCommand, TParameter> : ScalarMaterializer<TCommand, TParameter, TimeOnly> where TCommand : DbCommand
	where TParameter : DbParameter

{
	/// <summary>
	/// Initializes a new instance of the <see cref="TimeOnlyMaterializer{TCommand, TParameter}"/> class.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <param name="columnName">Name of the desired column.</param>
	public TimeOnlyMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName = null)
		: base(commandBuilder, columnName)
	{ }

	/// <summary>
	/// Execute the operation synchronously.
	/// </summary>
	/// <returns></returns>
	public override TimeOnly Execute(object? state = null)
	{
		object? temp = null;
		ExecuteCore(cmd => temp = cmd.ExecuteScalar(), state);
		if (temp == null || temp == DBNull.Value)
			throw new MissingDataException("Unexpected null result");

		return ConvertValue(temp);
	}

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns></returns>
	public override async Task<TimeOnly> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		object? temp = null;
		await ExecuteCoreAsync(async cmd => temp = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false), cancellationToken, state).ConfigureAwait(false);
		if (temp == null || temp == DBNull.Value)
			throw new MissingDataException("Unexpected null result");

		return ConvertValue(temp);
	}

	static TimeOnly ConvertValue(object temp)
	{
		return temp switch
		{
			TimeOnly timeOnly => timeOnly,
			TimeSpan timeSpan => TimeOnly.FromTimeSpan(timeSpan),
			DateTime dateTime => TimeOnly.FromDateTime(dateTime),
			DateTimeOffset dateTimeOffset => TimeOnly.FromDateTime(dateTimeOffset.DateTime),
			_ => (TimeOnly)temp,
		};
	}

}