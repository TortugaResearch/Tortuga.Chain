using Tests.Models;
using Tortuga.Chain.CommandBuilders;

#if SQL_SERVER_SDS || SQL_SERVER_MDS

using Tortuga.Chain.SqlServer;

#endif

namespace Tests.Aggregate;

[TestClass]
public class CountTests : TestBase
{
	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void Count(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		//WriteLine($"Table {tableName}");
		try
		{
			var count = dataSource.From(tableName).AsCount().Execute();
			Assert.IsTrue(count >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

#if COUNT64

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void Count64(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		//WriteLine($"Table {tableName}");
		try
		{
			var count = ((ISupportsCount64)dataSource.From(tableName)).AsCount64().Execute();
			Assert.IsTrue(count >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

#if SQL_SERVER_SDS || SQL_SERVER_MDS

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void AsCountDistinctApproximate_Auto(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		//WriteLine($"Table {tableName}");
		try
		{
			var count = dataSource.From<Employee>().AsCountDistinctApproximate().Execute();
			Assert.IsTrue(count >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public async Task Count_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode).ConfigureAwait(false);
		try
		{
			var count = await dataSource.From(tableName).AsCount().ExecuteAsync().ConfigureAwait(false);
			Assert.IsTrue(count >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

#if COUNT64

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public async Task Count64_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode).ConfigureAwait(false);
		try
		{
			var count = await ((ISupportsCount64)dataSource.From(tableName)).AsCount64().ExecuteAsync().ConfigureAwait(false);
			Assert.IsTrue(count >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

#if NO_DISTINCT_COUNT

    [DataTestMethod, TablesAndViewColumnsData(DataSourceGroup.All)]
    public void CountByColumn(string dataSourceName, DataSourceType mode, string tableName, string columnName)
    {
        var dataSource = DataSource(dataSourceName, mode);
        WriteLine($"Table {tableName}");
        try
        {
            var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
            if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image" || columnType == "geography" || columnType == "geometry")
                return; //SQL Server limitation

            var count = dataSource.From(tableName).AsCount(columnName).Execute();
            Assert.IsTrue(count >= 0, "Count cannot be less than zero");
        }
        finally
        {
            Release(dataSource);
        }
    }

#else

	[DataTestMethod, TablesAndViewColumnsData(DataSourceGroup.All)]
	public void CountByColumn(string dataSourceName, DataSourceType mode, string tableName, string columnName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		//WriteLine($"Table {tableName}");
		try
		{
			var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
			if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image" || columnType == "geography" || columnType == "geometry")
				return; //SQL Server limitation

			var count = dataSource.From(tableName).AsCount(columnName).Execute();
			var countDistinct = dataSource.From(tableName).AsCount(columnName, true).Execute();
			Assert.IsTrue(count >= 0, "Count cannot be less than zero");
			Assert.IsTrue(countDistinct <= count, "Count distinct cannot be greater than count");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

#if COUNT64

	[DataTestMethod, TablesAndViewColumnsData(DataSourceGroup.All)]
	public void Count64ByColumn(string dataSourceName, DataSourceType mode, string tableName, string columnName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		//WriteLine($"Table {tableName}");
		try
		{
			var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
			if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image" || columnType == "geography" || columnType == "geometry")
				return; //SQL Server limitation

			var count = ((ISupportsCount64)dataSource.From(tableName)).AsCount64(columnName).Execute();
			var countDistinct = ((ISupportsCount64)dataSource.From(tableName)).AsCount64(columnName, true).Execute();
			Assert.IsTrue(count >= 0, "Count cannot be less than zero");
			Assert.IsTrue(countDistinct <= count, "Count distinct cannot be greater than count");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewColumnsData(DataSourceGroup.All)]
	public async Task Count64ByColumn_Async(string dataSourceName, DataSourceType mode, string tableName, string columnName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode).ConfigureAwait(false);
		//WriteLine($"Table {tableName}");
		try
		{
			var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
			if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image" || columnType == "geography" || columnType == "geometry")
				return; //SQL Server limitation

			var count = await ((ISupportsCount64)dataSource.From(tableName)).AsCount64(columnName).ExecuteAsync().ConfigureAwait(false);
			var countDistinct = await ((ISupportsCount64)dataSource.From(tableName)).AsCount64(columnName, true).ExecuteAsync().ConfigureAwait(false);
			Assert.IsTrue(count >= 0, "Count cannot be less than zero");
			Assert.IsTrue(countDistinct <= count, "Count distinct cannot be greater than count");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

#if SQL_SERVER_SDS || SQL_SERVER_MDS

	[DataTestMethod, TablesAndViewColumnsData(DataSourceGroup.All)]
	public void CountByColumn_DistinctApproximate(string dataSourceName, DataSourceType mode, string tableName, string columnName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		//WriteLine($"Table {tableName}");
		try
		{
			var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
			if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image" || columnType == "geography" || columnType == "geometry")
				return; //SQL Server limitation

			var countDistinct = dataSource.From(tableName).AsCountDistinctApproximate(columnName).Execute();
			Assert.IsTrue(countDistinct >= 0, "Count cannot be less than zero");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

#if NO_DISTINCT_COUNT

    [DataTestMethod, TablesAndViewColumnsData(DataSourceGroup.All)]
    public async Task CountByColumn_Async(string dataSourceName, DataSourceType mode, string tableName, string columnName)
    {
        var dataSource = await DataSourceAsync(dataSourceName, mode).ConfigureAwait(false);
        //WriteLine($"Table {tableName}");
        try
        {
            var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
            if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image" || columnType == "geography" || columnType == "geometry")
                return; //SQL Server limitation

            var count = await dataSource.From(tableName).AsCount(columnName).ExecuteAsync().ConfigureAwait(false);
            Assert.IsTrue(count >= 0, "Count cannot be less than zero");
        }
        finally
        {
            Release(dataSource);
        }
    }

#else

	[DataTestMethod, TablesAndViewColumnsData(DataSourceGroup.All)]
	public async Task CountByColumn_Async(string dataSourceName, DataSourceType mode, string tableName, string columnName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode).ConfigureAwait(false);
		//WriteLine($"Table {tableName}");
		try
		{
			var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
			if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image" || columnType == "geography" || columnType == "geometry")
				return; //SQL Server limitation

			var count = await dataSource.From(tableName).AsCount(columnName).ExecuteAsync().ConfigureAwait(false);
			var countDistinct = await dataSource.From(tableName).AsCount(columnName, true).ExecuteAsync().ConfigureAwait(false);
			Assert.IsTrue(count >= 0, "Count cannot be less than zero");
			Assert.IsTrue(countDistinct <= count, "Count distinct cannot be greater than count");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void AsCount(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var key = Guid.NewGuid().ToString();

			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var count = dataSource.From(EmployeeTableName, new { Title = key }).AsCount().Execute();
			var columnCount = dataSource.From(EmployeeTableName, new { Title = key }).AsCount("Title").Execute();
			var columnCount2 = dataSource.From(EmployeeTableName, new { Title = key }).AsCount("MiddleName").Execute();

#if !ACCESS
			var distinctColumnCount = dataSource.From(EmployeeTableName, new { Title = key }).AsCount("Title", true).Execute();
			var distinctColumnCount2 = dataSource.From(EmployeeTableName, new { Title = key }).AsCount("LastName", true).Execute();
#endif

			Assert.AreEqual(10, count, "All of the rows");
			Assert.AreEqual(10, columnCount, "No nulls");
			Assert.AreEqual(5, columnCount2, "Half of the rows are nul");

#if !ACCESS
			Assert.AreEqual(1, distinctColumnCount, "Only one distinct value");
			Assert.AreEqual(10, distinctColumnCount2, "Every value is distinct");
#endif
		}
		finally
		{
			Release(dataSource);
		}
	}

#if !NO_DISTINCT_COUNT

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Counts(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).AsCount().Execute();
			var columnCount = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).AsCount("Title").Execute();
			var columnCount2 = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).AsCount("MiddleName").Execute();
			var distinctColumnCount = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).AsCount("Title", true).Execute();
			var distinctColumnCount2 = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).AsCount("LastName", true).Execute();

			Assert.AreEqual(1000, count, "All of the rows");
			Assert.AreEqual(1000, columnCount, "No nulls");
			Assert.AreEqual(500, columnCount2, "Half of the rows are nul");
			Assert.AreEqual(1, distinctColumnCount, "Only one distinct value");
			Assert.AreEqual(1000, distinctColumnCount2, "Every value is distinct");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}
