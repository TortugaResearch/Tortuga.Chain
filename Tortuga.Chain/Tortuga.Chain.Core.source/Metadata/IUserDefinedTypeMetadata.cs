using System.Collections.Generic;

namespace Tortuga.Chain.Metadata
{

    /// <summary>
    /// This interface represents user defined types.
    /// </summary>
    public interface IUserDefinedTypeMetadata
    {

        /// <summary>
        /// Gets a value indicating whether this instance is table type or a normal UDF.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is a table type; otherwise, <c>false</c>.
        /// </value>
        bool IsTableType { get; }

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
