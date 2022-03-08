using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.PostgreSql
{
	/// <summary>
	/// Class PostgreSqlDataSourceSettings.
	/// </summary>
	/// <seealso cref="DataSourceSettings" />
	public class PostgreSqlDataSourceSettings : DataSourceSettings
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PostgreSqlDataSourceSettings"/> class.
		/// </summary>
		public PostgreSqlDataSourceSettings()
		{
		}

		internal PostgreSqlDataSourceSettings(PostgreSqlDataSource dataSource, bool forwardEvents = false)
		{
			if (dataSource == null)
				throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");

			DefaultCommandTimeout = dataSource.DefaultCommandTimeout;
			StrictMode = dataSource.StrictMode;
			SequentialAccessMode = dataSource.SequentialAccessMode;
			SuppressGlobalEvents = dataSource.SuppressGlobalEvents || forwardEvents;
		}
	}
}
