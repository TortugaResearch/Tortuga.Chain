using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// This is used to return an IEnumerable/IAsynEnumerable stream of objects.
/// </summary>
internal sealed class ObjectStreamMaterializer<TCommand, TParameter, TObject> : ConstructibleMaterializer<TCommand, TParameter, ObjectStream<TObject>, TObject>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObject : class
{
	readonly CollectionOptions m_CollectionOptions;

	/// <summary>
	/// Initializes a new instance of the <see cref="CollectionMaterializer{TCommand, TParameter, TObject, TCollection}"/> class.
	/// </summary>
	/// <param name="commandBuilder">The associated operation.</param>
	/// <param name="collectionOptions">The collection options.</param>
	public ObjectStreamMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, CollectionOptions collectionOptions)
		: base(commandBuilder)
	{
		m_CollectionOptions = collectionOptions;

		if (m_CollectionOptions.HasFlag(CollectionOptions.InferConstructor))
			Constructor = InferConstructor();
	}

	/// <summary>
	/// Execute the operation synchronously.
	/// </summary>
	/// <returns></returns>
	public override ObjectStream<TObject> Execute(object? state = null)
	{
		StreamingObjectConstructor<TObject> result = null!;

		var connection = Prepare().ExecuteStream(cmd =>
		{
			result = cmd.ExecuteReader(CommandBehavior).AsObjectConstructor<TObject>(Constructor, CommandBuilder.TryGetNonNullableColumns(), Converter);
		}, state);

		return new ObjectStream<TObject>(result, connection);
	}

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns></returns>
	public override async Task<ObjectStream<TObject>> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		StreamingObjectConstructor<TObject> result = null!;

		var connection = await Prepare().ExecuteStreamAsync(async cmd =>
		{
			result = (await cmd.ExecuteReaderAsync(CommandBehavior, cancellationToken).ConfigureAwait(false)).AsObjectConstructor<TObject>(Constructor, CommandBuilder.TryGetNonNullableColumns(), Converter);
		}, cancellationToken, state).ConfigureAwait(false);

		return new ObjectStream<TObject>(result, connection);
	}
}
