using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tortuga.Chain;

namespace Performance.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseTest();
        }

        static SqlServerDataSource DataSource;


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
        /// Fetches the complete set of elements and returns this set as an IEnumerable.
        /// </summary>
        /// <returns>the set fetched</returns>
        public static IEnumerable<SalesOrderHeader> FetchSet()
        {
            return DataSource.From("[Sales].[SalesOrderHeader]").ToCollection<SalesOrderHeader>().Execute();
        }
    }

    public class SalesOrderHeader
    {
        public string AccountNumber { get; set; }

        public string Comment { get; set; }

        public string CreditCardApprovalCode { get; set; }

        public DateTime DueDate { get; set; }

        public decimal Freight { get; set; }

        public DateTime ModifiedDate { get; set; }

        public bool OnlineOrderFlag { get; set; }

        public DateTime OrderDate { get; set; }

        public string PurchaseOrderNumber { get; set; }

        public byte RevisionNumber { get; set; }

        public Guid Rowguid { get; set; }

        public int SalesOrderId { get; set; }

        public string SalesOrderNumber { get; set; }

        public DateTime? ShipDate { get; set; }

        public byte Status { get; set; }

        public decimal SubTotal { get; set; }

        public decimal TaxAmt { get; set; }

        public decimal TotalDue { get; set; }

        public int CustomerID { get; set; }
        public int? SalesPersonID { get; set; }
        public int? TerritoryID { get; set; }
        public int BillToAddressID { get; set; }
        public int ShipToAddressID { get; set; }
        public int ShipMethodID { get; set; }
        public int? CreditCardID { get; set; }
        public int? CurrencyRateID { get; set; }

    }
}
