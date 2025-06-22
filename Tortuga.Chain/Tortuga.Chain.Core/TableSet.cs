using System.Collections;
using System.Collections.ObjectModel;
using System.Data.Common;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain;

/// <summary>
/// A set of named tables.
/// </summary>
/// <remarks>This is much faster than a DataSet, but lacks most of its features.</remarks>
[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
public sealed class TableSet : IReadOnlyList<Table>
{
	readonly TSKeyedCollection m_Internal = new TSKeyedCollection();

	/// <summary>
	/// Initializes a new instance of the <see cref="TableSet" /> class.
	/// </summary>
	/// <param name="reader">The data reader used to populate this TableSet.</param>
	/// <param name="converter">The type converter.</param>
	/// <param name="tableNames">Optional list of table names.</param>
	/// <exception cref="System.ArgumentNullException">reader</exception>
	public TableSet(DbDataReader reader, MaterializerTypeConverter converter, params string[] tableNames)
	{
		if (reader == null)
			throw new ArgumentNullException(nameof(reader), $"{nameof(reader)} is null.");
		if (tableNames == null)
			tableNames = Array.Empty<string>();

		var index = 0;
		do
		{
			var tableName = (index < tableNames.Length) ? tableNames[index] : ("Table " + index);
			m_Internal.Add(new Table(tableName, reader, converter));
			index += 1;
		}
		while (reader.NextResult());
	}

	/// <summary>
	/// Gets the number of elements in the collection.
	/// </summary>
	public int Count => m_Internal.Count;

	/// <summary>
	/// Gets the <see cref="Table"/> at the specified index.
	/// </summary>
	/// <value>
	/// The <see cref="Table"/>.
	/// </value>
	/// <param name="index">The index.</param>
	/// <returns></returns>
	public Table this[int index] => m_Internal[index];

	/// <summary>
	/// Gets the <see cref="Table"/> with the specified key.
	/// </summary>
	/// <value>
	/// The <see cref="Table"/>.
	/// </value>
	/// <param name="key">The key.</param>
	/// <returns></returns>
	public Table this[string key] => m_Internal[key];

	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>
	/// An enumerator that can be used to iterate through the collection.
	/// </returns>
	public IEnumerator<Table> GetEnumerator() => m_Internal.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => m_Internal.GetEnumerator();

	/// <summary>
	/// Copies this TableSet into a DataSet.
	/// </summary>
	/// <returns>DataSet.</returns>
	public DataSet ToDataSet()
	{
		var ds = new DataSet();
		foreach (var table in this)
			ds.Tables.Add(table.ToDataTable());

		return ds;
	}

	class TSKeyedCollection : KeyedCollection<string, Table>
	{
		/// <summary>
		/// When implemented in a derived class, extracts the key from the specified element.
		/// </summary>
		/// <param name="item">The element from which to extract the key.</param>
		/// <returns>The key for the specified element.</returns>
		protected override string GetKeyForItem(Table item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item), $"{nameof(item)} is null.");

			return item.TableName;
		}
	}
}
