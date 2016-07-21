namespace Tortuga.Chain.Metadata
{


    /// <summary>
    /// Metadata for a stored procedure parameter
    /// </summary>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public sealed class ParameterMetadata<TDbType> : ParameterMetadata, ISqlBuilderEntryDetails<TDbType>
        where TDbType : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetadata{TDbType}" /> class.
        /// </summary>
        /// <param name="sqlParameterName">Name of the SQL parameter.</param>
        /// <param name="sqlVariableName">Name of the SQL variable.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="dbType">Type of the database.</param>
        public ParameterMetadata(string sqlParameterName, string sqlVariableName, string typeName, TDbType? dbType)
        {
            TypeName = typeName;
            SqlParameterName = sqlParameterName;
            ClrName = Utilities.ToClrName(sqlParameterName);
            SqlVariableName = sqlVariableName;
            DbType = dbType;
            base.DbType = dbType;
        }

        /// <summary>
        /// Gets the type used by the database.
        /// </summary>
        public new TDbType? DbType { get; }

        bool ISqlBuilderEntryDetails.IsIdentity
        {
            get { return false; }
        }

        string ISqlBuilderEntryDetails.QuotedSqlName
        {
            get { return null; }
        }

        string ISqlBuilderEntryDetails.SqlName
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the name of the SQL variable.
        /// </summary>
        /// <value>The name of the SQL variable.</value>
        public string SqlVariableName { get; }

    }
}
