using System;
using Tortuga.Chain.Core;

namespace Tortuga.Chain
{
    /// <summary>
    /// Events indicating the activities performed by a given datasource.
    /// </summary>
    public class ExecutionEventArgs : EventArgs
    {

        readonly Exception m_Error;
        readonly DateTimeOffset? m_EndTime;
        readonly DateTimeOffset m_StartTime;
        readonly int? m_RowsAffected;
        readonly object m_State;
        readonly ExecutionToken m_ExecutionDetails;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionEventArgs" /> class.
        /// </summary>
        /// <param name="executionDetails">The execution details.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <exception cref="ArgumentNullException">executionDetails;executionDetails is null.</exception>
        public ExecutionEventArgs(ExecutionToken executionDetails, DateTimeOffset startTime, object state)
        {
            if (executionDetails == null)
                throw new ArgumentNullException("executionDetails", "executionDetails is null.");

            m_ExecutionDetails = executionDetails;
            m_StartTime = startTime;
            m_State = state;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionEventArgs" /> class.
        /// </summary>
        /// <param name="executionDetails">The execution details.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <exception cref="ArgumentNullException">executionDetails;executionDetails is null.</exception>
        public ExecutionEventArgs(ExecutionToken executionDetails, DateTimeOffset startTime, DateTimeOffset endTime, object state)
        {
            if (executionDetails == null)
                throw new ArgumentNullException("executionDetails", "executionDetails is null.");

            m_StartTime = startTime;
            m_EndTime = endTime;
            m_State = state;
            m_ExecutionDetails = executionDetails;
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
        public ExecutionEventArgs(ExecutionToken executionDetails, DateTimeOffset startTime, DateTimeOffset endTime, int? rowsAffected, object state)
        {
            if (executionDetails == null)
                throw new ArgumentNullException("executionDetails", "executionDetails is null.");

            m_StartTime = startTime;
            m_EndTime = endTime;
            m_RowsAffected = rowsAffected;
            m_State = state;
            m_ExecutionDetails = executionDetails;
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
        public ExecutionEventArgs(ExecutionToken executionDetails, DateTimeOffset startTime, DateTimeOffset endTime, Exception error, object state)
        {
            if (executionDetails == null)
                throw new ArgumentNullException("executionDetails", "executionDetails is null.");

            m_StartTime = startTime;
            m_EndTime = endTime;
            m_Error = error;
            m_State = state;
            m_ExecutionDetails = executionDetails;
        }

        /// <summary>
        /// Gets the end time.
        /// </summary>
        /// <value>
        /// The end time.
        /// </value>
        public DateTimeOffset? EndTime
        {
            get { return m_EndTime; }
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public Exception Error
        {
            get { return m_Error; }
        }

        /// <summary>
        /// If available, this shows the number of rows affected by the execution.
        /// </summary>
        public int? RowsAffected
        {
            get { return m_RowsAffected; }
        }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public DateTimeOffset StartTime
        {
            get { return m_StartTime; }
        }

        /// <summary>
        /// Gets the duration of the request, if available.
        /// </summary>
        public TimeSpan? Duration
        {
            get { return (EndTime - StartTime); }
        }

        /// <summary>
        /// Gets the user-defined state.
        /// </summary>
        public object State
        {
            get { return m_State; }
        }

        /// <summary>
        /// Gets the details of the execution.
        /// </summary>
        /// <value>Returns null or details of the execution.</value>
        /// <remarks>You can cast this to a concrete type for more information.</remarks>
        public ExecutionToken ExecutionDetails
        {
            get { return m_ExecutionDetails; }
        }
    }

}
