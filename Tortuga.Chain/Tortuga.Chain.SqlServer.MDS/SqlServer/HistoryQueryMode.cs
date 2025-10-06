namespace Tortuga.Chain.SqlServer;


/// <summary>
/// Used to control which type of query will be used for system versioned tables
/// </summary>
public enum HistoryQueryMode
{
	/// <summary>
	/// Only return the current version of the record.
	/// </summary>
	None = 0,

	/// <summary>
	/// Return all versions of the record.
	/// </summary>
	All = 1,

	/// <summary>
	/// Return the version of the record as of a specific date.
	/// </summary>
	AsOfDate = 2,

	/// <summary>
	/// Return all versions of the record matching `ValidFrom &lt; end_date_time AND ValidTo &gt; start_date_time`
	/// </summary>
	FromTo = 3,

	/// <summary>
	/// Return all versions of the record matching `ValidFrom &lt;= end_date_time AND ValidTo &gt; start_date_time`
	/// </summary>
	Between = 4,

	/// <summary>
	/// Return all versions of the record matching `ValidFrom &gt;= start_date_time AND ValidTo &lt;= end_date_time`
	/// </summary>
	Contains = 5
}