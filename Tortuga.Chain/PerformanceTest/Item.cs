using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Tortuga.Chain;

namespace PerformanceTest
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
}
