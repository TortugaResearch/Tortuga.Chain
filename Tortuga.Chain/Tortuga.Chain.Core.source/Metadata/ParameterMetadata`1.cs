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
        /// <param name="typeName">Name of the type.</param>
        /// <param name="dbType">Type of the database.</param>
        public ParameterMetadata(string sqlParameterName, string typeName, TDbType? dbType)
        {
            TypeName = typeName;
            SqlParameterName = sqlParameterName;
            ClrName = Utilities.ToClrName(sqlParameterName);
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

        string ISqlBuilderEntryDetails.SqlVariableName
        {
            get { return SqlParameterName; }
        }

    }
}
