using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SQLite;
using Tortuga.Chain.SQLite;

namespace Tortuga.Chain
{
    /// <summary>
    /// Class SqlServerExtensions.
    /// </summary>
    public static class SQLiteExtensions
    {
        readonly static ConcurrentDictionary<string, SQLiteDataSource> s_CachedDataSources = new ConcurrentDictionary<string, SQLiteDataSource>();

        /// <summary>
        /// Returns a data source wrapped around the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>SqlServerOpenDataSource.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SQLiteOpenDataSource AsDataSource(this SQLiteConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var dataSourceBase = s_CachedDataSources.GetOrAdd(connection.ConnectionString, cs => new SQLiteDataSource(cs));
            return new SQLiteOpenDataSource(dataSourceBase, connection, null);
        }

        /// <summary>
        /// Returns a data source wrapped around the transaction.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>SqlServerOpenDataSource.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SQLiteOpenDataSource AsDataSource(this SQLiteConnection connection, SQLiteTransaction transaction)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var dataSourceBase = s_CachedDataSources.GetOrAdd(connection.ConnectionString, cs => new SQLiteDataSource(cs));
            return new SQLiteOpenDataSource(dataSourceBase, connection, transaction);
        }
    }
}
