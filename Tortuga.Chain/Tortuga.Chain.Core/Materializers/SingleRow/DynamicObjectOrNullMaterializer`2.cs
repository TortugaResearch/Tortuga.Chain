using System.Data.Common;
using System.Dynamic;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// Materializes the result set as a dynamic object.
	/// </summary>
	/// <typeparam name="TCommand">The type of the t command type.</typeparam>
	/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
	internal sealed class DynamicObjectOrNullMaterializer<TCommand, TParameter> : Materializer<TCommand, TParameter, dynamic?> where TCommand : DbCommand
		where TParameter : DbParameter
	{
		readonly RowOptions m_RowOptions;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicObjectMaterializer{TCommand, TParameter}" /> class.
		/// </summary>
		/// <param name="commandBuilder">The associated operation.</param>
		/// <param name="rowOptions">The row options.</param>
		public DynamicObjectOrNullMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, RowOptions rowOptions)
			: base(commandBuilder)
		{
			m_RowOptions = rowOptions;
		}

		/// <summary>
		/// Returns the list of columns the materializer would like to have.
		/// </summary>
		/// <returns>
		/// IReadOnlyList&lt;System.String&gt;.
		/// </returns>
		/// <remarks>
		/// If AutoSelectDesiredColumns is returned, the command builder is allowed to choose which columns to return. If NoColumns is returned, the command builder should omit the SELECT/OUTPUT clause.
		/// </remarks>
		public override IReadOnlyList<string> DesiredColumns() => AllColumns;

		/// <summary>
		/// Execute the operation synchronously.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <returns>System.Nullable&lt;dynamic&gt;.</returns>
		/// <exception cref="MissingDataException">No rows were returned</exception>
		/// <exception cref="UnexpectedDataException">Expected 1 row but received " + rowCount + " rows</exception>
		public override dynamic? Execute(object? state = null)
		{
			ExpandoObject? row = null;
			var rowCount = Prepare().Execute(cmd =>
			{
				using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
				{
					if (reader.Read())
					{
						row = new ExpandoObject();
						reader.PopulateDictionary(row);
					}
					return (row != null ? 1 : 0) + reader.RemainingRowCount();
				}
			}, state);

			if (rowCount == 0 || row == null)
			{
				if (!m_RowOptions.HasFlag(RowOptions.PreventEmptyResults))
					return null;
				else
					throw new MissingDataException($"No rows were returned and {nameof(RowOptions)}.{nameof(RowOptions.PreventEmptyResults)} was enabled.");
			}
			else if (rowCount > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
			{
				throw new UnexpectedDataException("Expected 1 row but received " + rowCount + " rows. If this was expected, use `RowOptions.DiscardExtraRows`.");
			}

			return row;
		}

		/// <summary>
		/// Execute the operation asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">User defined state, usually used for logging.</param>
		/// <returns>Task&lt;System.Nullable&lt;dynamic&gt;&gt;.</returns>
		/// <exception cref="MissingDataException">No rows were returned</exception>
		/// <exception cref="UnexpectedDataException">Expected 1 row but received " + rowCount + " rows</exception>
		public override async Task<dynamic?> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
		{
			ExpandoObject? row = null;

			var rowCount = await Prepare().ExecuteAsync(async cmd =>
			{
				using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
				{
					if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
					{
						row = new ExpandoObject();
						reader.PopulateDictionary(row);
					}
					return (row != null ? 1 : 0) + await reader.RemainingRowCountAsync().ConfigureAwait(false);
				}
			}, cancellationToken, state).ConfigureAwait(false);

			if (rowCount == 0 || row == null)
			{
				if (!m_RowOptions.HasFlag(RowOptions.PreventEmptyResults))
					return null;
				else
					throw new MissingDataException($"No rows were returned and {nameof(RowOptions)}.{nameof(RowOptions.PreventEmptyResults)} was enabled.");
			}
			else if (rowCount > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
			{
				throw new UnexpectedDataException("Expected 1 row but received " + rowCount + " rows. If this was expected, use `RowOptions.DiscardExtraRows`.");
			}

			return row;
		}
	}
}
