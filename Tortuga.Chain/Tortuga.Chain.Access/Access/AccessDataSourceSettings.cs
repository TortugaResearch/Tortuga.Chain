using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Access
{
	/// <summary>
	/// This class is used to modify settings that are not represented by the connection string.
	/// </summary>
	/// <seealso cref="DataSourceSettings" />
	public class AccessDataSourceSettings : DataSourceSettings
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccessDataSourceSettings"/> class.
		/// </summary>
		public AccessDataSourceSettings() { }

		internal AccessDataSourceSettings(AccessDataSource dataSource, bool forwardEvents)
		{
			if (dataSource == null)
				throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");

			DefaultCommandTimeout = dataSource.DefaultCommandTimeout;
			StrictMode = dataSource.StrictMode;
			SuppressGlobalEvents = dataSource.SuppressGlobalEvents || forwardEvents;
		}
	}
}
