using System;
using System.Linq;
using Tests.Models;
using Tortuga.Chain;
using Xunit;
using Xunit.Abstractions;


namespace Tests.CommandBuilders
{
    public class DeleteTests : TestBase
    {
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);

        public DeleteTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, MemberData("Prime")]
        public void DeleteTests_Delete(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var original = new Employee()
                {
                    FirstName = "Test",
                    LastName = "Employee" + DateTime.Now.Ticks,
                    Title = "Mail Room"
                };

                var key = dataSource.Insert(EmployeeTableName, original).ToInt32().Execute();
                var inserted = dataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();

                dataSource.Delete(EmployeeTableName, inserted).Execute();
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public void DeleteTests_Delete_Attribute(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var original = new Employee()
                {
                    FirstName = "Test",
                    LastName = "Employee" + DateTime.Now.Ticks,
                    Title = "Mail Room"
                };

                var key = dataSource.Insert(EmployeeTableName, original).ToInt32().Execute();
                var inserted = dataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();

                dataSource.Delete(inserted).Execute();

                try
                {
                    dataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();
                    Assert.Fail("Expected a missing data exception");
                }
                catch (MissingDataException) { }
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public void DeleteTests_Delete_Implied(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {

                var original = new Employee()
                {
                    FirstName = "Test",
                    LastName = "Employee" + DateTime.Now.Ticks,
                    Title = "Mail Room"
                };

                var key = dataSource.Insert(EmployeeTableName, original).ToInt32().Execute();

                dataSource.Delete(new HR.Employee() { EmployeeKey = key }).Execute();

                try
                {
                    dataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();
                    Assert.Fail("Excpected a missing data exception");
                }
                catch (MissingDataException) { }
            }
            finally
            {
                Release(dataSource);
            }

        }

#if SQL_SERVER

        [Theory, MemberData("Prime")]
        public void DeleteByKey(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var lookupKey = Guid.NewGuid().ToString();
                for (var i = 0; i < 10; i++)
                    dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

                var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
                var keyToUpdate = allKeys.First();

                dataSource.DeleteByKey(EmployeeTableName, keyToUpdate).Execute();

                var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
                Assert.Equal(9, allRows.Count, "The wrong number of rows remain");
            }
            finally
            {
                Release(dataSource);
            }

        }


        [Theory, MemberData("Prime")]
        public void DeleteByKeyList(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var lookupKey = Guid.NewGuid().ToString();
                for (var i = 0; i < 10; i++)
                    dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

                var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
                var keysToUpdate = allKeys.Take(5).ToList();

                var updatedRows = dataSource.DeleteByKeyList(EmployeeTableName, keysToUpdate).ToCollection<Employee>().Execute();

                Assert.Equal(5, updatedRows.Count, "The wrong number of rows were deleted");

                var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
                Assert.Equal(5, allRows.Count, "The wrong number of rows remain");


            }
            finally
            {
                Release(dataSource);
            }

        }


#endif


    }


    namespace HR
    {
        public class Employee
        {
            public int EmployeeKey { get; set; }
        }
    }
}
