namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Metadata for a stored procedure parameter
    /// </summary>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public class ParameterMetadata<TDbType>
        where TDbType : struct
    {
        private readonly string m_ClrName;
        private readonly string m_SqlParameterName;
        private readonly TDbType? m_DbType;
        private readonly string m_TypeName;


        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetadata{TDbType}" /> class.
        /// </summary>
        /// <param name="sqlParameterName">Name of the SQL parameter.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="dbType">Type of the database.</param>
        public ParameterMetadata(string sqlParameterName, string typeName, TDbType? dbType)
        {
            m_TypeName = typeName;
            m_SqlParameterName = sqlParameterName;
            m_ClrName = Utilities.ToClrName(sqlParameterName);
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
        /// Gets the name used by the database.
        /// </summary>
        public string SqlParameterName
        {
            get { return m_SqlParameterName; }
        }

        /// <summary>
        /// Gets the type used by the database.
        /// </summary>
        public TDbType? SqlDbType
        {
            get { return m_DbType; }
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public string TypeName
        {
            get { return m_TypeName; }
        }
    }
}
