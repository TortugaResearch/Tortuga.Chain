namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Metadata for an index.
    /// </summary>
    /// <typeparam name="TName">The type of the name.</typeparam>
    /// <typeparam name="TDbType">The type of the database type.</typeparam>
    public class IndexMetadata<TName, TDbType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexMetadata{TName, TDbType}"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table (or view).</param>
        /// <param name="name">The name.</param>
        /// <param name="isPrimaryKey">if set to <c>true</c> is a primary key.</param>
        /// <param name="isUnique">if set to <c>true</c> is a unique index.</param>
        public IndexMetadata(TName tableName, string name, bool isPrimaryKey, bool isUnique)
        {
            TableName = tableName;
            Name = name;
            IsUnique = isUnique;
            IsPrimaryKey = isPrimaryKey;
        }

        /// <summary>
        /// Gets a value indicating whether this nidex is a primary key.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is primary key; otherwise, <c>false</c>.
        /// </value>
        public bool IsPrimaryKey { get; }

        /// <summary>
        /// Gets a value indicating whether this is a unique index.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is unique; otherwise, <c>false</c>.
        /// </value>
        public bool IsUnique { get; }

        /// <summary>
        /// Gets the name of the index.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the name of the table (or view) the index applies to.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public TName TableName { get; }
    }
}

