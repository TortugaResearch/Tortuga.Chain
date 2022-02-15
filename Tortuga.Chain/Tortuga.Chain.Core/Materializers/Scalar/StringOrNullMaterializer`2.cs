using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// Materializes the result set as a string.
	/// </summary>
	/// <typeparam name="TCommand">The type of the t command type.</typeparam>
	/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
	internal sealed class StringMaterializerOrNull<TCommand, TParameter> : ScalarMaterializer<TCommand, TParameter, string?> where TCommand : DbCommand
		where TParameter : DbParameter
	{
		/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.Materializers.StringMaterializerOrNull{TCommand, TParameter}"/> class.</summary>
		/// <param name="commandBuilder">The command builder.</param>
		/// <param name="columnName">Name of the desired column.</param>
		public StringMaterializerOrNull(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName = null)
			: base(commandBuilder, columnName)
		{ }

		/// <summary>
		/// Execute the operation synchronously.
		/// </summary>
		/// <returns></returns>
		public override string? Execute(object? state = null)
		{
			object? temp = null;
			ExecuteCore(cmd => temp = cmd.ExecuteScalar(), state);
			if (temp == DBNull.Value)
				return null;

			return (string?)temp;
		}

		/// <summary>
		/// Execute the operation asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">User defined state, usually used for logging.</param>
		/// <returns></returns>
		public override async Task<string?> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
		{
			object? temp = null;
			await ExecuteCoreAsync(async cmd => temp = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false), cancellationToken, state).ConfigureAwait(false);
			if (temp == DBNull.Value)
				return null;

			return (string?)temp;
		}
	}
}
