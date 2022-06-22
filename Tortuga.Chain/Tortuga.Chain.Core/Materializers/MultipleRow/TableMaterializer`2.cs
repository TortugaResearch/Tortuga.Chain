using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// Materializes the result set as a Table.
/// </summary>
/// <typeparam name="TCommand">The type of the t command type.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
internal sealed class TableMaterializer<TCommand, TParameter> : Materializer<TCommand, TParameter, Table> where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TableMaterializer{TCommand, TParameter}" /> class.
	/// </summary>
	/// <param name="commandBuilder">The associated operation.</param>
	public TableMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder)
		: base(commandBuilder)
	{ }

	/// <summary>
	/// Returns the list of columns the materializer would like to have.
	/// </summary>
	/// <returns>
	/// IReadOnlyList&lt;System.String&gt;.
	/// </returns>
	/// <remarks>
	/// If AutoSelectDesiredColumns is returned, the command builder is allowed to choose which columns to return. If NoColumns is returned, the command builder should omit the SELECT/OUTPUT clause.
	/// </remarks>
	public override IReadOnlyList<string> DesiredColumns()
	{
		return AllColumns;
	}

	/// <summary>
	/// Execute the operation synchronously.
	/// </summary>
	/// <returns></returns>
	public override Table Execute(object? state)
	{
		Table? table = null;
		Prepare().Execute(cmd =>
		{
			using (var reader = cmd.ExecuteReader(CommandBehavior))
			{
				table = new Table(reader);
				return table.Rows.Count;
			}
		}, state);

		return table!;
	}

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns></returns>
	public override async Task<Table> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		Table? table = null;
		await Prepare().ExecuteAsync(async cmd =>
		{
			using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior, cancellationToken).ConfigureAwait(false))
			{
				table = new Table(reader);
				return table.Rows.Count;
			}
		}, cancellationToken, state).ConfigureAwait(false);

		return table!;
	}
}
