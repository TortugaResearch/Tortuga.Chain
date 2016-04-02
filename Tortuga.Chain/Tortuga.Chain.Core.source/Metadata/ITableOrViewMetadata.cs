using System.Collections.Generic;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Abstract version of TableOrViewMetadata.
    /// </summary>
    public interface ITableOrViewMetadata
    {
        /// <summary>
        /// Gets a value indicating whether this instance is table or a view.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is a table; otherwise, <c>false</c>.
        /// </value>
        bool IsTable { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        IReadOnlyList<IColumnMetadata> Columns { get; }
    }
}
