using System;

namespace Tortuga.Chain
{

    [Flags]
    public enum FilterOptions
    {
        None = 0,
        /// <summary>
        /// The ignore properties that are null when constructing a WHERE clause.
        /// </summary>
        IgnoreNullProperties = 1
    }
}

