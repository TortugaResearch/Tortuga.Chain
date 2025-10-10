using Tests.Models;
using Tortuga.Chain.Metadata;

namespace Tests.Core;

[TestClass]
public class IndexTests : TestBase
{
#if SQL_SERVER_MDS || ACCESS || SQLITE || POSTGRESQL || MYSQL

	[DataTestMethod, TableData(DataSourceGroup.All)]
	public void TableIndexes(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
			var indexes = table.GetIndexes();
			Assert.IsTrue(indexes.Where(i => i.IsPrimaryKey).Count() <= 1, "No more than one primary key");

#if SQL_SERVER_MDS
			if (table.Columns.Any(c => c.IsPrimaryKey))
				Assert.IsTrue(indexes.Where(i => i.IsPrimaryKey).Count() == 1, "A column is marked as primary, so there should be a primary index.");
#endif
			foreach (var index in indexes)
			{
				if (index.IndexType != IndexType.Heap)
				{
					Assert.IsFalse(string.IsNullOrWhiteSpace(index.Name), $"Indexes should have names. Table name {table.Name}");
					Assert.IsTrue(index.Columns.Count > 0, $"Indexes should have columns. Table name {table.Name} Index name {index.Name}");
				}
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

#if SQL_SERVER_MDS

	[DataTestMethod, ViewData(DataSourceGroup.All)]
	public void ViewIndexes(string dataSourceName, DataSourceType mode, string viewName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(viewName);
			var indexes = table.GetIndexes();
			Assert.IsTrue(indexes.Where(i => i.IsPrimaryKey).Count() <= 1, "No more than one primary key");

			if (table.Columns.Any(c => c.IsPrimaryKey))
				Assert.IsTrue(indexes.Where(i => i.IsPrimaryKey).Count() <= 1, "A column is marked as primary, so there should be a primary index.");

			foreach (var index in indexes)
			{
				//Assert.IsTrue(index.Columns.Count > 0, "Indexes should have columns");
				Assert.IsFalse(string.IsNullOrWhiteSpace(index.Name), "indexes should have names");
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

#if SQL_SERVER_MDS || POSTGRESQL

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void DisableIndexes(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			dataSource.DisableIndexes<Employee>().Execute();
			dataSource.EnableIndexes<Employee>().Execute();

			var table = dataSource.DatabaseMetadata.GetTableOrViewFromClass<Employee>();
			dataSource.DisableIndexes(table.Name).Execute();
			dataSource.EnableIndexes(table.Name).Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}
