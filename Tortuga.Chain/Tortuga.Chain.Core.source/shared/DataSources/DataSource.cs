using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using Tortuga.Chain.Core;
using Tortuga.Chain.AuditRules;
using System.Threading.Tasks;
using Tortuga.Chain.Metadata;

#if !WINDOWS_UWP
using System.Runtime.Caching;
#endif

namespace Tortuga.Chain.DataSources
{
    /// <summary>
    /// Class DataSource.
    /// </summary>
    public abstract class DataSource : IDataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSource"/> class.
        /// </summary>
        /// <param name="settings">Optional settings object.</param>
        protected DataSource(DataSourceSettings settings)
        {
            if (settings != null)
            {
                DefaultCommandTimeout = settings.DefaultCommandTimeout;
                StrictMode = settings.StrictMode ?? false;
                SuppressGlobalEvents = settings.SuppressGlobalEvents ?? false;
                m_Cache = settings.Cache;
            }

#if !WINDOWS_UWP
            if (m_Cache == null)
                m_Cache = new ObjectCacheAdapter(MemoryCache.Default);
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSource"/> class, copying its cache and extension data.
        /// </summary>
        /// <param name="parent">The parent data source.</param>
        /// <param name="settings">The settings.</param>
        protected DataSource(DataSource parent, DataSourceSettings settings)
        {
            if (settings != null)
            {
                DefaultCommandTimeout = settings.DefaultCommandTimeout;
                StrictMode = settings.StrictMode ?? false;
                SuppressGlobalEvents = settings.SuppressGlobalEvents ?? false;
                m_Cache = settings.Cache ?? parent.Cache;
            }
            else
            {
                m_Cache = parent.Cache;
            }

            m_ExtensionCache = parent.m_ExtensionCache;
        }

        /// <summary>
        /// Raised when a executionDetails is canceled.
        /// </summary>
        /// <remarks>This is not used for timeouts.</remarks>
        public event EventHandler<ExecutionEventArgs> ExecutionCanceled;

        /// <summary>
        /// Raised when a procedure call fails.
        /// </summary>
        public event EventHandler<ExecutionEventArgs> ExecutionError;

        /// <summary>
        /// Raised when a procedure call is successfully completed
        /// </summary>
        public event EventHandler<ExecutionEventArgs> ExecutionFinished;

        /// <summary>
        /// Raised when a procedure call is started
        /// </summary>
        public event EventHandler<ExecutionEventArgs> ExecutionStarted;

        /// <summary>
        /// Raised when a executionDetails is canceled in any dispatcher.
        /// </summary>
        /// <remarks>This is not used for timeouts.</remarks>
        public static event EventHandler<ExecutionEventArgs> GlobalExecutionCanceled;

        /// <summary>
        /// Raised when a procedure call fails in any dispatcher.
        /// </summary>
        public static event EventHandler<ExecutionEventArgs> GlobalExecutionError;

        /// <summary>
        /// Raised when a procedure call is successfully completed in any dispatcher
        /// </summary>
        public static event EventHandler<ExecutionEventArgs> GlobalExecutionFinished;

        /// <summary>
        /// Raised when a procedure call is started in any dispatcher
        /// </summary>
        public static event EventHandler<ExecutionEventArgs> GlobalExecutionStarted;

        /// <summary>
        /// Data sources can use this to indicate an executionDetails was canceled.
        /// </summary>
        /// <param name="executionDetails">The executionDetails.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <remarks>This is not used for timeouts.</remarks>
        protected void OnExecutionCanceled(ExecutionToken executionDetails, DateTimeOffset startTime, DateTimeOffset endTime, object state)
        {
            if (executionDetails == null)
                throw new ArgumentNullException(nameof(executionDetails), "executionDetails is null.");

            ExecutionCanceled?.Invoke(this, new ExecutionEventArgs(executionDetails, startTime, endTime, state));
            if (!SuppressGlobalEvents)
                GlobalExecutionCanceled?.Invoke(this, new ExecutionEventArgs(executionDetails, startTime, endTime, state));
        }

        /// <summary>
        /// Data sources can use this to indicate an error has occurred.
        /// </summary>
        /// <param name="executionDetails">The executionDetails.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="error">The error.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        protected void OnExecutionError(ExecutionToken executionDetails, DateTimeOffset startTime, DateTimeOffset endTime, Exception error, object state)
        {
            if (executionDetails == null)
                throw new ArgumentNullException(nameof(executionDetails), "executionDetails is null.");
            if (error == null)
                throw new ArgumentNullException(nameof(error), "error is null.");


            ExecutionError?.Invoke(this, new ExecutionEventArgs(executionDetails, startTime, endTime, error, state));
            if (!SuppressGlobalEvents)
                GlobalExecutionError?.Invoke(this, new ExecutionEventArgs(executionDetails, startTime, endTime, error, state));
        }

        /// <summary>
        /// Data sources can use this to indicate an executionDetails has finished.
        /// </summary>
        /// <param name="executionDetails">The executionDetails.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="rowsAffected">The number of rows affected.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        protected void OnExecutionFinished(ExecutionToken executionDetails, DateTimeOffset startTime, DateTimeOffset endTime, int? rowsAffected, object state)
        {
            if (executionDetails == null)
                throw new ArgumentNullException(nameof(executionDetails), "executionDetails is null.");

            ExecutionFinished?.Invoke(this, new ExecutionEventArgs(executionDetails, startTime, endTime, rowsAffected, state));
            if (!SuppressGlobalEvents)
                GlobalExecutionFinished?.Invoke(this, new ExecutionEventArgs(executionDetails, startTime, endTime, rowsAffected, state));
        }

        /// <summary>
        /// Data sources can use this to indicate an executionDetails has begun.
        /// </summary>
        /// <param name="executionDetails">The executionDetails.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        protected void OnExecutionStarted(ExecutionToken executionDetails, DateTimeOffset startTime, object state)
        {
            if (executionDetails == null)
                throw new ArgumentNullException(nameof(executionDetails), "executionDetails is null.");


            ExecutionStarted?.Invoke(this, new ExecutionEventArgs(executionDetails, startTime, state));
            if (!SuppressGlobalEvents)
                GlobalExecutionStarted?.Invoke(this, new ExecutionEventArgs(executionDetails, startTime, state));
        }

        /// <summary>
        /// Gets the extension data.
        /// </summary>
        /// <typeparam name="TTKey">The type of extension data desired.</typeparam>
        /// <returns>T.</returns>
        /// <remarks>Chain extensions can use this to store data source specific data. The key should be a data type defined by the extension.
        /// 
        /// Transactional data sources should override this method and return the value held by their parent data source.</remarks>
        public virtual TTKey GetExtensionData<TTKey>()
            where TTKey : new()
        {
            return (TTKey)ExtensionCache.GetOrAdd(typeof(TTKey), x => new TTKey());
        }

        protected

        /// <summary>
        /// Raises the <see cref="E:ExecutionCanceled" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ExecutionEventArgs"/> instance containing the event data.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnExecutionCanceled(ExecutionEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e), "e is null.");

            ExecutionCanceled?.Invoke(this, e);
            if (!SuppressGlobalEvents && GlobalExecutionCanceled != null)
                GlobalExecutionCanceled(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:ExecutionError" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ExecutionEventArgs"/> instance containing the event data.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnExecutionError(ExecutionEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e), "e is null.");

            ExecutionError?.Invoke(this, e);
            if (!SuppressGlobalEvents && GlobalExecutionError != null)
                GlobalExecutionError(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:ExecutionFinished" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ExecutionEventArgs"/> instance containing the event data.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnExecutionFinished(ExecutionEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e), "e is null.");

            ExecutionFinished?.Invoke(this, e);
            if (!SuppressGlobalEvents && GlobalExecutionFinished != null)
                GlobalExecutionFinished(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:ExecutionStarted" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ExecutionEventArgs"/> instance containing the event data.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnExecutionStarted(ExecutionEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e), "e is null.");

            ExecutionStarted?.Invoke(this, e);
            if (!SuppressGlobalEvents && GlobalExecutionStarted != null)
                GlobalExecutionStarted(this, e);
        }

        /// <summary>
        /// Gets or sets the default command timeout.
        /// </summary>
        /// <value>The default command timeout.</value>
        public TimeSpan? DefaultCommandTimeout { get; }

        /// <summary>
        /// Gets the name of the data source.
        /// </summary>
        /// <value>
        /// The name of the data source.
        /// </value>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether strict mode is enabled.
        /// </summary>
        /// <remarks>Strict mode requires all properties that don't represent columns to be marked with the NotMapped attribute.</remarks>
        public bool StrictMode { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to suppress global events.
        /// </summary>
        /// <value>If <c>true</c>, this data source will not honor global event handlers.</value>
        public bool SuppressGlobalEvents { get; }


        /// <summary>
        /// Gets or sets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
        /// </summary>
        public abstract ICacheAdapter Cache { get; }


        /// <summary>
        /// The extension cache is used by extensions to store data source specific information.
        /// </summary>
        /// <value>The extension cache.</value>
        protected abstract ConcurrentDictionary<Type, object> ExtensionCache { get; }

        /// <summary>
        /// Gets or sets the user value to use with audit rules.
        /// </summary>
        /// <value>
        /// The user value.
        /// </value>
        public object UserValue { get; protected set; }

        /// <summary>
        /// Gets or sets the audit rules.
        /// </summary>
        /// <value>
        /// The audit rules.
        /// </value>
        public AuditRuleCollection AuditRules { get; protected set; } = AuditRuleCollection.Empty;

        /// <summary>
        /// Tests the connection.
        /// </summary>
        public abstract void TestConnection();

        /// <summary>
        /// Tests the connection asynchronously.
        /// </summary>
        /// <returns></returns>
        public abstract Task TestConnectionAsync();

        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        /// <value>
        /// The database metadata.
        /// </value>
        /// <remarks>Data sources are expected to shadow this with their specific version.</remarks>
        public IDatabaseMetadataCache DatabaseMetadata { get { return OnGetDatabaseMetadata(); } }

        /// <summary>
        /// Called when Database.DatabaseMetadata is invoked.
        /// </summary>
        /// <returns></returns>
        protected abstract IDatabaseMetadataCache OnGetDatabaseMetadata();

    }
}
