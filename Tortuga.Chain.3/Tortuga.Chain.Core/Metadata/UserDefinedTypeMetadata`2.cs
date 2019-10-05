using System;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// This class represents user defined types.
    /// </summary>
    /// <typeparam name="TName">The type of the t name.</typeparam>
    /// <typeparam name="TDbType">The type of the t database type.</typeparam>
    public sealed class UserDefinedTypeMetadata<TName, TDbType> : UserDefinedTypeMetadata
        where TName : struct
        where TDbType : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDefinedTypeMetadata{TName, TDbType}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isTableType">if set to <c>true</c> [is table].</param>
        /// <param name="columns">The columns.</param>
        public UserDefinedTypeMetadata(TName name, bool isTableType, ColumnMetadataCollection<TDbType> columns)
            : base(name.ToString(), isTableType, columns?.GenericCollection!)
        {
            Name = name;
            Columns = columns ?? throw new ArgumentNullException(nameof(columns), $"{nameof(columns)} is null.");
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public new ColumnMetadataCollection<TDbType> Columns { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public new TName Name { get; }
    }
}
