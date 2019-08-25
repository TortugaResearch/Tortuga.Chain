using System;
using System.Linq;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// The type of index.
    /// </summary>
    public enum SqlServerIndexType
    {
        /// <summary>
        /// Heap
        /// </summary>
        Heap = 0,

        /// <summary>
        /// Clustered
        /// </summary>
        Clustered = 1,

        /// <summary>
        /// Nonclustered
        /// </summary>
        Nonclustered = 2,

        /// <summary>
        /// XML
        /// </summary>
        Xml = 3,

        /// <summary>
        /// Spatial
        /// </summary>
        Spatial = 4,

        /// <summary>
        /// Clustered columnstore index
        /// </summary>
        ClusteredColumnstoreIndex = 5,

        /// <summary>
        /// Nonclustered columnstore index
        /// </summary>
        NonclusteredColumnstoreIndex = 6,

        /// <summary>
        /// Nonclustered hash index
        /// </summary>
        NonclusteredHashIndex = 7


    }
}
