namespace Tortuga.Chain.Metadata
{
	/// <summary>
	/// This represents a foreign key relationship between two tables.
	/// </summary>
	/// <typeparam name="TName">The type of the name.</typeparam>
	/// <typeparam name="TDbType">The database column type.</typeparam>
	public class ForeignKeyConstraint<TName, TDbType> : ForeignKeyConstraint
		where TName : struct
		where TDbType : struct
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ForeignKeyConstraint{TName, TDbType}" /> class.
		/// </summary>
		/// <param name="parentTableName">Name of the parent table.</param>
		/// <param name="parentColumns">The parent columns.</param>
		/// <param name="childTableName">Name of the child table.</param>
		/// <param name="childColumns">The child columns.</param>
		public ForeignKeyConstraint(TName parentTableName, ColumnMetadataCollection<TDbType> parentColumns, TName childTableName, ColumnMetadataCollection<TDbType> childColumns) : base(parentTableName.ToString()!, parentColumns?.GenericCollection!, childTableName.ToString()!, childColumns?.GenericCollection!)
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
		public new ColumnMetadataCollection<TDbType> ChildColumns { get; }

		/// <summary>
		/// Gets the name of the child table.
		/// </summary>
		public new TName ChildTableName { get; }

		/// <summary>
		/// Gets the columns in the parent table. This will usually be the primary key(s).
		/// </summary>
		public new ColumnMetadataCollection<TDbType> ParentColumns { get; }

		/// <summary>
		/// Gets the name of the parent table.
		/// </summary>
		public new TName ParentTableName { get; }
	}
}
