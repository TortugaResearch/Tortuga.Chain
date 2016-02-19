using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain
{
    /// <summary>
    /// A light-weight object to hold tabular data
    /// </summary>
    /// <remarks>
    /// This is much faster than a DataTable, but lacks most of its features.
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class Table
    {
        readonly IReadOnlyList<IReadOnlyDictionary<string, object>> m_Rows;
        readonly IReadOnlyList<string> m_Columns;
        readonly IReadOnlyDictionary<string, Type> m_ColumnTypes;

        /// <summary>
        /// Creates a new NamedTable from an IDataReader
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="source">The source.</param>
        public Table(string tableName, IDataReader source)
            : this(source)
        {
            TableName = tableName;
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; set; }

        /// <summary>
        /// Creates a new Table from an IDataReader
        /// </summary>
        /// <param name="source"></param>
        public Table(IDataReader source)
        {
            if (source == null)
                throw new ArgumentNullException("source", "source is null.");
            if (source.FieldCount == 0)
                throw new DataException("No columns were returned");
            Contract.EndContractBlock();

            var cols = new List<string>(source.FieldCount);
            var colTypes = new Dictionary<string, Type>(source.FieldCount, StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < source.FieldCount; i++)
            {
                cols.Add(source.GetName(i));
                colTypes.Add(source.GetName(i), source.GetFieldType(i));
            }
            m_Columns = new ReadOnlyCollection<string>(cols);
            m_ColumnTypes = new ReadOnlyDictionary<string, Type>(colTypes);

            var rows = new Collection<ReadOnlyDictionary<string, object>>();
            while (source.Read())
            {
                var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < source.FieldCount; i++)
                {
                    var temp = source[i];
                    if (temp == DBNull.Value)
                        temp = null;

                    row.Add(m_Columns[i], temp);
                }

                rows.Add(new ReadOnlyDictionary<string, object>(row));
            }
            m_Rows = new ReadOnlyCollection<ReadOnlyDictionary<string, object>>(rows);
        }


        /// <summary>
        /// List of columns and their types.
        /// </summary>
        public IReadOnlyDictionary<string, Type> ColumnTypeMap
        {
            get { return m_ColumnTypes; }
        }

        /// <summary>
        /// List of column names in their original order.
        /// </summary>
        public IReadOnlyList<string> ColumnNames
        {
            get { return m_Columns; }
        }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public IReadOnlyList<IReadOnlyDictionary<string, object>> Rows
        {
            get { return m_Rows; }
        }


        /// <summary>
        /// Converts the table into an enumeration of models of the indicated type.
        /// </summary>
        /// <typeparam name="T">Desired model type</typeparam>
        public IEnumerable<T> ToModels<T>() where T : new()
        {
            foreach (var row in Rows)
            {
                var item = new T();
                MetadataCache.PopulateComplexObject(row, item, null);
                yield return item;
            }
        }
    }
}
