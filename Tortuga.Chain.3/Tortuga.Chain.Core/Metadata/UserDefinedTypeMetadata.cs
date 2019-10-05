using System;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// This represents user defined types in the database.
    /// </summary>
    public abstract class UserDefinedTypeMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDefinedTypeMetadata{TName, TDbType}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isTableType">if set to <c>true</c> [is table].</param>
        /// <param name="columns">The columns.</param>
        protected UserDefinedTypeMetadata(string name, bool isTableType, ColumnMetadataCollection columns)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"{nameof(name)} is null or empty.", nameof(name));

            IsTableType = isTableType;
            Name = name;
            Columns = columns ?? throw new ArgumentNullException(nameof(columns), $"{nameof(columns)} is null.");
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public ColumnMetadataCollection Columns { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is table type or a normal UDF.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is a table type; otherwise, <c>false</c>.
        /// </value>
        public bool IsTableType { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }
    }
}
