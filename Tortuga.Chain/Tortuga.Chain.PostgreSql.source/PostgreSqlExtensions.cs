using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Data;
using Tortuga.Chain.PostgreSql;

namespace Tortuga.Chain
{
    /// <summary>
    /// Class SqlServerExtensions.
    /// </summary>
    public static class PostgreSqlExtensions
    {
        readonly static ConcurrentDictionary<string, PostgreSqlDataSource> s_CachedDataSources = new ConcurrentDictionary<string, PostgreSqlDataSource>();

        /// <summary>
        /// Returns a data source wrapped around the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>SqlServerOpenDataSource.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static PostgreSqlOpenDataSource AsDataSource(this NpgsqlConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var dataSourceBase = s_CachedDataSources.GetOrAdd(connection.ConnectionString, cs => new PostgreSqlDataSource(cs));
            return new PostgreSqlOpenDataSource(dataSourceBase, connection, null);
        }

        /// <summary>
        /// Returns a data source wrapped around the transaction.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>SqlServerOpenDataSource.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static PostgreSqlOpenDataSource AsDataSource(this NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var dataSourceBase = s_CachedDataSources.GetOrAdd(connection.ConnectionString, cs => new PostgreSqlDataSource(cs));
            return new PostgreSqlOpenDataSource(dataSourceBase, connection, transaction);
        }
    }
}
