using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain.SqlServer
{
    partial class SqlServerTransactionalDataSource : IDisposable, ITransactionalDataSource
#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
    partial class OleDbSqlServerTransactionalDataSource : IDisposable, ITransactionalDataSource

#elif SQLITE

namespace Tortuga.Chain.SQLite
{
    partial class SQLiteTransactionalDataSource : IDisposable, ITransactionalDataSource

#elif MYSQL

namespace Tortuga.Chain.MySql
{
    partial class MySqlTransactionalDataSource : IDisposable, ITransactionalDataSource

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
    partial class PostgreSqlTransactionalDataSource : IDisposable, ITransactionalDataSource

#elif ACCESS

namespace Tortuga.Chain.Access
{
    partial class AccessTransactionalDataSource : IDisposable, ITransactionalDataSource

#endif
    {
        [SuppressMessage("Design", "CA1033")]
        DbConnection IOpenDataSource.AssociatedConnection => m_Connection;

        [SuppressMessage("Design", "CA1033")]
        DbTransaction? IOpenDataSource.AssociatedTransaction => m_Transaction;

        [SuppressMessage("Design", "CA1033")]
        void IOpenDataSource.Close()
        {
            Dispose();
        }

        [SuppressMessage("Design", "CA1033")]
        bool IOpenDataSource.TryCommit()
        {
            Commit();
            return true;
        }

        /// <summary>
        /// The extension cache is used by extensions to store data source specific information.
        /// </summary>
        /// <value>
        /// The extension cache.
        /// </value>
        protected override ConcurrentDictionary<Type, object> ExtensionCache
        {
            get { return m_BaseDataSource.m_ExtensionCache; }
        }

        /// <summary>
        /// Commits the transaction and disposes the underlying connection.
        /// </summary>
        public void Commit()
        {
            if (m_Disposed)
                throw new ObjectDisposedException("Transaction is disposed");

            m_Transaction.Commit();
            Dispose(true);
        }

        /// <summary>
        /// Closes the current transaction and connection. If not committed, the transaction is rolled back.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the extension data.
        /// </summary>
        /// <typeparam name="TTKey">The type of extension data desired.</typeparam>
        /// <returns>T.</returns>
        /// <remarks>Chain extensions can use this to store data source specific data. The key should be a data type defined by the extension.
        /// Transactional data sources should override this method and return the value held by their parent data source.</remarks>
        public override TTKey GetExtensionData<TTKey>()
        {
            return m_BaseDataSource.GetExtensionData<TTKey>();
        }

        /// <summary>
        /// Rolls back the transaction and disposes the underlying connection.
        /// </summary>
        public void Rollback()
        {
            if (m_Disposed)
                throw new ObjectDisposedException("Transaction is disposed");

            m_Transaction.Rollback();
            Dispose(true);
        }

        /// <summary>
        /// Gets or sets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
        /// </summary>
        public override ICacheAdapter Cache
        {
            get { return m_BaseDataSource.Cache; }
        }

        /// <summary>
        /// Closes the current transaction and connection. If not committed, the transaction is rolled back.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (m_Disposed)
                return;

            if (disposing)
            {
                m_Transaction.Dispose();
                m_Connection.Dispose();
                AdditionalDispose();
                m_Disposed = true;
            }
        }

        partial void AdditionalDispose();
    }
}