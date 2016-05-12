namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Abstract version of ColumnMetadata.
    /// </summary>
    public abstract class ColumnMetadata
    {
        /// <summary>
        /// Gets the name used by CLR objects.
        /// </summary>
        public string ClrName { get; protected set; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="ColumnMetadata{TDbType}"/> is computed.
        /// </summary>
        /// <value>
        /// <c>true</c> if computed; otherwise, <c>false</c>.
        /// </value>
        public bool IsComputed { get; protected set; }
        /// <summary>
        /// Gets a value indicating whether this column is an identity column.
        /// </summary>
        /// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
        public bool IsIdentity { get; protected set; }
        /// <summary>
        /// Gets a value indicating whether this column is a primary key.
        /// </summary>
        /// <value><c>true</c> if this instance is primary key; otherwise, <c>false</c>.</value>
        public bool IsPrimaryKey { get; protected set; }
        /// <summary>
        /// Gets the name used by SQL Server, quoted.
        /// </summary>
        public string QuotedSqlName { get; protected set; }
        /// <summary>
        /// Gets the name used by SQL Server.
        /// </summary>
        public string SqlName { get; protected set; }
        /// <summary>
        /// Gets the column, formatted as a SQL variable.
        /// </summary>
        public string SqlVariableName { get; protected set; }

        /// <summary>
        /// Gets the type used by the database.
        /// </summary>
        public object DbType { get; protected set; }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        public string TypeName { get; protected set; }
    }

}
