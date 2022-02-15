using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// Materializes the result set as a TableSet.
	/// </summary>
	/// <typeparam name="TCommand">The type of the t command type.</typeparam>
	/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
	internal sealed class TableSetMaterializer<TCommand, TParameter> : Materializer<TCommand, TParameter, TableSet> where TCommand : DbCommand
		where TParameter : DbParameter
	{
		readonly string[] m_TableNames;

		/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.Materializers.TableSetMaterializer{TCommand, TParameter}"/> class.</summary>
		/// <param name="commandBuilder">The command builder.</param>
		/// <param name="tableNames">The table names.</param>
		public TableSetMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string[] tableNames)
			: base(commandBuilder)
		{
			m_TableNames = tableNames;
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
		/// <returns></returns>
		public override TableSet Execute(object? state = null)
		{
			TableSet? result = null;

			var executionToken = Prepare();
			executionToken.Execute(cmd =>
						{
							using (var reader = cmd.ExecuteReader(CommandBehavior))
							{
								result = new TableSet(reader, m_TableNames);
								return result.Sum(t => t.Rows.Count);
							}
						}, state);

			if (m_TableNames.Length > result!.Count)
				throw new DataException($"Expected at least {m_TableNames.Length} tables but received {result.Count} tables");

			return result;
		}

		/// <summary>
		/// Execute the operation asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">User defined state, usually used for logging.</param>
		/// <returns></returns>
		public override async Task<TableSet> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
		{
			TableSet? result = null;

			var executionToken = Prepare();

			await executionToken.ExecuteAsync(async cmd =>
			{
				using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior).ConfigureAwait(false))
				{
					result = new TableSet(reader, m_TableNames);
					return result.Sum(t => t.Rows.Count);
				}
			}, cancellationToken, state).ConfigureAwait(false);

			if (m_TableNames.Length > result!.Count)
				throw new DataException($"Expected at least {m_TableNames.Length} tables but received {result.Count} tables");

			return result;
		}
	}
}
