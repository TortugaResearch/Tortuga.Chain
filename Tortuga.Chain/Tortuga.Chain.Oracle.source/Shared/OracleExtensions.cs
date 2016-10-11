using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Concurrent;
using System.Data;
using Tortuga.Chain.Oracle;

namespace Tortuga.Chain
{
    /// <summary>
    /// Class SqlServerExtensions.
    /// </summary>
    public static class OracleExtensions
    {
        readonly static ConcurrentDictionary<string, OracleDataSource> s_CachedDataSources = new ConcurrentDictionary<string, OracleDataSource>();

        /// <summary>
        /// Returns a data source wrapped around the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>SqlServerOpenDataSource.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static OracleOpenDataSource AsDataSource(this OracleConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var dataSourceBase = s_CachedDataSources.GetOrAdd(connection.ConnectionString, cs => new OracleDataSource(cs));
            return new OracleOpenDataSource(dataSourceBase, connection, null);
        }

        /// <summary>
        /// Returns a data source wrapped around the transaction.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>SqlServerOpenDataSource.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static OracleOpenDataSource AsDataSource(this OracleConnection connection, OracleTransaction transaction)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var dataSourceBase = s_CachedDataSources.GetOrAdd(connection.ConnectionString, cs => new OracleDataSource(cs));
            return new OracleOpenDataSource(dataSourceBase, connection, transaction);
        }
    }
}
