using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Tortuga.Chain;

namespace PerformanceTest
{
    class Program
    {

        [Table("Items", Schema = "dbo")]
        public class Item
        {
            public string ItemNbr { get; set; } = "";
            public string CategoryID { get; set; } = "";
            public string ItemDesc { get; set; } = "";
            public string ItemContents { get; set; } = "";
            public decimal? Price { get; set; } = 0.0M;
        }

        static SqlServerDataSource DataSource;

        static void Issue213()
        {
            DataSource = new SqlServerDataSource("data source=.;initial catalog=Test;integrated security=True");

            //DataSource.Sql("Truncate Table dbo.Items").Execute();

            //var list = new List<Item>();
            //for (var i = 0; i < 100; i++)
            //    for (var j = 0; j < 100; j++)
            //    {
            //        list.Add(new Item()
            //        {
            //            ItemNbr = "G" + i.ToString("000"),
            //            CategoryID = "J" + j.ToString("000"),
            //            ItemDesc = "G" + i.ToString("000") + "-" + "J" + j.ToString("000"),
            //            Price = i + j * 0.01M
            //        });

            //    }
            //var sw = Stopwatch.StartNew();
            //DataSource.InsertBulk("dbo.Items", list).Execute();
            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds.ToString("N0"));

            var single2 = DataSource.From<Item>(new { ItemDesc = "G000-J064" }).ToObject<Item>(RowOptions.DiscardExtraRows).Execute();
            var list2 = DataSource.From<Item>(new { ItemDesc = "G000-J064" }).ToCollection<Item>().Execute();

            var searchFilter = DataSource.From<Item>().ToString("CategoryID").Execute(); //grab any value

            var single1 = DataSource.From<Item>(new { CategoryID = searchFilter }).ToObject<Item>(RowOptions.DiscardExtraRows).Execute();
            var list1 = DataSource.From<Item>(new { CategoryID = searchFilter }).ToCollection<Item>().Execute();

        }
        static void Main()
        {
            //ReflectionTest();

            //DatabaseTest();

            Issue213();
        }

        private static void ReflectionTest()
        {
            const int runCount = 1000 * 1000;// * 1000;


            var propertyInfo = typeof(SalesOrderHeader).GetProperty("SalesOrderId");
            var reflectionSetter = propertyInfo.GetSetMethod();
            var delegateSetter = (Action<SalesOrderHeader, int>)Delegate.CreateDelegate(typeof(Action<SalesOrderHeader, int>), reflectionSetter);

            {
                var startTime = DateTime.Now;
                for (var i = 0; i < runCount; i++)
                {
                    var foo = (Action<SalesOrderHeader, int>)Delegate.CreateDelegate(typeof(Action<SalesOrderHeader, int>), reflectionSetter);

                }
                var endTime = DateTime.Now;
                Console.WriteLine(((endTime - startTime).TotalMilliseconds / runCount / 1000.0000).ToString("#,##0.000 sec"));

            }
            //var fieldInfo = typeof(SalesOrderHeader).GetField("MaxValue");

            //var dynamicMethod = new DynamicMethod(String.Empty, typeof(void), new Type[] { fieldInfo.ReflectedType.MakeByRefType(), fieldInfo.FieldType }, true);
            //var ilGenerator = dynamicMethod.GetILGenerator();
            //ilGenerator.Emit(OpCodes.Ldarg_0);
            //ilGenerator.Emit(OpCodes.Ldarg_1);
            //ilGenerator.Emit(OpCodes.Stfld, fieldInfo);
            //ilGenerator.Emit(OpCodes.Ret);
            //var fieldSetter = (Action<SalesOrderHeader, int>)dynamicMethod.CreateDelegate(typeof(Action<SalesOrderHeader, int>));

            var fields = new SalesOrderHeader();

            {
                var startTime = DateTime.Now;
                for (var i = 0; i < runCount; i++)
                {
                    reflectionSetter.Invoke(fields, new Object[] { i });
                }
                var endTime = DateTime.Now;
                Console.WriteLine("Reflection:\t\t" + ((endTime - startTime).TotalMilliseconds / 1000.0000).ToString("#,##0.000 sec"));

            }
            Thread.Sleep(TimeSpan.FromSeconds(2));
            {
                var startTime = DateTime.Now;
                for (var i = 0; i < runCount; i++)
                {
                    delegateSetter.Invoke(fields, i);
                }
                var endTime = DateTime.Now;
                Console.WriteLine("Delegate:\t\t" + ((endTime - startTime).TotalMilliseconds / 1000.0000).ToString("#,##0.000 sec"));

            }
            Thread.Sleep(TimeSpan.FromSeconds(2));
            {
                var startTime = DateTime.Now;
                for (var i = 0; i < runCount; i++)
                {
                    object value = i;
                    reflectionSetter.Invoke(fields, new Object[] { value });
                }
                var endTime = DateTime.Now;
                Console.WriteLine("Reflection casting:\t\t" + ((endTime - startTime).TotalMilliseconds / 1000.0000).ToString("#,##0.000 sec"));

            }

            Thread.Sleep(TimeSpan.FromSeconds(2));
            {
                var startTime = DateTime.Now;
                for (var i = 0; i < runCount; i++)
                {
                    object value = i;
                    delegateSetter.Invoke(fields, (dynamic)value);
                }
                var endTime = DateTime.Now;
                Console.WriteLine("Delegate dynamic case:\t\t" + ((endTime - startTime).TotalMilliseconds / 1000.0000).ToString("#,##0.000 sec"));

            }
            Thread.Sleep(TimeSpan.FromSeconds(2));
            {
                var startTime = DateTime.Now;
                for (var i = 0; i < runCount; i++)
                {
                    object value = i;
                    delegateSetter.Invoke(fields, (int)value);
                }
                var endTime = DateTime.Now;
                Console.WriteLine("Delegate explicit:\t\t" + ((endTime - startTime).TotalMilliseconds / 1000.00).ToString("#,##0.000 sec"));

            }
            //fieldSetter(ref fields, 90);
            //Console.WriteLine(fields.MaxValue);
        }

        private static void DatabaseTest()
        {
            DataSource = new SqlServerDataSource("data source=.;initial catalog=AdventureWorks;integrated security=True");
            var set = FetchSet();
            var record = FetchSet().First();
            //Console.Write(record.AccountNumber);
            //Console.Write(record.BillToAddressID);
            //Console.Write(record.Comment);
            //Console.Write(record.CreditCardApprovalCode);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            TimeSpan totalRunTime = default;

            const int runCount = 10;
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


        /// <summary>
        /// Fetches the individual element
        /// </summary>
        /// <param name="key">The key of the element to fetch.</param>
        /// <returns>The fetched element, or null if not found</returns>
        public static SalesOrderHeader FetchIndividual(int key)
        {
            return DataSource.From("[Sales].[SalesOrderHeader]", new { SalesOrderId = key }).ToObject<SalesOrderHeader>().Execute();
        }


        /// <summary>
        /// Fetches the complete set of elements and returns this set as an IEnumerable.
        /// </summary>
        /// <returns>the set fetched</returns>
        public static IEnumerable<SalesOrderHeader> FetchSet()
        {
            return DataSource.From("[Sales].[SalesOrderHeader]").ToCollection<SalesOrderHeader>().Execute();
        }
    }
}
