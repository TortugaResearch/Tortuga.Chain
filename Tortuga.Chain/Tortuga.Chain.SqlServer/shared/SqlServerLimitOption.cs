namespace Tortuga.Chain
{
	/// <summary>
	/// Limit options supported by SQL Server.
	/// </summary>
	/// <remarks>This is a strict subset of LimitOptions</remarks>
	public enum SqlServerLimitOption
	{
		/// <summary>
		/// No limits were applied.
		/// </summary>
		None = LimitOptions.None,

		/// <summary>
		/// Uses OFFSET/FETCH
		/// </summary>
		Rows = LimitOptions.Rows,

		/// <summary>
		/// Uses TOP (N) PERCENT
		/// </summary>
		Percentage = LimitOptions.Percentage,

		/// <summary>
		/// Uses TOP (N) WITH TIES
		/// </summary>
		RowsWithTies = LimitOptions.RowsWithTies,

		/// <summary>
		/// Uses TOP (N) PERCENT WITH TIES
		/// </summary>
		PercentageWithTies = LimitOptions.PercentageWithTies,

		/// <summary>
		/// Uses TABLESAMPLE SYSTEM (N ROWS)
		/// </summary>
		TableSampleSystemRows = LimitOptions.TableSampleSystemRows,

		/// <summary>
		/// Uses TABLESAMPLE SYSTEM (N PERCENT)
		/// </summary>
		TableSampleSystemPercentage = LimitOptions.TableSampleSystemPercentage,
	}
}
