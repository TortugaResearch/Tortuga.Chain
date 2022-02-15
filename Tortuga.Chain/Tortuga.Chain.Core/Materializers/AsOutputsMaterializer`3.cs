using System.Data.Common;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// This Materializer captures output parameters and returns them as an object.
	/// </summary>
	public class AsOutputsMaterializer<TCommand, TParameter, TObject> : Materializer<TCommand, TParameter, TObject>
		where TCommand : DbCommand
		where TParameter : DbParameter
		where TObject : class, new()
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsOutputsMaterializer{TCommand, TParameter}"/> class.
		/// </summary>
		/// <param name="commandBuilder">The associated operation.</param>
		public AsOutputsMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder)
	: base(commandBuilder)
		{
		}

		/// <summary>
		/// Returns the list of columns the materializer would like to have.
		/// </summary>
		/// <returns>IReadOnlyList&lt;System.String&gt;.</returns>
		/// <remarks>If AutoSelectDesiredColumns is returned, the command builder is allowed to choose which columns to return. If NoColumns is returned, the command builder should omit the SELECT/OUTPUT clause.</remarks>
		public override IReadOnlyList<string> DesiredColumns() => NoColumns;

		/// <summary>
		/// Execute the operation synchronously.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <returns>Dictionary&lt;System.String, System.Nullable&lt;System.Object&gt;&gt;.</returns>
		public override TObject Execute(object? state = null)
		{
			var commandToken = Prepare();

			commandToken.Execute(cmd => cmd.ExecuteNonQuery(), state);

			return CaptureOutputParameters(commandToken);
		}

		static TObject CaptureOutputParameters(CommandExecutionToken<TCommand, TParameter> commandToken)
		{
			var result = AsOutputsMaterializer<TCommand, TParameter>.CaptureOutputParameters(commandToken);
			var objectResult = new TObject();
			MaterializerUtilities.PopulateComplexObject(result, objectResult, null);
			return objectResult;
		}

		/// <summary>
		/// execute as an asynchronous operation.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">User defined state, usually used for logging.</param>
		/// <returns>Task&lt;Dictionary&lt;System.String, System.Nullable&lt;System.Object&gt;&gt;&gt;.</returns>
		public async override Task<TObject> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
		{
			var commandToken = Prepare();

			await commandToken.ExecuteAsync(async cmd =>
			{
				return await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
			}, cancellationToken, state).ConfigureAwait(false);

			return CaptureOutputParameters(commandToken);
		}
	}
}
