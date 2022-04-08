using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.SqlServer;

/// <summary>
/// This contains the connection options that are currently in effect.
/// </summary>
public class OleDbSqlServerEffectiveSettings
{
	int m_Options;

	internal OleDbSqlServerEffectiveSettings()
	{
	}

	/// <summary>
	/// ANSI_NULL_DFLT_OFF. Alters the session's behavior not to use ANSI compatibility for nullability. New columns defined without explicit nullability do not allow nulls.
	/// </summary>
	public bool AnsiNullDefaultOff { get { return (m_Options & 2048) > 0; } }

	/// <summary>
	/// ANSI_NULL_DFLT_ON. Alters the session's behavior to use ANSI compatibility for nullability. New columns defined without explicit nullability are defined to allow nulls.
	/// </summary>
	public bool AnsiNullDefaultOn { get { return (m_Options & 1024) > 0; } }

	/// <summary>
	/// ANSI_NULLS. Controls NULL handling when using equality operators.
	/// </summary>
	public bool AnsiNulls { get { return (m_Options & 32) > 0; } }

	/// <summary>
	/// ANSI_PADDING. Controls padding of fixed-length variables.
	/// </summary>
	public bool AnsiPadding { get { return (m_Options & 16) > 0; } }

	/// <summary>
	/// ANSI_WARNINGS. Controls truncation and NULL in aggregate warnings.
	/// </summary>
	public bool AnsiWarning { get { return (m_Options & 8) > 0; } }

	/// <summary>
	/// ARITHABORT. Terminates a query when an overflow or divide-by-zero error occurs during query execution.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Arith")]
	public bool ArithAbort { get { return (m_Options & 64) > 0; } }

	/// <summary>
	/// ARITHIGNORE. Returns NULL when an overflow or divide-by-zero error occurs during a query.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Arith")]
	public bool ArithIgnore { get { return (m_Options & 128) > 0; } }

	/// <summary>
	/// CONCAT_NULL_YIELDS_NULL. Returns NULL when concatenating a NULL value with a string.
	/// </summary>
	public bool ConcatNullYieldsNull { get { return (m_Options & 4096) > 0; } }

	/// <summary>
	/// CURSOR_CLOSE_ON_COMMIT. Controls behavior of cursors after a commit operation has been performed.
	/// </summary>
	public bool CursorCloseOnCommit { get { return (m_Options & 4) > 0; } }

	/// <summary>
	/// DISABLE_DEF_CNST_CHK. Controls interim or deferred constraint checking.
	/// </summary>
	public bool DisableDeferredConstraintChecking { get { return (m_Options & 1) > 0; } }

	/// <summary>
	/// NOCOUNT. Turns off the message returned at the end of each statement that states how many rows were affected.
	/// </summary>
	public bool NoCount { get { return (m_Options & 512) > 0; } }

	/// <summary>
	/// NUMERIC_ROUNDABORT. Generates an error when a loss of precision occurs in an expression.
	/// </summary>
	public bool NumericRoundAbort { get { return (m_Options & 8192) > 0; } }

	/// <summary>
	/// QUOTED_IDENTIFIER. Differentiates between single and double quotation marks when evaluating an expression.
	/// </summary>
	public bool QuotedIdentifier { get { return (m_Options & 256) > 0; } }

	/// <summary>
	/// XACT_ABORT. Rolls back a transaction if a Transact-SQL statement raises a run-time error.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xact")]
	public bool XactAbort { get { return (m_Options & 16384) > 0; } }

	[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
	internal void Reload(OleDbConnection connection, OleDbTransaction? transaction)
	{
		using (var cmd = new OleDbCommand("SELECT @@Options") { Connection = connection, Transaction = transaction })
			m_Options = (int)cmd.ExecuteScalar()!;
	}

	internal async Task ReloadAsync(OleDbConnection connection, OleDbTransaction? transaction)
	{
		using (var cmd = new OleDbCommand("SELECT @@Options") { Connection = connection, Transaction = transaction })
			m_Options = (int)(await cmd.ExecuteScalarAsync().ConfigureAwait(false))!;
	}
}
