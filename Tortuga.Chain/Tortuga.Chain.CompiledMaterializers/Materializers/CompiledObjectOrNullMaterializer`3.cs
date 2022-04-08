using System.Data.Common;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

internal sealed class CompiledObjectOrNullMaterializer<TCommand, TParameter, TObject> : Materializer<TCommand, TParameter, TObject?>
where TCommand : DbCommand
where TObject : class, new()
where TParameter : DbParameter
{
	private RowOptions m_RowOptions;

	public CompiledObjectOrNullMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, RowOptions rowOptions) : base(commandBuilder)
	{
		m_RowOptions = rowOptions;

		if (rowOptions.HasFlag(RowOptions.InferConstructor))
			throw new NotSupportedException("Compiled materializers do not support non-default constructors");
	}

	/// <summary>
	/// Returns the list of columns the materializer would like to have.
	/// </summary>
	/// <returns></returns>
	public override IReadOnlyList<string> DesiredColumns() => MetadataCache.GetMetadata(typeof(TObject)).ColumnsFor;

	public override TObject? Execute(object? state = null)
	{
		var result = new List<TObject>();

		var executionToken = Prepare();
		executionToken.Execute(cmd =>
		{
			using (var reader = cmd.ExecuteReader(CommandBehavior))
			{
				var factory = CompiledMaterializers.CreateBuilder<TObject>(DataSource, cmd.CommandText, reader, CommandBuilder.TryGetNonNullableColumns());
				while (reader.Read())
					result.Add(factory(reader));
				return result.Count;
			}
		}, state);

		if (result.Count == 0)
		{
			if (!m_RowOptions.HasFlag(RowOptions.PreventEmptyResults))
				return null;
			else
				throw new MissingDataException($"No rows were returned and {nameof(RowOptions)}.{nameof(RowOptions.PreventEmptyResults)} was enabled.");
		}
		else if (result.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
		{
			throw new UnexpectedDataException($"Expected 1 row but received {result.Count} rows. Use {nameof(RowOptions)}.{nameof(RowOptions.DiscardExtraRows)} to suppress this error.");
		}

		return result.First();
	}

	public override async Task<TObject?> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		var result = new List<TObject>();

		var executionToken = Prepare();
		await executionToken.ExecuteAsync(async cmd =>
		{
			using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior, cancellationToken).ConfigureAwait(false))
			{
				var factory = CompiledMaterializers.CreateBuilder<TObject>(DataSource, cmd.CommandText, reader, CommandBuilder.TryGetNonNullableColumns());
				while (await reader.ReadAsync().ConfigureAwait(false))
					result.Add(factory(reader));
				return result.Count;
			}
		}, cancellationToken, state).ConfigureAwait(false);

		if (result.Count == 0)
		{
			if (!m_RowOptions.HasFlag(RowOptions.PreventEmptyResults))
				return null;
			else
				throw new MissingDataException($"No rows were returned and {nameof(RowOptions)}.{nameof(RowOptions.PreventEmptyResults)} was enabled.");
		}
		else if (result.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
		{
			throw new UnexpectedDataException($"Expected 1 row but received {result.Count} rows. Use {nameof(RowOptions)}.{nameof(RowOptions.DiscardExtraRows)} to suppress this error.");
		}

		return result.First();
	}
}
