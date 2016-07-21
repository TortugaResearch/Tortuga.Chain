using System;
using System.Threading.Tasks;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.DataSources
{
    /// <summary>
    /// Interface IDataSource is used to expose a data source to appenders. 
    /// </summary>
    public interface IDataSource
    {
        /// <summary>
        /// Gets or sets a value indicating whether strict mode is enabled.
        /// </summary>
        /// <remarks>Strict mode requires all properties that don't represent columns to be marked with the NotMapped attribute.</remarks>
        bool StrictMode { get; }

        /// <summary>
        /// Gets the extension data.
        /// </summary>
        /// <typeparam name="TTKey">The type of extension data desired.</typeparam>
        /// <returns>T.</returns>
        /// <remarks>Chain extensions can use this to store data source specific data. The key should be a data type defined by the extension.</remarks>
        TTKey GetExtensionData<TTKey>()
            where TTKey : new();

        /// <summary>
        /// Gets the name of the data source.
        /// </summary>
        /// <value>
        /// The name of the data source.
        /// </value>
        string Name { get; }


        /// <summary>
        /// Gets or sets the user value to use with audit rules.
        /// </summary>
        /// <value>
        /// The user value.
        /// </value>
        object UserValue { get; }

        /// <summary>
        /// Gets or sets the audit rules.
        /// </summary>
        /// <value>
        /// The audit rules.
        /// </value>
        AuditRuleCollection AuditRules { get; }

        /// <summary>
        /// Tests the connection.
        /// </summary>
        void TestConnection();

        /// <summary>
        /// Tests the connection asynchronously.
        /// </summary>
        /// <returns></returns>
        Task TestConnectionAsync();

        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        /// <value>
        /// The database metadata.
        /// </value>
        IDatabaseMetadataCache DatabaseMetadata { get; }

        /// <summary>
        /// Gets or sets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
        /// </summary>
        ICacheAdapter Cache { get; }


        /// <summary>
        /// Raised when a executionDetails is canceled.
        /// </summary>
        /// <remarks>This is not used for timeouts.</remarks>
        event EventHandler<ExecutionEventArgs> ExecutionCanceled;

        /// <summary>
        /// Raised when a procedure call fails.
        /// </summary>
        event EventHandler<ExecutionEventArgs> ExecutionError;

        /// <summary>
        /// Raised when a procedure call is successfully completed
        /// </summary>
        event EventHandler<ExecutionEventArgs> ExecutionFinished;

        /// <summary>
        /// Raised when a procedure call is started
        /// </summary>
        event EventHandler<ExecutionEventArgs> ExecutionStarted;

    }
}
