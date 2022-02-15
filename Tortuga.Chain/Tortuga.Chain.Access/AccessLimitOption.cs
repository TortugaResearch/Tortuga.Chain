namespace Tortuga.Chain
{
	/// <summary>
	/// Limit options supported by Access.
	/// </summary>
	/// <remarks>This is a strict subset of LimitOptions</remarks>
	public enum AccessLimitOption
	{
		/// <summary>
		/// No limits were applied.
		/// </summary>
		None = LimitOptions.None,

		/// <summary>
		/// Uses TOP
		/// </summary>
		RowsWithTies = LimitOptions.RowsWithTies,

	}
}
