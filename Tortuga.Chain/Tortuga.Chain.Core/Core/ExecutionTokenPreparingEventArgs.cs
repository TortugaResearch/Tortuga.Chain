using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Core
{
	/// <summary>
	/// This occurs just before an execution token is prepared.
	/// </summary>
	/// <seealso cref="EventArgs" />
	public class ExecutionTokenPreparingEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExecutionTokenPreparingEventArgs"/> class.
		/// </summary>
		/// <param name="commandBuilder">The command builder.</param>
		public ExecutionTokenPreparingEventArgs(DbCommandBuilder commandBuilder)
		{
			CommandBuilder = commandBuilder;
		}

		/// <summary>
		/// Gets the command builder being used to generate the execution token.
		/// </summary>
		/// <value>
		/// The command builder.
		/// </value>
		public DbCommandBuilder CommandBuilder { get; }
	}
}
