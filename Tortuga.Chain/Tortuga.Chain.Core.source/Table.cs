using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
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
    public sealed class Table
    {
        readonly ReadOnlyCollection<string> m_Columns;
        readonly ReadOnlyDictionary<string, Type> m_ColumnTypes;
        readonly RowCollection m_Rows;
        /// <summary>
        /// Creates a new NamedTable from an IDataReader
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="source">The source.</param>
        public Table(string tableName, DbDataReader source)
            : this(source)
        {
            TableName = tableName;
        }

        /// <summary>
        /// Creates a new Table from an IDataReader
        /// </summary>
        /// <param name="source"></param>
        public Table(DbDataReader source)
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

            var rows = new Collection<Row>();
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

                rows.Add(new Row(row));
            }
            m_Rows = new RowCollection(rows);
        }

        /// <summary>
        /// List of column names in their original order.
        /// </summary>
        public IReadOnlyList<string> ColumnNames
        {
            get { return m_Columns; }
        }

        /// <summary>
        /// List of columns and their types.
        /// </summary>
        public IReadOnlyDictionary<string, Type> ColumnTypeMap
        {
            get { return m_ColumnTypes; }
        }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public IReadOnlyList<Row> Rows
        {
            get { return m_Rows; }
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; set; }
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

                //Change tracking objects shouldn't be materialized as unchanged.
                var tracking = item as IChangeTracking;
                tracking?.AcceptChanges();

                yield return item;
            }
        }

        internal IEnumerable<KeyValuePair<Row, T>> ToObjectsWithEcho<T>(Type[] constructorSignature)
        {
            var desiredType = typeof(T);
            var constructor = desiredType.GetConstructor(constructorSignature);

            if (constructor == null)
            {
                var types = string.Join(", ", constructorSignature.Select(t => t.Name));
                throw new MappingException($"Cannot find a constructor on {desiredType.Name} with the types [{types}]");
            }

            var constructorParameters = constructor.GetParameters();
            for (var i = 0; i < constructorSignature.Length; i++)
            {
                if (!ColumnNames.Any(p => p.Equals(constructorParameters[i].Name, StringComparison.OrdinalIgnoreCase)))
                    throw new MappingException($"Cannot find a column that matches the parameter {constructorParameters[i].Name}");
            }

            foreach (var item in Rows)
            {
                var parameters = new object[constructorSignature.Length];
                for (var i = 0; i < constructorSignature.Length; i++)
                {
                    parameters[i] = item[constructorParameters[i].Name];
                }
                var result = constructor.Invoke(parameters);
                yield return new KeyValuePair<Row, T>(item, (T)result);
            }

        }

        internal IEnumerable<T> ToObjects<T>(Type[] constructorSignature)
        {
            var desiredType = typeof(T);
            var constructor = desiredType.GetConstructor(constructorSignature);

            if (constructor == null)
            {
                var types = string.Join(", ", constructorSignature.Select(t => t.Name));
                throw new MappingException($"Cannot find a constructor on {desiredType.Name} with the types [{types}]");
            }

            var constructorParameters = constructor.GetParameters();
            for (var i = 0; i < constructorSignature.Length; i++)
            {
                if (!ColumnNames.Any(p => p.Equals(constructorParameters[i].Name, StringComparison.OrdinalIgnoreCase)))
                    throw new MappingException($"Cannot find a column that matches the parameter {constructorParameters[i].Name}");
            }

            foreach (var item in Rows)
            {
                var parameters = new object[constructorSignature.Length];
                for (var i = 0; i < constructorSignature.Length; i++)
                {
                    parameters[i] = item[constructorParameters[i].Name];
                }
                var result = constructor.Invoke(parameters);
                yield return (T)result;
            }

        }
        /// <summary>
        /// Converts the table into an enumeration of objects of the indicated type.
        /// </summary>
        /// <typeparam name="T">Desired object type</typeparam>
        internal IEnumerable<KeyValuePair<Row, T>> ToObjectsWithEcho<T>() where T : new()
        {
            foreach (var row in Rows)
            {
                var item = new T();
                PopulateComplexObject(row, item, null);

                //Change tracking objects shouldn't be materialized as unchanged.
                var tracking = item as IChangeTracking;
                tracking?.AcceptChanges();

                yield return new KeyValuePair<Row, T>(row, item);
            }
        }

        /// <summary>
        /// Populates the complex object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The object being populated.</param>
        /// <param name="decompositionPrefix">The decomposition prefix.</param>
        /// <remarks>This honors the Column and Decompose attributes.</remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        static private void PopulateComplexObject(Row source, object target, string decompositionPrefix)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");
            if (target == null)
                throw new ArgumentNullException(nameof(target), $"{nameof(target)} is null.");

            foreach (var property in MetadataCache.GetMetadata(target.GetType()).Properties)
            {
                string mappedColumnName = decompositionPrefix + property.MappedColumnName;
                if (property.CanWrite && source.ContainsKey(mappedColumnName))
                {
                    var value = source[mappedColumnName];

                    if (value != null && property.PropertyType != value.GetType())
                    {
                        var targetType = property.PropertyType;
                        var targetTypeInfo = targetType.GetTypeInfo();

                        //For Nullable<T>, we only care about the type parameter
                        if (targetType.Name == "Nullable`1" && targetTypeInfo.IsGenericType)
                        {
                            targetType = targetType.GenericTypeArguments[0];
                            targetTypeInfo = targetType.GetTypeInfo();
                        }

                        //some database return strings when we want strong types
                        if (value is string)
                        {
                            if (targetType == typeof(XElement))
                                value = XElement.Parse((string)value);
                            else if (targetType == typeof(XDocument))
                                value = XDocument.Parse((string)value);
                            else if (targetTypeInfo.IsEnum)
                                value = Enum.Parse(targetType, (string)value);

                            else if (targetType == typeof(bool))
                                value = bool.Parse((string)value);
                            else if (targetType == typeof(short))
                                value = short.Parse((string)value);
                            else if (targetType == typeof(int))
                                value = int.Parse((string)value);
                            else if (targetType == typeof(long))
                                value = long.Parse((string)value);
                            else if (targetType == typeof(float))
                                value = float.Parse((string)value);
                            else if (targetType == typeof(double))
                                value = double.Parse((string)value);
                            else if (targetType == typeof(decimal))
                                value = decimal.Parse((string)value);

                            else if (targetType == typeof(DateTime))
                                value = DateTime.Parse((string)value);
                            else if (targetType == typeof(DateTimeOffset))
                                value = DateTimeOffset.Parse((string)value);
                        }
                        else
                        {
                            if (targetTypeInfo.IsEnum)
                                value = Enum.ToObject(targetType, value);
                        }

                        //this will handle numeric conversions
                        if (value != null && targetType != value.GetType())
                        {
                            try
                            {
                                value = Convert.ChangeType(value, targetType);
                            }
                            catch (Exception ex)
                            {
                                throw new MappingException($"Cannot map value of type {value.GetType().FullName} to property {property.Name} of type {targetType.Name}.", ex);
                            }
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
