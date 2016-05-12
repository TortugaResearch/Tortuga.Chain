namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// This interface is used to allow SqlBuilder to be used with stored procs, TVFs, and other non-table objects.
    /// </summary>
    /// <typeparam name="TDbType">The type of the database type.</typeparam>
    /// <remarks>For internal use only.</remarks>
    public interface ISqlBuilderEntryDetails<TDbType> : ISqlBuilderEntryDetails where TDbType : struct
    {
        /// <summary>
        /// Gets the database specific DbType
        /// </summary>
        /// <value>The type of the database.</value>
        TDbType? DbType { get; }
    }

    /// <summary>
    /// This interface is used to allow SqlBuilder to be used with stored procs, TVFs, and other non-table objects.
    /// </summary>
    /// <seealso cref="ISqlBuilderEntryDetails" />
    /// <remarks>For internal use only.</remarks>
    public interface ISqlBuilderEntryDetails
    {
        /// <summary>
        /// Gets the name of the color.
        /// </summary>
        /// <value>The name of the color.</value>
        string ClrName { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is identity.
        /// </summary>
        /// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
        bool IsIdentity { get; }
        /// <summary>
        /// Gets the name of the quoted SQL.
        /// </summary>
        /// <value>The name of the quoted SQL.</value>
        string QuotedSqlName { get; }
        /// <summary>
        /// Gets the name of the SQL.
        /// </summary>
        /// <value>The name of the SQL.</value>
        string SqlName { get; }
        /// <summary>
        /// Gets the name of the SQL variable.
        /// </summary>
        /// <value>The name of the SQL variable.</value>
        string SqlVariableName { get; }
    }
}