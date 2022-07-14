using Tortuga.Chain.DataSources;

namespace Tests.Core;

[TestClass]
public class DataSourceTest : TestBase
{
	[DataTestMethod, BasicData(DataSourceGroup.All)]
	public void TestConnection(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			dataSource.TestConnection();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.All)]
	public async Task TestConnectionAsync(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			await dataSource.TestConnectionAsync();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void Transaction_Dispose(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			dataSource.TestConnection();
			var trans = ((IRootDataSource)dataSource).BeginTransaction();
			trans.Dispose();
		}
		finally
		{
			Release(dataSource);
		}
	}

#if NET6_0_OR_GREATER

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public async Task Transaction_DisposeAsync_Using(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			dataSource.TestConnection();
			await using var trans = await ((IRootDataSource)dataSource).BeginTransactionAsync();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public async Task Transaction_DisposeAsync(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			dataSource.TestConnection();
			var trans = await ((IRootDataSource)dataSource).BeginTransactionAsync();
			await trans.DisposeAsync().ConfigureAwait(false);
		}
		finally
		{
			Release(dataSource);
		}
	}
#endif
}