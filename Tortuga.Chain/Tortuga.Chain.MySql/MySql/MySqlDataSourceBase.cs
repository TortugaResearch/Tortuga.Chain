using MySqlConnector;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.MySql
{
	/// <summary>
	/// Class MySqlDataSourceBase.
	/// </summary>
	public abstract partial class MySqlDataSourceBase : DataSource<MySqlConnection, MySqlTransaction, MySqlCommand, MySqlParameter>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MySqlDataSourceBase"/> class.
		/// </summary>
		/// <param name="settings">Optional settings object.</param>
		protected MySqlDataSourceBase(DataSourceSettings? settings) : base(settings)
		{
		}

		/// <summary>
		/// Gets the database metadata.
		/// </summary>
		public abstract new MySqlMetadataCache DatabaseMetadata { get; }


	}
}
