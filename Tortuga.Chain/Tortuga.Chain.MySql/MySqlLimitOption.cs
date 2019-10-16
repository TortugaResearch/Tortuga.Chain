namespace Tortuga.Chain
{
    /// <summary>
    /// Limit options supported by PostgreSQL.
    /// </summary>
    /// <remarks>This is a strict subset of LimitOptions</remarks>
    public enum MySqlLimitOption
    {
        /// <summary>
        /// No limits were applied.
        /// </summary>
        None = LimitOptions.None,

        /// <summary>
        /// Returns the indicated number of rows with optional offset
        /// </summary>
        Rows = LimitOptions.Rows,

        /// <summary>
        /// Randomly sample the indicated number of rows
        /// </summary>
        /// <remarks>WARNING: This uses "ORDER BY RAND()", which is inappropriate for large tables.</remarks>
        RandomSampleRows = LimitOptions.RandomSampleRows,

    }
}
