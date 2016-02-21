using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=.;Database=AdventureWorks2014;Trusted_Connection=True;";
            var dataSource = new Tortuga.Chain.SqlServerDataSource(connectionString);

            var currencyList = dataSource.From("Sales.Currency").AsCollection<Currency>().Execute();
            foreach (var item in currencyList)
                Console.WriteLine(item.CurrencyCode + " " + item.CurrencyName);

            var cards1 = dataSource.From("Sales.CreditCard", new { @CardType = "Vista" }).AsCollection<CreditCard>().Execute();
            foreach (var item in cards1)
                Console.WriteLine(item.CardNumber);
            

        }
    }

    class Currency
    {
        public string CurrencyCode { get; set; }

        [Column("Name")]
        public string CurrencyName { get; set; }

    }

    class CreditCard
    {
        public int CreditCardID { get; set; }
        public string  CardType { get; set; }
        public string CardNumber { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
