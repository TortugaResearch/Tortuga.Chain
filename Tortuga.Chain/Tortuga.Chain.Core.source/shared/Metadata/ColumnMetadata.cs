namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Abstract version of ColumnMetadata.
    /// </summary>
    public abstract class ColumnMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnMetadata{TDbType}" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isComputed">if set to <c>true</c> is a computed column.</param>
        /// <param name="isPrimaryKey">if set to <c>true</c> is a primary key.</param>
        /// <param name="isIdentity">if set to <c>true</c> [is identity].</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="dbType">Type used by the database.</param>
        /// <param name="quotedSqlName">Name of the quoted SQL.</param>
        /// <param name="isNullable">Indicates if the column is nullable.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="fullTypeName">Full name of the type.</param>
        protected ColumnMetadata(string name, bool isComputed, bool isPrimaryKey, bool isIdentity, string typeName, object dbType, string quotedSqlName, bool? isNullable, int? maxLength, int? precision, int? scale, string fullTypeName)
        {
            TypeName = typeName;
            SqlName = name;
            IsComputed = isComputed;
            IsPrimaryKey = isPrimaryKey;
            IsIdentity = isIdentity;
            DbType = dbType;
            QuotedSqlName = quotedSqlName;

            if (!string.IsNullOrWhiteSpace(name))
            {
                ClrName = Utilities.ToClrName(name);
                SqlVariableName = "@" + ClrName;
            }

            IsNullable = isNullable;
            Precision = precision;
            MaxLength = maxLength;
            Scale = scale;
            FullTypeName = fullTypeName;
        }

        /// <summary>
        /// Gets the name used by CLR objects.
        /// </summary>
        public string ClrName { get; }

        /// <summary>
        /// Gets the type used by the database.
        /// </summary>
        public object DbType { get; }

        /// <summary>
        /// Gets or sets the full name of the type including max length, precision, and/or scale.
        /// </summary>
        /// <value>
        /// The full name of the type.
        /// </value>
        public string FullTypeName { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ColumnMetadata{TDbType}"/> is computed.
        /// </summary>
        /// <value>
        /// <c>true</c> if computed; otherwise, <c>false</c>.
        /// </value>
        public bool IsComputed { get; }

        /// <summary>
        /// Gets a value indicating whether this column is an identity column.
        /// </summary>
        /// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
        public bool IsIdentity { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this column is nullable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this column is nullable; otherwise, <c>false</c>.
        /// </value>
        public bool? IsNullable { get; }

        /// <summary>
        /// Gets a value indicating whether this column is a primary key.
        /// </summary>
        /// <value><c>true</c> if this instance is primary key; otherwise, <c>false</c>.</value>
        public bool IsPrimaryKey { get; }

        /// <summary>
        /// Gets or sets the maximum length.
        /// </summary>
        /// <value>
        /// The maximum length.
        /// </value>
        public int? MaxLength { get; }

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>
        /// The precision.
        /// </value>
        public int? Precision { get; }

        /// <summary>
        /// Gets the name used by SQL Server, quoted.
        /// </summary>
        public string QuotedSqlName { get; }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public int? Scale { get; }

        /// <summary>
        /// Gets the name used by the database.
        /// </summary>
        public string
            SqlName
        { get; }

        /// <summary>
        /// Gets the column, formatted as a SQL variable.
        /// </summary>
        public string SqlVariableName { get; }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        public string TypeName { get; }
    }
}
