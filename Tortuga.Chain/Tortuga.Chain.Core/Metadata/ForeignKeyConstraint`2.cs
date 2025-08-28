namespace Tortuga.Chain.Metadata;

/// <summary>
/// This represents a foreign key relationship between two tables.
/// </summary>
/// <typeparam name="TObjectName">The type of the name.</typeparam>
/// <typeparam name="TDbType">The database column type.</typeparam>
public class ForeignKeyConstraint<TObjectName, TDbType> : ForeignKeyConstraint
	where TObjectName : struct
	where TDbType : struct
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ForeignKeyConstraint{TObjectName, TDbType}" /> class.
	/// </summary>
	/// <param name="name">Name of the constraint.</param>
	/// <param name="constrainedTableName">Name of the constrained table.</param>
	/// <param name="constrainedColumns">The constrained columns.</param>
	/// <param name="referencedTableName">Name of the referenced table.</param>
	/// <param name="referencedColumns">The referenced columns.</param>
	/// <exception cref="System.ArgumentException">constrainedColumns</exception>
	/// <exception cref="System.ArgumentException">referencedColumns</exception>
	public ForeignKeyConstraint(string name, TObjectName constrainedTableName, ColumnMetadataCollection<TDbType> constrainedColumns, TObjectName referencedTableName, ColumnMetadataCollection<TDbType> referencedColumns) : base(name, constrainedTableName.ToString()!, constrainedColumns?.GenericCollection!, referencedTableName.ToString()!, referencedColumns?.GenericCollection!)
	{
		if (constrainedColumns == null || constrainedColumns.Count == 0)
			throw new ArgumentException($"{nameof(constrainedColumns)} is null or empty.", nameof(constrainedColumns));

		if (referencedColumns == null || referencedColumns.Count == 0)
			throw new ArgumentException($"{nameof(referencedColumns)} is null or empty.", nameof(referencedColumns));

		ConstrainedTableName = constrainedTableName;
		ConstrainedColumns = constrainedColumns;
		ReferencedTableName = referencedTableName;
		ReferencedColumns = referencedColumns;
	}

	/// <summary>
	/// Gets the columns in the parent table.
	/// </summary>
	public new ColumnMetadataCollection<TDbType> ConstrainedColumns { get; }

	/// <summary>
	/// Gets the name of the parent table.
	/// </summary>
	public new TObjectName ConstrainedTableName { get; }

	/// <summary>
	/// Gets the columns in the referenced table. This will usually be the primary key(s).
	/// </summary>
	public new ColumnMetadataCollection<TDbType> ReferencedColumns { get; }

	/// <summary>
	/// Gets the name of the referenced table.
	/// </summary>
	public new TObjectName ReferencedTableName { get; }
}
