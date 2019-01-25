using System;

namespace Tortuga.Chain
{
    /// <summary>
    /// Controls what happens when performing a model-based insert or update
    /// </summary>
    [Flags]
    public enum UpsertOptions
    {
        /// <summary>
        /// Update all non-primary key columns using the primary key columns for the where clause.
        /// </summary>
        None = 0,

        /// <summary>
        /// Uses the IPropertyChangeTracking interface to only update changed properties. This flag has no effect when performing an insert.
        /// </summary>
        /// <remarks>If this flag is set and IPropertyChangeTracking.IsChanged is false, an error will occur.</remarks>
        ChangedPropertiesOnly = 1,

        /// <summary>
        /// Ignore the primary keys on the table and perform the update using the Key attribute on properties to construct the where clause.
        /// </summary>
        /// <remarks>This is generally used for heap-style tables, though technically heap tables may have primary, non-clustered keys.</remarks>
        UseKeyAttribute = 2,

        /// <summary>
        /// Allows overriding identity/auto-number column
        /// </summary>
        /// <remarks>This may require elevated privileges. This is not supported by all databases.</remarks>
        IdentityInsert = 4,

        /*
         * Might need this for PostgreSQL
        /// <summary>
        /// Do not reset the identity/auto-number column after performing an identity insert.
        /// </summary>
        /// <remarks>Use this when performing a series of identity inserts to improve performance. Then invoke ResetIdentity on the DataSource. This is a no-op when resetting the identity column is not necessary (Access, SQL Server, SQLite).</remarks>
        DoNotResetIdentityColumn = 8
        */
    }
}