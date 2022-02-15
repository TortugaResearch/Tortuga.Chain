using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// Materializes the result set as a TableSet.
	/// </summary>
	/// <typeparam name="TCommand">The type of the t command type.</typeparam>
	/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
	/// <typeparam name="T1">The type of the 1.</typeparam>
	/// <typeparam name="T2">The type of the 2.</typeparam>
	/// <typeparam name="T3">The type of the 3.</typeparam>
	/// <typeparam name="T4">The type of the 4.</typeparam>
	/// <typeparam name="T5">The type of the 5.</typeparam>
	internal sealed class CollectionSetMaterializer<TCommand, TParameter, T1, T2, T3, T4, T5> : Materializer<TCommand, TParameter, Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>>>
		where TCommand : DbCommand
		where TParameter : DbParameter
		where T1 : class, new()
		where T2 : class, new()
		where T3 : class, new()
		where T4 : class, new()
		where T5 : class, new()
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionSetMaterializer{TCommand, TParameter, T1, T2}"/> class.
		/// </summary>
		/// <param name="commandBuilder">The command builder.</param>
		public CollectionSetMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder)
			: base(commandBuilder)
		{
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
		public override Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>> Execute(object? state = null)
		{
			TableSet? result = null;

			var executionToken = Prepare();
			executionToken.Execute(cmd =>
			{
				using (var reader = cmd.ExecuteReader(CommandBehavior))
				{
					result = new TableSet(reader);
					return result.Sum(t => t.Rows.Count);
				}
			}, state);

			return BuildResult(result!);
		}

		/// <summary>
		/// Execute the operation asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">User defined state, usually used for logging.</param>
		/// <returns></returns>
		public override async Task<Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>>> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
		{
			TableSet? result = null;

			var executionToken = Prepare();

			await executionToken.ExecuteAsync(async cmd =>
			{
				using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior).ConfigureAwait(false))
				{
					result = new TableSet(reader);
					return result.Sum(t => t.Rows.Count);
				}
			}, cancellationToken, state).ConfigureAwait(false);

			return BuildResult(result!);
		}

		static Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>> BuildResult(TableSet result)
		{
			if (result.Count != 5)
				throw new DataException($"Expected 5 tables but received {result.Count} tables");

			return Tuple.Create(
				result[0].ToObjects<T1>().ToList(),
				result[1].ToObjects<T2>().ToList(),
				result[2].ToObjects<T3>().ToList(),
				result[3].ToObjects<T4>().ToList(),
				result[4].ToObjects<T5>().ToList()
				);
		}
	}
}
