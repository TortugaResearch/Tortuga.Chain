using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

#nullable disable

namespace Tortuga.Chain.Core.Tests
{
	public partial class GenericDbDataSource3_MaterializerTests
	{
		static readonly GenericDbDataSource s_DataSource;

		static GenericDbDataSource3_MaterializerTests()
		{
			var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json").Build();
			var connectionString = configuration.GetConnectionString("SqlServerTestDatabase");

			s_DataSource = new GenericDbDataSource(SqlClientFactory.Instance, "SqlServerTestDatabase", connectionString);
		}

		public static GenericDbDataSource DataSource
		{
			get { return s_DataSource; }
		}

		//async Task ListWithNullsTest<TResult>(string columnName, Type materializerType)
		//{
		//	var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 3, 4)");
		//	ILink<List<TResult>> materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None });
		//	var result1 = materializer1.Execute();
		//	var result1a = await materializer1.ExecuteAsync();
		//	Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
		//}
	}
}
