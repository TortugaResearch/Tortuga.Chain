namespace Tortuga.Chain
{
	/// <summary>
	/// Represents a notification from an abortable process such as a bulk insert.
	/// </summary>
	public class AbortableOperationEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbortableOperationEventArgs"/> class.
		/// </summary>
		/// <param name="rowsAffected">The number of rows affected.</param>
		public AbortableOperationEventArgs(long rowsAffected) { RowsAffected = rowsAffected; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="AbortableOperationEventArgs"/> is abort.
		/// </summary>
		/// <value>Set to True to abort the current operation.</value>
		public bool Abort { get; set; }

		/// <summary>
		/// Gets the number of rows copied.
		/// </summary>
		public long RowsAffected { get; }
	}
}
