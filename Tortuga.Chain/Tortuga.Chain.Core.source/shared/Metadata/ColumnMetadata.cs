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
        public string 
            SqlName { get; protected set; }
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

        /// <summary>
        /// Gets or sets the maximum length.
        /// </summary>
        /// <value>
        /// The maximum length.
        /// </value>
        public int? MaxLength { get; protected set; }

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>
        /// The precision.
        /// </value>
        public int? Precision { get; protected set; }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public int? Scale { get; protected set; }

        /// <summary>
        /// Gets or sets the full name of the type including max length, precision, and/or scale.
        /// </summary>
        /// <value>
        /// The full name of the type.
        /// </value>
        public string FullTypeName { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column is nullable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this column is nullable; otherwise, <c>false</c>.
        /// </value>
        public bool IsNullable { get; protected set; }
    }

}
