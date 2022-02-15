using MySqlConnector;
using System.Collections.Concurrent;
using Tortuga.Chain.MySql;

namespace Tortuga.Chain
{
	/// <summary>
	/// Class SqlServerExtensions.
	/// </summary>
	public static class MySqlExtensions
	{
		readonly static ConcurrentDictionary<string, MySqlDataSource> s_CachedDataSources = new ConcurrentDictionary<string, MySqlDataSource>();

		/// <summary>
		/// Returns a data source wrapped around the connection.
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <returns>SqlServerOpenDataSource.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static MySqlOpenDataSource AsDataSource(this MySqlConnection connection)
		{
			if (connection == null)
				throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");
			if (connection.State == ConnectionState.Closed)
				connection.Open();

			var dataSourceBase = s_CachedDataSources.GetOrAdd(connection.ConnectionString, cs => new MySqlDataSource(cs));
			return new MySqlOpenDataSource(dataSourceBase, connection, null);
		}

		/// <summary>
		/// Returns a data source wrapped around the transaction.
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <param name="transaction">The transaction.</param>
		/// <returns>SqlServerOpenDataSource.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static MySqlOpenDataSource AsDataSource(this MySqlConnection connection, MySqlTransaction transaction)
		{
			if (connection == null)
				throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");
			if (connection.State == ConnectionState.Closed)
				connection.Open();

			var dataSourceBase = s_CachedDataSources.GetOrAdd(connection.ConnectionString, cs => new MySqlDataSource(cs));
			return new MySqlOpenDataSource(dataSourceBase, connection, transaction);
		}
	}
}
