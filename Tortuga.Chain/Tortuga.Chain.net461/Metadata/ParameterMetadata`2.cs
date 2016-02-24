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
        private readonly string m_SqlName;
        private readonly TDbType? m_DbType;
        private readonly string m_TypeName;


        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetadata{TDbType}" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="dbType">Type of the database.</param>
        public ParameterMetadata(string name, string typeName, TDbType? dbType)
        {
            m_TypeName = typeName;
            m_SqlName = name;
            m_ClrName = Utilities.ToClrName(name);
            m_DbType = dbType;
            //m_DbType = Utilities.TypeNameToSqlDbType(typeName);
        }

        /// <summary>
        /// Gets the name used by CLR objects.
        /// </summary>
        public string ClrName
        {
            get { return m_ClrName; }
        }

        /// <summary>
        /// Gets the name used by SQL Server.
        /// </summary>
        public string SqlName
        {
            get { return m_SqlName; }
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
