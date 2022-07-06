using System.Data.Common;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// This class represents result materializers that read from a single column and return a ICollection.
/// </summary>
public abstract class CollectionColumnMaterializer<TCommand, TParameter, TCollection, TResult> : SingleColumnMaterializer<TCommand, TParameter, TCollection>
where TCommand : DbCommand
where TParameter : DbParameter
where TCollection : ICollection<TResult>, new()
{
	static readonly bool s_AllowNulls = MetadataCache.GetMetadata<TResult>().IsNullable;
	static bool m_AllowNulls;
	readonly ListOptions m_ListOptions;

	/// <summary>
	/// Initializes a new instance of the <see cref="CollectionColumnMaterializer{TCommand, TParameter, TCollection, TResult}" /> class.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <param name="columnName">Name of the desired column.</param>
	/// <param name="listOptions">The list options.</param>
	/// <param name="allowNulls">If allowNulls is set, the list will allow or prohibit nulls. If not set, infer this value from TResult.</param>
	protected CollectionColumnMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName, ListOptions listOptions, bool? allowNulls = null)
		: base(commandBuilder, columnName)
	{
		m_ListOptions = listOptions;
		m_AllowNulls = allowNulls ?? s_AllowNulls;
	}

	/// <summary>
	/// Execute the operation synchronously.
	/// </summary>
	/// <returns></returns>
	public sealed override TCollection Execute(object? state = null)
	{
		var result = new TCollection();

		Prepare().Execute(cmd =>
		{
			using (var reader = cmd.ExecuteReader(CommandBehavior))
			{
				if (reader.FieldCount > 1 && !m_ListOptions.HasFlag(ListOptions.IgnoreExtraColumns))
				{
					throw new UnexpectedDataException($"Expected one column but found {reader.FieldCount} columns");
				}

				var columnCount = m_ListOptions.HasFlag(ListOptions.FlattenExtraColumns) ? reader.FieldCount : 1;
				var discardNulls = m_ListOptions.HasFlag(ListOptions.DiscardNulls);
				var rowCount = 0;
				while (reader.Read())
				{
					rowCount++;
					for (var i = 0; i < columnCount; i++)
					{
						if (!reader.IsDBNull(i))
							result.Add(ReadValue(reader, i));
						else if (!discardNulls)
						{
							if (m_AllowNulls)
								result.Add(default!); //This will be a null if TResult is nullable.
							else
								throw new MissingDataException("Unexpected null value");
						}
					}
				}
				return rowCount;
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
	public sealed override async Task<TCollection> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		var result = new TCollection();

		await Prepare().ExecuteAsync(async cmd =>
		{
			using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior, cancellationToken).ConfigureAwait(false))
			{
				if (reader.FieldCount > 1 && !m_ListOptions.HasFlag(ListOptions.IgnoreExtraColumns))
				{
					throw new UnexpectedDataException($"Expected one column but found {reader.FieldCount} columns");
				}

				var columnCount = m_ListOptions.HasFlag(ListOptions.FlattenExtraColumns) ? reader.FieldCount : 1;
				var discardNulls = m_ListOptions.HasFlag(ListOptions.DiscardNulls);

				var rowCount = 0;
				while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
				{
					rowCount++;
					for (var i = 0; i < columnCount; i++)
					{
						if (!reader.IsDBNull(i))
							result.Add(ReadValue(reader, i));
						else if (!discardNulls)
						{
							if (m_AllowNulls)
								result.Add(default!); //This will be a null if TResult is nullable.
							else
								throw new MissingDataException("Unexpected null value");
						}
					}
				}
				return rowCount;
			}
		}, cancellationToken, state).ConfigureAwait(false);

		return result;
	}

	/// <summary>
	/// Reads the value.
	/// </summary>
	/// <param name="reader">The active data reader.</param>
	/// <param name="ordinal">The ordinal.</param>
	/// <returns>TResult.</returns>
	private protected abstract TResult ReadValue(DbDataReader reader, int ordinal);
}