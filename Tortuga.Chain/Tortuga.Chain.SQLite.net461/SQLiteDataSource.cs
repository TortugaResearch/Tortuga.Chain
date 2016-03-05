using System;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SQLite;

namespace Tortuga.Chain
{
    /// <summary>
    /// Class that represets a SQLite Datasource.
    /// </summary>
    public class SQLiteDataSource : SQLiteDataSourceBase, IClass1DataSource
    {
        private readonly SQLiteConnectionStringBuilder m_ConnectionBuilder;
        private readonly SQLiteMetadataCache m_DatabaseMetadata;
        private readonly ReaderWriterLockSlim m_SyncLock = new ReaderWriterLockSlim(); //Sqlite is single-threaded for writes. It says otherwise, but it spams the trace window with excpetions.

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDataSource" /> class.
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="connectionString"></param>
        public SQLiteDataSource(string connectionName, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string is null or emtpy.", "connectionString");

            m_ConnectionBuilder = new SQLiteConnectionStringBuilder(connectionString);
            if (string.IsNullOrEmpty(connectionName))
                Name = m_ConnectionBuilder.DataSource;
            else
                Name = connectionName;

            m_DatabaseMetadata = new SQLiteMetadataCache(m_ConnectionBuilder);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDataSource" /> class.
        /// </summary>
        /// <param name="connectionString"></param>
        public SQLiteDataSource(string connectionString)
            : this(null, connectionString)
        {
        }

        /// <summary>
        /// This object can be used to lookup database information.
        /// </summary>
        public override SQLiteMetadataCache DatabaseMetadata
        {
            get { return m_DatabaseMetadata; }
        }

        /// <summary>
        /// This object can be used to access the database connection string.
        /// </summary>
        internal string ConnectionString
        {
            get { return m_ConnectionBuilder.ConnectionString; }
        }

        /// <summary>
        /// Creates and opens a new SQLite connection
        /// </summary>
        /// <returns></returns>
        /// <remarks>The caller of this method is responsible for closing the connection.</remarks>
        public SQLiteConnection CreateSQLiteConnection()
        {
            var con = new SQLiteConnection(ConnectionString);
            con.Open();

            //TODO: Research any potential PRAGMA/Rollback options

            return con;
        }

        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken"></param>
        /// <param name="implementation"></param>
        /// <param name="state"></param>
        protected override void Execute(ExecutionToken<SQLiteCommand, SQLiteParameter> executionToken, Func<SQLiteCommand, int?> implementation, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var mode = DisableLocks ? LockType.None : (executionToken as SQLiteExecutionToken)?.LockType ?? LockType.Write;

            try
            {
                switch (mode)
                {
                    case LockType.Read: m_SyncLock.EnterReadLock(); break;
                    case LockType.Write: m_SyncLock.EnterWriteLock(); break;
                }

                using (var con = CreateSQLiteConnection())
                {
                    using (var cmd = new SQLiteCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = executionToken.CommandText;
                        //TODO: add potential check for this type.
                        cmd.CommandType = executionToken.CommandType;
                        foreach (var param in executionToken.Parameters)
                            cmd.Parameters.Add(param);

                        var rows = implementation(cmd);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                ex.Data["DataSource"] = Name;
                ex.Data["Operation"] = executionToken.OperationName;
                ex.Data["CommandText"] = executionToken.CommandText;
                ex.Data["Parameters"] = executionToken.Parameters;
                throw;
            }
            finally
            {
                switch (mode)
                {
                    case LockType.Read: m_SyncLock.ExitReadLock(); break;
                    case LockType.Write: m_SyncLock.ExitWriteLock(); break;
                }
            }
        }

        /// <summary>
        /// Executes the specified operation asynchronously.
        /// </summary>
        /// <param name="executionToken"></param>
        /// <param name="implementation"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(ExecutionToken<SQLiteCommand, SQLiteParameter> executionToken, Func<SQLiteCommand, Task<int?>> implementation, CancellationToken cancellationToken, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var mode = DisableLocks ? LockType.None : (executionToken as SQLiteExecutionToken)?.LockType ?? LockType.Write;

            try
            {
                switch (mode)
                {
                    case LockType.Read: m_SyncLock.EnterReadLock(); break;
                    case LockType.Write: m_SyncLock.EnterWriteLock(); break;
                }

                using (var con = await CreateSqlConnectionAsync(cancellationToken).ConfigureAwait(false))
                {
                    using (var cmd = new SQLiteCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = executionToken.CommandText;
                        cmd.CommandType = executionToken.CommandType;
                        foreach (var param in executionToken.Parameters)
                            cmd.Parameters.Add(param);

                        var rows = await implementation(cmd).ConfigureAwait(false);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                if (cancellationToken.IsCancellationRequested) //convert SQLiteException into a OperationCanceledException 
                {
                    var ex2 = new OperationCanceledException("Operation was canceled.", ex, cancellationToken);
                    ex2.Data["DataSource"] = Name;
                    ex2.Data["Operation"] = executionToken.OperationName;
                    ex2.Data["CommandText"] = executionToken.CommandText;
                    ex2.Data["Parameters"] = executionToken.Parameters;
                    throw ex2;
                }
                else
                {
                    ex.Data["DataSource"] = Name;
                    ex.Data["Operation"] = executionToken.OperationName;
                    ex.Data["CommandText"] = executionToken.CommandText;
                    ex.Data["Parameters"] = executionToken.Parameters;
                    throw;
                }
            }
            finally
            {
                switch (mode)
                {
                    case LockType.Read: m_SyncLock.ExitReadLock(); break;
                    case LockType.Write: m_SyncLock.ExitWriteLock(); break;
                }
            }
        }

        private async Task<SQLiteConnection> CreateSqlConnectionAsync(CancellationToken cancellationToken)
        {
            var con = new SQLiteConnection(ConnectionString);
            await con.OpenAsync(cancellationToken).ConfigureAwait(false);

            //TODO: Add in needed PRAGMA statements

            return con;
        }

        ISingleRowDbCommandBuilder IClass1DataSource.Insert(string tableName, object argumentValue)
        {
            return Insert(tableName, argumentValue);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.Update(string tableName, object argumentValue, UpdateOptions options)
        {
            throw new NotImplementedException();
        }

        IDbCommandBuilder IClass1DataSource.Delete(string tableName, object argumentValue, DeleteOptions options)
        {
            return Delete(tableName, argumentValue, options);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.From(string tableOrViewName)
        {
            return From(tableOrViewName);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.From(string tableOrViewName, string whereClause)
        {
            return From(tableOrViewName, whereClause);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.From(string tableOrViewName, string whereClause, object argumentValue)
        {
            return From(tableOrViewName, whereClause, argumentValue);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.From(string tableOrViewName, object filterValue)
        {
            return From(tableOrViewName, filterValue);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.InsertOrUpdate(string tableName, object argumentValue, InsertOrUpdateOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Normally we use a reader/writer lock to avoid simutaneous writes to a SQlite database. If you disable this locking, you may see extra noise in your tracing output or unexcepted exceptions.
        /// </summary>
        public bool DisableLocks { get; set; }

        IDatabaseMetadataCache IClass1DataSource.DatabaseMetadata
        {
            get { return m_DatabaseMetadata; }
        }
    }
}
