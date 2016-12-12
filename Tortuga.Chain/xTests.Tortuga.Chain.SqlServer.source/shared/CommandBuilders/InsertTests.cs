using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Models;
using Xunit;
using Xunit.Abstractions;


namespace Tests.CommandBuilders
{
    public class InsertTests : TestBase
    {
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);

        public InsertTests(ITestOutputHelper output) : base(output)
        {
        }


        [Theory, MemberData("Prime")]
        public void Insert(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var lookupKey = Guid.NewGuid().ToString();
                for (var i = 0; i < 10; i++)
                    dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).Execute();

                var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
                Assert.Equal(10, allKeys.Count, "Count if inserted rows is off.");


            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("Prime")]
        public void InsertEchoObject(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var list = new List<Employee>();
                var lookupKey = Guid.NewGuid().ToString();
                for (var i = 0; i < 10; i++)
                    list.Add(dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute());

                var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
                Assert.Equal(list.Count, allKeys.Count, "Count if inserted rows is off.");


            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("Prime")]
        public void InsertEchoNewKey(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var list = new List<int>();
                var lookupKey = Guid.NewGuid().ToString();
                for (var i = 0; i < 10; i++)
                    list.Add(dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToInt32().Execute());

                var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
                Assert.Equal(list.Count, allKeys.Count, "Count if inserted rows is off.");


            }
            finally
            {
                Release(dataSource);
            }

        }


#if SQL_SERVER || OLE_SQL_SERVER
        [Theory, MemberData("Prime")]
        public void Insert_Trigger(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var lookupKey = Guid.NewGuid().ToString();
                for (var i = 0; i < 10; i++)
                    dataSource.Insert(EmployeeTableName_Trigger, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).Execute();

                var allKeys = dataSource.From(EmployeeTableName_Trigger, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
                Assert.Equal(10, allKeys.Count, "Count if inserted rows is off.");


            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("Prime")]
        public void InsertEchoObject_Trigger(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var list = new List<Employee>();
                var lookupKey = Guid.NewGuid().ToString();
                for (var i = 0; i < 10; i++)
                    list.Add(dataSource.Insert(EmployeeTableName_Trigger, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute());

                var allKeys = dataSource.From(EmployeeTableName_Trigger, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
                Assert.Equal(list.Count, allKeys.Count, "Count if inserted rows is off.");


            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("Prime")]
        public void InsertEchoNewKey_Trigger(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var list = new List<int>();
                var lookupKey = Guid.NewGuid().ToString();
                for (var i = 0; i < 10; i++)
                    list.Add(dataSource.Insert(EmployeeTableName_Trigger, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToInt32().Execute());

                var allKeys = dataSource.From(EmployeeTableName_Trigger, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
                Assert.Equal(list.Count, allKeys.Count, "Count if inserted rows is off.");


            }
            finally
            {
                Release(dataSource);
            }

        }
#endif

    }
}



