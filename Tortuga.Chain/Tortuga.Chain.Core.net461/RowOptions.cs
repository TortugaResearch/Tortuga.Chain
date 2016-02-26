using System;

namespace Tortuga.Chain
{
    /// <summary>
    /// Controls what happens when the wrong number of rows are returned.
    /// </summary>
    [Flags]
    public enum RowOptions
    {
        /// <summary>
        /// An error will be raised unless exactly one row is returned
        /// </summary>
        /// <remarks></remarks>
        None = 0,
        /// <summary>
        /// An error will not be raised if no rows are returned.
        /// </summary>
        /// <remarks></remarks>
        AllowEmptyResults = 1,
        /// <summary>
        /// An error will not be raised if extra rows are returned. The extras will be discarded.
        /// </summary>
        /// <remarks></remarks>
        DiscardExtraRows = 2
    }
}
