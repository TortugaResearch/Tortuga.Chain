using System.Collections.Concurrent;
using System.Collections.Immutable;
using Tests.Models;
using Tortuga.Chain;


namespace Tests.CommandBuilders;

[TestClass]
public class FromTests_ToDictionary : TestBase
{

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToDictionary_InferredObject(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
			var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup>("FirstName", DictionaryOptions.InferConstructor).Execute();

			Assert.AreEqual("1", test1["A"].LastName);
			Assert.AreEqual("2", test1["B"].LastName);
			Assert.AreEqual("3", test1["C"].LastName);
			Assert.AreEqual("4", test1["D"].LastName);

			var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName), DictionaryOptions.InferConstructor).Execute();

			Assert.AreEqual("A", test2[1].FirstName);
			Assert.AreEqual("B", test2[2].FirstName);
			Assert.AreEqual("C", test2[3].FirstName);
			Assert.AreEqual("D", test2[4].FirstName);

			var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup, ConcurrentDictionary<string, EmployeeLookup>>("FirstName", DictionaryOptions.InferConstructor).Execute();
			Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
			Assert.AreEqual("1", test3["A"].LastName);
			Assert.AreEqual("2", test3["B"].LastName);
			Assert.AreEqual("3", test3["C"].LastName);
			Assert.AreEqual("4", test3["D"].LastName);

			var test4 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup, ConcurrentDictionary<int, EmployeeLookup>>(e => int.Parse(e.LastName), DictionaryOptions.InferConstructor).Execute();
			Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
			Assert.AreEqual("A", test4[1].FirstName);
			Assert.AreEqual("B", test4[2].FirstName);
			Assert.AreEqual("C", test4[3].FirstName);
			Assert.AreEqual("D", test4[4].FirstName);

			var test5 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<string, EmployeeLookup>("FirstName", DictionaryOptions.InferConstructor).Execute();
			Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
			Assert.AreEqual("1", test5["A"].LastName);
			Assert.AreEqual("2", test5["B"].LastName);
			Assert.AreEqual("3", test5["C"].LastName);
			Assert.AreEqual("4", test5["D"].LastName);

			var test6 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName), DictionaryOptions.InferConstructor).Execute();
			Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
			Assert.AreEqual("A", test6[1].FirstName);
			Assert.AreEqual("B", test6[2].FirstName);
			Assert.AreEqual("C", test6[3].FirstName);
			Assert.AreEqual("D", test6[4].FirstName);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToDictionary_AutoTableSelection(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
			var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

			emp1 = dataSource.Insert(emp1).ToObject().Execute();
			emp2 = dataSource.Insert(emp2).ToObject().Execute();
			emp3 = dataSource.Insert(emp3).ToObject().Execute();
			emp4 = dataSource.Insert(emp4).ToObject().Execute();

			var test1 = dataSource.From<Employee>(new { Title = uniqueKey }).ToDictionary<string>("FirstName").Execute();

			Assert.AreEqual("1", test1["A"].LastName);
			Assert.AreEqual("2", test1["B"].LastName);
			Assert.AreEqual("3", test1["C"].LastName);
			Assert.AreEqual("4", test1["D"].LastName);

			var test2 = dataSource.From<Employee>(new { Title = uniqueKey }).ToDictionary(e => int.Parse(e.LastName)).Execute();

			Assert.AreEqual("A", test2[1].FirstName);
			Assert.AreEqual("B", test2[2].FirstName);
			Assert.AreEqual("C", test2[3].FirstName);
			Assert.AreEqual("D", test2[4].FirstName);

			var test3 = dataSource.From<Employee>(new { Title = uniqueKey }).ToDictionary<string, Employee, ConcurrentDictionary<string, Employee>>("FirstName").Execute();
			Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, Employee>));
			Assert.AreEqual("1", test3["A"].LastName);
			Assert.AreEqual("2", test3["B"].LastName);
			Assert.AreEqual("3", test3["C"].LastName);
			Assert.AreEqual("4", test3["D"].LastName);

			var test4 = dataSource.From<Employee>(new { Title = uniqueKey }).ToDictionary<int, Employee, ConcurrentDictionary<int, Employee>>(e => int.Parse(e.LastName)).Execute();
			Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, Employee>));
			Assert.AreEqual("A", test4[1].FirstName);
			Assert.AreEqual("B", test4[2].FirstName);
			Assert.AreEqual("C", test4[3].FirstName);
			Assert.AreEqual("D", test4[4].FirstName);

			var test5 = dataSource.From<Employee>(new { Title = uniqueKey }).ToImmutableDictionary<string>("FirstName").Execute();
			Assert.IsInstanceOfType(test5, typeof(ImmutableDictionary<string, Employee>));
			Assert.AreEqual("1", test5["A"].LastName);
			Assert.AreEqual("2", test5["B"].LastName);
			Assert.AreEqual("3", test5["C"].LastName);
			Assert.AreEqual("4", test5["D"].LastName);

			var test6 = dataSource.From<Employee>(new { Title = uniqueKey }).ToImmutableDictionary(e => int.Parse(e.LastName)).Execute();
			Assert.IsInstanceOfType(test6, typeof(ImmutableDictionary<int, Employee>));
			Assert.AreEqual("A", test6[1].FirstName);
			Assert.AreEqual("B", test6[2].FirstName);
			Assert.AreEqual("C", test6[3].FirstName);
			Assert.AreEqual("D", test6[4].FirstName);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToDictionary(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
			var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, Employee>("FirstName").Execute();

			Assert.AreEqual("1", test1["A"].LastName);
			Assert.AreEqual("2", test1["B"].LastName);
			Assert.AreEqual("3", test1["C"].LastName);
			Assert.AreEqual("4", test1["D"].LastName);

			var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, Employee>(e => int.Parse(e.LastName)).Execute();

			Assert.AreEqual("A", test2[1].FirstName);
			Assert.AreEqual("B", test2[2].FirstName);
			Assert.AreEqual("C", test2[3].FirstName);
			Assert.AreEqual("D", test2[4].FirstName);

			var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, Employee, ConcurrentDictionary<string, Employee>>("FirstName").Execute();
			Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, Employee>));
			Assert.AreEqual("1", test3["A"].LastName);
			Assert.AreEqual("2", test3["B"].LastName);
			Assert.AreEqual("3", test3["C"].LastName);
			Assert.AreEqual("4", test3["D"].LastName);

			var test4 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, Employee, ConcurrentDictionary<int, Employee>>(e => int.Parse(e.LastName)).Execute();
			Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, Employee>));
			Assert.AreEqual("A", test4[1].FirstName);
			Assert.AreEqual("B", test4[2].FirstName);
			Assert.AreEqual("C", test4[3].FirstName);
			Assert.AreEqual("D", test4[4].FirstName);

			var test5 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<string, Employee>("FirstName").Execute();
			Assert.IsInstanceOfType(test5, typeof(ImmutableDictionary<string, Employee>));
			Assert.AreEqual("1", test5["A"].LastName);
			Assert.AreEqual("2", test5["B"].LastName);
			Assert.AreEqual("3", test5["C"].LastName);
			Assert.AreEqual("4", test5["D"].LastName);

			var test6 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<int, Employee>(e => int.Parse(e.LastName)).Execute();
			Assert.IsInstanceOfType(test6, typeof(ImmutableDictionary<int, Employee>));
			Assert.AreEqual("A", test6[1].FirstName);
			Assert.AreEqual("B", test6[2].FirstName);
			Assert.AreEqual("C", test6[3].FirstName);
			Assert.AreEqual("D", test6[4].FirstName);
		}
		finally
		{
			Release(dataSource);
		}
	}

#if SQLITE

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToDictionary_ImmutableObject(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
			var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

			dataSource.Insert(EmployeeTableName, emp1).WithRefresh().Execute();
			dataSource.Insert(EmployeeTableName, emp2).WithRefresh().Execute();
			dataSource.Insert(EmployeeTableName, emp3).WithRefresh().Execute();
			dataSource.Insert(EmployeeTableName, emp4).WithRefresh().Execute();

			var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup>("FirstName").WithConstructor<long, string, string>().Execute();

			Assert.AreEqual("1", test1["A"].LastName);
			Assert.AreEqual("2", test1["B"].LastName);
			Assert.AreEqual("3", test1["C"].LastName);
			Assert.AreEqual("4", test1["D"].LastName);

			var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName)).WithConstructor<long, string, string>().Execute();

			Assert.AreEqual("A", test2[1].FirstName);
			Assert.AreEqual("B", test2[2].FirstName);
			Assert.AreEqual("C", test2[3].FirstName);
			Assert.AreEqual("D", test2[4].FirstName);

			var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup, ConcurrentDictionary<string, EmployeeLookup>>("FirstName").WithConstructor<long, string, string>().Execute();
			Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
			Assert.AreEqual("1", test3["A"].LastName);
			Assert.AreEqual("2", test3["B"].LastName);
			Assert.AreEqual("3", test3["C"].LastName);
			Assert.AreEqual("4", test3["D"].LastName);

			var test4 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup, ConcurrentDictionary<int, EmployeeLookup>>(e => int.Parse(e.LastName)).WithConstructor<long, string, string>().Execute();
			Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
			Assert.AreEqual("A", test4[1].FirstName);
			Assert.AreEqual("B", test4[2].FirstName);
			Assert.AreEqual("C", test4[3].FirstName);
			Assert.AreEqual("D", test4[4].FirstName);

			var test5 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<string, EmployeeLookup>("FirstName").WithConstructor<long, string, string>().Execute();
			Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
			Assert.AreEqual("1", test5["A"].LastName);
			Assert.AreEqual("2", test5["B"].LastName);
			Assert.AreEqual("3", test5["C"].LastName);
			Assert.AreEqual("4", test5["D"].LastName);

			var test6 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName)).WithConstructor<long, string, string>().Execute();
			Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
			Assert.AreEqual("A", test6[1].FirstName);
			Assert.AreEqual("B", test6[2].FirstName);
			Assert.AreEqual("C", test6[3].FirstName);
			Assert.AreEqual("D", test6[4].FirstName);
		}
		finally
		{
			Release(dataSource);
		}
	}

#elif MYSQL

		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void ToDictionary_ImmutableObject(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var uniqueKey = Guid.NewGuid().ToString();

				var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
				var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
				var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
				var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

				dataSource.Insert(EmployeeTableName, emp1).WithRefresh().Execute();
				dataSource.Insert(EmployeeTableName, emp2).WithRefresh().Execute();
				dataSource.Insert(EmployeeTableName, emp3).WithRefresh().Execute();
				dataSource.Insert(EmployeeTableName, emp4).WithRefresh().Execute();

				var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup>("FirstName").WithConstructor<ulong, string, string>().Execute();

				Assert.AreEqual("1", test1["A"].LastName);
				Assert.AreEqual("2", test1["B"].LastName);
				Assert.AreEqual("3", test1["C"].LastName);
				Assert.AreEqual("4", test1["D"].LastName);

				var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName)).WithConstructor<ulong, string, string>().Execute();

				Assert.AreEqual("A", test2[1].FirstName);
				Assert.AreEqual("B", test2[2].FirstName);
				Assert.AreEqual("C", test2[3].FirstName);
				Assert.AreEqual("D", test2[4].FirstName);

				var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup, ConcurrentDictionary<string, EmployeeLookup>>("FirstName").WithConstructor<ulong, string, string>().Execute();
				Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
				Assert.AreEqual("1", test3["A"].LastName);
				Assert.AreEqual("2", test3["B"].LastName);
				Assert.AreEqual("3", test3["C"].LastName);
				Assert.AreEqual("4", test3["D"].LastName);

				var test4 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup, ConcurrentDictionary<int, EmployeeLookup>>(e => int.Parse(e.LastName)).WithConstructor<ulong, string, string>().Execute();
				Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
				Assert.AreEqual("A", test4[1].FirstName);
				Assert.AreEqual("B", test4[2].FirstName);
				Assert.AreEqual("C", test4[3].FirstName);
				Assert.AreEqual("D", test4[4].FirstName);

				var test5 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<string, EmployeeLookup>("FirstName").WithConstructor<ulong, string, string>().Execute();
				Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
				Assert.AreEqual("1", test5["A"].LastName);
				Assert.AreEqual("2", test5["B"].LastName);
				Assert.AreEqual("3", test5["C"].LastName);
				Assert.AreEqual("4", test5["D"].LastName);

				var test6 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName)).WithConstructor<ulong, string, string>().Execute();
				Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
				Assert.AreEqual("A", test6[1].FirstName);
				Assert.AreEqual("B", test6[2].FirstName);
				Assert.AreEqual("C", test6[3].FirstName);
				Assert.AreEqual("D", test6[4].FirstName);
			}
			finally
			{
				Release(dataSource);
			}
		}

#else

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToDictionary_ImmutableObject(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
			var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

			dataSource.Insert(EmployeeTableName, emp1).WithRefresh().Execute();
			dataSource.Insert(EmployeeTableName, emp2).WithRefresh().Execute();
			dataSource.Insert(EmployeeTableName, emp3).WithRefresh().Execute();
			dataSource.Insert(EmployeeTableName, emp4).WithRefresh().Execute();

			var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup>("FirstName").WithConstructor<int, string, string>().Execute();

			Assert.AreEqual("1", test1["A"].LastName);
			Assert.AreEqual("2", test1["B"].LastName);
			Assert.AreEqual("3", test1["C"].LastName);
			Assert.AreEqual("4", test1["D"].LastName);

			var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName)).WithConstructor<int, string, string>().Execute();

			Assert.AreEqual("A", test2[1].FirstName);
			Assert.AreEqual("B", test2[2].FirstName);
			Assert.AreEqual("C", test2[3].FirstName);
			Assert.AreEqual("D", test2[4].FirstName);

			var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup, ConcurrentDictionary<string, EmployeeLookup>>("FirstName").WithConstructor<int, string, string>().Execute();
			Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
			Assert.AreEqual("1", test3["A"].LastName);
			Assert.AreEqual("2", test3["B"].LastName);
			Assert.AreEqual("3", test3["C"].LastName);
			Assert.AreEqual("4", test3["D"].LastName);

			var test4 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup, ConcurrentDictionary<int, EmployeeLookup>>(e => int.Parse(e.LastName)).WithConstructor<int, string, string>().Execute();
			Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
			Assert.AreEqual("A", test4[1].FirstName);
			Assert.AreEqual("B", test4[2].FirstName);
			Assert.AreEqual("C", test4[3].FirstName);
			Assert.AreEqual("D", test4[4].FirstName);

			var test5 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<string, EmployeeLookup>("FirstName").WithConstructor<int, string, string>().Execute();
			Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
			Assert.AreEqual("1", test5["A"].LastName);
			Assert.AreEqual("2", test5["B"].LastName);
			Assert.AreEqual("3", test5["C"].LastName);
			Assert.AreEqual("4", test5["D"].LastName);

			var test6 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName)).WithConstructor<int, string, string>().Execute();
			Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
			Assert.AreEqual("A", test6[1].FirstName);
			Assert.AreEqual("B", test6[2].FirstName);
			Assert.AreEqual("C", test6[3].FirstName);
			Assert.AreEqual("D", test6[4].FirstName);
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToDictionary_NoDefaultConstructor(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
			var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup>("FirstName").Execute();

			Assert.AreEqual("1", test1["A"].LastName);
			Assert.AreEqual("2", test1["B"].LastName);
			Assert.AreEqual("3", test1["C"].LastName);
			Assert.AreEqual("4", test1["D"].LastName);

			var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName)).Execute();

			Assert.AreEqual("A", test2[1].FirstName);
			Assert.AreEqual("B", test2[2].FirstName);
			Assert.AreEqual("C", test2[3].FirstName);
			Assert.AreEqual("D", test2[4].FirstName);

			var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup, ConcurrentDictionary<string, EmployeeLookup>>("FirstName").Execute();
			Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
			Assert.AreEqual("1", test3["A"].LastName);
			Assert.AreEqual("2", test3["B"].LastName);
			Assert.AreEqual("3", test3["C"].LastName);
			Assert.AreEqual("4", test3["D"].LastName);

			var test4 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup, ConcurrentDictionary<int, EmployeeLookup>>(e => int.Parse(e.LastName)).Execute();
			Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
			Assert.AreEqual("A", test4[1].FirstName);
			Assert.AreEqual("B", test4[2].FirstName);
			Assert.AreEqual("C", test4[3].FirstName);
			Assert.AreEqual("D", test4[4].FirstName);

		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToImmutableDictionary_NoDefaultConstructor(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
			var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			var test5 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<string, EmployeeLookup>("FirstName").Execute();
			Assert.AreEqual("1", test5["A"].LastName);
			Assert.AreEqual("2", test5["B"].LastName);
			Assert.AreEqual("3", test5["C"].LastName);
			Assert.AreEqual("4", test5["D"].LastName);

			var test6 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName)).Execute();
			Assert.AreEqual("A", test6[1].FirstName);
			Assert.AreEqual("B", test6[2].FirstName);
			Assert.AreEqual("C", test6[3].FirstName);
			Assert.AreEqual("D", test6[4].FirstName);
		}
		finally
		{
			Release(dataSource);
		}
	}

}
