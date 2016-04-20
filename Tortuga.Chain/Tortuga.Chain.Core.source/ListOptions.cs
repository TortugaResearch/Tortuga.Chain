using System;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain
{
    /// <summary>
    /// Indicates how the list will be generated from a result set.
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags")]
    [Flags]
    public enum ListOptions
    {
        /// <summary>
        /// An error will occur unless exactly one column is returned.
        /// </summary>
        None = 0,

        /// <summary>
        /// Null values will be removed from the list.
        /// </summary>
        DiscardNulls = 1,

        /// <summary>
        /// If extra columns are returned, all but the first will be ignored.
        /// </summary>
        IgnoreExtraColumns = 2,

        /// <summary>
        /// All columns will be incorporated into the result set. Values are read left to right, then top to bottom.
        /// </summary>
        FlattenExtraColumns = 2 + 4,

    }


}
