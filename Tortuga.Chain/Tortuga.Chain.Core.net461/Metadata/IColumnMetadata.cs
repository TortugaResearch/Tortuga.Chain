namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Abstract version of ColumnMetadata.
    /// </summary>
    public interface IColumnMetadata
    {
        /// <summary>
        /// Gets the name used by CLR objects.
        /// </summary>
        string ClrName { get; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="ColumnMetadata{TDbType}"/> is computed.
        /// </summary>
        /// <value>
        /// <c>true</c> if computed; otherwise, <c>false</c>.
        /// </value>
        bool IsComputed { get; }
        /// <summary>
        /// Gets a value indicating whether this column is an identity column.
        /// </summary>
        /// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
        bool IsIdentity { get; }
        /// <summary>
        /// Gets a value indicating whether this column is a primary key.
        /// </summary>
        /// <value><c>true</c> if this instance is primary key; otherwise, <c>false</c>.</value>
        bool IsPrimaryKey { get; }
        /// <summary>
        /// Gets the name used by SQL Server, quoted.
        /// </summary>
        string QuotedSqlName { get; }
        /// <summary>
        /// Gets the name used by SQL Server.
        /// </summary>
        string SqlName { get; }
        /// <summary>
        /// Gets the column, formatted as a SQL variable.
        /// </summary>
        string SqlVariableName { get; }

        /// <summary>
        /// Gets the type used by the database.
        /// </summary>
        object SqlDbType { get; }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        string TypeName { get; }
    }
}
