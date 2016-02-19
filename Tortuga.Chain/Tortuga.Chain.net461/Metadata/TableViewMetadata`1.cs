using System.Collections.Generic;
using System.Collections.ObjectModel;



namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Metadata for a database table or view.
    /// </summary>
    /// <typeparam name="TName">The type used to represent database object names.</typeparam>
    public class TableOrViewMetadata<TName>
    {
        private readonly bool m_IsTable;
        private readonly TName m_Name;
        private readonly ReadOnlyCollection<ColumnMetadata> m_Columns;

        /// <summary>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isTable">if set to <c>true</c> [is table].</param>
        /// <param name="columns">The columns.</param>
        public TableOrViewMetadata(TName name, bool isTable, IList<ColumnMetadata> columns)
        {
            m_IsTable = isTable;
            m_Name = name;
            m_Columns = new ReadOnlyCollection<ColumnMetadata>(columns);
        }


        /// <summary>
        /// Gets a value indicating whether this instance is table or a view.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is a table; otherwise, <c>false</c>.
        /// </value>
        public bool IsTable
        {
            get { return m_IsTable; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public TName Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public ReadOnlyCollection<ColumnMetadata> Columns
        {
            get { return m_Columns; }
        }

    }
}
