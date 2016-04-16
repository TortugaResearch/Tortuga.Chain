namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// This interface is used to allow SqlBuilder to be used with stored procs, TVFs, and other non-table objects.
    /// </summary>
    /// <typeparam name="TDbType">The type of the database type.</typeparam>
    internal interface ISqlBuilderEntryDetails<TDbType> : ISqlBuilderEntryDetails where TDbType : struct
    {
        TDbType? DbType { get; }
    }

    /// <summary>
    /// This interface is used to allow SqlBuilder to be used with stored procs, TVFs, and other non-table objects.
    /// </summary>
    internal interface ISqlBuilderEntryDetails
    {
        string ClrName { get; }
        bool IsIdentity { get; }
        string QuotedSqlName { get; }
        string SqlName { get; }
        string SqlVariableName { get; }
    }
}