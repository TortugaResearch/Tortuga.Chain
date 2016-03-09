using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor.Modeling;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// This allows overriding connection options.
    /// </summary>
    public class SqlServerSettings : ModelBase
    {
        internal SqlServerSettings() { }

        /// <summary>
        /// Rolls back a transaction if a Transact-SQL statement raises a run-time error.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xact")]
        public bool? XactAbort { get { return Get<bool?>(); } set { Set(value); } }

        /// <summary>
        /// Terminates a query when an overflow or divide-by-zero error occurs during query execution.
        /// </summary>
        /// <remarks>Microsoft recommends setting ArithAbort=On for all connections. To avoid an additional round-trip to the server, do this at the server level instead of at the connection level.</remarks>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Arith")]
        public bool? ArithAbort { get { return Get<bool?>(); } set { Set(value); } }
    }
}
