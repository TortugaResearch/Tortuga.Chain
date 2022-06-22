using System.Collections.ObjectModel;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// Collection of Column Metadata
/// </summary>
/// <typeparam name="TDbType">The type of the database type.</typeparam>
public class ColumnMetadataCollection<TDbType> : ReadOnlyCollection<ColumnMetadata<TDbType>>
	where TDbType : struct
{
	readonly string m_Name;

	/// <summary>
	/// Initializes a new instance of the <see cref="ColumnMetadataCollection{TDbType}" /> class.
	/// </summary>
	/// <param name="name">The name of the parent object.</param>
	/// <param name="list">The list to wrap.</param>
	public ColumnMetadataCollection(string name, IList<ColumnMetadata<TDbType>> list) : base(list)
	{
		m_Name = name;
		GenericCollection = new ColumnMetadataCollection(name, this);
	}

	/// <summary>
	/// Gets the <see cref="ColumnMetadata{TDbType}"/> with the specified column name.
	/// </summary>
	/// <value>
	/// The <see cref="ColumnMetadata{TDbType}"/>.
	/// </value>
	/// <param name="columnName">Name of the column.</param>
	/// <returns></returns>
	public ColumnMetadata<TDbType> this[string columnName]
	{
		get
		{
			foreach (var item in this)
				if (item.SqlName.Equals(columnName, System.StringComparison.OrdinalIgnoreCase))
					return item;

#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
			throw new KeyNotFoundException($"Could not find column named {columnName} in object {m_Name}");
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
		}
	}

	/// <summary>
	/// Returns the column associated with the column name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns></returns>
	/// <remarks>If the column name was not found, this will return null</remarks>
	public ColumnMetadata<TDbType>? TryGetColumn(string columnName)
	{
		foreach (var item in this)
			if (item.SqlName.Equals(columnName, System.StringComparison.OrdinalIgnoreCase))
				return item;

		return null;
	}

	/// <summary>
	/// Gets the generic version of this collection.
	/// </summary>
	/// <remarks>We can't make this implement IReadOnlyList because it breaks LINQ.</remarks>
	public ColumnMetadataCollection GenericCollection { get; }
}
