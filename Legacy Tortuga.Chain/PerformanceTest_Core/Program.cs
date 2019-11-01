using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Tortuga.Chain;

namespace PerformanceTest_Core
{
    [Table("Items", Schema = "dbo")]
    public class Item
    {
        public string ItemNbr { get; set; } = "";
        public string CategoryID { get; set; } = "";
        public string ItemDesc { get; set; } = "";
        public string ItemContents { get; set; } = "";
        public decimal Price { get; set; } = 0.0M;
    }

    class Program
    {

        static string tableDef = @"
CREATE TABLE dbo.Items(
ItemNbr NVARCHAR(50) NOT NULL,
CategoryID NVARCHAR(50) NOT NULL,
ItemDesc NVARCHAR(50) NOT NULL,
ItemContents NVARCHAR(50) NOT NULL,
Price DECIMAL NULL,
CONSTRAINT PK_Items PRIMARY KEY(ItemNbr, CategoryID)
)
";

        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");


            var DataSource = new SqlServerDataSource("data source=.;initial catalog=Test;integrated security=True");

            //DataSource.Sql("DROP TABLE dbo.Items").Execute();


            DataSource.DatabaseMetadata.PreloadTables();

            //if (!DataSource.DatabaseMetadata.GetTablesAndViews().Any(t => t.Name == "dbo.Items"))
            //{
            //    DataSource.Sql(tableDef).Execute();

            //    var list = new List<Item>();
            //    for (var i = 0; i < 100; i++)
            //        for (var j = 0; j < 100; j++)
            //        {
            //            list.Add(new Item()
            //            {
            //                ItemNbr = "G" + i.ToString("000"),
            //                CategoryID = "J" + j.ToString("000"),
            //                ItemDesc = "G" + i.ToString("000") + "-" + "J" + j.ToString("000"),
            //                Price = i % 2 == 0 ? (decimal?)null : i + j * 0.01M
            //            });

            //        }

            //    DataSource.InsertBulk("dbo.Items", list).Execute();
            //}

            var searchFilter = DataSource.From<Item>().ToString("CategoryID").Execute(); //grab any value

            var single1 = DataSource.From<Item>(new { CategoryID = searchFilter }).ToObject<Item>(RowOptions.DiscardExtraRows).Execute();
            var list1 = DataSource.From<Item>(new { CategoryID = searchFilter }).ToCollection<Item>().Execute();
        }
    }
}
