using System.Data;
namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Metadata for a stored procedure parameter
    /// </summary>
    public class ParameterMetadata
    {
        private readonly string m_ClrName;
        private readonly string m_SqlName;
        private readonly SqlDbType? m_SqlDbType;
        private readonly string m_TypeName;


        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetadata" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="typeName">Name of the type.</param>
        public ParameterMetadata(string name, string typeName)
        {
            m_TypeName = typeName;
            m_SqlName = name;
            m_ClrName = Utilities.ToClrName(name);
            m_SqlDbType = Utilities.TypeNameToSqlDbType(typeName);
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
        /// Gets the type used by SQL Server.
        /// </summary>
        public SqlDbType? SqlDbType
        {
            get { return m_SqlDbType; }
        }
    }
}
