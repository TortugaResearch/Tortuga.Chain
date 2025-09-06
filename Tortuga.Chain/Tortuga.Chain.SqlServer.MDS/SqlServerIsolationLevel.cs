namespace Tortuga.Chain;

/// <summary>
/// Specifies isolation level for the operation.
/// </summary>
[SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "<Pending>")]
[SuppressMessage("Design", "CA1027:Mark enums with FlagsAttribute", Justification = "<Pending>")]
public enum SqlServerIsolationLevel
{

	/// <summary>
	/// A dirty read is possible, meaning that no shared locks are issued and no exclusive locks are honored.
	/// </summary>
	ReadUncommitted = IsolationLevel.ReadUncommitted,

	/// <summary>
	/// Shared locks are held while the data is being read to avoid dirty reads, but the data can be changed before the end of the transaction, resulting in non-repeatable reads or phantom data.
	/// </summary>
	ReadCommitted = IsolationLevel.ReadCommitted,

	/// <summary>
	/// Locks are placed on all data that is used in a query, preventing other users from updating the data. Prevents non-repeatable reads but phantom rows are still possible.
	/// </summary>
	RepeatableRead = IsolationLevel.RepeatableRead,

	/// <summary>
	/// A range lock is placed on the System.Data.DataSet, preventing other users from updating or inserting rows into the dataset until the transaction is complete.
	/// </summary>
	Serializable = IsolationLevel.Serializable,

	/// <summary>
	/// Reduces blocking by storing a version of data that one application can read while another is modifying the same data. Indicates that from one transaction you cannot see changes made in other transactions, even if you requery.
	/// </summary>
	Snapshot = IsolationLevel.Snapshot
}