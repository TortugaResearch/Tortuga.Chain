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
        public ParameterMetadata(string sqlParameterName, string sqlVariableName, string typeName, TDbType? dbType, bool? isNullable, int? maxLength, int? precision, int? scale, string fullTypeName)
        {
            SqlParameterName = sqlParameterName;
            SqlVariableName = sqlVariableName;
            TypeName = typeName;
            ClrName = Utilities.ToClrName(sqlParameterName);
            DbType = dbType;
            base.DbType = dbType;
            IsNullable = isNullable;
            MaxLength = maxLength;
            Precision = precision;
            Scale = scale;
            FullTypeName = fullTypeName;
        }

        /// <summary>
        /// Gets the type used by the database.
        /// </summary>
        public new TDbType? DbType { get; }

        /// <summary>
        /// Gets or sets the full name of the type including max length, precision, and/or scale.
        /// </summary>
        /// <value>
        /// The full name of the type.
        /// </value>
        /// <remarks>This will be null if the data source doesn't support detailed parameter metadata.</remarks>
        public string FullTypeName { get; }

        bool ISqlBuilderEntryDetails.IsIdentity => false;

        /// <summary>
        /// Gets a value indicating whether this instance is nullable.
        /// </summary>
        /// <remarks>This will be null if the data source doesn't support detailed parameter metadata.</remarks>
        public bool? IsNullable { get; private set; }

        /// <summary>
        /// Gets the maximum length.
        /// </summary>
        /// <value>The maximum length.</value>
        /// <remarks>This will be null if the data source doesn't support detailed parameter metadata or if this value isn't applicable to the data type.</remarks>
        public int? MaxLength { get; private set; }

        /// <summary>
        /// Gets the precision.
        /// </summary>
        /// <value>The precision.</value>
        /// <remarks>This will be null if the data source doesn't support detailed parameter metadata or if this value isn't applicable to the data type.</remarks>
        public int? Precision { get; private set; }

        string ISqlBuilderEntryDetails.QuotedSqlName => null;

        int? ISqlBuilderEntryDetails.Scale => null;

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale.</value>
        /// <remarks>This will be null if the data source doesn't support detailed parameter metadata or if this value isn't applicable to the data type.</remarks>
        public int? Scale { get; private set; }

        string ISqlBuilderEntryDetails.SqlName => null;

        /// <summary>
        /// Gets the name of the SQL variable.
        /// </summary>
        /// <value>The name of the SQL variable.</value>
        public string SqlVariableName { get; }
    }
}
