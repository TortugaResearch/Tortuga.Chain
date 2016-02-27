using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SQLite;
using System.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //SqlServerTest();
            SQLiteTest();
        }

        private static void SQLiteTest()
        {
            var connectionString = "Data Source=C:\\Users\\DocEvaad\\Desktop\\SQLiteStudio\\TortugaChainTestDb.sqlite3; Version=3;";
            /* using (var con = new SQLiteConnection(connectionString))
             {
                 con.Open();
                 using (var cmd = new SQLiteCommand("PRAGMA table_info('@TableName');", con))
                 {
                     //cmd.Parameters.Add(new SQLiteParameter("@TableName", "Currency"));
                     cmd.
                     using (var reader = cmd.ExecuteReader())
                     {
                         while (reader.Read())
                         {
                             var name = reader.GetString(reader.GetOrdinal("name"));
                             var typeName = reader.GetString(reader.GetOrdinal("type"));

                             Console.WriteLine($"Table: {name}, Type: {typeName}");
                         }
                     }
                 } 
             }
             ;*/
            var dataSource = new Tortuga.Chain.SQLiteDataSource(connectionString);

            dataSource.DatabaseMetaData.PreloadTables();
            dataSource.DatabaseMetaData.PreloadViews();

            var currencyList = dataSource.From("Currency").AsCollection<Currency>().Execute();


            var test = dataSource.From("test");

            foreach (var item in currencyList)
                Console.WriteLine(item.CurrencyCode + " " + item.CurrencyName);

            var cards1 = dataSource.From("CreditCard", new { @CardType = "Vista" }).AsCollection<CreditCard>().Execute();
            foreach (var item in cards1.Take(10))
                Console.WriteLine(item.CardNumber);
            Console.ReadKey();
        }

        private static void SqlServerTest()
        {
            var connectionString = "Server=.;Database=AdventureWorks2014;Trusted_Connection=True;";
            var dataSource = new Tortuga.Chain.SqlServerDataSource(connectionString);

            dataSource.DatabaseMetadata.PreloadTables();
            dataSource.DatabaseMetadata.PreloadViews();

            const string SalesCurrency = "Sales.Currency";
            var currencyList = dataSource.From(SalesCurrency).AsCollection<Currency>().Execute();
            foreach (var item in currencyList)
                Console.WriteLine(item.CurrencyCode + " " + item.CurrencyName);

            var cards1 = dataSource.From("Sales.CreditCard", new { @CardType = "Vista" }).AsCollection<CreditCard>().Execute();
            foreach (var item in cards1.Take(10))
                Console.WriteLine(item.CardNumber);

            {
                var code = new Currency();
                code.CurrencyCode = (DateTime.Now.Ticks % 1000).ToString();
                code.CurrencyName = "Test " + code.CurrencyCode;

                dataSource.StrictMode = true;

                var newCode = dataSource.Insert(SalesCurrency, code).AsString().Execute();

                code.CurrencyName += " modified";

                dataSource.Update(SalesCurrency, code).Execute();

                dataSource.Delete(SalesCurrency, code).Execute();
            }

            {
                var code = new Currency();
                code.CurrencyCode = (DateTime.Now.Ticks % 1000).ToString();
                code.CurrencyName = "Test " + code.CurrencyCode;

                dataSource.StrictMode = true;

                var newCode = dataSource.InsertOrUpdate(SalesCurrency, code).AsString().Execute();

                code.CurrencyName += " modified";

                dataSource.InsertOrUpdate(SalesCurrency, code).Execute();

                dataSource.Delete(SalesCurrency, code).Execute();
            }

            Console.ReadKey();
        }
    }

    class Currency
    {
        public string CurrencyCode { get; set; }

        [Column("Name")]
        public string CurrencyName { get; set; }

        [NotMapped]
        public bool FakeColumn { get; set; }
    }

    class CreditCard
    {
        public int CreditCardId { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
