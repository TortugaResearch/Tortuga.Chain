using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// This is the non-generic version of the base class from which all other command builders are created.
/// </summary>
/// <seealso cref="IDbCommandBuilder" />
public abstract class DbCommandBuilder : IDbCommandBuilder
{
	/// <summary>
	/// Gets or sets a value indicating whether or not to use CommandBehavior.SequentialAccess.
	/// </summary>
	internal protected bool SequentialAccessMode { get; internal set; }

	/// <summary>
	/// Gets or sets a value indicating whether strict mode is enabled.
	/// </summary>
	/// <remarks>Strict mode requires all properties that don't represent columns to be marked with the NotMapped attribute.</remarks>
	internal protected bool StrictMode { get; internal set; }

	/// <summary>
	/// Indicates this operation has no result set.
	/// </summary>
	/// <returns></returns>
	public abstract ILink<int?> AsNonQuery();

	/// <summary>
	/// Execute the operation synchronously.
	/// </summary>
	/// <param name="state">User defined state, usually used for logging.</param>
	public int? Execute(object? state = null) => AsNonQuery().Execute(state);

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns>Task.</returns>
	public Task<int?> ExecuteAsync(object? state = null) => AsNonQuery().ExecuteAsync(state);

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns>Task.</returns>
	public Task<int?> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		return AsNonQuery().ExecuteAsync(cancellationToken, state);
	}

	/// <summary>
	/// Returns the column associated with the column name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns>If the column name was not found, this will return null</returns>
	/// <remarks></remarks>
	public abstract ColumnMetadata? TryGetColumn(string columnName);

	/// <summary>
	/// Returns a list of columns.
	/// </summary>
	/// <returns>If the command builder doesn't know which columns are available, an empty list will be returned.</returns>
	/// <remarks>This is used by materializers to skip exclude columns.</remarks>
	public abstract IReadOnlyList<ColumnMetadata> TryGetColumns();

	/// <summary>
	/// Returns a list of columns known to be non-nullable.
	/// </summary>
	/// <returns>If the command builder doesn't know which columns are non-nullable, an empty list will be returned.</returns>
	/// <remarks>This is used by materializers to skip IsNull checks.</remarks>
	public abstract IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns();
}
