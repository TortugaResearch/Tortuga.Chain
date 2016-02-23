using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=.;Database=AdventureWorks2014;Trusted_Connection=True;";
            var dataSource = new Tortuga.Chain.SqlServerDataSource(connectionString);

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
        public int CreditCardID { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
