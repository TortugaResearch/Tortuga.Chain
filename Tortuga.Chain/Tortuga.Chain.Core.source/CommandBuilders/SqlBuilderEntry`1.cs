using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders
{
    /// <typeparam name="TDbType">The type of the database type.</typeparam>
    /// <remarks>This is a struct because we want fast allocations and copies. Try to keep it at 16 bytes or less.</remarks>
    /// <remarks>For internal use only.</remarks>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    public struct SqlBuilderEntry<TDbType>
        where TDbType : struct
    {
        /// <summary>
        /// Gets or sets the immutable column metadata.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        public ISqlBuilderEntryDetails<TDbType> Details { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a formal parameter for a stored procedure or table value function.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [formal parameter]; otherwise, <c>false</c>.
        /// </value>
        public bool IsFormalParameter { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SqlBuilderEntry{TDbType}"/> participates in insert operations.
        /// </summary>
        /// <value>
        ///   <c>true</c> if insert; otherwise, <c>false</c>.
        /// </value>
        public bool UseForInsert { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column should be treated as primary key.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is primary key; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This can be overridden. For example, if the parameter object defines its own alternate keys.
        /// </remarks>
        public bool IsKey { get; set; }

        /// <summary>
        /// Gets or sets the value to be used when constructing parameters.
        /// </summary>
        /// <value>
        /// The parameter value.
        /// </value>
        /// <remarks>A null means this parameter's value was not set. A DBNull.Value means it is passed to the database as a null.</remarks>
        public object ParameterValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SqlBuilderEntry{TDbType}"/> participates in read operations.
        /// </summary>
        /// <value>
        ///   <c>true</c> if read; otherwise, <c>false</c>.
        /// </value>
        public bool UseForRead { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SqlBuilderEntry{TDbType}"/> participates in update operations.
        /// </summary>
        /// <value>
        ///   <c>true</c> if update; otherwise, <c>false</c>.
        /// </value>
        public bool UseForUpdate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SqlBuilderEntry{TDbType}"/> participates in parameter generation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use parameter]; otherwise, <c>false</c>.
        /// </value>
        public bool UseParameter { get; set; }

        /// <summary>
        /// When non-null, this indicates that we want to use a table value parameter instead of a normal parameter.
        /// </summary>
        public ISqlBuilderEntryDetails<TDbType> ParameterColumn { get; set; }
    }
}
