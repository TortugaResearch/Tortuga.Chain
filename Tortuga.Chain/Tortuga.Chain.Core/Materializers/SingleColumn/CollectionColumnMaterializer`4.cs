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
				var rowCount = 0;
				var matchingColumns = CalculateColumnsToRead(reader);
				var discardNulls = m_ListOptions.HasFlag(ListOptions.DiscardNulls);

				while (reader.Read())
				{
					rowCount++;
					foreach (var ordinal in matchingColumns)
					{
						if (!reader.IsDBNull(ordinal))
							result.Add(ReadValue(reader, ordinal));
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
				var rowCount = 0;
				var matchingColumns = CalculateColumnsToRead(reader);
				var discardNulls = m_ListOptions.HasFlag(ListOptions.DiscardNulls);

				while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
				{
					rowCount++;
					foreach (var ordinal in matchingColumns)
					{
						if (!reader.IsDBNull(ordinal))
							result.Add(ReadValue(reader, ordinal));
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

	List<int> CalculateColumnsToRead(DbDataReader reader)
	{
		List<int> matchingColumns;
		if (ColumnName != null && reader.FieldCount > 1)//special handling for stored procedures
		{
			var columns = new (int ordinal, string columnName)[reader.FieldCount];
			for (var i = 0; i < reader.FieldCount; i++)
				columns[i] = (i, reader.GetName(i));

			//We only care about columns that match the desired column name.
			matchingColumns = columns.Where(c => ColumnName.Equals(c.columnName, StringComparison.InvariantCultureIgnoreCase)).Select(c => c.ordinal).ToList();

			if (matchingColumns.Count > 1)
			{
				if (!m_ListOptions.HasFlag(ListOptions.IgnoreExtraColumns))

					throw new UnexpectedDataException($"Found more than one column named '{ColumnName}'. If this was expected, use ListOptions.IgnoreExtraColumns or  ListOptions.FlattenExtraColumns.");

				//If we aren't flattening, then only take the first column with a matching name.
				if (!m_ListOptions.HasFlag(ListOptions.FlattenExtraColumns))
					matchingColumns = matchingColumns.Take(1).ToList();
			}
			else if (matchingColumns.Count == 0)
			{
				throw new MappingException($"The column name '{ColumnName}' was provided, but none of the {reader.FieldCount} columns match that name.");
			}
		}
		else //normal path
		{
			if (reader.FieldCount > 1 && !m_ListOptions.HasFlag(ListOptions.IgnoreExtraColumns))
				throw new UnexpectedDataException($"Expected one column but found {reader.FieldCount} columns. If this was expected, use ListOptions.IgnoreExtraColumns or ListOptions.FlattenExtraColumns.");

			var columnCount = m_ListOptions.HasFlag(ListOptions.FlattenExtraColumns) ? reader.FieldCount : 1;
			matchingColumns = new();

			for (var i = 0; i < columnCount; i++)
				matchingColumns.Add(i);
		}

		return matchingColumns;
	}
}