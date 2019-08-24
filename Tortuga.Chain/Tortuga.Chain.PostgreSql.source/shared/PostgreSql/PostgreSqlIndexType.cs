namespace Tortuga.Chain.PostgreSql
{
    /// <summary>
    /// The type of index.
    /// </summary>
    public enum PostgreSqlIndexType
    {
        /// <summary>
        /// Unknown index type.
        /// </summary>
        /// <remarks>
        /// This usually means you are using a newer version of PostgreSQL than what's supported by this library.
        /// Please file a bug report.
        /// </remarks>
        Unknown = 0,

        /// <summary>
        /// B-Tree Index
        /// </summary>
        BTree = 1,

        /// <summary>
        /// Hash
        /// </summary>
        Hash = 2,

        /// <summary>
        /// Generalized Inverted Seach Tree (GiST)
        /// </summary>
        Gist = 3,

        /// <summary>
        /// Generalized Inverted Index (GIN)
        /// </summary>
        Gin = 4,

        /// <summary>
        /// Space partitioned GiST (SP-GiST)
        /// </summary>
        Spgist = 5,

        /// <summary>
        /// Block Range Indexes (BRIN)
        /// </summary>
        Brin = 6
    }
}
