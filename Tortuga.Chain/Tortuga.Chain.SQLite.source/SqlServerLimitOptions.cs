using System;

namespace Tortuga.Chain
{
    /// <summary>
    /// Limit options supported by SQL Server.
    /// </summary>
    /// <remarks>This is a strict subset of LimitOptions</remarks>
    [Flags]
    public enum SQLiteLimitOptions
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
        /// Randomly sample the indicated number of rows
        /// </summary>
        RandomSampleRows = LimitOptions.RandomSampleRows,


    }
}
