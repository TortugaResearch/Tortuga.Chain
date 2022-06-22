using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// Materializes the result set as an instance of the indicated type.
	/// </summary>
	/// <typeparam name="TCommand">The type of the t command type.</typeparam>
	/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
	/// <typeparam name="TObject">The type of the object returned.</typeparam>
	/// <seealso cref="Materializer{TCommand, TParameter, TTObject}"/>
	internal sealed class ObjectOrNullMaterializer<TCommand, TParameter, TObject> : ConstructibleMaterializer<TCommand, TParameter, TObject?, TObject>
		where TCommand : DbCommand
		where TParameter : DbParameter
		where TObject : class
	{
		readonly RowOptions m_RowOptions;

		/// <summary>
		/// Initializes a new instance of the <see
		/// cref="Tortuga.Chain.Materializers.ObjectMaterializer{TCommand, TParameter, TObject}"/> class.
		/// </summary>
		/// <param name="commandBuilder">The command builder.</param>
		/// <param name="rowOptions">The row options.</param>
		/// <exception cref="MappingException">
		/// Type {typeof(TObject).Name} has does not have any non-default constructors. or Type
		/// {typeof(TObject).Name} has more than one non-default constructor. Please use the
		/// WithConstructor method to specify which one to use.
		/// </exception>
		public ObjectOrNullMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, RowOptions rowOptions)
			: base(commandBuilder)
		{
			m_RowOptions = rowOptions;

			if (m_RowOptions.HasFlag(RowOptions.InferConstructor))
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
		/// <param name="state">User defined state, usually used for logging.</param>
		/// <returns>System.Nullable&lt;TObject&gt;.</returns>
		public override TObject? Execute(object? state = null)
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

			return ConstructObject(row, rowCount);
		}

		/// <summary>
		/// Execute the operation asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">User defined state, usually used for logging.</param>
		/// <returns></returns>
		public override async Task<TObject?> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
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

			return ConstructObject(row, rowCount);
		}

		TObject? ConstructObject(IReadOnlyDictionary<string, object?>? row, int? rowCount)
		{
			if (rowCount == 0 || row == null)
			{
				if (!m_RowOptions.HasFlag(RowOptions.PreventEmptyResults))
					return null;
				else
					throw new MissingDataException($"No rows were returned and {nameof(RowOptions)}.{nameof(RowOptions.PreventEmptyResults)} was enabled.");
			}
			else if (rowCount > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
			{
				throw new UnexpectedDataException($"Expected 1 row but received {rowCount} rows. If this was expected, use `RowOptions.DiscardExtraRows`.");
			}

			bool? populateProperties = m_RowOptions.HasFlag(RowOptions.WithProperties) ? true : null;
			return MaterializerUtilities.ConstructObject<TObject>(row, Constructor, populateProperties);
		}
	}
}
