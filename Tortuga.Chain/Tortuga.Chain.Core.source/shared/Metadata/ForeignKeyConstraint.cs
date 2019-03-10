#pragma warning disable CA2227 // Collection properties should be read only

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// This represents a foreign key relationship between two tables.
    /// </summary>
    public abstract class ForeignKeyConstraint
    {
        /// <summary>
        /// Gets the columns in the child table.
        /// </summary>
        public ColumnMetadataCollection ChildColumns { get; protected set; }

        /// <summary>
        /// Gets the name of the child table.
        /// </summary>
        public string ChildTableName { get; protected set; }

        /// <summary>
        /// Gets the columns in the parent table. This will usually be the primary key(s).
        /// </summary>
        public ColumnMetadataCollection ParentColumns { get; protected set; }

        /// <summary>
        /// Gets the name of the parent table.
        /// </summary>
        public string ParentTableName { get; protected set; }
    }
}

#pragma warning restore CA2227 // Collection properties should be read only
