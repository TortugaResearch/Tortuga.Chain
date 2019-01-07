using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Tortuga.Chain;

namespace PerformanceTest
{
    internal class Program
    {
        private static SqlServerDataSource DataSource;

        public static void Main()
        {
            //ReflectionTest();

            DatabaseTest();

            //Issue213();

            //MySqlMetadata();

            //MyCompiledTestFailure();
        }

        private static void MyCompiledTestFailure()
        {
            var dataSource = MySqlDataSource.CreateFromConfig("MySqlTestDatabase");

            dataSource.TestConnection();

            var sql = "INSERT INTO hr.employee (`FirstName`, `LastName`, `ManagerKey`, `MiddleName`, `Title`, `UpdatedDate`) VALUES (@FirstName, @LastName, @ManagerKey, @MiddleName, @Title, @UpdatedDate);SELECT `CreatedDate`, `EmployeeKey`, `FirstName`, `LastName`, `ManagerKey`, `MiddleName`, `Title`, `UpdatedDate` FROM `hr`.`employee` WHERE `EmployeeKey` = LAST_INSERT_ID();";

            var con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlTestDatabase"].ConnectionString);
            con.Open();

            var cmd = new MySqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@FirstName", "Test");
            cmd.Parameters.AddWithValue("@LastName", "Employee636768087033711525");
            cmd.Parameters.AddWithValue("@ManagerKey", DBNull.Value);
            cmd.Parameters.AddWithValue("@MiddleName", DBNull.Value);
            cmd.Parameters.AddWithValue("@Title", "Mail Room");
            cmd.Parameters.AddWithValue("@UpdatedDate", DBNull.Value);

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var result = Load(reader);
            }
        }

        private static Tests.Models.ChangeTrackingEmployee Load(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            var result = new Tests.Models.ChangeTrackingEmployee();
            if (reader.IsDBNull(0))
                result.CreatedDate = null;
            else
                result.CreatedDate = reader.GetDateTime(0);
            if (reader.IsDBNull(1))
                result.EmployeeKey = null;
            else
                result.EmployeeKey = (System.Nullable<System.Int64>)reader.GetValue(1);
            if (reader.IsDBNull(2))
                result.FirstName = null;
            else
                result.FirstName = reader.GetString(2);
            if (reader.IsDBNull(3))
                result.LastName = null;
            else
                result.LastName = reader.GetString(3);
            if (reader.IsDBNull(4))
                result.ManagerKey = null;
            else
                result.ManagerKey = reader.GetInt32(4);
            if (reader.IsDBNull(5))
                result.MiddleName = null;
            else
                result.MiddleName = reader.GetString(5);
            if (reader.IsDBNull(6))
                result.Title = null;
            else
                result.Title = reader.GetString(6);
            if (reader.IsDBNull(7))
                result.UpdatedDate = null;
            else
                result.UpdatedDate = reader.GetDateTime(7);
            ((System.ComponentModel.IChangeTracking)result).AcceptChanges();
            return result;
        }

        private static void MySqlMetadata()
        {
            var dataSource = MySqlDataSource.CreateFromConfig("MySqlTestDatabase");

            dataSource.TestConnection();

            /*
            var table1 = dataSource.DatabaseMetadata.GetTableOrView("Film");
            Console.WriteLine($"{table1.Name} Columns {table1.Columns.Count}");
            Console.WriteLine();

            dataSource.TestConnection();
            dataSource.DatabaseMetadata.PreloadTables();

            foreach (var item in dataSource.DatabaseMetadata.GetTablesAndViews())
            {
                var x = item.IsTable ? "TABLE" : "VIEW";
                Console.WriteLine($"{item.Name} {x} Columns {item.Columns.Count}");
            }

            Console.WriteLine();

            dataSource.DatabaseMetadata.PreloadStoredProcedures();

            foreach (var item in dataSource.DatabaseMetadata.GetStoredProcedures())
            {
                Console.WriteLine($"{item.Name} Parameters {item.Parameters.Count}");
            }
            Console.WriteLine();

            dataSource.DatabaseMetadata.PreloadScalarFunctions();

            foreach (var item in dataSource.DatabaseMetadata.GetScalarFunctions())
            {
                Console.WriteLine($"{item.Name} Parameters {item.Parameters.Count}");
            }
            Console.WriteLine();
            */
        }

        //private static void ReflectionTest()
        //{
        //    const int runCount = 1000 * 1000;// * 1000;

        //    var propertyInfo = typeof(SalesOrderHeader).GetProperty("SalesOrderId");
        //    var reflectionSetter = propertyInfo.GetSetMethod();
        //    var delegateSetter = (Action<SalesOrderHeader, int>)Delegate.CreateDelegate(typeof(Action<SalesOrderHeader, int>), reflectionSetter);

        //    {
        //        var startTime = DateTime.Now;
        //        for (var i = 0; i < runCount; i++)
        //        {
        //            var foo = (Action<SalesOrderHeader, int>)Delegate.CreateDelegate(typeof(Action<SalesOrderHeader, int>), reflectionSetter);

        //        }
        //        var endTime = DateTime.Now;
        //        Console.WriteLine(((endTime - startTime).TotalMilliseconds / runCount / 1000.0000).ToString("#,##0.000 sec"));

        //    }
        //    //var fieldInfo = typeof(SalesOrderHeader).GetField("MaxValue");

        //    //var dynamicMethod = new DynamicMethod(String.Empty, typeof(void), new Type[] { fieldInfo.ReflectedType.MakeByRefType(), fieldInfo.FieldType }, true);
        //    //var ilGenerator = dynamicMethod.GetILGenerator();
        //    //ilGenerator.Emit(OpCodes.Ldarg_0);
        //    //ilGenerator.Emit(OpCodes.Ldarg_1);
        //    //ilGenerator.Emit(OpCodes.Stfld, fieldInfo);
        //    //ilGenerator.Emit(OpCodes.Ret);
        //    //var fieldSetter = (Action<SalesOrderHeader, int>)dynamicMethod.CreateDelegate(typeof(Action<SalesOrderHeader, int>));

        //    var fields = new SalesOrderHeader();

        //    {
        //        var startTime = DateTime.Now;
        //        for (var i = 0; i < runCount; i++)
        //        {
        //            reflectionSetter.Invoke(fields, new Object[] { i });
        //        }
        //        var endTime = DateTime.Now;
        //        Console.WriteLine("Reflection:\t\t" + ((endTime - startTime).TotalMilliseconds / 1000.0000).ToString("#,##0.000 sec"));

        //    }
        //    Thread.Sleep(TimeSpan.FromSeconds(2));
        //    {
        //        var startTime = DateTime.Now;
        //        for (var i = 0; i < runCount; i++)
        //        {
        //            delegateSetter.Invoke(fields, i);
        //        }
        //        var endTime = DateTime.Now;
        //        Console.WriteLine("Delegate:\t\t" + ((endTime - startTime).TotalMilliseconds / 1000.0000).ToString("#,##0.000 sec"));

        //    }
        //    Thread.Sleep(TimeSpan.FromSeconds(2));
        //    {
        //        var startTime = DateTime.Now;
        //        for (var i = 0; i < runCount; i++)
        //        {
        //            object value = i;
        //            reflectionSetter.Invoke(fields, new Object[] { value });
        //        }
        //        var endTime = DateTime.Now;
        //        Console.WriteLine("Reflection casting:\t\t" + ((endTime - startTime).TotalMilliseconds / 1000.0000).ToString("#,##0.000 sec"));

        //    }

        //    Thread.Sleep(TimeSpan.FromSeconds(2));
        //    {
        //        var startTime = DateTime.Now;
        //        for (var i = 0; i < runCount; i++)
        //        {
        //            object value = i;
        //            delegateSetter.Invoke(fields, (dynamic)value);
        //        }
        //        var endTime = DateTime.Now;
        //        Console.WriteLine("Delegate dynamic case:\t\t" + ((endTime - startTime).TotalMilliseconds / 1000.0000).ToString("#,##0.000 sec"));

        //    }
        //    Thread.Sleep(TimeSpan.FromSeconds(2));
        //    {
        //        var startTime = DateTime.Now;
        //        for (var i = 0; i < runCount; i++)
        //        {
        //            object value = i;
        //            delegateSetter.Invoke(fields, (int)value);
        //        }
        //        var endTime = DateTime.Now;
        //        Console.WriteLine("Delegate explicit:\t\t" + ((endTime - startTime).TotalMilliseconds / 1000.00).ToString("#,##0.000 sec"));

        //    }
        //    //fieldSetter(ref fields, 90);
        //    //Console.WriteLine(fields.MaxValue);
        //}

        private static void DatabaseTest()
        {
            DataSource = new SqlServerDataSource("data source=.;initial catalog=AdventureWorks2017;integrated security=True");
            var set = FetchSet();
            var record = FetchSet().First();
            //Console.Write(record.AccountNumber);
            //Console.Write(record.BillToAddressID);
            //Console.Write(record.Comment);
            //Console.Write(record.CreditCardApprovalCode);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            TimeSpan totalRunTime = default;

            const int runCount = 25;
            for (var i = 0; i < runCount; i++)
            {
                var startTime = DateTime.Now;
                Console.Write("Fetching set.....");
                FetchSet();
                var endTime = DateTime.Now;
                Console.WriteLine(((endTime - startTime).TotalMilliseconds / 1000.00).ToString("#,##0.000 sec"));
                totalRunTime = totalRunTime.Add(endTime - startTime);
            }
            Console.WriteLine("Average run time: " + ((totalRunTime.TotalMilliseconds / runCount) / 1000.00).ToString("#,##0.000 sec"));
        }

        ///// <summary>
        ///// Fetches the individual element
        ///// </summary>
        ///// <param name="key">The key of the element to fetch.</param>
        ///// <returns>The fetched element, or null if not found</returns>
        //public static SalesOrderHeader FetchIndividual(int key)
        //{
        //    return DataSource.From("[Sales].[SalesOrderHeader]", new { SalesOrderId = key }).ToObject<SalesOrderHeader>().Execute();
        //}

        /// <summary>
        /// Fetches the complete set of elements and returns this set as an IEnumerable.
        /// </summary>
        /// <returns>the set fetched</returns>
        public static IEnumerable<SalesOrderHeader> FetchSet()
        {
            return DataSource.From("[Sales].[SalesOrderHeader]").Compile().ToCollection<SalesOrderHeader>().Execute();
        }

        //static SqlServerDataSource DataSource;

        //static void Issue213()
        //{
        //    DataSource = new SqlServerDataSource("data source=.;initial catalog=Test;integrated security=True");

        //    //DataSource.Sql("Truncate Table dbo.Items").Execute();

        //    //var list = new List<Item>();
        //    //for (var i = 0; i < 100; i++)
        //    //    for (var j = 0; j < 100; j++)
        //    //    {
        //    //        list.Add(new Item()
        //    //        {
        //    //            ItemNbr = "G" + i.ToString("000"),
        //    //            CategoryID = "J" + j.ToString("000"),
        //    //            ItemDesc = "G" + i.ToString("000") + "-" + "J" + j.ToString("000"),
        //    //            Price = i + j * 0.01M
        //    //        });

        //    //    }
        //    //var sw = Stopwatch.StartNew();
        //    //DataSource.InsertBulk("dbo.Items", list).Execute();
        //    //sw.Stop();
        //    //Console.WriteLine(sw.ElapsedMilliseconds.ToString("N0"));

        //    var single2 = DataSource.From<Item>(new { ItemDesc = "G000-J064" }).ToObject<Item>(RowOptions.DiscardExtraRows).Execute();
        //    var list2 = DataSource.From<Item>(new { ItemDesc = "G000-J064" }).ToCollection<Item>().Execute();

        //    var searchFilter = DataSource.From<Item>().ToString("CategoryID").Execute(); //grab any value

        //    var single1 = DataSource.From<Item>(new { CategoryID = searchFilter }).ToObject<Item>(RowOptions.DiscardExtraRows).Execute();
        //    var list1 = DataSource.From<Item>(new { CategoryID = searchFilter }).ToCollection<Item>().Execute();

        //}
    }
}