namespace Tortuga.Chain.Metadata
{


    /// <summary>
    /// Metadata for a table or view column
    /// </summary>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public sealed class ColumnMetadata<TDbType> : ColumnMetadata, ISqlBuilderEntryDetails<TDbType>
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
            base.DbType = dbType;
            QuotedSqlName = quotedSqlName;

            if (!string.IsNullOrWhiteSpace(name))
            {
                ClrName = Utilities.ToClrName(name);
                SqlVariableName = "@" + name;
            }
        }

        /// <summary>
        /// Gets the type used by the database.
        /// </summary>
        public new TDbType? DbType { get; }


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
