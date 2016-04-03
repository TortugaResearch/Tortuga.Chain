namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDbType">The type of the database type.</typeparam>
    /// <remarks>This is a struct because we want fast allocations and copies. Try to keep it at 16 bytes or less.</remarks>
    public struct SqlBuilderEntry<TDbType>
        where TDbType : struct
    {
        /// <summary>
        /// Gets or sets the immutable column metadata.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        public ColumnMetadata<TDbType> Column { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SqlBuilderEntry{TDbType}"/> participates in insert operations.
        /// </summary>
        /// <value>
        ///   <c>true</c> if insert; otherwise, <c>false</c>.
        /// </value>
        public bool Insert { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is key.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is key; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>This can be overriden. For example, if the parameter object defines its own alternate keys.</remarks>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// Gets or sets the value to be used when constructing parameters.
        /// </summary>
        /// <value>
        /// The parameter value.
        /// </value>
        /// <remarks>A null means this parameter is not used. A DBNull.Value means it is passed to the database as a null.</remarks>
        public object ParameterValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SqlBuilderEntry{TDbType}"/> participates in read operations.
        /// </summary>
        /// <value>
        ///   <c>true</c> if read; otherwise, <c>false</c>.
        /// </value>
        public bool Read { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SqlBuilderEntry{TDbType}"/> participates in update operations.
        /// </summary>
        /// <value>
        ///   <c>true</c> if update; otherwise, <c>false</c>.
        /// </value>
        public bool Update { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SqlBuilderEntry{TDbType}"/> participates in parameter generation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use parameter]; otherwise, <c>false</c>.
        /// </value>
        public bool UseParameter { get; set; }
    }
}
