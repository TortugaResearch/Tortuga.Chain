using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain
{
    /// <summary>
    /// A set of named tables.
    /// </summary>
    /// <remarks>This is much faster than a DataSet, but lacks most of its features.</remarks>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public sealed class TableSet : IReadOnlyList<Table>
    {
        internal class TSKeyedCollection : KeyedCollection<string, Table>
        {
            /// <summary>
            /// When implemented in a derived class, extracts the key from the specified element.
            /// </summary>
            /// <param name="item">The element from which to extract the key.</param>
            /// <returns>The key for the specified element.</returns>
            protected override string GetKeyForItem(Table item)
            {
                if (item == null)
                    throw new ArgumentNullException("item", "item is null.");

                return item.TableName;
            }
        }

        readonly TSKeyedCollection m_Internal = new TSKeyedCollection();

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count
        {
            get { return m_Internal.Count; }
        }

        /// <summary>
        /// Gets the <see cref="Table"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="Table"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Table this[int index]
        {
            get { return m_Internal[index]; }
        }

        /// <summary>
        /// Gets the <see cref="Table"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="Table"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Table this[string key]
        {
            get { return m_Internal[key]; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="TableSet"/> class.
        /// </summary>
        /// <param name="reader">The data reader used to populate this TableSet.</param>
        /// <param name="tableNames">Optional list of table names.</param>
        public TableSet(DbDataReader reader, params string[] tableNames)
        {
            if (reader == null)
                throw new ArgumentNullException("reader", "reader is null.");
            if (tableNames == null)
                tableNames = new string[0];

            var index = 0;
            do
            {
                var tableName = (index < tableNames.Length) ? tableNames[index] : ("Table " + index);
                m_Internal.Add(new Table(tableName, reader));
                index += 1;
            }
            while (reader.NextResult());
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Table> GetEnumerator()
        {
            return m_Internal.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Internal.GetEnumerator();
        }


    }
}
