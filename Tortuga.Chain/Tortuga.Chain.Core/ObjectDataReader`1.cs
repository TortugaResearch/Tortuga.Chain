using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Data.Common;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain;

/// <summary>
/// Creates a DbDataReader wrapper over a list of objects.
/// </summary>
/// <typeparam name="TObject"></typeparam>
/// <seealso cref="DbDataReader" />
[SuppressMessage("Design", "CA1010:Generic interface should also be implemented", Justification = "<Pending>")]
public class ObjectDataReader<TObject> : DbDataReader
{
	readonly ImmutableArray<PropertyMetadata> m_PropertyList;
	readonly FrozenDictionary<string, int> m_ColumnPropertyIndexMap;
	readonly int? m_RecordCount;
	readonly DataTable m_Schema;
	IEnumerator<TObject>? m_Source;

	/// <summary>
	/// Initializes a new instance of the <see cref="ObjectDataReader{TObject}" /> class.
	/// </summary>
	/// <param name="tableType">Type of the table.</param>
	/// <param name="source">The source.</param>
	/// <param name="operationType">Type of the operation being performed.</param>
	public ObjectDataReader(UserDefinedTableTypeMetadata tableType, IEnumerable<TObject> source, OperationTypes operationType = OperationTypes.None)
	{
		if (tableType == null)
			throw new ArgumentNullException(nameof(tableType), $"{nameof(tableType)} is null.");
		if (source == null)
			throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");

		//Don't use IEnumerable<T>.Count(), as we don't want to preemptively materialize a lazy collection
		if (source is ICollection collection)
			m_RecordCount = collection.Count;

		m_Source = source.GetEnumerator();
		var metadata = BuildStructure(tableType.Name, tableType.Columns, true, operationType);
		m_Schema = metadata.Schema;
		m_PropertyList = metadata.Properties;
		m_ColumnPropertyIndexMap = metadata.ColumnPropertyIndexMap;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ObjectDataReader{TObject}" /> class.
	/// </summary>
	/// <param name="tableOrView">The table or view.</param>
	/// <param name="source">The source.</param>
	/// <param name="operationType">Type of the operation being performed.</param>
	public ObjectDataReader(TableOrViewMetadata tableOrView, IEnumerable<TObject> source, OperationTypes operationType = OperationTypes.None)
	{
		if (tableOrView == null)
			throw new ArgumentNullException(nameof(tableOrView), $"{nameof(tableOrView)} is null.");
		if (source == null)
			throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");

		//Don't use IEnumerable<T>.Count(), as we don't want to preemptively materialize a lazy collection
		if (source is ICollection collection)
			m_RecordCount = collection.Count;

		m_Source = source.GetEnumerator();
		var metadata = BuildStructure(tableOrView.Name, tableOrView.Columns, false, operationType);
		m_Schema = metadata.Schema;
		m_PropertyList = metadata.Properties;
		m_ColumnPropertyIndexMap = metadata.ColumnPropertyIndexMap;

		if (m_PropertyList.Length == 0)
			throw new MappingException($"Unable to map object of type {typeof(TObject).Name} to a the table {tableOrView.Name}.");
	}

	/// <summary>
	/// Gets a value indicating the depth of nesting for the current row.
	/// </summary>
	/// <value>The depth.</value>
	public override int Depth => 0;

	/// <summary>
	/// Gets the number of columns in the current row.
	/// </summary>
	/// <value>The field count.</value>
	public override int FieldCount => m_PropertyList.Length;

	/// <summary>
	/// Gets a value that indicates whether this <see cref="System.Data.Common.DbDataReader" /> contains one or more rows.
	/// </summary>
	/// <value><c>true</c> if this instance has rows; otherwise, <c>false</c>.</value>
	public override bool HasRows => true;

	/// <summary>
	/// Gets a value indicating whether the <see cref="System.Data.Common.DbDataReader" /> is closed.
	/// </summary>
	/// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
	public override bool IsClosed => m_Source != null;

	/// <summary>
	/// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
	/// </summary>
	/// <value>The records affected.</value>
	public override int RecordsAffected => m_RecordCount ?? -1;

	/// <summary>
	/// Gets the <see cref="object"/> with the specified name.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <returns>System.Object.</returns>
	public override object? this[string name]
	{
#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
		get
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
		{
			ObjectDisposedException.ThrowIf(m_Source == null, this);

			return m_PropertyList[m_ColumnPropertyIndexMap[name]].InvokeGet(m_Source.Current!);
		}
	}

	/// <summary>
	/// Gets the <see cref="object"/> with the specified ordinal.
	/// </summary>
	/// <param name="ordinal">The ordinal.</param>
	/// <returns>System.Object.</returns>
	public override object? this[int ordinal]
	{
#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
		get
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
		{
			ObjectDisposedException.ThrowIf(m_Source == null, this);

			return m_PropertyList[ordinal].InvokeGet(m_Source.Current!);
		}
	}

	/// <summary>
	/// Closes the <see cref="System.Data.Common.DbDataReader" /> object.
	/// </summary>
	public override void Close() => Dispose();

	/// <summary>
	/// Gets the value of the specified column as a Boolean.
	/// </summary>
	/// <param name="ordinal">The zero-based column ordinal.</param>
	/// <returns>The value of the specified column.</returns>
	public override bool GetBoolean(int ordinal) => (bool)(this[ordinal] ?? throw new InvalidOperationException($"Value in ordinal {ordinal} is null. Use IsDBNull before calling this method."));

	/// <summary>
	/// Gets the value of the specified column as a byte.
	/// </summary>
	/// <param name="ordinal">The zero-based column ordinal.</param>
	/// <returns>The value of the specified column.</returns>
	public override byte GetByte(int ordinal) => (byte)(this[ordinal] ?? throw new InvalidOperationException($"Value in ordinal {ordinal} is null. Use IsDBNull before calling this method."));

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
	public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Gets the value of the specified column as a single character.
	/// </summary>
	/// <param name="ordinal">The zero-based column ordinal.</param>
	/// <returns>The value of the specified column.</returns>
	public override char GetChar(int ordinal) => (char)(this[ordinal] ?? throw new InvalidOperationException($"Value in ordinal {ordinal} is null. Use IsDBNull before calling this method."));

	/// <summary>
	/// Reads a stream of characters from the specified column, starting at location indicated by <paramref name="dataOffset" />, into the buffer, starting at the location indicated by <paramref name="bufferOffset" />.
	/// </summary>
	/// <param name="ordinal">The zero-based column ordinal.</param>
	/// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
	/// <param name="buffer">The buffer into which to copy the data.</param>
	/// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
	/// <param name="length">The maximum number of characters to read.</param>
	/// <returns>The actual number of characters read.</returns>
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).

	public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
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
	public override string GetDataTypeName(int ordinal) => (string)(this[ordinal] ?? throw new InvalidOperationException($"Value in ordinal {ordinal} is null. Use IsDBNull before calling this method."));

	/// <summary>
	/// Gets the value of the specified column as a <see cref="System.DateTime" /> object.
	/// </summary>
	/// <param name="ordinal">The zero-based column ordinal.</param>
	/// <returns>The value of the specified column.</returns>
	public override DateTime GetDateTime(int ordinal) => (DateTime)(this[ordinal] ?? throw new InvalidOperationException($"Value in ordinal {ordinal} is null. Use IsDBNull before calling this method."));

	/// <summary>
	/// Gets the value of the specified column as a <see cref="System.Decimal" /> object.
	/// </summary>
	/// <param name="ordinal">The zero-based column ordinal.</param>
	/// <returns>The value of the specified column.</returns>
	public override decimal GetDecimal(int ordinal) => (decimal)(this[ordinal] ?? throw new InvalidOperationException($"Value in ordinal {ordinal} is null. Use IsDBNull before calling this method."));

	/// <summary>
	/// Gets the value of the specified column as a double-precision floating point number.
	/// </summary>
	/// <param name="ordinal">The zero-based column ordinal.</param>
	/// <returns>The value of the specified column.</returns>
	public override double GetDouble(int ordinal) => (double)(this[ordinal] ?? throw new InvalidOperationException($"Value in ordinal {ordinal} is null. Use IsDBNull before calling this method."));

	/// <summary>
	/// Returns an <see cref="System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.
	/// </summary>
	/// <returns>An <see cref="System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.</returns>
	public override IEnumerator GetEnumerator()
	{
		ObjectDisposedException.ThrowIf(m_Source == null, this);

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
	public override float GetFloat(int ordinal) => (float)(this[ordinal] ?? throw new InvalidOperationException($"Value in ordinal {ordinal} is null. Use IsDBNull before calling this method."));

	/// <summary>
	/// Gets the value of the specified column as a globally-unique identifier (GUID).
	/// </summary>
	/// <param name="ordinal">The zero-based column ordinal.</param>
	/// <returns>The value of the specified column.</returns>
	public override Guid GetGuid(int ordinal) => (Guid)(this[ordinal] ?? throw new InvalidOperationException($"Value in ordinal {ordinal} is null. Use IsDBNull before calling this method."));

	/// <summary>
	/// Gets the value of the specified column as a 16-bit signed integer.
	/// </summary>
	/// <param name="ordinal">The zero-based column ordinal.</param>
	/// <returns>The value of the specified column.</returns>
	public override short GetInt16(int ordinal) => (short)(this[ordinal] ?? throw new InvalidOperationException($"Value in ordinal {ordinal} is null. Use IsDBNull before calling this method."));

	/// <summary>
	/// Gets the value of the specified column as a 32-bit signed integer.
	/// </summary>
	/// <param name="ordinal">The zero-based column ordinal.</param>
	/// <returns>The value of the specified column.</returns>
	public override int GetInt32(int ordinal) => (int)(this[ordinal] ?? throw new InvalidOperationException($"Value in ordinal {ordinal} is null. Use IsDBNull before calling this method."));

	/// <summary>
	/// Gets the value of the specified column as a 64-bit signed integer.
	/// </summary>
	/// <param name="ordinal">The zero-based column ordinal.</param>
	/// <returns>The value of the specified column.</returns>
	public override long GetInt64(int ordinal) => (long)(this[ordinal] ?? throw new InvalidOperationException($"Value in ordinal {ordinal} is null. Use IsDBNull before calling this method."));

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
	public override int GetOrdinal(string name) => m_ColumnPropertyIndexMap[name];

	/// <summary>
	/// Returns a <see cref="System.Data.DataTable" /> that describes the column metadata of the <see cref="System.Data.Common.DbDataReader" />.
	/// </summary>
	/// <returns>A <see cref="System.Data.DataTable" /> that describes the column metadata.</returns>
	public override DataTable GetSchemaTable() => m_Schema;

	/// <summary>
	/// Gets the value of the specified column as an instance of <see cref="System.String" />.
	/// </summary>
	/// <param name="ordinal">The zero-based column ordinal.</param>
	/// <returns>The value of the specified column.</returns>
	public override string GetString(int ordinal)
	{
		switch (this[ordinal])
		{
			case string s: return s;
			case char c: return c.ToString();
			case DBNull:
			case null:
				throw new InvalidOperationException($"Value in ordinal {ordinal} is null. Use IsDBNull before calling this method.");
		}
		throw new InvalidOperationException($"Value in ordinal {ordinal} is not a string or char. It is a {ordinal.GetType().Name}.");
	}

	/// <summary>
	/// Gets the value of the specified column as an instance of <see cref="System.Object" />.
	/// </summary>
	/// <param name="ordinal">The zero-based column ordinal.</param>
	/// <returns>The value of the specified column.</returns>
#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).

	public override object? GetValue(int ordinal) => this[ordinal];

#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).

	/// <summary>
	/// Populates an array of objects with the column values of the current row.
	/// </summary>
	/// <param name="values">An array of <see cref="System.Object" /> into which to copy the attribute columns.</param>
	/// <returns>The number of instances of <see cref="System.Object" /> in the array.</returns>
	public override int GetValues(object[] values)
	{
		var result = new object?[m_PropertyList.Length];
		for (var i = 0; i < m_PropertyList.Length; i++)
			result[i] = this[i];
		return m_PropertyList.Length;
	}

	/// <summary>
	/// Gets a value that indicates whether the column contains nonexistent or missing values.
	/// </summary>
	/// <param name="ordinal">The zero-based column ordinal.</param>
	/// <returns>true if the specified column is equivalent to <see cref="System.DBNull" />; otherwise false.</returns>
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
		ObjectDisposedException.ThrowIf(m_Source == null, this);

		return m_Source.MoveNext();
	}

	/// <summary>
	/// Releases the managed resources used by the <see cref="System.Data.Common.DbDataReader" /> and optionally releases the unmanaged resources.
	/// </summary>
	/// <param name="disposing">true to release managed and unmanaged resources; false to release only unmanaged resources.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && m_Source != null)
		{
			m_Source.Dispose();
			m_Source = null;
		}

		base.Dispose(disposing);
	}

	[SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
	[SuppressMessage("Microsoft.Globalization", "CA1306:SetLocaleForDataTypes")]
	[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
	static ObjectDataReaderMetadata BuildStructure(string targetName, ColumnMetadataCollection columns, bool allColumnsRequired, OperationTypes operationType)
	{
		var propertyList = MetadataCache.GetMetadata<TObject>().Properties.Where(p => p.CanRead && p.MappedColumnName != null).ToList();
		var checkIgnoreOnInsert = operationType == OperationTypes.Insert;
		var checkIgnoreOnUpdate = operationType == OperationTypes.Update;

		var dtSchema = new DataTable();
		dtSchema.Columns.Add("ColumnName", typeof(string));
		dtSchema.Columns.Add("ColumnOrdinal", typeof(int));
		dtSchema.Columns.Add("ColumnSize", typeof(int));
		dtSchema.Columns.Add("NumericPrecision", typeof(short));
		dtSchema.Columns.Add("NumericScale", typeof(short));
		dtSchema.Columns.Add("DataType", typeof(Type));
		dtSchema.Columns.Add("ProviderType", typeof(int));
		dtSchema.Columns.Add("IsLong", typeof(bool));
		dtSchema.Columns.Add("AllowDBNull", typeof(bool));
		dtSchema.Columns.Add("IsReadOnly", typeof(bool));
		dtSchema.Columns.Add("IsRowVersion", typeof(bool));
		dtSchema.Columns.Add("IsUnique", typeof(bool));
		dtSchema.Columns.Add("IsKey", typeof(bool));
		dtSchema.Columns.Add("IsAutoIncrement", typeof(bool));
		dtSchema.Columns.Add("BaseCatalogName", typeof(string));
		dtSchema.Columns.Add("BaseSchemaName", typeof(string));
		dtSchema.Columns.Add("BaseTableName", typeof(string));
		dtSchema.Columns.Add("BaseColumnName", typeof(string));
		dtSchema.Columns.Add("AutoIncrementSeed", typeof(long));
		dtSchema.Columns.Add("AutoIncrementStep", typeof(long));
		dtSchema.Columns.Add("DefaultValue", typeof(object));
		dtSchema.Columns.Add("Expression", typeof(string));
		dtSchema.Columns.Add("ColumnMapping", typeof(MappingType));
		dtSchema.Columns.Add("BaseTableNamespace", typeof(string));
		dtSchema.Columns.Add("BaseColumnNamespace", typeof(string));

		var ordinal = 0;
		var realPropertyList = new List<(ColumnMetadata Column, PropertyMetadata Property)>(columns.Count);
		foreach (var column in columns)
		{
			PropertyMetadata? property = null;
			foreach (var item in propertyList)
			{
				if (column.ClrName.Equals(item.MappedColumnName, StringComparison.OrdinalIgnoreCase)
					|| column.ClrNameStandardized.Equals(item.MappedColumnName, StringComparison.OrdinalIgnoreCase)
					|| column.SqlName.Equals(item.MappedColumnName, StringComparison.OrdinalIgnoreCase))
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

			realPropertyList.Add((column, property));

			var row = dtSchema.NewRow();
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
			dtSchema.Rows.Add(row);
		}

		return new ObjectDataReaderMetadata(
			schema: dtSchema,
			properties: realPropertyList.Select(x => x.Property).ToImmutableArray(),
			//propertyLookup: realPropertyList.Select((p, x) => new { Index = x, Property = p }).ToFrozenDictionary(px => px.Property.Property.Name, px => px.Index, StringComparer.OrdinalIgnoreCase),
			columnPropertyIndexMap: realPropertyList.Select((p, x) => new { Index = x, Item = p }).ToFrozenDictionary(px => px.Item.Column.SqlName, px => px.Index, StringComparer.OrdinalIgnoreCase)

			/*,
			columnMap : realPropertyList.ToFrozenDictionary(px => px.Column.SqlName, px => px.Index, StringComparer.OrdinalIgnoreCase)*/);
	}

	class ObjectDataReaderMetadata
	{
		public ObjectDataReaderMetadata(DataTable schema, ImmutableArray<PropertyMetadata> properties, FrozenDictionary<string, int> columnPropertyIndexMap)
		{
			Schema = schema;
			Properties = properties;
			ColumnPropertyIndexMap = columnPropertyIndexMap;
		}

		public ImmutableArray<PropertyMetadata> Properties { get; }
		public FrozenDictionary<string, int> ColumnPropertyIndexMap { get; }
		//public FrozenDictionary<string, int> PropertyLookup { get; }
		public DataTable Schema { get; }
	}
}
