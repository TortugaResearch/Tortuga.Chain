using System.Data.Common;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// This Materializer captures output parameters and returns them as a dictionary.
	/// </summary>
	public class AsOutputsMaterializer<TCommand, TParameter> : Materializer<TCommand, TParameter, Dictionary<string, object?>>
		where TCommand : DbCommand
		where TParameter : DbParameter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsOutputsMaterializer{TCommand, TParameter}"/> class.
		/// </summary>
		/// <param name="commandBuilder">The associated operation.</param>
		public AsOutputsMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder)
	: base(commandBuilder)
		{ }

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
		public override Dictionary<string, object?> Execute(object? state = null)
		{
			var commandToken = Prepare();

			commandToken.Execute(cmd => cmd.ExecuteNonQuery(), state);

			return CaptureOutputParameters(commandToken);
		}

		internal static Dictionary<string, object?> CaptureOutputParameters(CommandExecutionToken<TCommand, TParameter> commandToken)
		{
			var result = new Dictionary<string, object?>();
			foreach (var param in commandToken.Parameters
				.Where(p => p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput))
			{
				var name = param.ParameterName.Replace("@", "", StringComparison.OrdinalIgnoreCase); //TODO: Generalize this for all databases.
																									 //var name = CommandBuilder.DataSource.DatabaseMetadata.CleanParameterName(param.ParameterName);

				var value = param.Value == DBNull.Value ? null : param.Value;

				result[name] = value;
			}
			if (result.Count == 0)
				throw new MappingException("No output parameters found.");
			return result;
		}

		/// <summary>
		/// execute as an asynchronous operation.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">User defined state, usually used for logging.</param>
		/// <returns>Task&lt;Dictionary&lt;System.String, System.Nullable&lt;System.Object&gt;&gt;&gt;.</returns>
		public async override Task<Dictionary<string, object?>> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
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
