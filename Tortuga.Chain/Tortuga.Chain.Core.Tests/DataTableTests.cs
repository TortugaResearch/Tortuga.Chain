#nullable disable

namespace Tortuga.Chain.Core.Tests;

[TestClass]
public class DataTableTests : MaterializerTestBase
{
	const string CustomerWithOrdersByState = @"SELECT  *
    FROM    Sales.Customer c
    WHERE   c.State = @State;

    SELECT  o.*
    FROM    Sales.[Order] o
            INNER JOIN Sales.Customer c ON o.CustomerKey = c.CustomerKey
    WHERE   c.State = @State;";

	[TestMethod]
	public async Task DataTable()
	{
		var materializer = DataSource.Sql($"SELECT * FROM dbo.AllTypes").ToDataTable();
		materializer.Execute();
		await materializer.ExecuteAsync().ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DataTableSet()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new { State = "CA" }).ToDataSet("Customers", "Orders");
		materializer.Execute();
		await materializer.ExecuteAsync().ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DataTableSet_Dict()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new Dictionary<string, object>() { { "State", "CA" } }).ToDataSet("Customers", "Orders");
		materializer.Execute();
		await materializer.ExecuteAsync().ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DataTableSet_Dict2()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new Dictionary<string, object>() { { "@State", "CA" } }).ToDataSet("Customers", "Orders");
		materializer.Execute();
		await materializer.ExecuteAsync().ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DataTableSet_NoNames()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new { State = "CA" }).ToDataSet();
		materializer.Execute();
		await materializer.ExecuteAsync().ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DataTableSet_TooManyNames()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new { State = "CA" }).ToDataSet("Customers", "Orders", "Extra");
		var ds1 = materializer.Execute();
		Assert.AreEqual(2, ds1.Tables.Count);
		var ds2 = await materializer.ExecuteAsync().ConfigureAwait(false);
		Assert.AreEqual(2, ds1.Tables.Count);
	}

	[TestMethod]
	public async Task Table()
	{
		var materializer = DataSource.Sql($"SELECT * FROM dbo.AllTypes").ToTable();
		materializer.Execute();
		await materializer.ExecuteAsync().ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TableSet()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new { State = "CA" }).ToTableSet();
		materializer.Execute();
		await materializer.ExecuteAsync().ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TableSet_Dict()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new Dictionary<string, object>() { { "State", "CA" } }).ToTableSet();
		materializer.Execute();
		await materializer.ExecuteAsync().ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TableSet_Dict2()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new Dictionary<string, object>() { { "@State", "CA" } }).ToTableSet();
		materializer.Execute();
		await materializer.ExecuteAsync().ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TableSet2()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new { State = "CA" }).ToTableSet("Customers", "Orders");
		materializer.Execute();
		await materializer.ExecuteAsync().ConfigureAwait(false);
	}
}
