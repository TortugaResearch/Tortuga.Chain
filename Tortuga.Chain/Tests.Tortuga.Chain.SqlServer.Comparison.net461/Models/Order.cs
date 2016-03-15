namespace Tests.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sales.Order")]
    public partial class Order
    {
        [Key]
        public int OrderKey { get; set; }

        public int CustomerKey { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime OrderDate { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
