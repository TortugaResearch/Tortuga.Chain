using System;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain
{
    /// <summary>
    /// Used to indicate the sort direction
    /// </summary>
    public enum SortDirection : byte
    {
        Ascending = 0,
        Descending = 1,
    }

    /// <summary>
    /// Sort expressions are used for From and FromFunction command builders.
    /// </summary>
    public class SortExpression
    {
        private readonly string m_ColumnName;
        private readonly SortDirection m_Direction;

        /// <summary>
        /// Gets the name of the column to be sorted by.
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        public string ColumnName
        {
            get { return m_ColumnName; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="SortExpression"/> is descending.
        /// </summary>
        /// <value>
        ///   <c>true</c> if descending; otherwise, <c>false</c>.
        /// </value>
        public SortDirection Direction
        {
            get { return m_Direction; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortExpression"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column to be sorted by.</param>
        /// <param name="descending">if set to <c>true</c> [descending].</param>
        /// <exception cref="ArgumentException"></exception>
        public SortExpression(string columnName, SortDirection descending)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException($"{nameof(columnName)} is null or empty.", nameof(columnName));

            m_ColumnName = columnName;
            m_Direction = descending;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortExpression"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column to be sorted by.</param>
        /// <exception cref="ArgumentException"></exception>
        public SortExpression(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException($"{nameof(columnName)} is null or empty.", nameof(columnName));

            m_ColumnName = columnName;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="string"/> to <see cref="SortExpression"/>.
        /// </summary>
        /// <param name="columnName">The s.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator SortExpression(string columnName)
        {
            return new SortExpression(columnName);
        }

        internal ISqlBuilderEntryDetails Column { get; set; }


    }
}

