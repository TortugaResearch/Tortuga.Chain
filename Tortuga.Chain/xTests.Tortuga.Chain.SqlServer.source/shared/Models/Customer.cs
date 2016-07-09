using System;
using System.Collections.Generic;

namespace Tests.Models
{
    public class Customer
    {
        public int? CustomerKey { get; set; }
        public string FullName { get; set; }
        public string State { get; set; }

        public List<Order> Orders { get; } = new List<Order>();
    }
}
