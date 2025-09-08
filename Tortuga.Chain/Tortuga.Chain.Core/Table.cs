using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Reflection;
using Tortuga.Anchor;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain;

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
	readonly MaterializerTypeConverter m_Converter;
	readonly RowCollection m_Rows;

	/// <summary>
	/// Creates a new NamedTable from an IDataReader
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="source">The source.</param>
	/// <param name="converter">The type converter.</param>
	public Table(string tableName, IDataReader source, MaterializerTypeConverter converter)
		: this(source, converter)
	{
		TableName = tableName;
	}

	/// <summary>
	/// Creates a new NamedTable from an IDataReader
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="source">The source.</param>
	/// <param name="converter">The type converter.</param>
	public Table(string tableName, DbDataReader source, MaterializerTypeConverter converter)
		: this(source, converter)
	{
		TableName = tableName;
	}

	/// <summary>
	/// Creates a new Table from an IDataReader
	/// </summary>
	/// <param name="source">The source.</param>
	/// <exception cref="ArgumentNullException">nameof(s - rce), "source is</exception>
	/// <exception cref="ArgumentException">No columns were returned - source</exception>
	/// <param name="converter">The type converter.</param>
	public Table(DbDataReader source, MaterializerTypeConverter converter)
	{
		if (source == null)
			throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");
		if (source.FieldCount == 0)
			throw new ArgumentException("No columns were returned", nameof(source));

		m_Converter = converter ?? new();

		TableName = "";

		var cols = new List<string>(source.FieldCount);
		var colTypes = new Dictionary<string, Type>(source.FieldCount, StringComparer.OrdinalIgnoreCase);

		for (var i = 0; i < source.FieldCount; i++)
			cols.Add(source.GetName(i));

		DuplicateColumnNameHandling(cols);

		for (var i = 0; i < source.FieldCount; i++)
			colTypes.Add(cols[i], source.GetFieldType(i));

		m_Columns = new ReadOnlyCollection<string>(cols);
		m_ColumnTypes = new ReadOnlyDictionary<string, Type>(colTypes);

		var rows = new Collection<Row>();

		while (source.Read())
		{
			var row = new Dictionary<string, object?>(source.FieldCount, StringComparer.OrdinalIgnoreCase);
			for (var i = 0; i < source.FieldCount; i++)
			{
				object? temp = source[i];
				if (temp == DBNull.Value)
					temp = null;

				row.Add(cols[i], temp);
			}

			rows.Add(new Row(row));
		}

		m_Rows = new RowCollection(rows);
	}

	static void DuplicateColumnNameHandling(List<string> cols)
	{
		if (cols.Count != cols.Distinct().Count())
			for (var i = 0; i < cols.Count; i++)
			{
				var columnName = cols[i];
				if (columnName.IsNullOrEmpty() || cols.Take(i).Any(c => c == columnName)) //do any previous columns have the same name?
					cols[i] = columnName + "_" + i; //rename the column
			}
	}

	/// <summary>
	/// Creates a new Table from an IDataReader
	/// </summary>
	/// <param name="source"></param>
	/// <param name="converter">The type converter.</param>
	public Table(IDataReader source, MaterializerTypeConverter converter)
	{
		if (source == null)
			throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");
		if (source.FieldCount == 0)
			throw new ArgumentException("No columns were returned", nameof(source));

		m_Converter = converter ?? new();

		TableName = "";

		var cols = new List<string>(source.FieldCount);
		var colTypes = new Dictionary<string, Type>(source.FieldCount, StringComparer.OrdinalIgnoreCase);

		for (var i = 0; i < source.FieldCount; i++)
			cols.Add(source.GetName(i));

		DuplicateColumnNameHandling(cols);

		for (var i = 0; i < source.FieldCount; i++)
			colTypes.Add(cols[i], source.GetFieldType(i));

		m_Columns = new ReadOnlyCollection<string>(cols);
		m_ColumnTypes = new ReadOnlyDictionary<string, Type>(colTypes);

		var rows = new Collection<Row>();

		while (source.Read())
		{
			var row = new Dictionary<string, object?>(source.FieldCount, StringComparer.OrdinalIgnoreCase);
			for (var i = 0; i < source.FieldCount; i++)
			{
				object? temp = source[i];
				if (temp == DBNull.Value)
					temp = null;

				row.Add(cols[i], temp);
			}

			rows.Add(new Row(row));
		}

		m_Rows = new RowCollection(rows);
	}

	/// <summary>
	/// Creates a new NamedTable from an IDataReader
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="source">The source.</param>
	[Obsolete("Pass DataSource.DatabaseMetadata.Converter to the constructor")]
	public Table(string tableName, IDataReader source)
		: this(tableName, source, new())
	{
	}

	/// <summary>
	/// Creates a new NamedTable from an IDataReader
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="source">The source.</param>
	[Obsolete("Pass DataSource.DatabaseMetadata.Converter to the constructor")]
	public Table(string tableName, DbDataReader source)
		: this(tableName, source, new())
	{
	}

	/// <summary>
	/// Creates a new Table from an IDataReader
	/// </summary>
	/// <param name="source">The source.</param>
	/// <exception cref="ArgumentNullException">nameof(s - rce), "source is</exception>
	/// <exception cref="ArgumentException">No columns were returned - source</exception>
	[Obsolete("Pass DataSource.DatabaseMetadata.Converter to the constructor")]
	public Table(DbDataReader source) : this(source, new())
	{
	}

	/// <summary>
	/// Creates a new Table from an IDataReader
	/// </summary>
	/// <param name="source"></param>
	[Obsolete("Pass DataSource.DatabaseMetadata.Converter to the constructor")]
	public Table(IDataReader source) : this(source, new())
	{
	}

	/// <summary>
	/// List of column names in their original order.
	/// </summary>
	public IReadOnlyList<string> ColumnNames => m_Columns;

	/// <summary>
	/// List of columns and their types.
	/// </summary>
	public IReadOnlyDictionary<string, Type> ColumnTypeMap => m_ColumnTypes;

	/// <summary>
	/// Gets the rows.
	/// </summary>
	/// <value>The rows.</value>
	public IReadOnlyList<Row> Rows => m_Rows;

	/// <summary>
	/// Gets the name of the table.
	/// </summary>
	/// <value>The name of the table.</value>
	public string TableName { get; set; }

	/// <summary>
	/// Copies the contents of this Table into a DataTable.
	/// </summary>
	/// <returns>DataTable.</returns>
	public DataTable ToDataTable()
	{
		var dt = new DataTable();
		foreach (var column in ColumnNames)
			dt.Columns.Add(new DataColumn(column, ColumnTypeMap[column]));

		foreach (var row in Rows)
		{
			var dr = dt.NewRow();
			foreach (var column in ColumnNames)
			{
				var value = row[column];
				dr[column] = value == null ? DBNull.Value : value;
			}
			dt.Rows.Add(dr);
		}

		return dt;
	}

	/// <summary>
	/// Converts the table into an enumeration of objects of the indicated type.
	/// </summary>
	/// <typeparam name="T">Desired object type</typeparam>
	public IEnumerable<T> ToObjects<T>() where T : class, new()
	{
		foreach (var row in Rows)
		{
			yield return ToObject<T>(row);
		}
	}

	/// <summary>
	/// Converts the table into an enumeration of objects of the indicated type using the indicated constructor.
	/// </summary>
	/// <typeparam name="T">Desired object type</typeparam>
	/// <param name="constructorSignature">The constructor signature.</param>
	/// <returns>IEnumerable&lt;T&gt;.</returns>
	public IEnumerable<T> ToObjects<T>(IReadOnlyList<Type>? constructorSignature)
	{
		if (constructorSignature == null)
		{
			var methodType = GetType().GetMethod("ToObjects", Array.Empty<Type>())!;
			var genericMethod = methodType.MakeGenericMethod(typeof(T));
			return (IEnumerable<T>)genericMethod.Invoke(this, null)!;
		}
		else
			return ToObjects_Core<T>(constructorSignature);
	}

	/// <summary>
	/// Converts the table into an enumeration of objects of the indicated type using the indicated constructor.
	/// </summary>
	/// <typeparam name="T">Desired object type</typeparam>
	/// <param name="constructor">The constructor.</param>
	/// <returns>IEnumerable&lt;T&gt;.</returns>
	public IEnumerable<T> ToObjects<T>(ConstructorMetadata? constructor)
	{
		if (constructor == null)
		{
			var methodType = GetType().GetMethod("ToObjects", Array.Empty<Type>())!;
			var genericMethod = methodType.MakeGenericMethod(typeof(T));
			return (IEnumerable<T>)genericMethod.Invoke(this, null)!;
		}
		else
			return ToObjects_Core<T>(constructor);
	}

	internal T ToObject<T>(Row row)
		where T : class, new()
	{
		var result = new T();
		MaterializerUtilities.PopulateComplexObject(row, result, null, m_Converter);

		//Change tracking objects shouldn't be materialized as unchanged.
		var tracking = result as IChangeTracking;
		tracking?.AcceptChanges();
		return result;
	}

	internal IEnumerable<T> ToObjects_Core<T>(IReadOnlyList<Type> constructorSignature)
	{
		var desiredType = typeof(T);
		var constructor = MetadataCache.GetMetadata(desiredType).Constructors.Find(constructorSignature);
		if (constructor == null)
		{
			var types = string.Join(", ", constructorSignature.Select(t => t.Name));
			throw new MappingException($"Cannot find a constructor on {desiredType.Name} with the types [{types}]");
		}
		return ToObjects_Core<T>(constructor);
	}

	internal IEnumerable<T> ToObjects_Core<T>(ConstructorMetadata constructor)
	{
		if (constructor == null)
			throw new ArgumentNullException(nameof(constructor), $"{nameof(constructor)} is null.");

		var constructorParameters = constructor.ParameterNames;
		for (var i = 0; i < constructorParameters.Length; i++)
		{
			if (!ColumnNames.Any(p => p.Equals(constructorParameters[i], StringComparison.OrdinalIgnoreCase)))
				throw new MappingException($"Cannot find a column that matches the parameter {constructorParameters[i]}");
		}

		foreach (var item in Rows)
		{
			var parameters = new object?[constructorParameters.Length];
			for (var i = 0; i < constructorParameters.Length; i++)
			{
				parameters[i] = item[constructorParameters[i]];
			}
			var result = constructor.ConstructorInfo.Invoke(parameters);
			yield return (T)result;
		}
	}

	internal IEnumerable<KeyValuePair<Row, T>> ToObjectsWithEcho<T>(ConstructorMetadata? constructor)
	{
		if (constructor == null)
		{
			var methodType = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).Single(m => m.Name == "ToObjectsWithEcho_New");
			var genericMethod = methodType.MakeGenericMethod(typeof(T));
			return (IEnumerable<KeyValuePair<Row, T>>)genericMethod.Invoke(this, null)!;
		}
		else
			return ToObjectsWithEcho_Core<T>(constructor);
	}

	internal IEnumerable<KeyValuePair<Row, T>> ToObjectsWithEcho_Core<T>(ConstructorMetadata constructor)
	{
		if (constructor == null)
			throw new ArgumentNullException(nameof(constructor), $"{nameof(constructor)} is null.");

		var constructorParameters = constructor.ParameterNames;
		for (var i = 0; i < constructorParameters.Length; i++)
		{
			if (!ColumnNames.Any(p => p.Equals(constructorParameters[i], StringComparison.OrdinalIgnoreCase)))
				throw new MappingException($"Cannot find a column that matches the parameter {constructorParameters[i]}");
		}

		foreach (var item in Rows)
		{
			var parameters = new object?[constructorParameters.Length];
			for (var i = 0; i < constructorParameters.Length; i++)
			{
				parameters[i] = item[constructorParameters[i]];
			}
			var result = constructor.ConstructorInfo.Invoke(parameters);
			yield return new KeyValuePair<Row, T>(item, (T)result);
		}
	}

	/// <summary>
	/// Converts the table into an enumeration of objects of the indicated type.
	/// </summary>
	/// <typeparam name="T">Desired object type</typeparam>
	[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
	internal IEnumerable<KeyValuePair<Row, T>> ToObjectsWithEcho_New<T>() where T : class, new()
	{
		foreach (var row in Rows)
		{
			var item = new T();
			MaterializerUtilities.PopulateComplexObject(row, item, null, m_Converter);

			//Change tracking objects shouldn't be materialized as unchanged.
			var tracking = item as IChangeTracking;
			tracking?.AcceptChanges();

			yield return new KeyValuePair<Row, T>(row, item);
		}
	}
}
