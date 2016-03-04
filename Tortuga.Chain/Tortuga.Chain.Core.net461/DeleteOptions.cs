using System;

namespace Tortuga.Chain
{
    /// <summary>
    /// Controls what happens when performing a model-based delete
    /// </summary>
    [Flags]
    public enum DeleteOptions
    {

        /// <summary>
        /// Use the primary key columns for the where clause.
        /// </summary>
        None = 0,


        /// <summary>
        /// Ignore the primary keys on the table and perform the delete using the Key attribute on properties to construct the where clause.
        /// </summary>
        /// <remarks>This is generally used for heap-style tables, though technically heap tables may have primary, non-clustered keys.</remarks>
        UseKeyAttribute = 2
    }
}
