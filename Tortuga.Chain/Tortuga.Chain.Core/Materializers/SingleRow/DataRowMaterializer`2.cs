using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// Materializes the result set as a single row.
/// </summary>
/// <typeparam name="TCommand">The type of the t command type.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
internal sealed class DataRowMaterializer<TCommand, TParameter> : Materializer<TCommand, TParameter, DataRow> where TCommand : DbCommand
	where TParameter : DbParameter
{
	readonly RowOptions m_RowOptions;

	/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.Materializers.DataRowMaterializer{TCommand, TParameter}"/> class.</summary>
	/// <param name="commandBuilder">The command builder.</param>
	/// <param name="rowOptions">The row options.</param>
	public DataRowMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, RowOptions rowOptions)
		: base(commandBuilder)
	{
		m_RowOptions = rowOptions;
	}

	public override IReadOnlyList<string> DesiredColumns() => AllColumns;

	/// <summary>
	/// Execute the operation synchronously.
	/// </summary>
	/// <returns></returns>
	[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
	[SuppressMessage("Microsoft.Globalization", "CA1306:SetLocaleForDataTypes")]
	public override DataRow Execute(object? state = null)
	{
		var executionToken = Prepare();

		var ds = new DataSet() { EnforceConstraints = false /*needed for PostgreSql*/};
		var table = new DataTable();
		ds.Tables.Add(table);

		executionToken.Execute(cmd =>
		{
			using (var reader = cmd.ExecuteReader(CommandBehavior))
			{
				table.Load(reader);
				return table.Rows.Count;
			}
		}, state);

		if (table.Rows.Count == 0)
		{
			throw new MissingDataException("No rows were returned");
		}
		else if (table.Rows.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
		{
			throw new MissingDataException($"No rows were returned. It was this expected, use `.ToDataRowOrNull` instead of `.ToDataRow`.");
		}
		return table.Rows[0];
	}

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns></returns>
	[SuppressMessage("Microsoft.Globalization", "CA1306:SetLocaleForDataTypes")]
	[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
	public override async Task<DataRow> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		var executionToken = Prepare();

		var ds = new DataSet() { EnforceConstraints = false /*needed for PostgreSql*/};
		var table = new DataTable();
		ds.Tables.Add(table);

		await executionToken.ExecuteAsync(async cmd =>
		{
			using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior, cancellationToken).ConfigureAwait(false))
			{
				table.Load(reader);
				return table.Rows.Count;
			}
		}, cancellationToken, state).ConfigureAwait(false);

		if (table.Rows.Count == 0)
		{
			throw new MissingDataException($"No rows were returned. It was this expected, use `.ToDataRowOrNull` instead of `.ToDataRow`.");
		}
		else if (table.Rows.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
		{
			throw new UnexpectedDataException("Expected 1 row but received " + table.Rows.Count + " rows. If this was expected, use `RowOptions.DiscardExtraRows`.");
		}
		return table.Rows[0];
	}
}
