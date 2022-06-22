namespace Tortuga.Chain.Metadata;

/// <summary>
/// This represents a foreign key relationship between two tables.
/// </summary>
public abstract class ForeignKeyConstraint
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ForeignKeyConstraint" /> class.
	/// </summary>
	/// <param name="parentTableName">Name of the parent table.</param>
	/// <param name="parentColumns">The parent columns.</param>
	/// <param name="childTableName">Name of the child table.</param>
	/// <param name="childColumns">The child columns.</param>
	/// <exception cref="ArgumentException">
	/// parentColumns
	/// or
	/// childColumns
	/// </exception>
	protected ForeignKeyConstraint(string parentTableName, ColumnMetadataCollection parentColumns, string childTableName, ColumnMetadataCollection childColumns)
	{
		if (parentColumns == null || parentColumns.Count == 0)
			throw new ArgumentException($"{nameof(parentColumns)} is null or empty.", nameof(parentColumns));

		if (childColumns == null || childColumns.Count == 0)
			throw new ArgumentException($"{nameof(childColumns)} is null or empty.", nameof(childColumns));

		ParentTableName = parentTableName;
		ParentColumns = parentColumns;
		ChildTableName = childTableName;
		ChildColumns = childColumns;
	}

	/// <summary>
	/// Gets the columns in the child table.
	/// </summary>
	public ColumnMetadataCollection ChildColumns { get; }

	/// <summary>
	/// Gets the name of the child table.
	/// </summary>
	public string ChildTableName { get; }

	/// <summary>
	/// Gets the columns in the parent table. This will usually be the primary key(s).
	/// </summary>
	public ColumnMetadataCollection ParentColumns { get; }

	/// <summary>
	/// Gets the name of the parent table.
	/// </summary>
	public string ParentTableName { get; }
}
