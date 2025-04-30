using Tortuga.Chain;
using Tortuga.Chain.DataSources;

namespace Tests.Materializers;

[TestClass]
public class ToDataTableTests : TestBase
{
	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void ToDataRow(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = dataSource.From(tableName).WithLimits(1).WithSorting(table.GetDefaultSortOrder()).ToDataRowOrNull().Execute();
			if (result != null)
			{
				Assert.AreEqual(table.Columns.Count, result.Table.Columns.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public async Task ToDataRow_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = await dataSource.From(tableName).WithLimits(1).WithSorting(table.GetDefaultSortOrder()).ToDataRowOrNull().ExecuteAsync().ConfigureAwait(false);
			if (result != null)
			{
				Assert.AreEqual(table.Columns.Count, result.Table.Columns.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void ToDataTable(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = dataSource.From(tableName).WithLimits(10).WithSorting(table.GetDefaultSortOrder()).ToDataTable().Execute();
			Assert.IsTrue(result.Rows.Count <= 10);
			Assert.AreEqual(table.Columns.Count, result.Columns.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public async Task ToDataTable_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode).ConfigureAwait(false);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = await dataSource.From(tableName).WithLimits(10).WithSorting(table.GetDefaultSortOrder()).ToDataTable().ExecuteAsync().ConfigureAwait(false);
			Assert.IsTrue(result.Rows.Count <= 10);
			Assert.AreEqual(table.Columns.Count, result.Columns.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void ToRow(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = dataSource.From(tableName).WithLimits(1).WithSorting(table.GetDefaultSortOrder()).ToRowOrNull().Execute();
			if (result != null)
			{
				Assert.AreEqual(table.Columns.Count, result.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public async Task ToRow_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = await dataSource.From(tableName).WithLimits(1).WithSorting(table.GetDefaultSortOrder()).ToRowOrNull().ExecuteAsync().ConfigureAwait(false);
			if (result != null)
			{
				Assert.AreEqual(table.Columns.Count, result.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void ToTable(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = dataSource.From(tableName).WithLimits(10).WithSorting(table.GetDefaultSortOrder()).ToTable().Execute();
			Assert.IsTrue(result.Rows.Count <= 10);
			Assert.AreEqual(table.Columns.Count, result.ColumnNames.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public async Task ToTable_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = await dataSource.From(tableName).WithLimits(10).WithSorting(table.GetDefaultSortOrder()).ToTable().ExecuteAsync().ConfigureAwait(false);
			Assert.IsTrue(result.Rows.Count <= 10);
			Assert.AreEqual(table.Columns.Count, result.ColumnNames.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewLimitData(DataSourceGroup.AllNormalOnly)]
	public void ToTable_WithLimit(string dataSourceName, DataSourceType mode, string tableName, LimitOptions limitOptions)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var prep = ((ICrudDataSource)dataSource).From(tableName).WithLimits(10, limitOptions);
			switch (limitOptions)
			{
				case LimitOptions.RowsWithTies:
				case LimitOptions.PercentageWithTies:
					prep = prep.WithSorting(table.Columns[0].SqlName);
					break;
			}
			var result = prep.ToTable().Execute();
			//Assert.IsTrue(result.Rows.Count <= 10, $"Row count was {result.Rows.Count}");
			Assert.AreEqual(table.Columns.Count, result.ColumnNames.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewLimitData(DataSourceGroup.AllNormalOnly)]
	public async Task ToTable_WithLimit_Async(string dataSourceName, DataSourceType mode, string tableName, LimitOptions limitOptions)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var prep = ((ICrudDataSource)dataSource).From(tableName).WithLimits(10, limitOptions);
			switch (limitOptions)
			{
				case LimitOptions.RowsWithTies:
				case LimitOptions.PercentageWithTies:
					prep = prep.WithSorting(table.Columns[0].SqlName);
					break;
			}
			var result = await prep.ToTable().ExecuteAsync().ConfigureAwait(false);
			//Assert.IsTrue(result.Rows.Count <= 10, $"Row count was {result.Rows.Count}");
			Assert.AreEqual(table.Columns.Count, result.ColumnNames.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}
}
