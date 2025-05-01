using System.Data.Common;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

internal class RefreshMaterializer<TCommand, TParameter, TArgument> : Materializer<TCommand, TParameter, TArgument?>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TArgument : class
{
	readonly ObjectDbCommandBuilder<TCommand, TParameter, TArgument> m_CommandBuilder;
	readonly ClassMetadata m_ObjectMetadata;

	/// <summary>
	/// Initializes a new instance of the <see cref="RefreshMaterializer{TCommand, TParameter, TArgument}"/> class.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	public RefreshMaterializer(ObjectDbCommandBuilder<TCommand, TParameter, TArgument> commandBuilder)
		: base(commandBuilder)
	{
		m_ObjectMetadata = MetadataCache.GetMetadata<TArgument>();
		m_CommandBuilder = commandBuilder;
	}

	/// <summary>
	/// Returns the list of columns the result materializer would like to have.
	/// </summary>
	/// <returns></returns>
	public override IReadOnlyList<string> DesiredColumns() => m_ObjectMetadata.ColumnsFor;

	/// <summary>
	/// Execute the operation synchronously.
	/// </summary>
	/// <returns></returns>
	public override TArgument? Execute(object? state = null)
	{
		IReadOnlyDictionary<string, object?>? row = null;

		var executionToken = Prepare();
		var rowCount = executionToken.Execute(cmd =>
		{
			using (var reader = cmd.ExecuteReader(CommandBehavior))
			{
				row = reader.ReadDictionary();
				return (row != null ? 1 : 0) + reader.RemainingRowCount();
			}
		}, state);

		if (rowCount == 0 || row == null)
			throw new DataException("No rows were returned");
		else if (rowCount > 1)
			throw new DataException($"Expected 1 row but received {rowCount} rows.");

		//update the ArgumentValue with any new keys, calculated fields, etc.
		MaterializerUtilities.PopulateComplexObject(row, m_CommandBuilder.ArgumentValue, null, Converter);

		return m_CommandBuilder.ArgumentValue;
	}

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns></returns>
	public override async Task<TArgument?> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		IReadOnlyDictionary<string, object?>? row = null;

		var executionToken = Prepare();
		var rowCount = await executionToken.ExecuteAsync(async cmd =>
		 {
			 using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior, cancellationToken).ConfigureAwait(false))
			 {
				 row = await reader.ReadDictionaryAsync().ConfigureAwait(false);
				 return (row != null ? 1 : 0) + await reader.RemainingRowCountAsync().ConfigureAwait(false);
			 }
		 }, cancellationToken, state).ConfigureAwait(false);

		if (rowCount == 0 || row == null)
			throw new DataException("No rows were returned");
		else if (rowCount > 1)
			throw new DataException($"Expected 1 row but received {rowCount} rows.");

		//update the ArgumentValue with any new keys, calculated fields, etc.
		MaterializerUtilities.PopulateComplexObject(row, m_CommandBuilder.ArgumentValue, null, Converter);

		return m_CommandBuilder.ArgumentValue;
	}
}
