using System;

namespace Tortuga.Chain
{
    /// <summary>
    /// Controls what happens when performing a model-based update
    /// </summary>
    [Flags]
    public enum UpdateOptions
    {

        /// <summary>
        /// Update all non-primary key columns using the primary key columns for the where clause.
        /// </summary>
        None = 0,


        /// <summary>
        /// Uses the IPropertyChangeTracking interface to only update changed properties.
        /// </summary>
        ChangedPropertiesOnly = 1,

        /// <summary>
        /// Ignore the primary keys on the table and perform the update using the Key attribute on properties to construct the where clause.
        /// </summary>
        /// <remarks>This is generally used for heap-style tables, though technically heap tables may have primary, non-clustered keys.</remarks>
        UseKeyAttribute = 2

    }
}
