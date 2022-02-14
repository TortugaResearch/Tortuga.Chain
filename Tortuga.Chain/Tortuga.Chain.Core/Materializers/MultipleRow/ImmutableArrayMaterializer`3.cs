using System.Collections.Immutable;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// Materializes the result set as an immutable array of the indicated type.
	/// </summary>
	/// <typeparam name="TCommand">The type of the t command type.</typeparam>
	/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
	/// <typeparam name="TObject">The type of the object returned.</typeparam>
	/// <seealso cref="Materializer{TCommand, TParameter, TCollection}"/>
	[SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
	internal sealed class ImmutableArrayMaterializer<TCommand, TParameter, TObject> : ConstructibleMaterializer<TCommand, TParameter, ImmutableArray<TObject>, TObject>
		where TCommand : DbCommand
		where TParameter : DbParameter
		where TObject : class
	{
		readonly CollectionOptions m_CollectionOptions;

		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionMaterializer{TCommand,
		/// TParameter, TObject, TCollection}"/> class.
		/// </summary>
		/// <param name="commandBuilder">The associated operation.</param>
		/// <param name="collectionOptions">The collection options.</param>
		public ImmutableArrayMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, CollectionOptions collectionOptions)
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
		public override ImmutableArray<TObject> Execute(object? state = null)
		{
			ImmutableArray<TObject> result = default;
			Prepare().Execute(cmd =>
			{
				using (var reader = cmd.ExecuteReader().AsObjectConstructor<TObject>(Constructor, CommandBuilder.TryGetNonNullableColumns()))
				{
					result = reader.ToObjects().ToImmutableArray();
					return result.Length;
				}
			}, state);

			return result;
		}

		/// <summary>
		/// Execute the operation asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">User defined state, usually used for logging.</param>
		/// <returns></returns>
		public override async Task<ImmutableArray<TObject>> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
		{
			ImmutableArray<TObject> result = default;
			await Prepare().ExecuteAsync(async cmd =>
			{
				using (var reader = (await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false)).AsObjectConstructor<TObject>(Constructor, CommandBuilder.TryGetNonNullableColumns()))
				{
					result = (await reader.ToListAsync().ConfigureAwait(false)).ToImmutableArray();
					return result.Length;
				}
			}, cancellationToken, state).ConfigureAwait(false);

			return result;
		}
	}
}
