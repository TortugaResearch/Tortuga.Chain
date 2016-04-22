using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain
{
    /// <summary>
    /// Creates a DbDataReader wrapper over a list of objects.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <seealso cref="DbDataReader" />
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class ObjectDataReader<TObject> : DbDataReader
    {
        private IEnumerator<TObject> m_Source;
        private readonly DataTable m_Schema;
        private readonly ImmutableArray<PropertyMetadata> m_PropertyList;
        private readonly ImmutableDictionary<string, int> m_PropertyLookup;
        private int? m_RecordCount;


        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDataReader{TObject}" /> class.
        /// </summary>
        /// <param name="tableType">Type of the table.</param>
        /// <param name="source">The source.</param>
        /// <param name="operationType">Type of the operation being performed.</param>
        public ObjectDataReader(IUserDefinedTypeMetadata tableType, IEnumerable<TObject> source, OperationTypes operationType = OperationTypes.None)
        {
            if (tableType == null)
                throw new ArgumentNullException(nameof(tableType), $"{nameof(tableType)} is null.");
            if (source == null)
                throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");

            //Don't use IEnumerable<T>.Count(), as we don't want to preemptively materialize a lazy collection
            if (source is ICollection)
                m_RecordCount = ((ICollection)source).Count;

            m_Source = source.GetEnumerator();
            var metadata = BuildStructure(tableType.Name, tableType.Columns, true, operationType);
            m_Schema = metadata.Schema;
            m_PropertyList = metadata.Properties;
            m_PropertyLookup = metadata.PropertyLookup;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDataReader{TObject}" /> class.
        /// </summary>
        /// <param name="tableOrView">The table or view.</param>
        /// <param name="source">The source.</param>
        /// <param name="operationType">Type of the operation being performed.</param>
        public ObjectDataReader(ITableOrViewMetadata tableOrView, IEnumerable<TObject> source, OperationTypes operationType = OperationTypes.None)
        {
            if (tableOrView == null)
                throw new ArgumentNullException(nameof(tableOrView), $"{nameof(tableOrView)} is null.");
            if (source == null)
                throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");


            //Don't use IEnumerable<T>.Count(), as we don't want to preemptively materialize a lazy collection
            if (source is ICollection)
                m_RecordCount = ((ICollection)source).Count;

            m_Source = source.GetEnumerator();
            var metadata = BuildStructure(tableOrView.Name, tableOrView.Columns, false, operationType);
            m_Schema = metadata.Schema;
            m_PropertyList = metadata.Properties;
            m_PropertyLookup = metadata.PropertyLookup;
        }

        private class ObjectDataReaderMetatData
        {
            public ObjectDataReaderMetatData(DataTable schema, ImmutableArray<PropertyMetadata> properties, ImmutableDictionary<string, int> propertyLookup)
            {
                Schema = schema;
                Properties = properties;
                PropertyLookup = propertyLookup;
            }
            public DataTable Schema { get; }
            public ImmutableArray<PropertyMetadata> Properties { get; }
            public ImmutableDictionary<string, int> PropertyLookup { get; }

        }

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [SuppressMessage("Microsoft.Globalization", "CA1306:SetLocaleForDataTypes")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static ObjectDataReaderMetatData BuildStructure(string targetName, IReadOnlyList<IColumnMetadata> columns, bool allColumnsRequired, OperationTypes operationType)
        {

            var propertyList = MetadataCache.GetMetadata(typeof(TObject)).Properties.Where(p => p.CanRead && p.MappedColumnName != null).ToList();
            bool checkIgnoreOnInsert = operationType == OperationTypes.Insert;
            bool checkIgnoreOnUpdate = operationType == OperationTypes.Update;


            var dt = new DataTable();
            dt.Columns.Add("ColumnName", typeof(string));
            dt.Columns.Add("ColumnOrdinal", typeof(int));
            dt.Columns.Add("ColumnSize", typeof(int));
            dt.Columns.Add("NumericPrecision", typeof(short));
            dt.Columns.Add("NumericScale", typeof(short));
            dt.Columns.Add("DataType", typeof(Type));
            dt.Columns.Add("ProviderType", typeof(int));
            dt.Columns.Add("IsLong", typeof(bool));
            dt.Columns.Add("AllowDBNull", typeof(bool));
            dt.Columns.Add("IsReadOnly", typeof(bool));
            dt.Columns.Add("IsRowVersion", typeof(bool));
            dt.Columns.Add("IsUnique", typeof(bool));
            dt.Columns.Add("IsKey", typeof(bool));
            dt.Columns.Add("IsAutoIncrement", typeof(bool));
            dt.Columns.Add("BaseCatalogName", typeof(string));
            dt.Columns.Add("BaseSchemaName", typeof(string));
            dt.Columns.Add("BaseTableName", typeof(string));
            dt.Columns.Add("BaseColumnName", typeof(string));
            dt.Columns.Add("AutoIncrementSeed", typeof(long));
            dt.Columns.Add("AutoIncrementStep", typeof(long));
            dt.Columns.Add("DefaultValue", typeof(object));
            dt.Columns.Add("Expression", typeof(string));
            dt.Columns.Add("ColumnMapping", typeof(MappingType));
            dt.Columns.Add("BaseTableNamespace", typeof(string));
            dt.Columns.Add("BaseColumnNamespace", typeof(string));

            var ordinal = 0;
            var realPropertyList = new List<PropertyMetadata>(columns.Count);
            foreach (var column in columns)
            {
                PropertyMetadata property = null;
                foreach (var item in propertyList)
                {
                    if (column.ClrName.Equals(item.MappedColumnName, StringComparison.OrdinalIgnoreCase) || column.SqlName.Equals(item.MappedColumnName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (checkIgnoreOnInsert && item.IgnoreOnInsert)
                            continue; //look for another match

                        if (checkIgnoreOnUpdate && item.IgnoreOnUpdate)
                            continue; //look for another match

                        property = item;
                        break;
                    }
                }


                if (property == null)
                {
                    if (allColumnsRequired)
                        throw new MappingException($"Could not find a property on {typeof(TObject).Name} that can be mapped to column {column.SqlName} on {targetName}");
                    else
                        continue; //tables don't need every column
                }

                realPropertyList.Add(property);

                var row = dt.NewRow();
                row["ColumnName"] = column.SqlName;
                row["ColumnOrdinal"] = ordinal++;
                row["ColumnSize"] = -1;

                //this is probably wrong, but we don't have a good way to map from DbType to CLR types.
                if (property.PropertyType.Name == "Nullable`1" && property.PropertyType.IsGenericType)
                {
                    row["DataType"] = property.PropertyType.GenericTypeArguments[0];
                }
                else
                {
                    row["DataType"] = property.PropertyType;
                }
                row["IsLong"] = false;
                row["AllowDBNull"] = true;
                row["IsReadOnly"] = false;
                row["IsRowVersion"] = false;
                row["IsUnique"] = false;
                row["IsKey"] = false;
                row["IsAutoIncrement"] = false;
                row["BaseTableName"] = null;
                row["BaseColumnName"] = column.SqlName;
                row["AutoIncrementSeed"] = 0;
                row["AutoIncrementStep"] = 1;
                row["ColumnMapping"] = 1;
                row["BaseTableNamespace"] = null;
                row["BaseColumnNamespace"] = null;
                dt.Rows.Add(row);
            }


            return new ObjectDataReaderMetatData(dt, realPropertyList.ToImmutableArray(), realPropertyList.Select((p, x) => new { Index = x, Property = p }).ToImmutableDictionary(px => px.Property.Name, px => px.Index, StringComparer.OrdinalIgnoreCase));

        }




        /// <summary>
        /// Gets the <see cref="object"/> with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Object.</returns>
        public override object this[string name]
        {
            get { return m_PropertyList[m_PropertyLookup[name]].InvokeGet(m_Source.Current); }
        }

        /// <summary>
        /// Gets the <see cref="object"/> with the specified ordinal.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Object.</returns>
        public override object this[int ordinal]
        {
            get { return m_PropertyList[ordinal].InvokeGet(m_Source.Current); }
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        /// <value>The depth.</value>
        public override int Depth
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        /// <value>The field count.</value>
        public override int FieldCount
        {
            get { return m_PropertyList.Count(); }
        }

        /// <summary>
        /// Gets a value that indicates whether this <see cref="T:System.Data.Common.DbDataReader" /> contains one or more rows.
        /// </summary>
        /// <value><c>true</c> if this instance has rows; otherwise, <c>false</c>.</value>
        public override bool HasRows
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Data.Common.DbDataReader" /> is closed.
        /// </summary>
        /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
        public override bool IsClosed
        {
            get { return m_Source != null; }
        }

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        /// <value>The records affected.</value>
        public override int RecordsAffected
        {
            get { return m_RecordCount ?? -1; }
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override bool GetBoolean(int ordinal) => (bool)this[ordinal];

        /// <summary>
        /// Gets the value of the specified column as a byte.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override byte GetByte(int ordinal) => (byte)this[ordinal];

        /// <summary>
        /// Reads a stream of bytes from the specified column, starting at location indicated by <paramref name="dataOffset" />, into the buffer, starting at the location indicated by <paramref name="bufferOffset" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
        /// <param name="buffer">The buffer into which to copy the data.</param>
        /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
        /// <param name="length">The maximum number of characters to read.</param>
        /// <returns>The actual number of bytes read.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value of the specified column as a single character.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override char GetChar(int ordinal) => (char)this[ordinal];

        /// <summary>
        /// Reads a stream of characters from the specified column, starting at location indicated by <paramref name="dataOffset" />, into the buffer, starting at the location indicated by <paramref name="bufferOffset" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
        /// <param name="buffer">The buffer into which to copy the data.</param>
        /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
        /// <param name="length">The maximum number of characters to read.</param>
        /// <returns>The actual number of characters read.</returns>
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            var s = GetString(ordinal);
            if (length + dataOffset > s.Length)
                length = s.Length - (int)dataOffset;

            s.CopyTo((int)dataOffset, buffer, bufferOffset, length);
            return length;
        }

        /// <summary>
        /// Gets name of the data type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>A string representing the name of the data type.</returns>
        public override string GetDataTypeName(int ordinal) => (string)this[ordinal];

        /// <summary>
        /// Gets the value of the specified column as a <see cref="T:System.DateTime" /> object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override DateTime GetDateTime(int ordinal) => (DateTime)this[ordinal];

        /// <summary>
        /// Gets the value of the specified column as a <see cref="T:System.Decimal" /> object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override decimal GetDecimal(int ordinal) => (decimal)this[ordinal];


        /// <summary>
        /// Gets the value of the specified column as a double-precision floating point number.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override double GetDouble(int ordinal) => (double)this[ordinal];


        /// <summary>
        /// Returns an <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.</returns>
        public override IEnumerator GetEnumerator()
        {
            return m_Source;
        }

        /// <summary>
        /// Gets the data type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The data type of the specified column.</returns>
        public override Type GetFieldType(int ordinal) => m_PropertyList[ordinal].PropertyType;

        /// <summary>
        /// Gets the value of the specified column as a single-precision floating point number.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override float GetFloat(int ordinal) => (float)this[ordinal];

        /// <summary>
        /// Gets the value of the specified column as a globally-unique identifier (GUID).
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override Guid GetGuid(int ordinal) => (Guid)this[ordinal];

        /// <summary>
        /// Gets the value of the specified column as a 16-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override short GetInt16(int ordinal) => (short)this[ordinal];

        /// <summary>
        /// Gets the value of the specified column as a 32-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override int GetInt32(int ordinal) => (int)this[ordinal];

        /// <summary>
        /// Gets the value of the specified column as a 64-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override long GetInt64(int ordinal) => (long)this[ordinal];

        /// <summary>
        /// Gets the name of the column, given the zero-based column ordinal.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The name of the specified column.</returns>
        public override string GetName(int ordinal) => m_PropertyList[ordinal].Name;

        /// <summary>
        /// Gets the column ordinal given the name of the column.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The zero-based column ordinal.</returns>
        public override int GetOrdinal(string name) => m_PropertyLookup[name];

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="T:System.String" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override string GetString(int ordinal) => (string)this[ordinal];

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object GetValue(int ordinal) => this[ordinal];

        /// <summary>
        /// Populates an array of objects with the column values of the current row.
        /// </summary>
        /// <param name="values">An array of <see cref="T:System.Object" /> into which to copy the attribute columns.</param>
        /// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
        public override int GetValues(object[] values)
        {
            var result = new object[m_PropertyList.Length];
            for (var i = 0; i < m_PropertyList.Length; i++)
                result[i] = this[i];
            return m_PropertyList.Length;
        }

        /// <summary>
        /// Gets a value that indicates whether the column contains nonexistent or missing values.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>true if the specified column is equivalent to <see cref="T:System.DBNull" />; otherwise false.</returns>
        public override bool IsDBNull(int ordinal) => this[ordinal] == null;

        /// <summary>
        /// Advances the reader to the next result when reading the results of a batch of statements.
        /// </summary>
        /// <returns>true if there are more result sets; otherwise false.</returns>
        public override bool NextResult() => false;

        /// <summary>
        /// Advances the reader to the next record in a result set.
        /// </summary>
        /// <returns>true if there are more rows; otherwise false.</returns>
        public override bool Read()
        {
            return m_Source.MoveNext();
        }

        /// <summary>
        /// Releases the managed resources used by the <see cref="T:System.Data.Common.DbDataReader" /> and optionally releases the unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to release managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_Source.Dispose();
                m_Source = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.Common.DbDataReader" />.
        /// </summary>
        /// <returns>A <see cref="T:System.Data.DataTable" /> that describes the column metadata.</returns>
        public override DataTable GetSchemaTable()
        {
            return m_Schema;
        }
    }
}
