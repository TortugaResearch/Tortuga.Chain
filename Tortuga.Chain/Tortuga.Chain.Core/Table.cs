using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Materializers;

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
	readonly RowCollection m_Rows;

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
	/// <param name="source">The source.</param>
	/// <exception cref="ArgumentNullException">nameof(s - rce), "source is</exception>
	/// <exception cref="ArgumentException">No columns were returned - source</exception>
	public Table(DbDataReader source)
	{
		if (source == null)
			throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");
		if (source.FieldCount == 0)
			throw new ArgumentException("No columns were returned", nameof(source));

		TableName = "";

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
			var row = new Dictionary<string, object?>(source.FieldCount, StringComparer.OrdinalIgnoreCase);
			for (var i = 0; i < source.FieldCount; i++)
			{
				object? temp = source[i];
				if (temp == DBNull.Value)
					temp = null;

				row.Add(source.GetName(i), temp);
			}

			rows.Add(new Row(row));
		}

		m_Rows = new RowCollection(rows);
	}

	/// <summary>
	/// Creates a new Table from an IDataReader
	/// </summary>
	/// <param name="source"></param>
	public Table(IDataReader source)
	{
		if (source == null)
			throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");
		if (source.FieldCount == 0)
			throw new ArgumentException("No columns were returned", nameof(source));

		TableName = "";

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
			var row = new Dictionary<string, object?>(source.FieldCount, StringComparer.OrdinalIgnoreCase);
			for (var i = 0; i < source.FieldCount; i++)
			{
				object? temp = source[i];
				if (temp == DBNull.Value)
					temp = null;

				row.Add(source.GetName(i), temp);
			}

			rows.Add(new Row(row));
		}

		m_Rows = new RowCollection(rows);
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
	/// Converts the table into an enumeration of objects of the indicated type.
	/// </summary>
	/// <typeparam name="T">Desired object type</typeparam>
	public IEnumerable<T> ToObjects<T>() where T : class, new()
	{
		foreach (var row in Rows)
		{
			var item = new T();
			MaterializerUtilities.PopulateComplexObject(row, item, null);

			//Change tracking objects shouldn't be materialized as unchanged.
			var tracking = item as IChangeTracking;
			tracking?.AcceptChanges();

			yield return item;
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
			MaterializerUtilities.PopulateComplexObject(row, item, null);

			//Change tracking objects shouldn't be materialized as unchanged.
			var tracking = item as IChangeTracking;
			tracking?.AcceptChanges();

			yield return new KeyValuePair<Row, T>(row, item);
		}
	}
}
