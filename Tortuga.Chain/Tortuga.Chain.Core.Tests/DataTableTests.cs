#nullable disable

namespace Tortuga.Chain.Core.Tests;

[TestClass]
public class DataTableTests : GenericDbDataSource3_MaterializerTests
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
		var result1 = materializer.Execute();
		var result1a = await materializer.ExecuteAsync();
	}

	[TestMethod]
	public async Task DataTableSet()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new { State = "CA" }).ToDataSet("Customers", "Orders");
		var result1 = materializer.Execute();
		var result1a = await materializer.ExecuteAsync();
	}

	[TestMethod]
	public async Task DataTableSet_Dict()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new Dictionary<string, object>() { { "State", "CA" } }).ToDataSet("Customers", "Orders");
		var result1 = materializer.Execute();
		var result1a = await materializer.ExecuteAsync();
	}

	[TestMethod]
	public async Task DataTableSet_Dict2()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new Dictionary<string, object>() { { "@State", "CA" } }).ToDataSet("Customers", "Orders");
		var result1 = materializer.Execute();
		var result1a = await materializer.ExecuteAsync();
	}

	[TestMethod]
	public async Task Table()
	{
		var materializer = DataSource.Sql($"SELECT * FROM dbo.AllTypes").ToTable();
		var result1 = materializer.Execute();
		var result1a = await materializer.ExecuteAsync();
	}

	[TestMethod]
	public async Task TableSet()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new { State = "CA" }).ToTableSet();
		var result1 = materializer.Execute();
		var result1a = await materializer.ExecuteAsync();
	}

	[TestMethod]
	public async Task TableSet_Dict()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new Dictionary<string, object>() { { "State", "CA" } }).ToTableSet();
		var result1 = materializer.Execute();
		var result1a = await materializer.ExecuteAsync();
	}

	[TestMethod]
	public async Task TableSet_Dict2()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new Dictionary<string, object>() { { "@State", "CA" } }).ToTableSet();
		var result1 = materializer.Execute();
		var result1a = await materializer.ExecuteAsync();
	}

	[TestMethod]
	public async Task TableSet2()
	{
		var materializer = DataSource.Sql(CustomerWithOrdersByState, new { State = "CA" }).ToTableSet("Customers", "Orders");
		var result1 = materializer.Execute();
		var result1a = await materializer.ExecuteAsync();
	}
}
