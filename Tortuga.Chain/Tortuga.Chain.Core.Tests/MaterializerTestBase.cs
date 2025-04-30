using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace Tortuga.Chain.Core.Tests
{
	public partial class MaterializerTestBase
	{
		static readonly GenericDbDataSource s_DataSource;

		static MaterializerTestBase()
		{
			var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json").Build();
			var connectionString = configuration.GetConnectionString("SqlServerTestDatabase");

			s_DataSource = new GenericDbDataSource(SqlClientFactory.Instance, "SqlServerTestDatabase", connectionString);
		}

		public static GenericDbDataSource DataSource
		{
			get { return s_DataSource; }
		}
	}
}
