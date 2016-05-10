using System;
using Tests.Models;
using Tortuga.Chain;
using Xunit;
using Xunit.Abstractions;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.CommandBuilders
{
    public class DeleteTests : TestBase
    {
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);

        public DeleteTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [MemberData("Prime")]
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

        [Theory]
        [MemberData("Prime")]
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

        [Theory]
        [MemberData("Prime")]
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


    }


    namespace HR
    {
        public class Employee
        {
            public int EmployeeKey { get; set; }
        }
    }
}
