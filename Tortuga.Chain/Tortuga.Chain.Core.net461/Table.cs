using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
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
                throw new ArgumentException("No columns were returned", "source");

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
        /// Converts the table into an enumeration of objects of the indicated type.
        /// </summary>
        /// <typeparam name="T">Desired object type</typeparam>
        public IEnumerable<T> ToObjects<T>() where T : new()
        {
            foreach (var row in Rows)
            {
                var item = new T();
                PopulateComplexObject(row, item, null);
                yield return item;
            }
        }

        /// <summary>
        /// Populates the complex object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The object being populated.</param>
        /// <param name="decompositionPrefix">The decomposition prefix.</param>
        /// <remarks>This honors the Column and Decompose attributes.</remarks>
        static private void PopulateComplexObject(IReadOnlyDictionary<string, object> source, object target, string decompositionPrefix)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");
            if (target == null)
                throw new ArgumentNullException(nameof(target), $"{nameof(target)} is null.");

            foreach (var property in MetadataCache.GetMetadata(target.GetType()).Properties)
            {
                if (property.CanWrite && source.ContainsKey(decompositionPrefix + property.MappedColumnName))
                {
                    var value = source[property.MappedColumnName];

                    if (value != null && property.PropertyType != value.GetType())
                    {
                        var targetType = property.PropertyType;

                        //For Nullable<T>, we only care about the type parameter
                        if (targetType.Name == "Nullable`1" && targetType.IsGenericType)
                            targetType = targetType.GenericTypeArguments[0];


                        //XML values come to us as strings
                        if (value is string)
                        {
                            if (targetType == typeof(XElement))
                                value = XElement.Parse((string)value);
                            else if (targetType == typeof(XDocument))
                                value = XDocument.Parse((string)value);
                            else if (targetType.IsEnum)
                                value = Enum.Parse(targetType, (string)value);
                        }
                        else
                        {
                            if (targetType.IsEnum)
                                value = Enum.ToObject(targetType, value);
                        }

                        //this will handle integer conversions
                        if (value != null && targetType != value.GetType())
                        {
                            value = Convert.ChangeType(value, targetType);
                        }
                    }
                    property.InvokeSet(target, value);
                }
                else if (property.Decompose)
                {
                    object child = null;

                    if (property.CanRead)
                        child = property.InvokeGet(target);

                    if (child == null && property.CanWrite && property.PropertyType.GetConstructor(new Type[0]) != null)
                    {
                        child = Activator.CreateInstance(property.PropertyType);
                        property.InvokeSet(target, child);
                    }

                    if (child != null)
                        PopulateComplexObject(source, child, decompositionPrefix + property.DecompositionPrefix);
                }
            }
        }
    }
}
