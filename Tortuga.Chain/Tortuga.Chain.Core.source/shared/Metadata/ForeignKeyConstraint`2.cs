using System;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// This represents a foreign key relationship between two tables.
    /// </summary>
    /// <typeparam name="TName">The type of the name.</typeparam>
    /// <typeparam name="TDbType">The type of the database type.</typeparam>
    public class ForeignKeyConstraint<TName, TDbType> : ForeignKeyConstraint
        where TDbType : struct
        where TName : struct
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="parentTableName">Name of the parent table.</param>
        /// <param name="parentColumns">The parent columns.</param>
        /// <param name="childTableName">Name of the child table.</param>
        /// <param name="childColumns">The child columns.</param>
        public ForeignKeyConstraint(TName parentTableName, ColumnMetadataCollection<TDbType> parentColumns, TName childTableName, ColumnMetadataCollection<TDbType> childColumns)
        {
            if (parentColumns == null || parentColumns.Count == 0)
                throw new ArgumentException($"{nameof(parentColumns)} is null or empty.", nameof(parentColumns));

            if (childColumns == null || childColumns.Count == 0)
                throw new ArgumentException($"{nameof(childColumns)} is null or empty.", nameof(childColumns));

            ParentTableName = parentTableName;
            base.ParentTableName = parentTableName.ToString();
            ParentColumns = parentColumns;
            base.ParentColumns = parentColumns.GenericCollection;
            ChildTableName = childTableName;
            base.ChildTableName = childTableName.ToString();
            ChildColumns = childColumns;
            base.ChildColumns = childColumns.GenericCollection;
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
