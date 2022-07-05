using Tortuga.Chain.Core;

namespace Tortuga.Chain;

/// <summary>
/// Events indicating the activities performed by a given datasource.
/// </summary>
public class ExecutionEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ExecutionEventArgs" /> class.
	/// </summary>
	/// <param name="executionDetails">The execution details.</param>
	/// <param name="startTime">The start time.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <exception cref="ArgumentNullException">executionDetails;executionDetails is null.</exception>
	public ExecutionEventArgs(ExecutionToken executionDetails, DateTimeOffset startTime, object? state)
	{
		ExecutionDetails = executionDetails ?? throw new ArgumentNullException(nameof(executionDetails));
		StartTime = startTime;
		State = state;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ExecutionEventArgs" /> class.
	/// </summary>
	/// <param name="executionDetails">The execution details.</param>
	/// <param name="startTime">The start time.</param>
	/// <param name="endTime">The end time.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <exception cref="ArgumentNullException">executionDetails;executionDetails is null.</exception>
	public ExecutionEventArgs(ExecutionToken executionDetails, DateTimeOffset startTime, DateTimeOffset endTime, object? state)
	{
		StartTime = startTime;
		EndTime = endTime;
		State = state;
		ExecutionDetails = executionDetails ?? throw new ArgumentNullException(nameof(executionDetails));
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ExecutionEventArgs" /> class.
	/// </summary>
	/// <param name="executionDetails">The execution details.</param>
	/// <param name="startTime">The start time.</param>
	/// <param name="endTime">The end time.</param>
	/// <param name="rowsAffected">The number of rows affected.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <exception cref="ArgumentNullException">executionDetails;executionDetails is null.</exception>
	public ExecutionEventArgs(ExecutionToken executionDetails, DateTimeOffset startTime, DateTimeOffset endTime, int? rowsAffected, object? state)
	{
		StartTime = startTime;
		EndTime = endTime;
		RowsAffected = rowsAffected;
		State = state;
		ExecutionDetails = executionDetails ?? throw new ArgumentNullException(nameof(executionDetails));
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ExecutionEventArgs" /> class.
	/// </summary>
	/// <param name="executionDetails">The execution details.</param>
	/// <param name="startTime">The start time.</param>
	/// <param name="endTime">The end time.</param>
	/// <param name="error">The error.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <exception cref="ArgumentNullException">executionDetails;executionDetails is null.</exception>
	public ExecutionEventArgs(ExecutionToken executionDetails, DateTimeOffset startTime, DateTimeOffset endTime, Exception error, object? state)
	{
		StartTime = startTime;
		EndTime = endTime;
		Error = error;
		State = state;
		ExecutionDetails = executionDetails ?? throw new ArgumentNullException(nameof(executionDetails));
	}

	/// <summary>
	/// Gets the duration of the request, if available.
	/// </summary>
	public TimeSpan? Duration => EndTime - StartTime;

	/// <summary>
	/// Gets the end time.
	/// </summary>
	/// <value>
	/// The end time.
	/// </value>
	public DateTimeOffset? EndTime { get; }

	/// <summary>
	/// Gets the error.
	/// </summary>
	/// <value>
	/// The error.
	/// </value>
	public Exception? Error { get; }

	/// <summary>
	/// Gets the details of the execution.
	/// </summary>
	/// <value>Returns null or details of the execution.</value>
	/// <remarks>You can cast this to a concrete type for more information.</remarks>
	public ExecutionToken ExecutionDetails { get; }

	/// <summary>
	/// If available, this shows the number of rows affected by the execution.
	/// </summary>
	public int? RowsAffected { get; }

	/// <summary>
	/// Gets the start time.
	/// </summary>
	/// <value>
	/// The start time.
	/// </value>
	public DateTimeOffset StartTime { get; }

	/// <summary>
	/// Gets the user-defined state.
	/// </summary>
	public object? State { get; }
}
