using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// This maps database columns (tables and views) to class properties.
/// </summary>
/// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
public sealed class ColumnPropertyMap<TDbType>
	where TDbType : struct
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ColumnPropertyMap{TDbType}"/> class.
	/// </summary>
	/// <param name="column">The column.</param>
	/// <param name="property">The property.</param>
	public ColumnPropertyMap(ColumnMetadata<TDbType> column, PropertyMetadata property)
	{
		Column = column;
		Property = property;
	}

	/// <summary>
	/// Gets the column.
	/// </summary>
	/// <value>The column.</value>
	public ColumnMetadata<TDbType> Column { get; private set; }

	/// <summary>
	/// Gets the property.
	/// </summary>
	/// <value>The property.</value>
	public PropertyMetadata Property { get; private set; }
}
