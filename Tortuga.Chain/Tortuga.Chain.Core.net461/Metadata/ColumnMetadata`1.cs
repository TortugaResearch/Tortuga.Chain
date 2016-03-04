namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Metadata for a table or view column
    /// </summary>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public class ColumnMetadata<TDbType> : IColumnMetadata
        where TDbType : struct
    {
        private readonly string m_ClrName;
        private readonly bool m_IsComputed;
        private readonly bool m_IsIdentity;
        private readonly bool m_IsPrimaryKey;
        private readonly string m_SqlName;
        private readonly string m_TypeName;
        private readonly TDbType? m_DbType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnMetadata{TDbType}" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isComputed">if set to <c>true</c> is a computed column.</param>
        /// <param name="isPrimaryKey">if set to <c>true</c> is a primary key.</param>
        /// <param name="isIdentity">if set to <c>true</c> [is identity].</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="dbType">Type used by the database.</param>
        public ColumnMetadata(string name, bool isComputed, bool isPrimaryKey, bool isIdentity, string typeName, TDbType? dbType)
        {
            m_TypeName = typeName;
            m_SqlName = name;
            m_ClrName = Utilities.ToClrName(name);
            m_IsComputed = isComputed;
            m_IsPrimaryKey = isPrimaryKey;
            m_IsIdentity = isIdentity;
            m_DbType = dbType;
        }

        /// <summary>
        /// Gets the name used by CLR objects.
        /// </summary>
        public string ClrName
        {
            get { return m_ClrName; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ColumnMetadata{TDbType}"/> is computed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if computed; otherwise, <c>false</c>.
        /// </value>
        public bool IsComputed
        {
            get { return m_IsComputed; }
        }

        /// <summary>
        /// Gets a value indicating whether this column is an identity column.
        /// </summary>
        /// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
        public bool IsIdentity
        {
            get { return m_IsIdentity; }
        }

        /// <summary>
        /// Gets a value indicating whether this column is a primary key.
        /// </summary>
        /// <value><c>true</c> if this instance is primary key; otherwise, <c>false</c>.</value>
        public bool IsPrimaryKey
        {
            get { return m_IsPrimaryKey; }
        }

        /// <summary>
        /// Gets the name used by SQL Server, quoted.
        /// </summary>
        public string QuotedSqlName
        {
            get { return "[" + m_SqlName + "]"; }
        }

        /// <summary>
        /// Gets the name used by SQL Server.
        /// </summary>
        public string SqlName
        {
            get { return m_SqlName; }
        }
        /// <summary>
        /// Gets the column, formatted as a SQL variable.
        /// </summary>
        public string SqlVariableName
        {
            get { return "@" + m_SqlName; }
        }

        /// <summary>
        /// Gets the type used by the database.
        /// </summary>
        public TDbType? DbType
        {
            get { return m_DbType; }
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        public string TypeName
        {
            get { return m_TypeName; }
        }

        object IColumnMetadata.DbType
        {
            get { return DbType; }
        }

    }
}
