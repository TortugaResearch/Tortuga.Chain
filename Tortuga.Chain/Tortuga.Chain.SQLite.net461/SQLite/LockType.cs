namespace Tortuga.Chain.SQLite
{
    /// <summary>
    /// This is used for ensuring that only one writer is active at a time for a given SQLite database.
    /// </summary>
    public enum LockType
    {
        None = 0,

        /// <summary>
        /// The operation only performs reads.
        /// </summary>
        Read = 1,

        /// <summary>
        /// The operation potentially performs writes.
        /// </summary>
        /// <remarks>When in doubt, use this mode.</remarks>
        Write = 2
    }
}
