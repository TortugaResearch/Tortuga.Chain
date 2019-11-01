using System.Data.SQLite;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.SQLite
{

    /// <summary>
    /// Class SQLiteExecutionToken.
    /// </summary>
    public sealed class SQLiteOperationExecutionToken : OperationExecutionToken<SQLiteConnection, SQLiteTransaction>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteOperationExecutionToken"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="operationName">Name of the operation. This is used for logging.</param>
        /// <param name="lockType">Type of the lock.</param>
        public SQLiteOperationExecutionToken(IOperationDataSource<SQLiteConnection, SQLiteTransaction> dataSource, string operationName, LockType lockType = LockType.Write)
            : base(dataSource, operationName)
        {
            LockType = lockType;
        }

        /// <summary>
        /// Indicates whether or not the operation may perform writes.
        /// </summary>
        /// <value>The mode.</value>
        public LockType LockType { get; }

    }
}
