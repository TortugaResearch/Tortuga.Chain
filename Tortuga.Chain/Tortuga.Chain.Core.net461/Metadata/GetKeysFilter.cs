using System;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Enum GetKeysFilter is used similarly to GetPropertiesFilter, but for IDictionary style arguments;
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames")]
    [Flags]
    public enum GetKeysFilter
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

        ///// <summary>
        ///// Return key columns as defined by the Key attribute on object properties
        ///// </summary>
        //ObjectDefinedKey = 4,

        ///// <summary>
        ///// Return non-key columns as defined by the Key attribute on object properties
        ///// </summary>
        //ObjectDefinedNonKey = 8,

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
        MutableColumns = 128,

        ///// <summary>
        ///// An insert operation is being performed, so honor the IgnoreOnInsert attribute on properties.
        ///// </summary>
        ///// <remarks>Only applies to non-key columns.</remarks>
        //ForInsert = 256,

        ///// <summary>
        ///// An update operation is being performed, so honor the IgnoreOnUpdate attribute on properties.
        ///// </summary>
        ///// <remarks>Only applies to non-key columns.</remarks>
        //ForUpdate = 512




    }
}
