using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.MySql;

/// <summary>
/// Class MySqlDataSourceSettings.
/// </summary>
/// <seealso cref="DataSourceSettings" />
public class MySqlDataSourceSettings : DataSourceSettings
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MySqlDataSourceSettings"/> class.
	/// </summary>
	public MySqlDataSourceSettings() { }

	internal MySqlDataSourceSettings(MySqlDataSource dataSource, bool forwardEvents = false)
	{
		if (dataSource == null)
			throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");

		DefaultCommandTimeout = dataSource.DefaultCommandTimeout;
		StrictMode = dataSource.StrictMode;
		SequentialAccessMode = dataSource.SequentialAccessMode;
		SuppressGlobalEvents = dataSource.SuppressGlobalEvents || forwardEvents;
	}
}
