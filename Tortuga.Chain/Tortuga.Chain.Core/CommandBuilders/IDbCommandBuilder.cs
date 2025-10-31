using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// This allows executing command builders without returning anything.
/// </summary>
/// <remarks>Warning: This interface is meant to simulate multiple inheritance and work-around some issues with exposing generic types. Do not implement it in client code, as new method will be added over time.</remarks>
public interface IDbCommandBuilder
{
	/// <summary>
	/// Indicates this operation has no result set.
	/// </summary>
	/// <returns>This may contain the result code or number of rows affected.</returns>
	ILink<int?> AsNonQuery();

	/// <summary>
	/// Returns the command text (usually SQL) without executing it.
	/// </summary>
	/// <returns>System.String.</returns>
	public string? CommandText() => AsNonQuery().CommandText();

	/// <summary>
	/// Execute the operation synchronously.
	/// </summary>
	/// <param name="state">User defined state, usually used for logging.</param>
	int? Execute(object? state = null);

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns>Task.</returns>
	[SuppressMessage("Async/await", "CRR0035:No CancellationToken parameter in the asynchronous method", Justification = "<Pending>")]
	Task<int?> ExecuteAsync(object? state = null);

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns>Task.</returns>
	Task<int?> ExecuteAsync(CancellationToken cancellationToken, object? state = null);

	/// <summary>
	/// Returns the column associated with the column name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns></returns>
	/// <remarks>If the column name was not found, this will return null</remarks>
	ColumnMetadata? TryGetColumn(string columnName);
}
