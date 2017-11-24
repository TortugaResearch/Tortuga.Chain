using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tortuga.Chain;

namespace PerformanceTest
{
    class Program
    {
        static SqlServerDataSource DataSource;
        static void Main()
        {
            //ReflectionTest();

             DatabaseTest();

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
