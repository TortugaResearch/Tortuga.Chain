using System.Data.Common;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

internal sealed class CompiledCollectionMaterializer<TCommand, TParameter, TObject, TCollection> : Materializer<TCommand, TParameter, TCollection>
		where TCommand : DbCommand
		where TObject : class, new()
		where TCollection : ICollection<TObject>, new()
		where TParameter : DbParameter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CompiledCollectionMaterializer{TCommand, TParameter, TObject, TCollection}"/> class.
	/// </summary>
	/// <param name="commandBuilder">The associated operation.</param>
	/// <param name="collectionOptions">The collection options.</param>
	/// <exception cref="NotSupportedException">Compiled materializers do not support non-default constructors</exception>
	public CompiledCollectionMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, CollectionOptions collectionOptions)
		: base(commandBuilder)
	{
		if (collectionOptions.HasFlag(CollectionOptions.InferConstructor))
			throw new NotSupportedException("Compiled materializers do not support non-default constructors");
	}

	/// <summary>
	/// Returns the list of columns the materializer would like to have.
	/// </summary>
	/// <returns></returns>
	public override IReadOnlyList<string> DesiredColumns() => MetadataCache.GetMetadata<TObject>().ColumnsFor;

	public override TCollection Execute(object? state = null)
	{
		var result = new TCollection();
		Prepare().Execute(cmd =>
		{
			using (var reader = cmd.ExecuteReader(CommandBehavior))
			{
				var factory = CompiledMaterializers.CreateBuilder<TObject>(DataSource, cmd.CommandText, reader, CommandBuilder.TryGetNonNullableColumns());
				while (reader.Read())
					result.Add(factory(reader));
				return result.Count;
			}
		}, state);

		return result;
	}

	public override async Task<TCollection> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		var result = new TCollection();

		await Prepare().ExecuteAsync(async cmd =>
		{
			using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior, cancellationToken).ConfigureAwait(false))
			{
				var factory = CompiledMaterializers.CreateBuilder<TObject>(DataSource, cmd.CommandText, reader, CommandBuilder.TryGetNonNullableColumns());
				while (await reader.ReadAsync().ConfigureAwait(false))
					result.Add(factory(reader));
				return result.Count;
			}
		}, cancellationToken, state).ConfigureAwait(false);

		return result;
	}
}
