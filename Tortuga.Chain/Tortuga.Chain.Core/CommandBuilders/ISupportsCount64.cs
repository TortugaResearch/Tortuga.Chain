namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// Used to mark command builders that support Int64 counts;
/// </summary>
public interface ISupportsCount64
{
	/// <summary>
	/// Returns a 64 bit row count using a <c>SELECT Count(*)</c> style query.
	/// </summary>
	/// <returns></returns>
	public ILink<long> AsCount64();

	/// <summary>
	/// Returns a 64 bit row count for a given column. <c>SELECT Count(columnName)</c>
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <param name="distinct">if set to <c>true</c> use <c>SELECT COUNT(DISTINCT columnName)</c>.</param>
	/// <returns></returns>
	public ILink<long> AsCount64(string columnName, bool distinct = false);
}
