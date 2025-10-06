namespace Tortuga.Chain.SqlServer.CommandBuilders;

interface ISupportsHistory
{
	/// <summary>
	/// Adds the history clause.
	/// </summary>
	/// <param name="fromDate">From date.</param>
	/// <param name="toDate">To date.</param>
	/// <param name="mode">The mode.</param>
	void AddHistoryClause(DateTime? fromDate, DateTime? toDate, HistoryQueryMode mode);
}