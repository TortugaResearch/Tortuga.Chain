using System;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Enum GetPropertiesFilter
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames")]
    [Flags]
    public enum GetPropertiesFilter
    {
        /// <summary>
        /// Return all column/property matches
        /// </summary>
        None = 0,

        /// <summary>
        /// Only return primary key columns
        /// </summary>
        PrimaryKey = 1,

        /// <summary>
        /// Only return non-primary key columns
        /// </summary>
        NonPrimaryKey = 2,

        /// <summary>
        /// Return key columns as defined by the Key attribute on object properties
        /// </summary>
        ObjectDefinedKey = 4,

        /// <summary>
        /// Return non-key columns as defined by the Key attribute on object properties
        /// </summary>
        ObjectDefinedNonKey = 8,

        /// <summary>
        /// Throw an exception if there are no matches
        /// </summary>
        ThrowOnNoMatch = 16,

        /// <summary>
        /// Throw an exception if there are properties on the object that can't be mapped to a column.
        /// </summary>
        ThrowOnMissingColumns = 32,

        /// <summary>
        /// Throw an exception if there are key columns that can't be mapped to a property.
        /// </summary>
        /// <remarks>Only applies when using the PrimaryKey flag.</remarks>
        ThrowOnMissingProperties = 64,

        /// <summary>
        /// Only return columns that can be updated. This means no identity or computed columns
        /// </summary>
        UpdatableOnly = 128


    }
}
