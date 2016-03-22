namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Metadata for a stored procedure parameter
    /// </summary>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public class ParameterMetadata<TDbType>
        where TDbType : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetadata{TDbType}" /> class.
        /// </summary>
        /// <param name="sqlParameterName">Name of the SQL parameter.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="dbType">Type of the database.</param>
        public ParameterMetadata(string sqlParameterName, string typeName, TDbType? dbType)
        {
            TypeName = typeName;
            SqlParameterName = sqlParameterName;
            ClrName = Utilities.ToClrName(sqlParameterName);
            DbType = dbType;
        }

        /// <summary>
        /// Gets the name used by CLR objects.
        /// </summary>
        public string ClrName { get; }

        /// <summary>
        /// Gets the name used by the database.
        /// </summary>
        public string SqlParameterName { get; }

        /// <summary>
        /// Gets the type used by the database.
        /// </summary>
        public TDbType? DbType { get; }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public string TypeName { get; }
    }
}
