using System;
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
    public class TableSet : KeyedCollection<string, Table>
    {

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
                Add(new Table(tableName, reader));
                index += 1;
            }
            while (reader.NextResult());
        }

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
}
