using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// This class indicates the associated operation should be executed without returning a result set.
	/// </summary>
	public class NonQueryMaterializer<TCommand, TParameter> : Materializer<TCommand, TParameter, int?>
		where TCommand : DbCommand
		where TParameter : DbParameter
	{
		/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.Materializers.NonQueryMaterializer{TCommand, TParameter}"/> class.</summary>
		/// <param name="commandBuilder">The associated command builder.</param>
		public NonQueryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder)
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
		public override IReadOnlyList<string> DesiredColumns() => NoColumns;

		/// <summary>
		/// Execute the operation synchronously.
		/// </summary>
		/// <param name="state">User defined state, usually used for logging.</param>
		/// <returns>Number of rows affected, if available from the database.</returns>
		public override int? Execute(object? state = null)
		{
			return Prepare().Execute(cmd => cmd.ExecuteNonQuery(), state);
		}

		/// <summary>
		/// Execute the operation asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">User defined state, usually used for logging.</param>
		/// <returns>Number of rows affected, if available from the database.</returns>
		public override async Task<int?> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
		{
			return await Prepare().ExecuteAsync(async cmd =>
			{
				return await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
			}, cancellationToken, state).ConfigureAwait(false);
		}
	}
}
