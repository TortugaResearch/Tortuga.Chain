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
        public ParameterMetadata(string sqlParameterName, string sqlVariableName, string typeName, TDbType? dbType) : base(sqlParameterName, sqlVariableName, typeName, dbType)
        {
            DbType = dbType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetadata{TDbType}"/> class.
        /// </summary>
        /// <param name="sqlParameterName">Name of the SQL parameter.</param>
        /// <param name="sqlVariableName">Name of the SQL variable.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="dbType">Type of the database.</param>
        /// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="fullTypeName">Full name of the type.</param>
        public ParameterMetadata(string sqlParameterName, string sqlVariableName, string typeName, TDbType? dbType, bool? isNullable, int? maxLength, int? precision, int? scale, string fullTypeName) : base(sqlParameterName, sqlVariableName, typeName, dbType, isNullable, maxLength, precision, scale, fullTypeName)
        {
            DbType = dbType;
        }

        /// <summary>
        /// Gets the type used by the database.
        /// </summary>
        public new TDbType? DbType { get; }
    }
}
