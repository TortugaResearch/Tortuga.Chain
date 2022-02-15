using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// Materializes the result set as a collection of the indicated type.
	/// </summary>
	/// <typeparam name="TCommand">The type of the t command type.</typeparam>
	/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
	/// <typeparam name="TObject">The type of the object returned.</typeparam>
	/// <typeparam name="TCollection">The type of the collection.</typeparam>
	/// <seealso cref="Materializer{TCommand, TParameter, TCollection}"/>
	[SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
	internal sealed class CollectionMaterializer<TCommand, TParameter, TObject, TCollection> : ConstructibleMaterializer<TCommand, TParameter, TCollection, TObject>
		where TCommand : DbCommand
		where TParameter : DbParameter
		where TObject : class
		where TCollection : ICollection<TObject>, new()
	{
		readonly CollectionOptions m_CollectionOptions;

		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionMaterializer{TCommand,
		/// TParameter, TObject, TCollection}"/> class.
		/// </summary>
		/// <param name="commandBuilder">The associated operation.</param>
		/// <param name="collectionOptions">The collection options.</param>
		public CollectionMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, CollectionOptions collectionOptions)
			: base(commandBuilder)
		{
			m_CollectionOptions = collectionOptions;

			if (m_CollectionOptions.HasFlag(CollectionOptions.InferConstructor))
			{
				var constructors = ObjectMetadata.Constructors.Where(x => x.Signature.Length > 0).ToList();
				if (constructors.Count == 0)
					throw new MappingException($"Type {typeof(TObject).Name} has does not have any non-default constructors.");
				if (constructors.Count > 1)
					throw new MappingException($"Type {typeof(TObject).Name} has more than one non-default constructor. Please use the WithConstructor method to specify which one to use.");
				Constructor = constructors[0];
			}
		}

		/// <summary>
		/// Execute the operation synchronously.
		/// </summary>
		/// <returns></returns>
		public override TCollection Execute(object? state = null)
		{
			bool? populateProperties = m_CollectionOptions.HasFlag(CollectionOptions.WithProperties) ? true : null;
			var result = new TCollection();
			Prepare().Execute(cmd =>
			{
				using (var reader = cmd.ExecuteReader().AsObjectConstructor<TObject>(Constructor, CommandBuilder.TryGetNonNullableColumns(), populateProperties))
				{
					while (reader.Read(out var value))
						result.Add(value);
					return result.Count;
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
		public override async Task<TCollection> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
		{
			bool? populateProperties = m_CollectionOptions.HasFlag(CollectionOptions.WithProperties) ? true : null;
			var result = new TCollection();

			await Prepare().ExecuteAsync(async cmd =>
			{
				using (var reader = (await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false)).AsObjectConstructor<TObject>(Constructor, CommandBuilder.TryGetNonNullableColumns(), populateProperties))
				{
					while (await reader.ReadAsync().ConfigureAwait(false))
						result.Add(reader.Current!);
					return result.Count;
				}
			}, cancellationToken, state).ConfigureAwait(false);

			return result;
		}
	}
}
