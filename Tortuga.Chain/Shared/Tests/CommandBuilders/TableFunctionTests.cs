
#if SQL_SERVER_SDS || SQL_SERVER_MDS

using Tortuga.Chain.SqlServer;

#endif

#if SQL_SERVER_SDS || SQL_SERVER_MDS || POSTGRESQL || SQL_SERVER_OLEDB

namespace Tests.CommandBuilders;

[TestClass]
public class TableFunctionTests : TestBase
{
#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB
	static object Parameter1 = new { @State = "CA" };
	static object DictParameter1a = new Dictionary<string, object>() { { "State", "CA" } };
	static object DictParameter1b = new Dictionary<string, object>() { { "@State", "CA" } };
#elif POSTGRESQL
	static object Parameter1 = new { @param_state = "CA" };
	static object DictParameter1a = new Dictionary<string, object>() { { "param_state", "CA" } };
	static object DictParameter1b = new Dictionary<string, object>() { { "@param_state", "CA" } };
#endif

#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB
	//Only SQL Server has inline functions.

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TableFunction2_Object(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.TableFunction(TableFunction2Name, Parameter1).ToTable().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

#if SQL_SERVER_SDS || SQL_SERVER_MDS
	//Only SQL Server has inline functions.

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TableFunction2_ApproxCount(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = dataSource.TableFunction(TableFunction2Name, Parameter1).AsCountDistinctApproximate("CustomerKey").Execute();
			Assert.IsTrue(count >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TableFunction1_Object_Limit(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.TableFunction(TableFunction1Name, Parameter1).WithLimits(1).ToTable().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TableFunction1_Object_Filter(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.TableFunction(TableFunction1Name, Parameter1).WithFilter(new { @FullName = "Tom Jones" }).ToTable().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TableFunction1_Object_Sort(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.TableFunction(TableFunction1Name, Parameter1).WithSorting("FullName").ToTable().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TableFunction1_Object(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.TableFunction(TableFunction1Name, Parameter1).ToTable().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task TableFunction1_Object_Async(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = await dataSource.TableFunction(TableFunction1Name, Parameter1).ToTable().ExecuteAsync();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TableFunction1_Dictionary(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.TableFunction(TableFunction1Name, DictParameter1a).ToTable().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task TableFunction1_Dictionary_Async(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = await dataSource.TableFunction(TableFunction1Name, DictParameter1a).ToTable().ExecuteAsync();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TableFunction1_Dictionary2(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.TableFunction(TableFunction1Name, DictParameter1b).ToTable().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task TableFunction1_Dictionary2_Async(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = await dataSource.TableFunction(TableFunction1Name, DictParameter1b).ToTable().ExecuteAsync();
		}
		finally
		{
			Release(dataSource);
		}
	}
}

#endif
