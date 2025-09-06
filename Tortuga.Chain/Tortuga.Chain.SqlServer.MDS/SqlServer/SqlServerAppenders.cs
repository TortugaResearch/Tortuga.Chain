using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.SqlServer.Appenders;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer;

/// <summary>
/// Class SqlServerAppenders.
/// </summary>
public static class SqlServerAppenders
{
	/// <summary>
	/// Return the approximate row count using the APPROX_COUNT_DISTINCT function.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <remarks>This is only available on tables with a single primary key.</remarks>
	public static ILink<long> AsCountDistinctApproximate(this TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> tableDbCommand)
	{
		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		return ((ISupportsApproximateCount)tableDbCommand).AsCountApproximate();
	}

	/// <summary>
	/// Return the approximate distinct count using the APPROX_COUNT_DISTINCT function.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <param name="columnName">Name of the column.</param>
	public static ILink<long> AsCountDistinctApproximate(this TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> tableDbCommand, string columnName)
	{
		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		return ((ISupportsApproximateCount)tableDbCommand).AsCountApproximate(columnName);
	}

	/// <summary>
	/// Attaches a SQL Server dependency change listener to this operation that will automatically invalidate the cache.
	/// </summary>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	/// <param name="previousLink">The previous link.</param>
	/// <returns></returns>
	public static ILink<TResult> AutoInvalidate<TResult>(this ICacheLink<TResult> previousLink)
	{
		return new NotifyChangeAppender<TResult>(previousLink, (s, e) => previousLink.Invalidate());
	}

	/// <summary>
	/// Attaches a SQL Server dependency change listener to this operation.
	/// </summary>
	/// <typeparam name="TResult">The type of the t result type.</typeparam>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="eventHandler">The event handler to fire when the underlying data changes.</param>
	/// <returns>Tortuga.Chain.Core.ILink&lt;TResult&gt;.</returns>
	/// <remarks>This will only work for operations against non-transactional SQL Server data sources that also comform to the rules about using SQL Dependency.</remarks>
	public static ILink<TResult> WithChangeNotification<TResult>(this ILink<TResult> previousLink, OnChangeEventHandler eventHandler)
	{
		return new NotifyChangeAppender<TResult>(previousLink, eventHandler);
	}

	/// <summary>
	/// Attaches a listener for SQL Server Info Message events.
	/// </summary>
	/// <typeparam name="TResult">The type of the t result.</typeparam>
	/// <param name="previousLink">The previous link.</param>
	/// <param name="eventHandler">The event handler.</param>
	/// <returns>ILink&lt;TResult&gt;.</returns>
	public static ILink<TResult> WithInfoMessageNotification<TResult>(this ILink<TResult> previousLink, SqlInfoMessageEventHandler eventHandler)
	{
		return new InfoMessageNotificationAppender<TResult>(previousLink, eventHandler);
	}



	/// <summary>
	/// Sets the isolation level on the query using a table hint.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <param name="isolationLevel">The isolation level.</param>
	public static TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> WithIsolationLevel(this TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> tableDbCommand, SqlServerIsolationLevel isolationLevel)
	{
		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		var command = tableDbCommand as ISupportsTableHints;
		if (command == null)
			throw new ArgumentException($"The command {tableDbCommand.GetType().FullName} does not support table hints.", nameof(tableDbCommand));

		var queryHint = isolationLevel switch
		{
			SqlServerIsolationLevel.Serializable => "SERIALIZABLE",
			SqlServerIsolationLevel.ReadUncommitted => "READUNCOMMITTED",
			SqlServerIsolationLevel.ReadCommitted => "READCOMMITTED",
			SqlServerIsolationLevel.RepeatableRead => "REPEATABLEREAD",
			SqlServerIsolationLevel.Snapshot => "SNAPSHOT",
			_ => throw new ArgumentOutOfRangeException(nameof(isolationLevel), isolationLevel, $"Isolation level {isolationLevel} is not supported.")
		};

		command.AddTableHint(queryHint);

		return tableDbCommand;
	}
}
