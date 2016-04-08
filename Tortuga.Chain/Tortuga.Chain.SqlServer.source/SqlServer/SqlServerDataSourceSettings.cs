using System.Diagnostics.CodeAnalysis;


namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// This allows overriding connection options.
    /// </summary>
    public class SqlServerDataSourceSettings : DataSourceSettings
    {


        /// <summary>
        /// Rolls back a transaction if a Transact-SQL statement raises a run-time error.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xact")]
        public bool? XactAbort { get; set; }

        /// <summary>
        /// Terminates a query when an overflow or divide-by-zero error occurs during query execution.
        /// </summary>
        /// <remarks>Microsoft recommends setting ArithAbort=On for all connections. To avoid an additional round-trip to the server, do this at the server level instead of at the connection level.</remarks>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Arith")]
        public bool? ArithAbort { get; set; }
    }
}
