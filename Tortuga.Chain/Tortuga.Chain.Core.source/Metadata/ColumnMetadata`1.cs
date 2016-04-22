namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Metadata for a table or view column
    /// </summary>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public sealed class ColumnMetadata<TDbType> : IColumnMetadata, ISqlBuilderEntryDetails<TDbType>
        where TDbType : struct
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
        public ColumnMetadata(string name, bool isComputed, bool isPrimaryKey, bool isIdentity, string typeName, TDbType? dbType, string quotedSqlName)
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
                SqlVariableName = "@" + name;
            }
        }

        /// <summary>
        /// Gets the name used by CLR objects.
        /// </summary>
        public string ClrName { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ColumnMetadata{TDbType}"/> is computed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if computed; otherwise, <c>false</c>.
        /// </value>
        public bool IsComputed { get; }

        /// <summary>
        /// Gets a value indicating whether this column is an identity column.
        /// </summary>
        /// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
        public bool IsIdentity { get; }

        /// <summary>
        /// Gets a value indicating whether this column is a primary key.
        /// </summary>
        /// <value><c>true</c> if this instance is primary key; otherwise, <c>false</c>.</value>
        public bool IsPrimaryKey { get; }

        /// <summary>
        /// Gets the name used by SQL Server, quoted.
        /// </summary>
        public string QuotedSqlName { get; }

        /// <summary>
        /// Gets the name used by SQL Server.
        /// </summary>
        public string SqlName { get; }

        /// <summary>
        /// Gets the column, formatted as a SQL variable.
        /// </summary>
        public string SqlVariableName { get; }

        /// <summary>
        /// Gets the type used by the database.
        /// </summary>
        public TDbType? DbType { get; }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        public string TypeName { get; }

        object IColumnMetadata.DbType
        {
            get { return DbType; }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return SqlName + " (" + TypeName + ")";
        }
    }
}
