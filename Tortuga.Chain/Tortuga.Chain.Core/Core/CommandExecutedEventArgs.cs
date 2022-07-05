using System.Data.Common;

namespace Tortuga.Chain.Core;

/// <summary>
/// Class CommandExecutedEventArgs is fired .
/// </summary>
/// <seealso cref="EventArgs" />
public class CommandExecutedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CommandExecutedEventArgs" /> class.
	/// </summary>
	/// <param name="command">The command.</param>
	/// <param name="rowsAffected">The number of rows affected.</param>
	public CommandExecutedEventArgs(DbCommand command, int? rowsAffected)
	{
		Command = command;
		RowsAffected = rowsAffected;
	}

	/// <summary>
	/// Gets the command that was just built.
	/// </summary>
	public DbCommand Command { get; }

	/// <summary>
	/// Gets the number of rows affected, if known.
	/// </summary>
	/// <value>The rows affected.</value>
	public int? RowsAffected { get; }
}
