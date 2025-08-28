namespace Tortuga.Chain.Metadata;

/// <summary>
/// This represents a foreign key relationship between two tables.
/// </summary>
public abstract class ForeignKeyConstraint
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ForeignKeyConstraint" /> class.
	/// </summary>
	/// <param name="name">Name of the constraint.</param>
	/// <param name="constrainedTableName">Name of the constrained table.</param>
	/// <param name="constrainedColumns">The constrained columns.</param>
	/// <param name="referencedTableName">Name of the referenced table.</param>
	/// <param name="referencedColumns">The referenced columns.</param>
	/// <exception cref="System.ArgumentException">name</exception>
	/// <exception cref="System.ArgumentException">constrainedTableName</exception>
	/// <exception cref="System.ArgumentException">referencedTableName</exception>
	/// <exception cref="System.ArgumentException">constrainedColumns</exception>
	/// <exception cref="System.ArgumentException">referencedColumns</exception>
	protected ForeignKeyConstraint(string name, string constrainedTableName, ColumnMetadataCollection constrainedColumns, string referencedTableName, ColumnMetadataCollection referencedColumns)
	{
		if (string.IsNullOrEmpty(name))
			throw new ArgumentException($"{nameof(name)} is null or empty.", nameof(name));
		if (string.IsNullOrEmpty(constrainedTableName))
			throw new ArgumentException($"{nameof(constrainedTableName)} is null or empty.", nameof(constrainedTableName));
		if (string.IsNullOrEmpty(referencedTableName))
			throw new ArgumentException($"{nameof(referencedTableName)} is null or empty.", nameof(referencedTableName));
		if (constrainedColumns == null || constrainedColumns.Count == 0)
			throw new ArgumentException($"{nameof(constrainedColumns)} is null or empty.", nameof(constrainedColumns));

		if (referencedColumns == null || referencedColumns.Count == 0)
			throw new ArgumentException($"{nameof(referencedColumns)} is null or empty.", nameof(referencedColumns));

		Name = name;
		ConstrainedTableName = constrainedTableName;
		ConstrainedColumns = constrainedColumns;
		ReferencedTableName = referencedTableName;
		ReferencedColumns = referencedColumns;
	}

	/// <summary>
	/// Gets the columns in the parent table.
	/// </summary>
	public ColumnMetadataCollection ConstrainedColumns { get; }

	/// <summary>
	/// Gets the name of the parent table.
	/// </summary>
	public string ConstrainedTableName { get; }

	/// <summary>
	/// Gets the name of the constraint.
	/// </summary>
	/// <value>The name of the constraint.</value>
	public string Name { get; }

	/// <summary>
	/// Gets the columns in the referenced table. This will usually be the primary key(s).
	/// </summary>
	public ColumnMetadataCollection ReferencedColumns { get; }

	/// <summary>
	/// Gets the name of the referenced table.
	/// </summary>
	public string ReferencedTableName { get; }
}
