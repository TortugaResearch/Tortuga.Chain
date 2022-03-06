namespace Tests.CommandBuilders;

[TestClass]
public class FromTests_ToDynamic : TestBase
{

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void ToDynamicCollection(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = dataSource.From(tableName).WithLimits(10).ToDynamicCollection().Execute();
			Assert.IsTrue(result.Count <= 10);
			if (result.Count > 0)
			{
				var first = (IDictionary<string, object>)result.First();
				Assert.AreEqual(table.Columns.Count, first.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public async Task ToDynamicCollection_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = await dataSource.From(tableName).WithLimits(10).ToDynamicCollection().ExecuteAsync();
			Assert.IsTrue(result.Count <= 10);
			if (result.Count > 0)
			{
				var row = (IDictionary<string, object>)result.First();
				Assert.AreEqual(table.Columns.Count, row.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void ToDynamicObject(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = dataSource.From(tableName).WithLimits(1).ToDynamicObjectOrNull().Execute();
			if (result != null)
			{
				var row = (IDictionary<string, object>)result;
				Assert.AreEqual(table.Columns.Count, row.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public async Task ToDynamicObject_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = await dataSource.From(tableName).WithLimits(1).ToDynamicObjectOrNull().ExecuteAsync();
			if (result != null)
			{
				var row = (IDictionary<string, object>)result;
				Assert.AreEqual(table.Columns.Count, row.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}
}