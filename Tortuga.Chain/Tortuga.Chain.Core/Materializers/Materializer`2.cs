using System.Data.Common;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// This is the base class for materializers that don't return a value. Most operation are not executed without first attaching a materializer subclass.
	/// </summary>
	/// <typeparam name="TCommand">The type of the t command type.</typeparam>
	/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
	public abstract class Materializer<TCommand, TParameter> : Materializer
		where TCommand : DbCommand
		where TParameter : DbParameter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Materializer{TCommand, TParameter}"/> class.
		/// </summary>
		/// <param name="commandBuilder">The associated command builder.</param>
		/// <exception cref="ArgumentNullException">operation;operation is null.</exception>
		protected Materializer(DbCommandBuilder<TCommand, TParameter> commandBuilder)
		{
			CommandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder), $"{nameof(commandBuilder)} is null.");
		}

		/// <summary>
		/// Gets the associated operation.
		/// </summary>
		/// <value>The command builder.</value>
		protected DbCommandBuilder<TCommand, TParameter> CommandBuilder { get; }

		/// <summary>
		/// Returns the command text (usually SQL) without executing it.
		/// </summary>
		/// <returns>System.String.</returns>
		public string? CommandText() => CommandBuilder.Prepare(this).CommandText;

		/// <summary>
		/// Prepares this operation for execution.
		/// </summary>
		/// <returns>ExecutionToken&lt;TCommand, TParameter&gt;.</returns>
		protected CommandExecutionToken<TCommand, TParameter> Prepare()
		{
			OnExecutionTokenPreparing(new ExecutionTokenPreparingEventArgs(CommandBuilder));

			var executionToken = CommandBuilder.Prepare(this);

			OnExecutionTokenPrepared(new ExecutionTokenPreparedEventArgs(executionToken));

			return executionToken;
		}
	}
}
