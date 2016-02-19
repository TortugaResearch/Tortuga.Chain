namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Metadata for a stored procedure parameter
    /// </summary>
    public class ParameterMetadata
    {
        private readonly string m_ClrName;
        private readonly string m_SqlName;


        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetadata"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ParameterMetadata(string name)
        {
            m_SqlName = name;
            m_ClrName = Utilities.ToClrName(name);
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
    }
}
