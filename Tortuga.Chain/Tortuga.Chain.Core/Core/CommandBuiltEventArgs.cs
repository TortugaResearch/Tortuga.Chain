using System.Data.Common;

namespace Tortuga.Chain.Core;

/// <summary>
/// Class CommandBuiltEventArgs.
/// </summary>
public class CommandBuiltEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CommandBuiltEventArgs"/> class.
	/// </summary>
	/// <param name="command">The command.</param>
	public CommandBuiltEventArgs(DbCommand command)
	{
		Command = command;
	}

	/// <summary>
	/// Gets the command that was just built.
	/// </summary>
	public DbCommand Command { get; }
}
